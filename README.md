
# JavisWindows

## Project structure (follow MVVM architecture)

****JarvisWindows/**** (Project name folder)\
├── **Assets/** (Resources)\
│ ├── **Images/**\
│ ├── **Icons/**\
│\
├── **Sources/** (Sources code)\
│ ├── **Models/** (Folder for Models)\
│ ├───── AccountModel.cs\
│\
│ ├── **ViewModels/** (Folder for ViewModels)\
│ ├───── MainViewModel.cs\
│\
│ ├── **Views/** (Folder for Views)\
│ ├───── ***Main/***\
│ ├──────── MainWindows.xaml\
│ ├──────── MainWindows.xaml.cs\
│\
│ ├── **Commands/** (Handling command from UI components)\
│ ├───── RelayCommand.cs\
│\
│ ├── **DataAccess/** (Folder for API data handling)\
│ ├───── **Envs/** (Store environments configuration)\
│ ├──────── `settings.dev.json`\
│ ├──────── `settings.product.json`\
│ ├───── **Local/** (Storage accessToken, refreshToken, language, etc)\
│ ├───── **Network/** (Call data API)\
│ ├──────── ApiCaller.cs\
│ ├── `DataConfiguration.cs (Configuration ApiUrl)`\
│\
├── ****Utils/**** (Folder for utilities)\
│ ├── **NativeWindows/** (Native handling)\
│ ├──────── WindowsInjection.cs (Native Windows API)\
│ ├── **Converter/** (Convert value of UI components, Convert response to models, etc)\
│\
│ ├── App.xaml (Application entry point UI)\
│ ├── App.xaml.cs (Application entry point code-behind)\
│ ├── `JarvisWindows.csproj (Project file)`

## How to build the Project
### Prerequisites

Before you begin, make sure you have the following set up:
- [Visual Studio](https://visualstudio.microsoft.com/) or your preferred C# development environment.
- [.NET Framework](https://dotnet.microsoft.com/download/dotnet-framework) (if not using .NET Core or .NET 5+).

### Steps
1. **Clone or Download:** Clone this repository to your local machine using Git or download it as a ZIP file.

2. **Open Your WPF Project:**
- Launch Visual Studio 2022 and open your existing WPF project or create a new one.

3. **Access NuGet Package Manager:**
- In Visual Studio, go to the "Tools" menu.
- Select "NuGet Package Manager" and then "Manage NuGet Packages for Solution."

4. **Browse for Packages:**
- In the NuGet Package Manager, you'll see three tabs: "Browse," "Installed," and "Updates."
- Click on the "Browse" tab to search for packages.

5. **Search for Packages:**
- In the "Browse" tab, you can search for the packages you want to install. Type the following package names one by one into the search bar and press Enter:
     - `MaterialDesignColors`
     - `MaterialDesignThemes`
     - `Microsoft.Extentions.Configuration`
     - `Microsoft.Extentions.Configuration.Json`

5. **Select and Install Packages:**
- Click on each package in the search results, and then click the "Install" button to add them to your project.
- Follow the on-screen instructions to complete the installation process.

6. **Configure build environments:**
- Add more build target:
   - Right-click on your solution in Visual Studio and select "Configuration Manager" 
   - Add 2 targets: `dev` and `product`.
- Project Configuration:
   - Right-click on your project in Visual Studio and select "Properties."
   - In the project properties, navigate to the "Build" tab.
- Define Configuration Constants:
   - In the "Conditional compilation symbols" textbox, define constants for each build configuration, such as DEBUG, RELEASE, DEV, and PRODUCT. For the dev configuration, add DEV.

7. **Build and Run:**
- Build your WPF project to ensure that the packages are correctly referenced.
- `Chooce the target Build Configuration` and Run your project to make use of the added packages in your application.

