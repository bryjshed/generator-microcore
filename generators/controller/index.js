'use strict';
const BaseGenerator = require('../generator-base');
const Generator = require('yeoman-generator');

module.exports = BaseGenerator.extend({
  constructor: function () {
    Generator.apply(this, arguments);
    this.argument('namespace', {
      type: String,
      required: true
    });
    this.argument('appname', {
      type: String,
      required: true
    });
    this.argument('controllerName', {
      type: String,
      required: true
    });
    this.argument('addDatabase', {
      type: Boolean,
      required: true
    });
    this.argument('database', {
      type: String,
      required: true
    });
    this.argument('createModel', {
      type: Boolean,
      required: true
    });
    this.argument('modelName', {
      type: String,
      required: true
    });
    this.argument('authentication', {
      type: Boolean,
      required: true
    });
  },
  configuring: function () {
    this.templateOptions = this._createTemplateOptions();
    this.targetFolderPath = this.options.targetFolderPath ? this.options.targetFolderPath + "/" : "";
    this.targetTestFolderPath = this.options.targetTestFolderPath ? this.options.targetTestFolderPath + "/" : "";
  },

  writing: function () {
    if (this.templateOptions.addDatabase && this.templateOptions.createModel) {
      this.fs.copyTpl(this.templatePath('src/TemplateControllerServiceModel.cs'), this.destinationPath(this.targetFolderPath + 'Controllers/' + this.templateOptions.controllerName + 'Controller.cs'), this.templateOptions);
      
      //Tests
      this.fs.copyTpl(this.templatePath('test/TemplateContractTest.cs'), this.destinationPath(this.targetTestFolderPath + this.templateOptions.controllerName + 'ContractTest.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('test/TemplateControllerServiceModelTest.cs'), this.destinationPath(this.targetTestFolderPath + this.templateOptions.controllerName + 'ControllerTest.cs'), this.templateOptions);
    } else {
      this.fs.copyTpl(this.templatePath('src/TemplateControllerNoModel.cs'), this.destinationPath(this.targetFolderPath + 'Controllers/'  + this.templateOptions.controllerName + 'Controller.cs'), this.templateOptions);
    }
  },

  _createTemplateOptions: function () {
    const options = {
      namespace: this.options.namespace,
      appname: this.options.appname,
      controllerName: this.options.controllerName,
      addDatabase: this.options.addDatabase,
      database: this.options.database,
      createModel: this.options.createModel,
      modelName: this.options.modelName,
      authentication: this.options.authentication,
      modelNameCamel: this.toCamelCase(this.options.modelName),
      idType: this.options.addDatabase && this.options.database === "dynamodb" ? "string" : "long"
    };
    return options;
  }

});
