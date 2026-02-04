#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="$ROOT_DIR/.env"
ENV_EXAMPLE="$ROOT_DIR/.env.example"
API_CSPROJ="$ROOT_DIR/src/Api/Api.csproj"

require_cmd() {
  if ! command -v "$1" >/dev/null 2>&1; then
    echo "Missing required command: $1" >&2
    exit 1
  fi
}

require_cmd docker
require_cmd dotnet

if [[ ! -f "$ENV_FILE" ]]; then
  if [[ -f "$ENV_EXAMPLE" ]]; then
    cp "$ENV_EXAMPLE" "$ENV_FILE"
    echo "Created .env from .env.example"
  else
    cat >"$ENV_FILE" <<'EOF'
SA_PASSWORD=
DB_PLATFORM=linux/amd64
EOF
    echo "Created .env"
  fi
else
  echo ".env already exists"
fi

current_pw=""
if [[ -f "$ENV_FILE" ]]; then
  current_pw="$(grep -E '^SA_PASSWORD=' "$ENV_FILE" | head -n1 | cut -d= -f2- || true)"
fi

echo
if [[ -n "$current_pw" ]]; then
  echo "SA_PASSWORD is currently set in .env (hidden)."
else
  echo "SA_PASSWORD is not set yet."
fi

echo "Enter a SQL Server SA password to store in .env."
echo "- Must be at least 8 chars and include uppercase, lowercase, numbers, and symbols (SQL Server policy)."
echo "- Press Enter to keep the existing value (if set)."

read -r -s -p "SA_PASSWORD: " new_pw
echo

if [[ -n "$new_pw" ]]; then
  python3 - <<PY
from pathlib import Path
env_path = Path(r"$ENV_FILE")
lines = env_path.read_text(encoding="utf-8").splitlines()
updated = []
found = False
for line in lines:
    if line.startswith("SA_PASSWORD="):
        updated.append("SA_PASSWORD=" + r"$new_pw")
        found = True
    else:
        updated.append(line)
if not found:
    updated.insert(0, "SA_PASSWORD=" + r"$new_pw")
env_path.write_text("\n".join(updated) + "\n", encoding="utf-8")
PY
  echo "Updated SA_PASSWORD in .env"
else
  echo "Kept existing SA_PASSWORD"
fi

pw_for_conn="$(grep -E '^SA_PASSWORD=' "$ENV_FILE" | head -n1 | cut -d= -f2- || true)"
if [[ -z "$pw_for_conn" ]]; then
  echo "\nSA_PASSWORD is still empty in .env; cannot set API user-secrets yet." >&2
  echo "Edit .env and rerun this script." >&2
  exit 1
fi

conn="Server=localhost,1433;Database=WinecellarDb;User Id=sa;Password=$pw_for_conn;TrustServerCertificate=True"

echo
echo "Setting API connection string via dotnet user-secrets..."
dotnet user-secrets set --project "$API_CSPROJ" "ConnectionStrings:Default" "$conn" >/dev/null
echo "User-secrets configured for src/Api/Api.csproj"

echo
read -r -p "Start the database now with 'docker compose up -d'? [y/N] " start_db
if [[ "$start_db" =~ ^[Yy]$ ]]; then
  (cd "$ROOT_DIR" && docker compose up -d)
  echo "DB started. SQL Server is on localhost:1433"
else
  echo "Skipping DB start. You can run: docker compose up -d"
fi

echo
cat <<EOF
Next commands:
  dotnet restore
  dotnet run --project src/Api/Api.csproj
EOF
