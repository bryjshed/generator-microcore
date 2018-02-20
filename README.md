# Yeoman Microcore Generator [![NPM version][npm-image]][npm-url] [![Build Status][travis-image]][travis-url]

This project is a .NET Core generator. This is a scaffolding template engine for creating new microservices and angular 5 + Redux (Coming soon) apps.

## Features

* .NET CORE 2.0 microservice generator boilerplate. 
* Multiple database support including MYSQL, PostgreSQL, SQLServer, and AWS DynamoDB 
* Elasticsearch integration (Coming soon)
* Swagger for api discovery.
* JWT Bearer Authentication (Token Service coming soon with more support for other protocols)
* Kafka integration and approach.
* Dockerfile and docker compose.

## Installation
* Install [ASP.NET Core RTM](https://www.microsoft.com/net) 2.0
* Install [Yeoman](http://yeoman.io) and generator-microcore using [npm](https://www.npmjs.com/) (we assume you have pre-installed [node.js](https://nodejs.org/))
```bash
npm install -g yo
```
```bash
npm install -g generator-microcore
```

## Update
```bash
npm -g update generator-microcore
```

## Running
```bash
yo microcore
```

## Debugging

If you would like to debug this project you can create a `launch.json` file for debugging in VSCode.

* Add a VSCode folder at the top level of the project called `.vscode`. This folder might have already been gerenated.
* Create a new file called `launch.json`.
* You will need to find the path to your yo `cli.js` file. Reference http://yeoman.io/authoring/debugging.html on how to find it.
* Your launch file should look something like below where the location of your cli.js file is replaced with your own.
```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "type": "node",
            "request": "launch",
            "name": "yeoman code",
            "program": "C:\\Users\\<USER>\\AppData\\Roaming\\npm\\node_modules\\yo\\lib\\cli.js",
            "args": [ "microcore" ],
            "cwd": "${workspaceRoot}",
            "console": "integratedTerminal",
            "internalConsoleOptions": "neverOpen"
        }
    ]
}
```
* Now you can attach the debugger to your yo node process by starting the debugger normally in VSCode.
* Clone this repository
* Open terminal and navigate to repository root: `cd [Repository Root]`
* Restore the npm dependencies `npm install`
* Install Microcore Generator locally: `npm link` 
* Create a new folder where you want your new project generated and navigate to it.
* Then generate your new project `yo microcore`

## Helpful Links

 * http://yeoman.io/authoring/index.html
 * https://github.com/SBoudrias/Inquirer.js
 * http://ejs.co/
 * https://github.com/mde/ejs#tags

## License

Apache-2.0 © [MicroCore]()


[npm-image]: https://badge.fury.io/js/generator-microcore.svg
[npm-url]: https://npmjs.org/package/generator-microcore
[travis-image]: https://travis-ci.org/bryjshed/generator-microcore.svg?branch=master
[travis-url]: https://travis-ci.org/bryjshed/generator-microcore
[daviddm-image]: https://david-dm.org/generator-microcore.svg?theme=shields.io
[daviddm-url]: https://david-dm.org/generator-microcore

