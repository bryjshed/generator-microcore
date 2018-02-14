'use strict';
const Generator = require('yeoman-generator');
const chalk = require('chalk');
const packagejs = require('../package.json');
const fs = require('fs');

class BaseGenerator extends Generator {
  // The name `constructor` is important here
  toCamelCase(str) {
    if (!str) return;
    return str.replace(/(?:^\w|[A-Z]|\b\w)/g, function (letter, index) {
      return index == 0 ? letter.toLowerCase() : letter.toUpperCase();
    }).replace(/\s+/g, '');
  }
  toPascalCase(str) {
    if (!str) return;
    return str.replace(/(?:^\w|[A-Z]|\b\w)/g, function (letter, index) {
      return index == 0 ? letter.toUpperCase() : letter.toUpperCase();
    }).replace(/\s+/g, '');
  }

  printLogo() {
    this.log(' \n' +
      chalk.blue('                                       ') + chalk.white('                                 ') + ('\n') +
      chalk.blue('                                       ') + chalk.white('                                 ') + ('\n') +
      chalk.blue('                                       ') + chalk.white('                                 ') + ('\n') +
      chalk.blue('███╗   ███╗██╗ ██████╗██████╗  ██████╗ ') + chalk.white(' ██████╗ ██████╗ ██████╗ ███████╗') + ('\n') +
      chalk.blue('████╗ ████║██║██╔════╝██╔══██╗██╔═══██╗') + chalk.white('██╔════╝██╔═══██╗██╔══██╗██╔════╝') + ('\n') +
      chalk.blue('██╔████╔██║██║██║     ██████╔╝██║   ██║') + chalk.white('██║     ██║   ██║██████╔╝█████╗  ') + ('\n') +
      chalk.blue('██║╚██╔╝██║██║██║     ██╔══██╗██║   ██║') + chalk.white('██║     ██║   ██║██╔══██╗██╔══╝  ') + ('\n') +
      chalk.blue('██║ ╚═╝ ██║██║╚██████╗██║  ██║╚██████╔╝') + chalk.white('╚██████╗╚██████╔╝██║  ██║███████╗') + ('\n') +
      chalk.blue('╚═╝     ╚═╝╚═╝ ╚═════╝╚═╝  ╚═╝ ╚═════╝ ') + chalk.white(' ╚═════╝ ╚═════╝ ╚═╝  ╚═╝╚══════╝') + ('\n') 

    );
    this.log(chalk.white.bold('                            ' + packagejs.homepage + '\n'));
    this.log(chalk.white('Welcome, my name is MicroCore and I will be happy to help generate a new project.') + chalk.yellow('v' + packagejs.version));
    this.log(chalk.white('Application files will be generated in folder: ' + chalk.yellow(process.cwd())));
  };
  createSrcFolderPath(namespace) {
    return this.destinationPath('src') + '/' + namespace;
  }
  createTestFolderPath(namespace) {
    return this.destinationPath('test') + '/' + namespace + ".Tests";
  }
  containsWord(value, cotains) {
    return value.toLowerCase().includes(cotains.toLowerCase());
  }

  fsExistsSync(myDir) {
    try {
      fs.accessSync(myDir);
      return true;
    } catch (e) {
      return false;
    }
  }
};
module.exports = BaseGenerator;
