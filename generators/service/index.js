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
    this.argument('serviceName', {
      type: String,
      required: true
    });
    this.argument('serviceDtoName', {
      type: String,
      required: true
    });
  },
  initializing: function () {
    this.templateOptions = this._createTemplateOptions();
    this.targetFolderPath = this.options.targetFolderPath ? this.options.targetFolderPath + '/' : '';
    this.targetTestFolderPath = this.options.targetTestFolderPath ? this.options.targetTestFolderPath + '/' : '';
  },

  writing: function () {
    if (!this.fs.exists(this.destinationPath(this.targetFolderPath + 'Services/External/BaseService.cs'))) {
      this.fs.copyTpl(this.templatePath('src/BaseService.cs'), this.destinationPath(this.targetFolderPath + 'Services/External/BaseService.cs'), this.templateOptions);
    }
    if (!this.fs.exists(this.destinationPath(this.targetFolderPath + 'Services/External/ServiceResponse.cs'))) {
      this.fs.copyTpl(this.templatePath('src/ServiceResponse.cs'), this.destinationPath(this.targetFolderPath + 'Services/External/ServiceResponse.cs'), this.templateOptions);
    }
    if (!this.fs.exists(this.destinationPath(this.targetFolderPath + 'Exceptions/ServiceUnavailableException.cs'))) {
      this.fs.copyTpl(this.templatePath('src/ServiceUnavailableException.cs'), this.destinationPath(this.targetFolderPath + 'Exceptions/ServiceUnavailableException.cs'), this.templateOptions);
    }
    this.fs.copyTpl(this.templatePath('src/ITemplateService.cs'), this.destinationPath(this.targetFolderPath + 'Services/External/I' + this.templateOptions.serviceName + 'Service.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/TemplateService.cs'), this.destinationPath(this.targetFolderPath + 'Services/External/' + this.templateOptions.serviceName + 'Service.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/TemplateDto.cs'), this.destinationPath(this.targetFolderPath + 'Dtos/' + this.templateOptions.serviceDtoName + 'Dto.cs'), this.templateOptions);

    // Tests

    if (!this.fs.exists(this.destinationPath(this.targetTestFolderPath + 'Handlers/FakeResponseHandler.cs'))) {
      this.fs.copyTpl(this.templatePath('test/Handlers/FakeResponseHandler.cs'), this.destinationPath(this.targetTestFolderPath + 'Handlers/FakeResponseHandler.cs'), this.templateOptions);
    }
    if (!this.fs.exists(this.destinationPath(this.targetTestFolderPath + 'Stubs/BaseServiceStub.cs'))) {
      this.fs.copyTpl(this.templatePath('test/Stubs/BaseServiceStub.cs'), this.destinationPath(this.targetTestFolderPath + 'Stubs/BaseServiceStub.cs'), this.templateOptions);
    }
    if (!this.fs.exists(this.destinationPath(this.targetTestFolderPath + 'BaseServiceTest.cs'))) {
      this.fs.copyTpl(this.templatePath('test/BaseServiceTest.cs'), this.destinationPath(this.targetTestFolderPath + 'BaseServiceTest.cs'), this.templateOptions);
    }
  },

  _createTemplateOptions: function () {
    const options = {
      namespace: this.options.namespace,
      appname: this.options.appname,
      serviceName: this.options.serviceName,
      serviceDtoName: this.options.serviceDtoName
    };
    return options;
  }

});
