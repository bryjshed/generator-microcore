'use strict';
const mkdirp = require('mkdirp');
const BaseGenerator = require('../generator-base');
const Generator = require('yeoman-generator');
const chalk = require('chalk');
const process = require('process');
const os = require('os');
const guid = require('uuid');
const prompts = require('./prompts');

module.exports = BaseGenerator.extend({
  constructor: function () {
    Generator.apply(this, arguments);
    this.argument('username', {
      type: String,
      required: true
    });
    this.argument('email', {
      type: String,
      required: true
    });
  },

  initializing: function () {
    if (this.options.displayLogo !== false) {
      this.printLogo();
    }
  },

  prompting: {
    intialPrompt: prompts.intialPrompt,
    serviceRetry: prompts.servicePrompt,
    kafkaPrompt: prompts.kafkaPrompt,
    finalPrompt: prompts.finalPrompt
  },

  configuring: function () {
    // This is the check if the service already exists

    if (this.props.projectExist === false) {
      this.env.error('Canceling out of core generator. This is not an error.');
    }
    this.templateOptions = this._createTemplateOptions();
    this.targetFolderPath = this.createSrcFolderPath(this.templateOptions.namespace);
    this.targetTestFolderPath = this.createTestFolderPath(this.templateOptions.namespace);
    mkdirp.sync(this.targetFolderPath);
    mkdirp.sync(this.targetTestFolderPath);
  },

  writing: function () {
    // Readme

    this.fs.copyTpl(this.templatePath('README.md'), this.destinationPath('README.md'), this.templateOptions);

    // Src project files

    this.fs.copyTpl(this.templatePath('src/TemplateProjectFile.csproj'), this.destinationPath(this.targetFolderPath + '/' + this.templateOptions.namespace + '.csproj'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Startup.cs'), this.destinationPath(this.targetFolderPath + '/Startup.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Program.cs'), this.destinationPath(this.targetFolderPath + '/Program.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/appsettings.json'), this.destinationPath(this.targetFolderPath + '/appsettings.json'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/appsettings.prod.json'), this.destinationPath(this.targetFolderPath + '/appsettings.prod.json'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/appsettings.dev.json'), this.destinationPath(this.targetFolderPath + '/appsettings.dev.json'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/appsettings.qa.json'), this.destinationPath(this.targetFolderPath + '/appsettings.qa.json'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/appsettings.staging.json'), this.destinationPath(this.targetFolderPath + '/appsettings.staging.json'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/appsettings.hotfix.json'), this.destinationPath(this.targetFolderPath + '/appsettings.hotfix.json'), this.templateOptions);

    this.fs.copyTpl(this.templatePath('src/Dockerfile'), this.destinationPath(this.targetFolderPath + '/Dockerfile'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/docker-compose.debug.yml'), this.destinationPath(this.targetFolderPath + '/docker-compose.debug.yml'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/docker-compose.yml'), this.destinationPath(this.targetFolderPath + '/docker-compose.yml'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Dockerfile.debug'), this.destinationPath(this.targetFolderPath + '/Dockerfile.debug'), this.templateOptions);

    // Services

    if (this.templateOptions.kafkaConsumer) {
      this.fs.copyTpl(this.templatePath('src/Services/External/ServiceResponse.cs'), this.destinationPath(this.targetFolderPath + '/Services/External/ServiceResponse.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('src/Services/External/BaseService.cs'), this.destinationPath(this.targetFolderPath + '/Services/External/BaseService.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('src/Exceptions/ServiceUnavailableException.cs'), this.destinationPath(this.targetFolderPath + '/Exceptions/ServiceUnavailableException.cs'), this.templateOptions);
    }

    this.fs.copyTpl(this.templatePath('src/Services/ITemplateService.cs'), this.destinationPath(this.targetFolderPath + '/Services/I' + this.templateOptions.appname + 'Service.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Services/TemplateService.cs'), this.destinationPath(this.targetFolderPath + '/Services/' + this.templateOptions.appname + 'Service.cs'), this.templateOptions);

    // Exceptions

    this.fs.copyTpl(this.templatePath('src/Exceptions/NotFoundException.cs'), this.destinationPath(this.targetFolderPath + '/Exceptions/NotFoundException.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Exceptions/ServiceException.cs'), this.destinationPath(this.targetFolderPath + '/Exceptions/ServiceException.cs'), this.templateOptions);

    this.fs.copyTpl(this.templatePath('src/Config/AppSettings.cs'), this.destinationPath(this.targetFolderPath + '/Config/AppSettings.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Config/GlobalExceptionFilter.cs'), this.destinationPath(this.targetFolderPath + '/Config/GlobalExceptionFilter.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Config/Constants.cs'), this.destinationPath(this.targetFolderPath + '/Config/Constants.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Extensions/MvcExtensions.cs'), this.destinationPath(this.targetFolderPath + '/Extensions/MvcExtensions.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Extensions/StringExtensions.cs'), this.destinationPath(this.targetFolderPath + '/Extensions/StringExtensions.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Extensions/DateTimeExtensions.cs'), this.destinationPath(this.targetFolderPath + '/Extensions/DateTimeExtensions.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/ActionFilter/ValidateModelStateAttribute.cs'), this.destinationPath(this.targetFolderPath + '/ActionFilter/ValidateModelStateAttribute.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Mappings/MappingProfile.cs'), this.destinationPath(this.targetFolderPath + '/Mappings/MappingProfile.cs'), this.templateOptions);
    if (this.templateOptions.vscode) {
      this.fs.copyTpl(this.templatePath('vscode/launch.json'), this.destinationPath('.vscode/launch.json'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('vscode/tasks.json'), this.destinationPath('.vscode/tasks.json'), this.templateOptions);
    }

    // Tests

    this.fs.copyTpl(this.templatePath('test/TemplateServiceTest.cs'), this.destinationPath(this.targetTestFolderPath + '/' + this.templateOptions.appname + 'ServiceTest.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('test/TemplateProjectFile.csproj'), this.destinationPath(this.targetTestFolderPath + '/' + this.templateOptions.namespace + '.Tests.csproj'), this.templateOptions);

    this.fs.copyTpl(this.templatePath('test/TestStartup.cs'), this.destinationPath(this.targetTestFolderPath + '/TestStartup.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('test/ValidateModelStateAttributeTest.cs'), this.destinationPath(this.targetTestFolderPath + '/ValidateModelStateAttributeTest.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/appsettings.json'), this.destinationPath(this.targetTestFolderPath + '/appsettings.json'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('test/Fixtures/TestServerFixture.cs'), this.destinationPath(this.targetTestFolderPath + '/Fixtures/TestServerFixture.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('test/HttpContentExtensions.cs'), this.destinationPath(this.targetTestFolderPath + '/HttpContentExtensions.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('test/Utils/BogusData.cs'), this.destinationPath(this.targetTestFolderPath + '/Utils/BogusData.cs'), this.templateOptions);

    if (this.templateOptions.createModel) {
      this.fs.copyTpl(this.templatePath('test/MappingTest.cs'), this.destinationPath(this.targetTestFolderPath + '/MappingTest.cs'), this.templateOptions);
    }
    if (this.templateOptions.kafkaConsumer) {
      this.fs.copyTpl(this.templatePath('test/Stubs/BaseServiceStub.cs'), this.destinationPath(this.targetTestFolderPath + '/Stubs/BaseServiceStub.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('test/BaseServiceTest.cs'), this.destinationPath(this.targetTestFolderPath + '/BaseServiceTest.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('test/Handlers/FakeResponseHandler.cs'), this.destinationPath(this.targetTestFolderPath + '/Handlers/FakeResponseHandler.cs'), this.templateOptions);
    }

    const rds = this.templateOptions.addDatabase && this.templateOptions.database !== 'dynamodb' ? true : false;

    if (rds) {
      this.composeWith('microcore:entityframework', {
        targetFolderPath: this.targetFolderPath,
        targetTestFolderPath: this.targetTestFolderPath,
        arguments: [
          this.templateOptions.namespace,
          this.templateOptions.appname,
          this.templateOptions.createModel,
          this.templateOptions.modelName,
        ]
      });
    } else if (this.templateOptions.database === 'dynamodb') {
      this.composeWith('microcore:dynamodb', {
        targetFolderPath: this.targetFolderPath,
        targetTestFolderPath: this.targetTestFolderPath,
        arguments: [
          this.templateOptions.namespace,
          this.templateOptions.appname,
          this.templateOptions.createModel,
          this.templateOptions.modelName
        ]
      });
    }
    if (this.templateOptions.createController) {
      this.composeWith('microcore:controller', {
        targetFolderPath: this.targetFolderPath,
        targetTestFolderPath: this.targetTestFolderPath,
        arguments: [
          this.templateOptions.namespace,
          this.templateOptions.appname,
          this.templateOptions.controllerName,
          this.templateOptions.addDatabase,
          this.templateOptions.database,
          this.templateOptions.createModel,
          this.templateOptions.modelName,
          this.templateOptions.authentication
        ]
      });
    }
    if (this.templateOptions.createService) {
      for (let i = 0; i < this.templateOptions.externalServices.length; i++) {
        const extService = this.templateOptions.externalServices[i];
        this.composeWith('microcore:service', {
          targetFolderPath: this.targetFolderPath,
          targetTestFolderPath: this.targetTestFolderPath,
          arguments: [
            this.templateOptions.namespace,
            this.templateOptions.appname,
            extService.serviceName,
            extService.serviceDtoName
          ]
        });
      }
    }
    if (this.templateOptions.kafka) {

      this.composeWith('microcore:kafka', {
        targetFolderPath: this.targetFolderPath,
        targetTestFolderPath: this.targetTestFolderPath,
        arguments: [
          this.templateOptions.namespace,
          this.templateOptions.appname,
          this.templateOptions.kafkaConsumer,
          this.templateOptions.kafkaConsumerTopics
        ]
      });
    }
    if (this.templateOptions.authentication) {
      this.composeWith('microcore:auth', {
        targetFolderPath: this.targetFolderPath,
        targetTestFolderPath: this.targetTestFolderPath,
        arguments: [
          this.templateOptions.namespace,
          this.templateOptions.appname
        ]
      });
    }
  },

  install: function () {
    let sudoCmd = '';
    // Run command in privleged mode on OSX

    if (os.platform() === 'darwin') {
      sudoCmd = 'sudo ';
    }
    var exec = require('child_process').exec;
    var project = this.destinationPath();

    this.log(chalk.green.bold('Running: dotnet restore ' + this.targetFolderPath + '...'));
    process.chdir(this.destinationPath(this.targetFolderPath));
    var restoreProcess = exec(sudoCmd + 'dotnet restore', (error, stdout, stderr) => {
      process.chdir(this.destinationPath(this.targetTestFolderPath));
      this.log(chalk.green.bold('Running: dotnet restore ' + this.targetTestFolderPath + '...'));
      var restoreTestProcess = exec(sudoCmd + 'dotnet restore', (trerror, trstdout, trstderr) => {
        this.log(chalk.green.bold('Running: dotnet test...'));
        var testProsses = exec(sudoCmd + 'dotnet test', (terror, tstdout, tstderr) => {
          if (terror !== null) {
            this.log(terror);
          } else {
            // If vscode files selected, be a nice guy and open it for them.

            if (this.templateOptions.vscode) {
              process.chdir(project);
              exec(sudoCmd + 'code .', (verror, vstdout, vstderr) => {});
            }
          }
        });
        testProsses.stdout.pipe(process.stdout);
      });
      restoreTestProcess.stdout.pipe(process.stdout);
    });
    restoreProcess.stdout.pipe(process.stdout);
  },

  _createTemplateOptions: function () {
    const options = {
      namespace: this.props.namespace,
      appname: this.props.appname,
      createController: this.props.createController,
      controllerName: this.props.controllerName,
      addDatabase: this.props.addDatabase,
      database: this.props.database,
      createModel: this.props.createModel,
      modelNameCamel: this.toCamelCase(this.props.modelName),
      modelName: this.props.modelName,
      createService: this.props.createService,
      externalServices: this.props.externalServices,
      elasticSearch: this.props.elasticSearch,
      kafka: this.props.kafka,
      kafkaConsumer: this.props.kafkaConsumer,
      kafkaConsumerTopics: this.props.kafkaConsumerTopics,
      authentication: true,
      portNumber: this.props.portNumber,
      vscode: this.props.vscode,
      username: this.options.username,
      email: this.options.email,
      guid: guid.v4(),
      idType: this.props.addDatabase && this.props.database === 'dynamodb' ? 'string' : 'long'
    };

    if (this.props.kafkaConsumerTopics) {
      const topics = this.props.kafkaConsumerTopics;
      let topicConcat = '';
      for (let i = 0; i < topics.length; i++) {
        topicConcat += '"' + topics[i] + '", ';
      }
      options.kafkaConsumerTopicsConcat = topicConcat.slice(0, -2);
    }

    return options;
  }
});
