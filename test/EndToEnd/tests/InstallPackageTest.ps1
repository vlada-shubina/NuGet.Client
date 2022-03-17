# Verify Xunit 2.1.0 can be installed into a net45 project.
# https://github.com/NuGet/Home/issues/1711

function Test-SimpleBindingRedirectsIndirectReference {
    param($context)

    # Arrange
    $a = New-WebApplication
    $b = New-ClassLibrary
    $c = New-ClassLibrary

    Add-ProjectReference $a $b
    Add-ProjectReference $b $c

    # Act
    $c | Install-Package E -Source $context.RepositoryPath
    $c | Update-Package F -Safe -Source $context.RepositoryPath

    # Assert
    Assert-Null (Get-ProjectItem $b web.config)
    Assert-Null (Get-ProjectItem $c web.config)
    Assert-BindingRedirect $a web.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
    Assert-BindingRedirect $b app.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
    Assert-BindingRedirect $c app.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
}

function Test-SimpleBindingRedirectsNonWeb {
    param($context)

    # Arrange
    $a = New-ConsoleApplication
    $b = New-WPFApplication
    $projects = @($a, $b)

    # Act
    #$projects | Install-Package E -Source $context.RepositoryPath
    $a | Install-Package E -Source $context.RepositoryPath
    $b | Install-Package E -Source $context.RepositoryPath
    $a | Update-Package F -Safe -Source $context.RepositoryPath
    $b | Update-Package F -Safe -Source $context.RepositoryPath

    # Assert
    $projects | %{ Assert-Package $_ E;
                   Assert-BindingRedirect $_ app.config F '0.0.0.0-1.0.5.0' '1.0.5.0' }
}

function Test-BindingRedirectComplex {
    param($context)

    # Arrange
    $a = New-WebApplication
    $b = New-ConsoleApplication
    $c = New-ClassLibrary

    Add-ProjectReference $a $b
    Add-ProjectReference $b $c

    $projects = @($a, $b)

    # Act
    $c | Install-Package E -Source $context.RepositoryPath
    $c | Update-Package F -Safe -Source $context.RepositoryPath

    Assert-Package $c E;

    # Assert
    Assert-BindingRedirect $a web.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
    Assert-BindingRedirect $b app.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
}

function Test-SimpleBindingRedirectsWebsite {
    param(
        $context
    )
    # Arrange
    $a = New-WebSite

    # Act
    $a | Install-Package E -Source $context.RepositoryPath
    $a | Update-Package F -Safe -Source $context.RepositoryPath

    # Assert
    Assert-Package $a E;
    Assert-BindingRedirect $a web.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
}


function Test-BindingRedirectInstallLargeProject {
    param($context)

    $numProjects = 25
    $projects = 0..$numProjects | %{ New-ClassLibrary $_ }
    $p = New-WebApplication

    for($i = 0; $i -lt $numProjects; $i++) {
        Add-ProjectReference $projects[$i] $projects[$i+1]
    }

    Add-ProjectReference $p $projects[0]

    $projects[$projects.Length - 1] | Install-Package E -Source $context.RepositoryPath
    $projects[$projects.Length - 1] | Update-Package F -Safe -Source $context.RepositoryPath
    Assert-BindingRedirect $p web.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
}

function Test-BindingRedirectDuplicateReferences {
    param($context)

    # Arrange
    $a = New-WebApplication
    $b = New-ConsoleApplication
    $c = New-ClassLibrary

    ($a, $b) | Install-Package A -Source $context.RepositoryPath -IgnoreDependencies

    Add-ProjectReference $a $b
    Add-ProjectReference $b $c

    # Act
    $c | Install-Package E -Source $context.RepositoryPath
    $c | Update-Package F -Safe -Source $context.RepositoryPath

    Assert-Package $c E

    # Assert
    Assert-BindingRedirect $a web.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
    Assert-BindingRedirect $b app.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
}

