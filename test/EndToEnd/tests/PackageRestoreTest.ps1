# Test that during legacy packagereference project restore, PackageId property is considered for asset file creation. msbuild restore already does it.
# Priority: PackageId -> AssemblyName -> Project File Name.
function Test-VSRestore-PackageId-Considered-Over-AssemblyName {
    param($context)

    # Arrange
    $customPackageId = "MySpecialPackageId"
    $customAssemblyName = "MySpecialAssemblyName"
    $MSBuildExe = Get-MSBuildExe
    $p1 = New-Project PackageReferenceClassLibrary
    $solutionFile = Get-SolutionFullName
    $projectDirectoryPath = $p1.Properties.Item("FullPath").Value
    $projectPath = $p1.FullName
    $binDirectory = Join-Path $projectDirectoryPath "bin"
    $debugDirectory = Join-Path $binDirectory "debug"
    SaveAs-Solution($solutionFile)

    # Change assembly name in .csproj file
    $doc = [xml](Get-Content $projectPath)
    $ns = New-Object System.Xml.XmlNamespaceManager($doc.NameTable)
    $ns.AddNamespace("ns", $doc.DocumentElement.NamespaceURI)
    $assemblyNameNode = $doc.SelectSingleNode("//ns:AssemblyName",$ns)
    $assemblyNameNode.InnerText = $customAssemblyName
    $node = $doc.DocumentElement.ChildNodes[1]
    $packageIdNode = $doc.CreateElement("PackageId",$doc.DocumentElement.NamespaceURI)
    $packageIdInnerNode = $doc.CreateTextNode($customPackageId);
    $packageIdNode.AppendChild($packageIdInnerNode);
    $node.InsertAfter($packageIdNode, $node.FirstChild)
    $doc.Save($projectPath)
    Close-Solution
    Open-Solution $solutionFile

    $project = Get-Project

    # Act VS restore
    Build-Solution  # generate asset file

    # Assert VS restore
    $assetFilePath = Get-NetCoreLockFilePath $project
    $vsRestoredAsset = Get-Content -Raw -Path $assetFilePath
    $vsRestoredAssetJson = $vsRestoredAsset | ConvertFrom-Json
    $projectNameInAssetFile = $vsRestoredAssetJson.Project.Restore | Select-Object projectName
    # Assert generated asset file contains correct projectName = AssemblyName
    Assert-True ($projectNameInAssetFile.projectName -eq $customPackageId)

    # Arrange MSBuild restore
    # Remove VS asset file.
    Remove-Item -Force $assetFilePath

    # Act MSBuild restore
    & "$MSBuildExe" /t:restore $project.FullName
    Assert-True ($LASTEXITCODE -eq 0)

    # Main Assert
    $msBuildRestoredAsset = Get-Content -Raw -Path $assetFilePath
    # Assert msbuild and VS restore result in same asset file.
    Assert-True ($vsRestoredAsset -eq $msBuildRestoredAsset)
}

# Create a test package
function CreateTestPackage {
    param(
        [string]$id,
        [string]$version,
        [string]$outputDirectory
    )

    $builder = New-Object NuGet.PackageBuilder
    $builder.Authors.Add("test_author")
    $builder.Id = $id
    $builder.Version = New-Object NuGet.SemanticVersion($version)
    $builder.Description = "description"

    # add one content file
    $tempFile = [IO.Path]::GetTempFileName()
    "test" >> $tempFile
    $packageFile = New-Object NuGet.PhysicalPackageFile
    $packageFile.SourcePath = $tempFile
    $packageFile.TargetPath = "content\$id-test1.txt"
    $builder.Files.Add($packageFile)

    # create the package file
    $outputFileName = Join-Path $outputDirectory "$id.$version.nupkg"
    $outputStream = New-Object IO.FileStream($outputFileName, [System.IO.FileMode]::Create)
    try {
        $builder.Save($outputStream)
    }
    finally
    {
        $outputStream.Dispose()
        Remove-Item $tempFile
    }
}

function RemoveDirectory {
    param($dir)

    $iteration = 0
    while ($iteration++ -lt 10)
    {
        if (Test-Path $dir)
        {
            # because -Recurse parameter in Remove-Item has a known issue so using Get-ChildItem to
            # first delete all the children and then delete the folder.
            Get-ChildItem $dir -Recurse | Remove-Item -Force -Recurse -ErrorAction SilentlyContinue
            Remove-Item -Recurse -Force $dir -ErrorAction SilentlyContinue
        }
        else
        {
            break;
        }
    }
}
