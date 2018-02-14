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
  },

  initializing: function () {
    this.templateOptions = this._createTemplateOptions();
    this.targetFolderPath = this.options.targetFolderPath ? this.options.targetFolderPath + "/" : "";
    this.targetTestFolderPath = this.options.targetTestFolderPath ? this.options.targetTestFolderPath + "/" : "";
  },

  writing: function () {
    this.fs.copyTpl(this.templatePath('src/Auth/ApplicationUser.cs'), this.destinationPath(this.targetFolderPath + 'Auth/ApplicationUser.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Auth/AuthExtensions.cs'), this.destinationPath(this.targetFolderPath + 'Auth/AuthExtensions.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Auth/ClaimTypes.cs'), this.destinationPath(this.targetFolderPath + 'Auth/ClaimTypes.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Auth/GroupTypes.cs'), this.destinationPath(this.targetFolderPath + 'Auth/GroupTypes.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Auth/JwtConfig.cs'), this.destinationPath(this.targetFolderPath + 'Auth/JwtConfig.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Exceptions/NotAuthorizedException.cs'), this.destinationPath(this.targetFolderPath + 'Exceptions/NotAuthorizedException.cs'), this.templateOptions);

    
    //Tests
    this.fs.copyTpl(this.templatePath('test/Utils/AuthTokenHelper.cs'), this.destinationPath(this.targetTestFolderPath + 'Utils/AuthTokenHelper.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('test/Handlers/FakeTokenHandler.cs'), this.destinationPath(this.targetTestFolderPath + 'Handlers/FakeTokenHandler.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('test/Handlers/FakeAuthenticationHandler.cs'), this.destinationPath(this.targetTestFolderPath + 'Handlers/FakeAuthenticationHandler.cs'), this.templateOptions);
  },

  _createTemplateOptions: function () {
    const options = {
      namespace: this.options.namespace,
      appname: this.options.appname
    };
    return options;
  }
});
