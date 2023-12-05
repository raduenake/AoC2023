[CmdletBinding()]
param(
    [Parameter(Mandatory = 0)][string]$Day = '',
    [Parameter(Mandatory = 0)][string]$Session = '',
    [Parameter(Mandatory = 0)][string]$Year = ''
)

$Year = ($Year -eq '') ? (Get-Date -Format "yyyy") : $Year
$Day = ($Day -eq '') ? (Get-Date -Format "dd") : $Day
$Session = ($Session -eq '') ? ($env:AOC_SESSION) : $Session

Write-Host "Initializing for $Year/$Day with $($Session.Substring(0, 10))..."

$workingDir = Split-Path -Path $MyInvocation.MyCommand.Path -Parent
$child = Get-ChildItem -Path $workingDir -Filter $Day

if ($child.Count -eq 0) {
    $location = New-Item -ItemType Directory -Path $workingDir -Name $Day

    Set-Location $location

    & dotnet new console > $null
    & curl -b session=$Session "https://adventofcode.com/$($Year)/day/$([int]$Day)/input" -o "input.txt" -s > $null

    Set-Location $workingDir
}
else {
    Write-Error "Folder exists! Abort!"
}
