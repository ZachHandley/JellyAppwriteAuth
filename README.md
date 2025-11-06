AppwriteAuth for Jellyfin
=========================

AppwriteAuth is a lightweight authentication plugin for Jellyfin that delegates login to Appwrite while keeping all Jellyfin URLs and UI unchanged. It also provides admin tools to invite users and reset passwords via Appwrite Messaging (or SMTP fallback), with clean, branded HTML emails.

Author: Zach Handley (Black Leaf Digital) — zach@blackleafdigital.com

Features

- Appwrite-backed login (email or username) with immediate Appwrite session deletion.
- Jellyfin user name mirrors login input: email-login → email, username-login → username.
- Optional “mark email verified on login” (Users.UpdateEmailVerification).
- Admin Actions in plugin page: Send Invite, Send Reset, Send Test.
- Appwrite Messaging: ensures a single SMTP provider ($id = jellyfin_smtp) from your settings, then sends HTML emails. Fallback to SMTP if needed.
- Branding: logo (URL or local path), primary color, brand name, login URL, and fully editable HTML templates.

Requirements

- Jellyfin 10.11.x
- Appwrite Endpoint and Project ID
- Appwrite API Key (required for Messaging + user create/reset + username-login email resolution)
- SMTP credentials (used to back the Appwrite Messaging SMTP provider and as fallback)

Install (Repository)

1) In Jellyfin, go to: Dashboard → Plugins → Repositories → Add
2) Enter: https://zachhandley.github.io/JellyAppwriteAuth/manifest.json
3) Save. Then navigate to Plugins → Catalog and install “AppwriteAuth”.

Install (Manual)

1) Build and publish: `dotnet publish Jellyfin.Plugin.Template/Jellyfin.Plugin.Template.csproj -c Release -o publish/`
2) Copy contents of `publish/` to your Jellyfin plugins directory, e.g. `/var/lib/jellyfin/plugins/AppwriteAuth/`
3) Restart Jellyfin.

Configure

