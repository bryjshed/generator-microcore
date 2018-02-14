'use strict';
const BaseGenerator = require('../generator-base');
const Generator = require('yeoman-generator');
const dateFormat = require('dateformat');
const now = new Date();

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
    this.targetFolderPath = this.options.targetFolderPath ? this.options.targetFolderPath + "/" : "";
    this.targetTestFolderPath = this.options.targetTestFolderPath ? this.options.targetTestFolderPath + "/" : "";
  },

  writing: function () {
    //Check if context file already exits
    const contextPath = this.destinationPath(this.targetFolderPath + 'Models/' + this.templateOptions.appname + 'Context.cs');
    if (!this.fsExistsSync(contextPath)) {
      this.fs.copyTpl(this.templatePath('src/Models/TemplateContext.cs'), contextPath, this.templateOptions);
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

      //Migrations
      const migrations = this.destinationPath(this.targetFolderPath + 'Migrations');
      if (!this.fsExistsSync(migrations)) {
        this.fs.copyTpl(this.templatePath('src/Migrations/Template_InitialMigration.cs'), this.destinationPath(this.targetFolderPath + 'Migrations/' + this.templateOptions.migrationDate + '_InitialMigration.cs'), this.templateOptions);
        this.fs.copyTpl(this.templatePath('src/Migrations/Template_InitialMigration.Designer.cs'), this.destinationPath(this.targetFolderPath + 'Migrations/' + this.templateOptions.migrationDate + '_InitialMigration.Designer.cs'), this.templateOptions);
        this.fs.copyTpl(this.templatePath('src/Migrations/TemplateContextModelSnapshot.cs'), this.destinationPath(this.targetFolderPath + 'Migrations/' + this.templateOptions.modelName + 'ContextModelSnapshot.cs'), this.templateOptions);
      }

      //Tests
      this.fs.copyTpl(this.templatePath('test/TemplateRepositoryTest.cs'), this.destinationPath(this.targetTestFolderPath + this.templateOptions.modelName + 'RepositoryTest.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('test/TemplateDtoTest.cs'), this.destinationPath(this.targetTestFolderPath + this.templateOptions.modelName + 'DtoTest.cs'), this.templateOptions);
      this.fs.copyTpl(this.templatePath('test/Utils/TemplateComparer.cs'), this.destinationPath(this.targetTestFolderPath + 'Utils/' + this.templateOptions.modelName + 'Comparer.cs'), this.templateOptions);
    }
  },

  _createTemplateOptions: function () {
    const options = {
      namespace: this.options.namespace,
      appname: this.options.appname,
      createModel: this.options.createModel,
      modelName: this.options.modelName,
      migrationDate: dateFormat(now, "yyyyddmmHHMMss")
    };
    return options;
  }

});
