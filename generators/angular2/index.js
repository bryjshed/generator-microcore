'use strict';
const BaseGenerator = require('../generator-base');
const Generator = require('yeoman-generator');

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

  configuring: function () {
    this.templateOptions = this._createTemplateOptions();
  },

  writing: function () {

  },

  _createTemplateOptions: function () {
    const options = {
      type: this.options.appname,
    };
    return options;
  }
});
