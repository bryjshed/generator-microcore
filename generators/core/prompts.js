const chalk = require('chalk');

var kafkaConsumerTopics = [];
var externalServices = []

module.exports = {
    intialPrompt,
    servicePrompt,
    kafkaPrompt,
    finalPrompt
};

function intialPrompt() {
    var prompts = [{
      type: 'input',
      name: 'appname',
      message: 'What is the base name of your service?',
      default: "MyTest",
      validate: function (value) {
        var pass = value.match(/^[a-zA-Z]+$/);
        if (pass) {
          return true;
        }
        return 'Please enter a valid service name. (E.G. MyTest)';
      }
    }, {
      type: 'input',
      name: 'namespace',
      message: 'What is the base C# namespace?',
      default: function (response) {
        return "MicroCore." + response.appname + "Service";
      },
      validate: function (value) {
        var pass = value.match(/^(@?[a-z_A-Z]\w+(?:\.@?[a-z_A-Z]\w+)*)$/);
        if (pass) {
          return true;
        }
        return 'Please enter a valid namespace. (E.G. ' + 'MicroCore.' + response.appname + 'Service)';
      }
    }, {
      type: 'confirm',
      name: 'createController',
      message: 'Create a controller stub?',
      default: true
    }, {
      when: function (response) {
        return response.createController;
      },
      type: 'input',
      name: 'controllerName',
      message: 'The name of your Controller?',
      default: "MyTest",
      validate: (value) => {
        if (this.containsWord(value, "Controller")) {
          return 'Please do not include the word ' + chalk.red.bold('Controller') + ' in your controller name';
        }
        if (!value.match(/^[A-Z][a-zA-Z]+$/)) {
          return 'The first letter must be capital (E.G. MyTest)';
        }
        return true;
      }
    }, {
      type: 'confirm',
      name: 'addDatabase',
      message: 'Add database support?',
      default: true
    }, {
      when: function (response) {
        return response.addDatabase;
      },
      type: 'list',
      name: 'database',
      message: 'What database would you like to use with this application?',
      default: "mysql",
      choices: [{
        name: 'MySql',
        value: 'mysql'
      }, {
        name: 'PostgreSQL',
        value: 'postgresql'
      }, {
        name: 'SqlServer',
        value: 'sqlserver'
      }, {
        name: 'DynamoDB',
        value: 'dynamodb'
      }]
    }, {
      when: function (response) {
        return response.addDatabase;
      },
      type: 'confirm',
      name: 'createModel',
      message: 'Would you like to create a model stub?',
      default: true
    }, {
      when: function (response) {
        return response.createModel;
      },
      type: 'input',
      name: 'modelName',
      message: 'Enter model name.',
      validate: function (value) {
        var pass = value.match(/^[A-Z][a-zA-Z]+$/);
        if (pass) {
          return true;
        }
        return 'The first letter must be capital (E.G. MyTest)';
      },
      default: function (response) {
        if (response.createController) {
          return response.controllerName;
        }
        return "MyTest";
      }
    }];
  
    return this.prompt(prompts).then(function (props) {
      this.props = props;
    }.bind(this));
  }
  
  function servicePrompt() {
    return serviceRetry.call(this, true);
  }
  
  function serviceRetry(enableAdd) {
    const done = this.async();
    var prompts = [{
        when: function (response) {
          return enableAdd;
        },
        type: 'confirm',
        name: 'createService',
        message: 'Would you like to create an external service stub?',
        default: false
      }, {
        when: function (response) {
          return response.createService || !enableAdd;
        },
        type: 'input',
        name: 'serviceName',
        message: 'Enter external service name.',
        validate: function (value) {
          for (var i = 0; i < externalServices.length; i++) {
            if (externalServices[i].serviceName === value) {
              return 'You already have an external service called ' + value;
            }
          }
          var pass = value.match(/^[A-Z][a-zA-Z]+$/);
          if (pass) {
            return true;
          }
          return 'The first letter must be capital (E.G. Outside)';
        },
        default: function (response) {
          return "Outside";
        }
      }, {
        when: function (response) {
          return response.createService || !enableAdd;
        },
        type: 'input',
        name: 'serviceEndpoint',
        message: 'Enter external service endpoint(Url).',
        default: function (response) {
          return "http://localhost/";
        }
      }, {
        when: function (response) {
          return response.createService || !enableAdd;
        },
        type: 'input',
        name: 'serviceDtoName',
        message: 'Enter external service dto name. (Different from your model name if selected.)',
        validate: (value, response) => {
          for (var i = 0; i < externalServices.length; i++) {
            if (externalServices[i].serviceDtoName === value) {
              return 'You already have an external service dto called ' + value;
            }
          }
          if (this.props.modelName === value) {
            return 'Your service dto can not have the same name as your model.';
          }
          if (this.containsWord(value, "Dto")) {
            return 'Please do not include the word ' + chalk.red.bold('Dto') + ' in your service model name';
          }
          if (!value.match(/^[A-Z][a-zA-Z]+$/)) {
            return 'The first letter must be capital (E.G. MyTest)';
          }
          return true;
  
        },
        default: function (response) {
          return response.serviceName;
        }
      },
      {
        when: function (response) {
          return response.serviceName || !enableAdd;
        },
        type: 'confirm',
        name: 'topicAnother',
        message: "Is there another external service stub you would like to add?",
        default: true
      }
    ];
    return this.prompt(prompts).then(function (props) {
      if (enableAdd) {
        this.props.createService = props.createService;
      }
      if (this.props.createService) {
        var service = {
          serviceName: props.serviceName,
          serviceEndpoint: props.serviceEndpoint,
          serviceDtoName: props.serviceDtoName
        };
        externalServices.push(service);
      }
  
      if (props.topicAnother) {
        serviceRetry.call(this, false);
      } else {
        this.props.externalServices = externalServices;
        done();
      }
    }.bind(this));
  }
  
  function kafkaPrompt() {
    return kafkaRetry.call(this, true);
  }
  
  function kafkaRetry(enableAdd) {
    const done = this.async();
    var prompts = [{
        when: function (response) {
          return enableAdd;
        },
        type: 'confirm',
        name: 'kafka',
        message: 'Would you like to enable Kafka? Producer will be added by default for all create and update events.',
        default: true
      }, {
        when: function (response) {
          return response.kafka && enableAdd;
        },
        type: 'confirm',
        name: 'kafkaConsumer',
        message: 'Would you like to add a Kafka consumer?',
        default: true
      }, {
        when: function (response) {
          return response.kafkaConsumer || !enableAdd;
        },
        type: 'input',
        name: 'consumerTopic',
        message: 'What topic would you like to consume. Please use snake case. (E.G. patient_created)',
        validate: function (value) {
          if (value) {
            return true;
          }
          return 'Kafka consumer topic required.';
        }
      },
      {
        when: function (response) {
          return response.kafkaConsumer || !enableAdd;
        },
        type: 'confirm',
        name: 'topicAnother',
        message: "Is there another kafka topic you would like to consume?",
        default: true
      }
    ];
    return this.prompt(prompts).then(function (props) {
      if (enableAdd) {
        this.props.kafkaConsumer = props.kafkaConsumer;
        this.props.kafka = props.kafka;
      }
      kafkaConsumerTopics.push(props.consumerTopic)
      if (props.topicAnother) {
        kafkaRetry.call(this, false);
      } else {
        this.props.kafkaConsumerTopics = kafkaConsumerTopics;
        done();
      }
    }.bind(this));
  }
  
  function finalPrompt() {
    var prompts = [{
        type: 'confirm',
        name: 'elasticSearch',
        message: 'Would you like to enable Elastic Search ' + chalk.bgRed('(Not Implemented)') + '?',
        default: true
      }, {
        type: 'input',
        name: 'portNumber',
        message: 'What is the port that you want to use for this service?',
        default: 8000
      }, {
        type: 'confirm',
        name: 'vscode',
        message: 'Would you like to include VSCode ' + chalk.bgGreen('launch.json') + ' and ' + chalk.bgGreen('task.json') + ' files?',
        default: true
      }, {
        when: (response) => {
          return this.fsExistsSync(this.destinationPath('README.md'));
        },
        type: 'confirm',
        name: 'projectExist',
        message: chalk.red.bold('WARNING! A project already exists in this directory. This could cause unknown build errors and loss of data. Would you like to continue?'),
        default: false
      }
    ];
    return this.prompt(prompts).then(function (props) {
      this.props.elasticSearch = props.elasticSearch;
      this.props.portNumber = props.portNumber;
      this.props.vscode = props.vscode;
      this.props.projectExist = props.projectExist;
    }.bind(this));
  }
  
  function authPrompt() {
    var prompts = [{
      type: 'confirm',
      name: 'authentication',
      message: 'Would you like to enable JWT middleware?',
      default: true
    }];
    return this.prompt(prompts).then(function (props) {
      this.props.authentication = props.authentication;
    }.bind(this));
  }
  