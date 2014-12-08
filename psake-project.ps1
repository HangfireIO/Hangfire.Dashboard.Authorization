Properties {
    $solution = "Hangfire.Dashboard.Authorization.sln"
}

Include "packages\Hangfire.Build.*\tools\psake-common.ps1"

Task Default -Depends Collect

Task Merge -Depends Compile -Description "Run ILMerge /internalize to merge assemblies." {
    Merge-Assembly "Hangfire.Dashboard.Authorization" @("Microsoft.Owin")
}

Task Collect -Depends Merge -Description "Copy all artifacts to the build folder." {
    Collect-Assembly "Hangfire.Dashboard.Authorization" "Net45"
}

Task Pack -Depends Collect -Description "Create NuGet packages and archive files." {
    $version = Get-BuildVersion
    Create-Package "Hangfire.Dashboard.Authorization" $version
}