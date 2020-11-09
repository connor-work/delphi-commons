# About this file
This file tracks significant changes to the project setup that are not easily recognizable from file diffs (e.g., project creation wizard operations).

# Changes
1. Created a *[global.json file](https://docs.microsoft.com/en-us/dotnet/core/tools/global-json?tabs=netcore3x)* to fix .NET Core SDK version.

    ```powershell
    dotnet new globaljson --sdk-version $(dotnet --version)
    ```

2. Created a *[nuget.config file](https://docs.microsoft.com/en-us/nuget/reference/nuget-config-file)* to fix package sources.

    ```powershell
    dotnet new nugetconfig
    ```

3. Created new .NET Core project for a Class library (Delphi Commons Code Writer Extensions).

    ```powershell
    dotnet new classlib --language C`# --name code-writer-extensions --framework netcoreapp3.1 --output code-writer-extensions
    ```

4. Created new .NET Core solution (Delphi Commons).

    ```powershell
    dotnet new sln --name delphi-commons
    ```

5. Added `code-writer-extensions` project to `delphi-commons` solution.

    ```powershell
    dotnet sln add code-writer-extensions
    ```

6. Created new xUnit test project (tests for Delphi Commons Code Writer Extensions).

    ```powershell
    dotnet new xunit --name code-writer-extensions.tests --framework netcoreapp3.1 --output code-writer-extensions.tests
    ```

7. Added `code-writer-extensions.tests` project to `delphi-commons` solution.

    ```powershell
    dotnet sln add code-writer-extensions.tests
    ```

8. Added SonarAnalyzer for static code analysis to `code-writer-extensions` project.

    ```powershell
    dotnet add code-writer-extensions package SonarAnalyzer.CSharp --version 8.14.0.22654
    ```

9. Created a *[manifest file](https://docs.microsoft.com/en-us/dotnet/core/tools/local-tools-how-to-use)* for .NET Core local tools.

    ```powershell
    dotnet new tool-manifest
    ```

10. Added `code-writer-extensions` project as a dependency of its test project `code-writer-extensions.tests`.

    ```powershell
    dotnet add code-writer-extensions.tests reference code-writer-extensions
    ```
