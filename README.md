
# Jarvis Windows Application

## How to build
### Prerequisites

Before you begin, make sure you have the following set up:
- [Visual Studio](https://visualstudio.microsoft.com/) or your preferred C# development environment.
- [.NET Framework](https://dotnet.microsoft.com/download/dotnet-framework) (if not using .NET Core or .NET 5+).

### Steps
1. **Clone or Download:** 
- Use `Git Clone Command`.
- Or download the repositoty as a ZIP file.

2. **Access NuGet Package Manager:**
- In Visual Studio, go to:\
    `Tools` -> `NuGet Package Manager` -> `Manage NuGet Packages for Solution`

3. **Browse for Packages:**
- In the `NuGet Package Manager`, click on the `Browse` tab.

4. **Search and install Packages:**
- In the `Browse` tab, search and install some packages:
     - `MaterialDesignColors`
     - `MaterialDesignThemes`
     - `Microsoft.Extentions.Configuration`
     - `Microsoft.Extentions.Configuration.Json`

5. **Configure build environments:**
- Add more build targets:
   - Right-click on your solution and select `Configuration Manager`. 
   - Add 2 build targets: `dev` and `product`.
- Project Configuration:
   - Right-click on your project and select `Properties`.
   - Navigate to the `Build` tab.
   - In the `Conditional compilation symbols` textbox, define constants for each build configuration.
        - For the `dev` configuration add `DEV`.
        - For the `product` configuration, add `PRODUCT`.
        - For the `debug` configuration, add `DEBUG`.
        - For the `release` configuration, add `RELEASE`.

6. **Build and Run:**
- Build `(Ctrl + B)` the project to ensure that the packages are correctly referenced.
- Choose the target Build Configuration `(dev, product, ...)` and Run`(F5)` the project.

## Rules
- When developing, please follow this [Coding Convention](https://hoangtruong.atlassian.net/wiki/spaces/Jarvis/pages/623144/Jarvis+Windows+Coding+Convention).

