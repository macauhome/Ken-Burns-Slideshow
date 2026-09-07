param(
    [string]$Configuration = "Release",
    [switch]$Install,
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root "Ken Burns Slideshow.vbproj"
$outputDir = Join-Path $root "bin\$Configuration\net10.0-windows"
$exePath = Join-Path $outputDir "Ken Burns Slideshow.exe"
$scrPath = Join-Path $root "Ken Burns Slideshow.scr"

Write-Host "Project: $project"
Write-Host "Output: $outputDir"

if (-not $SkipBuild) {
    Write-Host "Running dotnet restore..."
    & dotnet restore $project

    Write-Host "Running dotnet build..."
    & dotnet build $project -c $Configuration --no-restore

    if ($LASTEXITCODE -ne 0) {
        throw "Build failed."
    }
}

if (-not (Test-Path $exePath)) {
    throw "Executable not found: $exePath"
}

Copy-Item $exePath $scrPath -Force
Write-Host "Created screen saver package: $scrPath"

if ($Install) {
    $target = Join-Path $env:SystemRoot "System32\Ken Burns Slideshow.scr"
    Copy-Item $scrPath $target -Force
    Write-Host "Installed to: $target"
}

Write-Host "Done."