function Test-BindingRedirectClassLibraryWithDifferentDependents {
    param($context)

    # Arrange
    $a = New-WebApplication
    $b = New-ConsoleApplication
    $c = New-ClassLibrary

    ($a, $b) | Install-Package A -Source $context.RepositoryPath -IgnoreDependencies

    Add-ProjectReference $a $c
    Add-ProjectReference $b $c

    # Act
    $c | Install-Package E -Source $context.RepositoryPath
    $c | Update-Package F -Safe -Source $context.RepositoryPath

    Assert-Package $c E

    # Assert
    Assert-BindingRedirect $a web.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
    Assert-BindingRedirect $b app.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
}

function Test-BindingRedirectProjectsThatReferenceSameAssemblyFromDifferentLocations {
    param($context)

    # Arrange
    $a = New-WebApplication
    $b = New-ConsoleApplication
    $c = New-ClassLibrary

    $a | Install-Package A -Source $context.RepositoryPath -IgnoreDependencies
    $aPath = ls (Get-SolutionDir) -Recurse -Filter A.dll
    cp $aPath.FullName (Get-SolutionDir)
    $aNewLocation = Join-Path (Get-SolutionDir) A.dll

    $b.Object.References.Add($aNewLocation)

    Add-ProjectReference $a $b
    Add-ProjectReference $b $c

    # Act
    $c | Install-Package E -Source $context.RepositoryPath
    $c | Update-Package F -Safe -Source $context.RepositoryPath

    Assert-Package $c E

    # Assert
    Assert-BindingRedirect $a web.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
    Assert-BindingRedirect $b app.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
}

function Test-BindingRedirectsMixNonStrongNameAndStrongNameAssemblies {
    param(
        $context
    )
    # Arrange
    $a = New-ConsoleApplication

    # Act
    $a | Install-Package PackageWithNonStrongNamedLibA -Source $context.RepositoryRoot
    $a | Install-Package PackageWithNonStrongNamedLibB -Source $context.RepositoryRoot

    # Assert
    Assert-Package $a PackageWithNonStrongNamedLibA
    Assert-Package $a PackageWithNonStrongNamedLibA
    Assert-Package $a PackageWithStrongNamedLib 1.1
    Assert-Reference $a A 1.0.0.0
    Assert-Reference $a B 1.0.0.0
    Assert-Reference $a Core 1.1.0.0

    Assert-BindingRedirect $a app.config Core '0.0.0.0-1.1.0.0' '1.1.0.0'
}

function Test-BindingRedirectProjectsThatReferenceDifferentVersionsOfSameAssembly {
    param($context)

    # Arrange
    $a = New-WebApplication
    $b = New-ConsoleApplication
    $c = New-ClassLibrary

    $a | Install-Package A -Source $context.RepositoryPath -IgnoreDependencies
    $b | Install-Package A -Version 1.0 -Source $context.RepositoryPath -IgnoreDependencies

    Add-ProjectReference $a $b
    Add-ProjectReference $b $c

    # Act
    $c | Install-Package E -Source $context.RepositoryPath
    $c | Update-Package F -Safe -Source $context.RepositoryPath

    Assert-Package $c E

    # Assert
    Assert-BindingRedirect $a web.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
    Assert-BindingRedirect $b app.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
}

# Tests the case when Skip is specified in nuget.config under bindingRedirects section
function Test-InstallPackageSkipsBindingRedirectWhenSetOnConfig
{
    param(
        $context
    )

    # Arrange
    Check-NuGetConfig

    $componentModel = Get-VSComponentModel
    $setting = $componentModel.GetService([NuGet.Configuration.ISettings])

    $a = New-WebSite

    try
    {
        # Act
        $o = [NuGet.Configuration.AddItem]::new('skip', 'true')
        $setting.AddOrUpdate('bindingRedirects', $o)

        $a | Install-Package E -Source $context.RepositoryPath
        $a | Update-Package F -Safe -Source $context.RepositoryPath

        # Assert
        Assert-Package $a E;
        Assert-NoBindingRedirect $a web.config F '0.0.0.0-1.0.5.0' '1.0.5.0'
    }
    finally {
        $section = $setting.GetSection('bindingRedirects')

        ForEach ($item in $section.Items) {
            $setting.Remove('bindingRedirects', $item)
        }
    }
}

