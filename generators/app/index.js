'use strict';
const BaseGenerator = require('../generator-base');
const Generator = require('yeoman-generator');
const chalk = require('chalk');

module.exports = BaseGenerator.extend({
  constructor: function () {
    Generator.apply(this, arguments);
    this.printLogo();
  },

  prompting: function () {
    const prompts = [{
      type: 'input',
      name: 'username',
      message: 'What is your full name?',
      store   : true,
      validate: function (value) {
        if (!value) {
          return 'Your full name is required.';
        }
        return true;
      }
    }, {
      type: 'input',
      name: 'email',
      message: 'What is your email address?',
      store   : true,
      validate: function (value) {
        if (!value) {
          return 'Your email address is required.';
        }
        const pass = value.match(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);
        if (pass) {
          return true;
        }
        return 'Email address is invalid. (E.G. joedirt@email.com)';
      }
    },{
      //Application Questions
      type: 'list',
      name: 'type',
      message: 'What type of application do you want to create?',
      choices: [{
        name: 'ASP.Net Core MicroService',
        value: 'microservice'
      }, {
        name: 'Angular 2 Application ' + chalk.bgRed('(Not Implemented)') + '?',
        value: 'angular2'
      }]
    }];

    return this.prompt(prompts).then(function (props) {
      // To access props later use this.props.someAnswer
      
      this.props = props;
    }.bind(this));
  },

  writing: function () {
    this.fs.copyTpl(this.templatePath('.gitignore'), this.destinationPath('.gitignore'), this.props);
    this.fs.copyTpl(this.templatePath('contributors.txt'), this.destinationPath('contributors.txt'), this.props);
    switch (this.props.type) {
      case 'microservice':
        this.composeWith('microcore:core', {
          displayLogo: false,
          arguments: [
            this.props.username,
            this.props.email
          ]
        });
        break;
      case 'angular2':
        this.log('Createing Angular2 project. Sorry this template still needs to be created.');
        break;
      case 'lambda':
        this.log('Createing Lambda project. Sorry this template still needs to be created.');
        break;
      default:
        this.log('Unknown project type');
    }
  }
});
