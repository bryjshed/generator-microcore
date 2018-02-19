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
    this.argument('createModel', {
      type: Boolean,
      required: true
    });
    this.argument('modelName', {
      type: String,
      required: true
    });
  },

  configuring: function () {
    this.templateOptions = this._createTemplateOptions();
    this.targetFolderPath = this.options.targetFolderPath ? this.options.targetFolderPath + '/' : '';
    this.targetTestFolderPath = this.options.targetTestFolderPath ? this.options.targetTestFolderPath + '/' : '';
  },

  writing: function () {
    // Check if provision file already exits

    const provisonPath = this.destinationPath(this.targetFolderPath + 'Provision/ProvisionDynamodb.cs');
    if (!this.fsExistsSync(provisonPath)) {
      this.fs.copyTpl(this.templatePath('src/Provision/ProvisionDynamodb.cs'), provisonPath, this.templateOptions);
    }

    if (this.templateOptions.createModel) {
      this.fs.copyTpl(this.templatePath('src/Dtos/DetailTemplateDto.cs'), this.destinationPath(this.targetFolderPath + 'Dtos/' + this.templateOptions.modelName + 'Dto.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('src/Dtos/CreateTemplateDto.cs'), this.destinationPath(this.targetFolderPath + 'Dtos/' + 'Create' + this.templateOptions.modelName + 'Dto.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('src/Dtos/UpdateTemplateDto.cs'), this.destinationPath(this.targetFolderPath + 'Dtos/' + 'Update' + this.templateOptions.modelName + 'Dto.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('src/Dtos/TemplateSearchDto.cs'), this.destinationPath(this.targetFolderPath + 'Dtos/' + 'Search' + this.templateOptions.modelName + 'Dto.cs'), this.templateOptions);

      this.fs.copyTpl(this.templatePath('src/Models/TemplateModel.cs'), this.destinationPath(this.targetFolderPath + 'Models/' + this.templateOptions.modelName + '.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('src/Repositories/ITemplateRepository.cs'), this.destinationPath(this.targetFolderPath + 'Repositories/I' + this.templateOptions.modelName + 'Repository.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('src/Repositories/TemplateRepository.cs'), this.destinationPath(this.targetFolderPath + 'Repositories/' + this.templateOptions.modelName + 'Repository.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('src/Repositories/PaginatedList.cs'), this.destinationPath(this.targetFolderPath + 'Repositories/PaginatedList.cs'), this.templateOptions);

      // Tests

      this.fs.copyTpl(this.templatePath('test/Stubs/TemplateRepositoryStub.cs'), this.destinationPath(this.targetTestFolderPath + 'Stubs/' + this.templateOptions.modelName + 'Repository.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('test/TemplateDtoTest.cs'), this.destinationPath(this.targetTestFolderPath + this.templateOptions.modelName + 'DtoTest.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('test/TemplateSearchDtoTest.cs'), this.destinationPath(this.targetTestFolderPath + this.templateOptions.modelName + 'SearchDtoTest.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('test/Utils/TemplateComparer.cs'), this.destinationPath(this.targetTestFolderPath + 'Utils/' + this.templateOptions.modelName + 'Comparer.cs'), this.templateOptions);
    }
  },

  _createTemplateOptions: function () {
    const options = {
      namespace: this.options.namespace,
      appname: this.options.appname,
      createModel: this.options.createModel,
      modelName: this.options.modelName
    };
    return options;
  }
});
