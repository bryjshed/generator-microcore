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
    this.argument('kafkaConsumer', {
      type: Boolean,
      required: true
    });
    this.argument('kafkaConsumerTopics', {
      type: String,
      required: false
    });
  },
  initializing: function () {
    this.templateOptions = this._createTemplateOptions();
    this.targetFolderPath = this.options.targetFolderPath ? this.options.targetFolderPath + "/" : "";
    this.targetTestFolderPath = this.options.targetTestFolderPath ? this.options.targetTestFolderPath + "/" : "";
  },

  writing: function () {
    this.fs.copyTpl(this.templatePath('src/Kafka/Event.cs'), this.destinationPath(this.targetFolderPath + 'Kafka/Event.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Kafka/KafkaConfig.cs'), this.destinationPath(this.targetFolderPath + 'Kafka/KafkaConfig.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Kafka/KafkaServiceExtension.cs'), this.destinationPath(this.targetFolderPath + 'Kafka/KafkaServiceExtension.cs'), this.templateOptions);

    // Consumer
    this.fs.copyTpl(this.templatePath('src/Consumer/TemplateKafkaConsumer.cs'), this.destinationPath(this.targetFolderPath + 'Consumer/' + this.templateOptions.appname + 'KafkaConsumer.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Kafka/Consumer/IKafkaConsumer.cs'), this.destinationPath(this.targetFolderPath + 'Kafka/Consumer/IKafkaConsumer.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Kafka/Consumer/KafkaConsumer.cs'), this.destinationPath(this.targetFolderPath + 'Kafka/Consumer/KafkaConsumer.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Kafka/Consumer/IKafkaConsumerManager.cs'), this.destinationPath(this.targetFolderPath + 'Kafka/Consumer/IKafkaConsumerManager.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Kafka/Consumer/KafkaConsumerManager.cs'), this.destinationPath(this.targetFolderPath + 'Kafka/Consumer/KafkaConsumerManager.cs'), this.templateOptions);

    // Producer
    this.fs.copyTpl(this.templatePath('src/Kafka/Producer/IKafkaProducer.cs'), this.destinationPath(this.targetFolderPath + 'Kafka/Producer/IKafkaProducer.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('src/Kafka/Producer/KafkaProducer.cs'), this.destinationPath(this.targetFolderPath + 'Kafka/Producer/KafkaProducer.cs'), this.templateOptions);

    // Exceptions
    this.fs.copyTpl(this.templatePath('src/Kafka/Exceptions/NullTopicException.cs'), this.destinationPath(this.targetFolderPath + 'Kafka/Exceptions/NullTopicException.cs'), this.templateOptions);

    //Tests
    this.fs.copyTpl(this.templatePath('test/Stubs/KafkaConsumerManagerStub.cs'), this.destinationPath(this.targetTestFolderPath + 'Stubs/KafkaConsumerManagerStub.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('test/Stubs/KafkaConsumerStub.cs'), this.destinationPath(this.targetTestFolderPath + 'Stubs/KafkaConsumerStub.cs'), this.templateOptions);
    this.fs.copyTpl(this.templatePath('test/Stubs/KafkaProducerStub.cs'), this.destinationPath(this.targetTestFolderPath + 'Stubs/KafkaProducerStub.cs'), this.templateOptions);
  },

  _createTemplateOptions: function () {
    const options = {
      namespace: this.options.namespace,
      appname: this.options.appname,
      kafkaConsumer: this.options.kafkaConsumer,
      kafkaConsumerTopics: this.options.kafkaConsumerTopics.split(',')
    };
    return options;
  }

});
