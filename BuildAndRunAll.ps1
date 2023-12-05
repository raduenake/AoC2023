Write-Host "Day`tBld[ms]`tRun[ms]"

$totalBuild = 0
$totalRun = 0

1..25 | 
    ForEach-Object { ([string]$_).PadLeft(2, '0') } | Where-Object { Test-Path $_ -PathType Container } | 
    ForEach-Object { 
        Set-Location $_
        
        $platform = $IsLinux -eq $true ? "linux-x64" : "win-x64"
        $prog = $IsLinux -eq $true ? "$($_)" : "$($_).exe"

        & dotnet clean > $null
        & dotnet restore > $null
        
        
        $build = (Measure-Command { & dotnet build --configuration Release --arch x64 --no-self-contained > $null }).TotalMilliseconds
        $run = (Measure-Command { & ".\bin\Release\net7.0\$($platform)\$($prog)" > $null }).TotalMilliseconds
        
        $totalBuild = $totalBuild + $build
        $totalRun = $totalRun + $run

        Write-Host "$($_)`t$('{0:f}' -f $build)`t$('{0:f}' -f $run)"
        Set-Location .. 
    }

Write-Host "Total`t$('{0:f}' -f $totalBuild)`t$('{0:f}' -f $totalRun)"