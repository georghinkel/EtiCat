# EtiCat

A tool to label and tag code in a monorepository with components that are versioned semi-independently. Thus, it is mainly targeted to libraries, more specifically libraries written in C#. Semi-independent versions means, not all libraries have the same version but it may be that multiple assemblies share a version number because they belong to the same sub-project. The super-power of *EtiCat* is that it manages version ranges. That is, if you care about semantic versioning and want to create libraries that have dedicated dependency ranges, *EtiCat* might become a good friend because it automatically gets the dependency version ranges right.

## Setup

*EtiCat* ships as a .NET tool, so you should be able to install it via the command-line:

```bash
dotnet tool install -g EtiCat
```

## Command-line Reference

Afterwards, you can call it from the command line. *EtiCat* supports the following verbs:

- **help** will give you explanations of the tool or to a specific verb
- **version** will print the version number of *EtiCat*
- **apply** will go through your repo and assign version numbers everywhere
- **check** will check that all changes you did are reflected in a version
- **changelog** will create a changelog of all your modules in Markdown format
- **ci** will compile and test all changes since the baseline (*main* by default or *HEAD~* if you are on *main*)
- **pack** will compile, test and pack all your components
- **publish** will give you the full paths of all components that need to be published
- **tag** will calculate a tag name for the current *HEAD*

## Module Histories

To do this, *EtiCat* relies on versioning files called module histories. These can be anywhere in your repository and define a module, seen as a unit of versioning. The key point is that every change you make in any of the folders controlled by this module must be reflected in this module history (checked using the **check** verb).

Module histories are denoted by the extension *.history* and are plain text files. An example is the following:

```plain
module Foo
folder src
compile src/Foo/Foo.csproj
component nuget/Foo.nuspec from src/Foo/Properties/AssemblyInfo.cs

major (0.1.0): Initial
minor: A feature
minor: Feature 2

Any line not starting with either major, minor or patch is regarded documentation of the change.
major: Major feature
patch: A patch
```

*EtiCat* determines the type of component primarily using file paths. It reads the component definitions and extracts dependency information. When the version of a component changes, *EtiCat* makes sure that all components using that component as a dependency also get a new version, if the new version falls out of the current dependency version range, for instance because an artifact such as a nuspec excludes newer major versions.

In addition, *EtiCat* can also determine which modules are affected by a given change from Git to limit the work done by CI jobs. That means, using the **ci** verb, *EtiCat* only compiles components and runs respective tests that are affected by a change relative to a given baseline (by default: *main*).

## GitHub Actions CI

Consider the following examples, how EtiCat can be used to build up efficient CI pipelines for mono-repos.

### CI Workflow (default)

The following GitHub Actions workflow will build only changed modules compared to the main branch. It will use the branch name as prerelease marker.

```yaml
name: default

on:
  pull_request:
    branches: [ "main" ]

jobs:
  build:

    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4
      with:
        fetch-depth: 0
    - name: Extract branch name
      shell: bash
      run: echo "branch=${GITHUB_HEAD_REF:-${GITHUB_REF#refs/heads/}}" >> $GITHUB_OUTPUT
      id: extract_branch

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: | 
          8.0.x
          9.0.x
    - name: Install Eticat
      run: dotnet tool install -g EtiCat
    - name: CI
      run: EtiCat ci --prerelease ${{ steps.extract_branch.outputs.branch }} --baseline origin/main
    - name: Upload Packages
      uses: actions/upload-artifact@v4
      with:
        name: Nuget Packages
        path: Build/*.nupkg
```

An important bit here is that the checkout action gets the `fetch-depth: 0` configuration as the checkout action will otherwise only fetch the commit that shall be built. However, EtiCat requires a history in order to detect changes. The last step assumes that NuGet packages are present under a directory `Build`.
*EtiCat* will make sure that only packages affected by the commit are actually built and the wildcard upload action will only upload packages that are affected by the build.

### Release CI Build

The second suggested build workflow is the following release build. In this workflow, no prerelease marker is added to the version number. To set the baseline, we use a GitHub action
to retrieve the last commit for which a successful workflow was created. 

```yaml
name: release

on:
  push:
    branches: [ "main" ]

jobs:
  build:

    runs-on: ubuntu-latest
    permissions:
      packages: write
      contents: read

    steps:
    - uses: actions/checkout@v4
      with:
        fetch-depth: 0
    - uses: nrwl/last-successful-commit-action@v1
      id: last_successful_commit
      with:
        branch: 'main'
        workflow_id: 'release.yml'
        github_token: ${{ secrets.GITHUB_TOKEN }}
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: | 
          8.0.x
          9.0.x
    - name: Install Eticat
      run: dotnet tool install -g EtiCat
    - name: CI
      run: EtiCat ci  --baseline ${{ steps.last_successful_commit.outputs.commit_hash }}
    - name: Publish the package
      run: if (Test-path Build/*.nupkg) { dotnet nuget push Build/*.nupkg --source https://api.nuget.org/v3/index.json --skip-duplicate --api-key ${{ secrets.NUGET_API_KEY }} }
      shell: pwsh
```