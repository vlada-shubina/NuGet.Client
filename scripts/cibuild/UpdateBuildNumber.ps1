<#
.SYNOPSIS
Updates Build Number

.DESCRIPTION
Updates Build Number

.PARAMETER SemanticVersion
Semantic Version

.PARAMETER BuildRevision
Build Counter
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$True)]
    [Alias('sv')]
    [string]$SemanticVersion,
    [Parameter(Mandatory=$True)]
    [Alias('r')]
    [int]$BuildRevision
)

try {
    $FullBuildNumber = "$(SemanticVersion).$(BuildRevision)"
    Write-Output "Full Build Number is $FullBuildNumber"
    $msbuildExe = 'C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\bin\msbuild.exe'
    $targetChannel = & $msbuildExe $env:BUILD_REPOSITORY_LOCALPATH\build\config.props /v:m /nologo /t:GetVsTargetChannel
    $targetChannel = $targetChannel.Trim()
    $targetMajorVersion = & $msbuildExe $env:BUILD_REPOSITORY_LOCALPATH\build\config.props /v:m /nologo /t:GetVsTargetMajorVersion
    $targetMajorVersion = $targetMajorVersion.Trim()
    Write-Host "##vso[task.setvariable variable=VsTargetChannel;isOutput=true]$targetChannel"
    Write-Host "##vso[task.setvariable variable=VsTargetMajorVersion;isOutput=true]$targetMajorVersion"
    Write-Host "##vso[build.updatebuildnumber]$FullBuildNumber"
    Write-Host "##vso[task.setvariable variable=BuildNumber;isOutput=true]$(BuildRevision)"
    Write-Host "##vso[task.setvariable variable=FullVstsBuildNumber;isOutput=true]$FullBuildNumber"
  } catch {
    Write-Host "##vso[task.LogIssue type=error;]Unable to set build number"
    exit 1
  }
