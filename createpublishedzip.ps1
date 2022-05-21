param(
    [String]$os="win;linux;osx",
    [String]$arch="x64",
    [String]$configuration="Release"
    )

function Build-SFP {
    param (
        [string]$TargetRuntime
    )

    Remove-Item -Path "./$configuration/publish" -Recurse -Force -ErrorAction Ignore
    dotnet publish --configuration $configuration --output $configuration/publish --self-contained --runtime $targetRuntime
    Get-ChildItem "./$configuration/publish/*" -Recurse -Exclude SteamResourcePatcher.config,*.log | Compress-Archive -DestinationPath "./$configuration/SteamResourcePatcher-SelfContained-$targetRuntime.zip" -Force
}

foreach ($currentOS in $os.Split(";"))
{
    $targetRuntime = "$currentOS-$arch"

    Build-SFP -TargetRuntime $targetRuntime
}