- Appwrite: Endpoint, Project ID, API Key
- Email Provider: Appwrite Messaging (default) or SMTP
- SMTP: Host, Port, Username, Password, From, SSL
- Branding: Brand Name, Logo URL or Server Path (default: https://appwrite.io/logo.svg), Primary Color, Login URL
- Templates: Invite/Reset HTML + Subjects
- Option: Mark Appwrite email verified on successful login

Admin Actions

- Test Email: Validate configuration and templates (creates provider jellyfin_smtp if missing).
- Invite Email: Creates the user (best-effort) and sends a branded invitation.
- Reset Password Email: Updates password (best-effort) and sends a branded reset email.

How It Works

- Login: We validate via Appwrite (CreateEmailPasswordSession) and immediately delete the Appwrite session; Jellyfin manages the user session.
- User mapping: On first login, we create/find a local Jellyfin user using the login input (email or username) as the Jellyfin Name.
- Messaging: We ensure or create an SMTP provider in Appwrite with id jellyfin_smtp from your SMTP settings, then send HTML emails through Messaging; fallback to local SMTP if Messaging fails.

Build

- `dotnet build Jellyfin.Plugin.Template/Jellyfin.Plugin.Template.csproj -c Release`
- For release packaging, see `.github/workflows/publish.yml` (publishes ZIP + manifest.json to GitHub Pages).

Support / Contact

- Zach Handley — zach@blackleafdigital.com
- Issues and PRs welcome.

License

- Based on the Jellyfin plugin template and follows its licensing model. See LICENSE.
There are loads of other interfaces that can be used, but you'll need to poke around the API to get some info. If you're an expert on a particular interface, you should help [contribute some documentation](https://docs.jellyfin.org/general/contributing/index.html)!

### 4b. Use plugin aimed interfaces to add custom functionality

If your plugin doesn't fit perfectly neatly into a predefined interface, never fear, there are a set of interfaces and classes that allow your plugin to extend Jellyfin any which way you please. Here's a quick overview on how to use them

- **IPluginConfigurationPage** - Allows you to have a plugin config page on the dashboard. If you used one of the quickstart example projects, a premade page with some useful components to work with has been created for you! If not you can check out this guide here for how to whip one up.

 **IPluginServiceRegistrator** - Will be located by Jellyfin at server startup and allows you to add services to the DI container to allow for injection in your plugin's classes later.

- **IHostedService** - Allows you to run code as a background task that will be started at program startup and will remain in memory. See [Microsoft's documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio#ihostedservice-interface) for more information. You can make as many of these as you need; make Jellyfin aware of them with an `IPluginServiceRegistrator`. It is wildly useful for loading configs or persisting state. **Be aware that your main plugin class (IBasePlugin) cannot also be a IHostedService.**

- **ControllerBase** - Allows you to define custom REST-API endpoints. This is the default ASP.NET Web-API controller. You can use it exactly as you would in a normal Web-API project. Learn more about it [here](https://docs.microsoft.com/aspnet/core/web-api/?view=aspnetcore-5.0).

Likewise you might need to get data and services from the Jellyfin core, Jellyfin provides a number of interfaces you can add as parameters to your plugin constructor which are then made available in your project (you can see the 2 mandatory ones that are needed by the plugin system in the constructor as is).

- **IBlurayExaminer** - Allows you to examine blu-ray folders
- **IDtoService** - Allows you to create data transport objects, presumably to send to other plugins or to the core
- **ILibraryManager** - Allows you to directly access the media libraries without hopping through the API
- **ILocalizationManager** - Allows you tap into the main localization engine which governs translations, rating systems, units etc...
- **INetworkManager** - Allows you to get information about the server's networking status
- **IServerApplicationPaths** - Allows you to get the running server's paths
- **IServerConfigurationManager** - Allows you to write or read server configuration data into the application paths
- **ITaskManager** - Allows you to execute and manipulate scheduled tasks
- **IUserManager** - Allows you to retrieve user info and user library related info
- **IXmlSerializer** - Allows you to use the main xml serializer
- **IZipClient** - Allows you to use the core zip client for compressing and decompressing data

## 5. Create a Repository

- [See blog post](https://jellyfin.org/posts/plugin-updates/)

## 6. Set Up Debugging

Debugging can be set up by creating tasks which will be executed when running the plugin project. The specifics on setting up these tasks are not included as they may differ from IDE to IDE. The following list describes the general process:

- Compile the plugin in debug mode.
- Create the plugin directory if it doesn't exist.
- Copy the plugin into your server's plugin directory. The server will then execute it.
- Make sure to set the working directory of the program being debugged to the working directory of the Jellyfin Server.
- Start the server.

Some IDEs like Visual Studio Code may need the following compile flags to compile the plugin:

```shell
dotnet build Your-Plugin.sln /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary
```

These flags generate the full paths for file names and **do not** generate a summary during the build process as this may lead to duplicate errors in the problem panel of your IDE.

### 6.a Set Up Debugging on Visual Studio

Visual Studio allows developers to connect to other processes and debug them, setting breakpoints and inspecting the variables of the program. We can set this up following this steps:
On this section we will explain how to set up our solution to enable debugging before the server starts.

1. Right-click on the solution, And click on Add -> Existing Project...
2. Locate Jellyfin executable in your installation folder and click on 'Open'. It is called `Jellyfin.exe`. Now The solution will have a new "Project" called Jellyfin. This is the executable, not the source code of Jellyfin.
3. Right-click on this new project and click on 'Set up as Startup Project'
4. Right-click on this new project and click on 'Properties'
5. Make sure that the 'Attach' parameter is set to 'No'

From now on, everytime you click on start from Visual Studio, it will start Jellyfin attached to the debugger!

The only thing left to do is to compile the project as it is specified a few lines above and you are done.

### 6.b Automate the Setup on Visual Studio Code

Visual Studio Code allows developers to automate the process of starting all necessary dependencies to start debugging the plugin. This guide assumes the reader is familiar with the [documentation on debugging in Visual Studio Code](https://code.visualstudio.com/docs/editor/debugging) and has read the documentation in this file. It is assumed that the Jellyfin Server has already been compiled once. However, should one desire to automatically compile the server before the start of the debugging session, this can be easily implemented, but is not further discussed here.

A full example, which aims to be portable may be found in this repo's `.vscode` folder.

This example expects you to clone `jellyfin`, `jellyfin-web` and `jellyfin-plugin-template` under the same parent directory, though you can customize this in `settings.json`

1. Create a `settings.json` file inside your `.vscode` folder, to specify common options specific to your local setup.
   ```jsonc
    {
        // jellyfinDir : The directory of the cloned jellyfin server project
        // This needs to be built once before it can be used
        "jellyfinDir"     : "${workspaceFolder}/../jellyfin/Jellyfin.Server",
        // jellyfinWebDir : The directory of the cloned jellyfin-web project
        // This needs to be built once before it can be used
        "jellyfinWebDir"  : "${workspaceFolder}/../jellyfin-web",
        // jellyfinDataDir : the root data directory for a running jellyfin instance
        // This is where jellyfin stores its configs, plugins, metadata etc
        // This is platform specific by default, but on Windows defaults to
        // ${env:LOCALAPPDATA}/jellyfin
        "jellyfinDataDir" : "${env:LOCALAPPDATA}/jellyfin",
        // The name of the plugin
        "pluginName" : "Jellyfin.Plugin.Template",
    }
   ```

1. To automate the launch process, create a new `launch.json` file for C# projects inside the `.vscode` folder. The example below shows only the relevant parts of the file. Adjustments to your specific setup and operating system may be required.

   ```jsonc
    {
        // Paths and plugin names are configured in settings.json
        "version": "0.2.0",
        "configurations": [
            {
                "type": "coreclr",
                "name": "Launch",
                "request": "launch",
                "preLaunchTask": "build-and-copy",
                "program": "${config:jellyfinDir}/bin/Debug/net8.0/jellyfin.dll",
                "args": [
                //"--nowebclient"
                "--webdir",
                "${config:jellyfinWebDir}/dist/"
                ],
                "cwd": "${config:jellyfinDir}",
            }
        ]
    }

   ```

   The `request` type is specified as `launch`, as this `launch.json` file will start the Jellyfin Server process. The `preLaunchTask` defines a task that will run before the Jellyfin Server starts. More on this later. It is important to set the `program` path to the Jellyin Server program and set the current working directory (`cwd`) to the working directory of the Jellyfin Server.
   The `args` option allows to specify arguments to be passed to the server, e.g. whether Jellyfin should start with the web-client or without it.

2. Create a `tasks.json` file inside your `.vscode` folder and specify a `build-and-copy` task that will run in `sequence` order. This tasks depends on multiple other tasks and all of those other tasks can be defined as simple `shell` tasks that run commands like the `cp` command to copy a file. The sequence to run those tasks in is given below. Please note that it might be necessary to adjust the examples for your specific setup and operating system.

   The full file is shown here - Specific sections will be discussed in depth
    ```jsonc
    {
        // Paths and plugin name are configured in settings.json
        "version": "2.0.0",
        "tasks": [
            {
            // A chain task - build the plugin, then copy it to your
            // jellyfin server's plugin directory
            "label": "build-and-copy",
            "dependsOrder": "sequence",
            "dependsOn": ["build", "make-plugin-dir", "copy-dll"]
            },
            {
            // Build the plugin
            "label": "build",
            "command": "dotnet",
            "type": "shell",
            "args": [
                "publish",
                "${workspaceFolder}/${config:pluginName}.sln",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "group": "build",
            "presentation": {
                "reveal": "silent"
            },
            "problemMatcher": "$msCompile"
            },
            {
                // Ensure the plugin directory exists before trying to use it
                "label": "make-plugin-dir",
                "type": "shell",
                "command": "mkdir",
                "args": [
                "-Force",
                "-Path",
                "${config:jellyfinDataDir}/plugins/${config:pluginName}/"
                ]
            },
            {
                // Copy the plugin dll to the jellyfin plugin install path
                // This command copies every .dll from the build directory to the plugin dir
                // Usually, you probablly only need ${config:pluginName}.dll
                // But some plugins may bundle extra requirements
                "label": "copy-dll",
                "type": "shell",
                "command": "cp",
                "args": [
                "./${config:pluginName}/bin/Debug/net8.0/publish/*",
                "${config:jellyfinDataDir}/plugins/${config:pluginName}/"
                ]

            },
        ]
    }

    ```
    1.  The "build-and-copy" task which triggers all of the other tasks
    ```jsonc
        {
        // A chain task - build the plugin, then copy it to your
        // jellyfin server's plugin directory
        "label": "build-and-copy",
        "dependsOrder": "sequence",
        "dependsOn": ["build", "make-plugin-dir", "copy-dll"]
        },
    ```
    2.  A build task. This task builds the plugin without generating summary, but with full paths for file names enabled.

        ```jsonc
            {
            // Build the plugin
            "label": "build",
            "command": "dotnet",
            "type": "shell",
            "args": [
                "publish",
                "${workspaceFolder}/${config:pluginName}.sln",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "group": "build",
            "presentation": {
                "reveal": "silent"
            },
            "problemMatcher": "$msCompile"
            },
        ```

    3.  A tasks which creates the necessary plugin directory and a sub-folder for the specific plugin. The plugin directory is located below the [data directory](https://jellyfin.org/docs/general/administration/configuration.html) of the Jellyfin Server. As an example, the following path can be used for the bookshelf plugin: `$HOME/.local/share/jellyfin/plugins/Bookshelf/`
        ```jsonc
            {
                // Ensure the plugin directory exists before trying to use it
                "label": "make-plugin-dir",
                "type": "shell",
                "command": "mkdir",
                "args": [
                "-Force",
                "-Path",
                "${config:jellyfinDataDir}/plugins/${config:pluginName}/"
                ]
            },
        ```

    4.  A tasks which copies the plugin dll which has been built in step 2.1. The file is copied into it's specific plugin directory within the server's plugin directory.

        ```jsonc
            {
                // Copy the plugin dll to the jellyfin plugin install path
                // This command copies every .dll from the build directory to the plugin dir
                // Usually, you probablly only need ${config:pluginName}.dll
                // But some plugins may bundle extra requirements
                "label": "copy-dll",
                "type": "shell",
                "command": "cp",
                "args": [
                "./${config:pluginName}/bin/Debug/net8.0/publish/*",
                "${config:jellyfinDataDir}/plugins/${config:pluginName}/"
                ]
            },
        ```

## Licensing

Licensing is a complex topic. This repository features a GPLv3 license template that can be used to provide a good default license for your plugin. You may alter this if you like, but if you do a permissive license must be chosen.

Due to how plugins in Jellyfin work, when your plugin is compiled into a binary, it will link against the various Jellyfin binary NuGet packages. These packages are licensed under the GPLv3. Thus, due to the nature and restrictions of the GPL, the binary plugin you get will also be licensed under the GPLv3.

If you accept the default GPLv3 license from this template, all will be good. However if you choose a different license, please keep this fact in mind, as it might not always be obvious that an, e.g. MIT-licensed plugin would become GPLv3 when compiled.

Please note that this also means making "proprietary", source-unavailable, or otherwise "hidden" plugins for public consumption is not permitted. To build a Jellyfin plugin for distribution to others, it must be under the GPLv3 or a permissive open-source license that can be linked against the GPLv3.

## AppwriteAuth: Lightweight Appwrite wrapper for Jellyfin login

This plugin keeps Jellyfin URLs and UI unchanged and delegates authentication to Appwrite.

- Configure Appwrite via environment variables (preferred):
  - `APPWRITE_ENDPOINT` (e.g., `https://cloud.appwrite.io/v1`)
  - `APPWRITE_PROJECT_ID`
  - `APPWRITE_API_KEY` (optional, for server-side team lookups)

- Or via the plugin settings page (overridden by ENV when set):
  - Appwrite Endpoint
  - Appwrite Project ID
  - Appwrite API Key (optional)
  - Team → Role Mapping (JSON)

### Flow

1) User submits Jellyfin’s standard login form.
2) The plugin validates credentials against Appwrite.
3) Optionally map Appwrite Teams to Jellyfin roles/policies (using `TeamToRoleMapJson`).
4) If valid, Jellyfin creates/updates the local user and proceeds as normal.

### Code

- `Jellyfin.Plugin.Template/Configuration/PluginConfiguration.cs`: Adds Appwrite settings.
- `Jellyfin.Plugin.Template/Configuration/configPage.html`: Admin UI for settings with ENV override note.
- `Jellyfin.Plugin.Template/Appwrite/AppwriteClientFactory.cs`: Creates an Appwrite `Client` using ENV → config fallback.
- `Jellyfin.Plugin.Template/Appwrite/AppwriteAuthService.cs`: Minimal credential validation against Appwrite.

### Next step (Auth provider)

Implement Jellyfin’s authentication provider and delegate to `AppwriteAuthService`. The exact interface methods can vary by Jellyfin version; use the interfaces from `MediaBrowser.Controller.Authentication` for the Jellyfin version referenced in the `.csproj` (10.9.11). A typical flow:

- Receive username/password in the provider.
- Call `AppwriteAuthService.ValidateCredentialsAsync`.
- On success, create/update a Jellyfin user and map teams to roles.

This approach remains a super lightweight wrapper over Jellyfin’s existing login and routes.
