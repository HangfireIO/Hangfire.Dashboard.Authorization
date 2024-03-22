Include "packages\Hangfire.Build.0.4.3\tools\psake-common.ps1"

Task Default -Depends Pack

Task Merge -Depends Compile -Description "Run ILRepack /internalize to merge assemblies." {
    Repack-Assembly @("Hangfire.Dashboard.Authorization", "net45") @("Microsoft.Owin")
}

Task Collect -Depends Merge -Description "Copy all artifacts to the build folder." {
    Collect-Assembly "Hangfire.Dashboard.Authorization" "net45"
    Collect-File "README.md"
}

Task Pack -Depends Collect -Description "Create NuGet packages and archive files." {
    $version = Get-PackageVersion
    
    Create-Package "Hangfire.Dashboard.Authorization" $version
    Create-Archive "Hangfire.Dashboard.Authorization-$version"
}

Task Sign -Depends Pack -Description "Sign artifacts." {
    $version = Get-PackageVersion
    Sign-ArchiveContents "Hangfire.Dashboard.Authorization-$version" "hangfire"
}