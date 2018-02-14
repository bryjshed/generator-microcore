# <%= appname %> Service

This project is the <%= appname %> Service. 

## To Test/Run Locally

  * Install [ASP.NET Core RTM](https://www.microsoft.com/net)
  * Clone this repository
  * Open a terminal and navigate to each repository containing `.csproj` file and run the below command.
  * Restore dependencies: `dotnet restore`
  * Move to test project folder (from repository root): `cd test/<%= namespace %>.Tests`
  * Test application: `dotnet test`
  * Move to app project folder (from repository root): `cd src/<%= namespace %>`
  * If you are running this using a local db provider. You will need to create and user with permission to create a database. See `appSettings.json` for localhost connection string.
  * Run application: `dotnet run` or `dotnet watch`
  * Navigate to http://localhost:<%= portNumber %>/swagger/index.html in order to view the swagger docs.
  
The environment can be set either via environmental variables (e.g. `ASPNETCORE_ENVIRONMENT=Staging`) or via the command line (e.g. `dotnet run --environment Development`). Environment-specific config will then be overlayed (e.g. `appsettings.prod.json`).

## To Run in Docker

   * Install [Docker](https://www.docker.com/).
   * Clone this repository.
   * Open terminal and navigate to the repository root (the directory containing the `Dockerfile`).
   * Build container: `docker build -t microcore/mh-<%= appname.toLowerCase() %>-service .`.
   * Start container: `docker run -p <%= portNumber %>:<%= portNumber %> microcore/mh-<%= appname.toLowerCase() %>-service`.
   * Navigate to http://localhost:<%= portNumber %>/swagger/index.html in order to view the swagger docs.

## To Run in Docker with DB support

   * Install [Docker](https://www.docker.com/)
   * Clone this repository
   * Open terminal and navigate to the repository root (the directory containing `src/<%= namespace %>/docker-compose.yml`)
   * Run the command `docker-compose -f docker-compose.yml up --build`.
   * Wait for the build to be ready and then head to http://localhost:<%= portNumber %>/swagger/index.html

## Debugging in Docker
   * Install [Docker](https://www.docker.com/)
   * Clone this repository
   * Create a new task in .vsvode `tasks.json`.
```json
{
    "taskName": "publish",
    "args": [
        "${workspaceRoot}/src/<%= namespace %>/<%= namespace %>.csproj"
    ],
    "isBuildCommand": true,
    "problemMatcher": "$msCompile"
}
``` 
   * Also create a new launch configuration in .vsvode `launch.json`.
   * If you are using a MAC please remove `preLaunchTask` and run the `dotnet publish` separately.
   * Make sure to fill in your aws access key and secret.
```json
{
    "name": ".NET Core Docker Debug",
    "type": "coreclr",
    "request": "launch",
    "preLaunchTask": "publish",
    "cwd": "/app",
    "program": "/app/<%= namespace %>.dll",
    "sourceFileMap": {
        "/app": "${workspaceRoot}/src/<%= namespace %>"
    },
    "env": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "VERSION_NUMBER": "1.1.1",
        "ELASTIC_CONNECTION": "Endpoint=http://elasticsearch1:9200;Username=;Password="
    },
    "pipeTransport": {
        "pipeProgram": "docker",
        "pipeArgs": ["exec", "-i", "mh-requisition-service"],
        "debuggerPath": "/vsdbg/vsdbg",
        "pipeCwd": "${workspaceRoot}",
        "quoteArgs": false
    },
    "launchBrowser": {
        "enabled": true,
        "args": "${auto-detect-url}/swagger/index.html",
        "windows": {
            "command": "cmd.exe",
            "args": "/C start ${auto-detect-url}/swagger/index.html"
        },
        "osx": {
            "command": "open"
        },
        "linux": {
            "command": "xdg-open"
        }
    }
}
``` 
   * This will add a new dropdown option to your vscode debug window called .NET Core Docker Debug.
   * Open terminal and navigate to the repository root (the directory containing the `Dockerfile.debug`)
   * Run the command `docker-compose -f docker-compose.yml -f docker-compose.debug.yml up --build`.
   * Wait until the terminal finishes loading.
   * From the debug window select `.NET Core Docker Debug` and run. 
   * The debugger will attach to the remote docker container.
   * You can view your database volumes using `docker volume ls`.
   * You can also remove the database volume by using `docker volume rm id`.
