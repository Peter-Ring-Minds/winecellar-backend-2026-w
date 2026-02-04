Param(
  [switch]$StartDb
)

$ErrorActionPreference = 'Stop'

function Require-Command($name) {
  if (-not (Get-Command $name -ErrorAction SilentlyContinue)) {
    throw "Missing required command: $name"
  }
}

Require-Command docker
Require-Command dotnet

$RootDir = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$EnvFile = Join-Path $RootDir '.env'
$EnvExample = Join-Path $RootDir '.env.example'
$ApiCsproj = Join-Path $RootDir 'src/Api/Api.csproj'

if (-not (Test-Path $EnvFile)) {
  if (Test-Path $EnvExample) {
    Copy-Item $EnvExample $EnvFile
    Write-Host 'Created .env from .env.example'
  } else {
    @(
      'SA_PASSWORD=',
      'DB_PLATFORM=linux/amd64'
    ) | Set-Content -Path $EnvFile -Encoding UTF8
    Write-Host 'Created .env'
  }
} else {
  Write-Host '.env already exists'
}

$lines = Get-Content -Path $EnvFile -ErrorAction SilentlyContinue
$currentPwLine = $lines | Where-Object { $_ -match '^SA_PASSWORD=' } | Select-Object -First 1
$currentPw = ''
if ($currentPwLine) { $currentPw = $currentPwLine.Substring('SA_PASSWORD='.Length) }

Write-Host ''
if ($currentPw) {
  Write-Host 'SA_PASSWORD is currently set in .env (hidden).'
} else {
  Write-Host 'SA_PASSWORD is not set yet.'
}

Write-Host 'Enter a SQL Server SA password to store in .env.'
Write-Host '- Must be at least 8 chars and include uppercase, lowercase, numbers, and symbols (SQL Server policy).'
Write-Host '- Press Enter to keep the existing value (if set).'

$secure = Read-Host 'SA_PASSWORD' -AsSecureString
$plain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure))

if ($plain) {
  $updated = @()
  $found = $false
  foreach ($line in $lines) {
    if ($line -match '^SA_PASSWORD=') {
      $updated += ('SA_PASSWORD=' + $plain)
      $found = $true
    } else {
      $updated += $line
    }
  }
  if (-not $found) { $updated = @('SA_PASSWORD=' + $plain) + $updated }
  $updated | Set-Content -Path $EnvFile -Encoding UTF8
  Write-Host 'Updated SA_PASSWORD in .env'
} else {
  Write-Host 'Kept existing SA_PASSWORD'
}

$lines = Get-Content -Path $EnvFile
$pw = ($lines | Where-Object { $_ -match '^SA_PASSWORD=' } | Select-Object -First 1)
if (-not $pw) {
  throw 'SA_PASSWORD is missing in .env'
}
$pwValue = $pw.Substring('SA_PASSWORD='.Length)
if (-not $pwValue) {
  throw 'SA_PASSWORD is empty in .env'
}

$conn = "Server=localhost,1433;Database=WinecellarDb;User Id=sa;Password=$pwValue;TrustServerCertificate=True"

Write-Host ''
Write-Host 'Setting API connection string via dotnet user-secrets...'
dotnet user-secrets set --project $ApiCsproj 'ConnectionStrings:Default' $conn | Out-Null
Write-Host 'User-secrets configured for src/Api/Api.csproj'

if ($StartDb) {
  Push-Location $RootDir
  docker compose up -d
  Pop-Location
  Write-Host 'DB started. SQL Server is on localhost:1433'
} else {
  Write-Host "Skipping DB start. Run: docker compose up -d"
  Write-Host "Or re-run with: .\scripts\setup-windows.ps1 -StartDb"
}

Write-Host ''
Write-Host 'Next commands:'
Write-Host '  dotnet restore'
Write-Host '  dotnet run --project src/Api/Api.csproj'
