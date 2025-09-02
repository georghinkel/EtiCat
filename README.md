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
