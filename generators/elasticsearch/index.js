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
    this.argument('documentname', {
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

  },

  _createTemplateOptions: function () {
    const options = {
      namespace: this.options.namespace,
      appname: this.options.appname,
      documentname: this.options.documentname
    };
    return options;
  }

});
