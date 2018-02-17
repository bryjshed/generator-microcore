using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;

namespace <%= namespace %>
{
    /// <summary>
    ///  Creates and provisions a table in dynamodb.
    /// </summary>
    public class ProvisionDynamodb
    {
        private readonly IAmazonDynamoDB _amazonDynamoDB;

        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new <see cref="ProvisionDynamodb"/>.
        /// </summary>
        /// <param name="amazonDynamoDB">The IAmazonDynamoDB to use.</param>
        /// <param name="logger">The logger handler to use.</param>
        public ProvisionDynamodb(
            IAmazonDynamoDB amazonDynamoDB,
            ILogger<ProvisionDynamodb> logger)
        {
            _amazonDynamoDB = amazonDynamoDB;
            _logger = logger;
        }

        /// <summary>
        /// Creates a table in dynamodb.
        /// </summary>
        /// <param name="tableName">The name of the table to create.</param>
        /// <param name="tableThroughput">Initial provisioned throughput settings for the table.</param>
        /// <param name="indexThroughput">Initial provisioned throughput settings for the indexes.</param>
        public async void CreateTable(
            string tableName,
            ProvisionedThroughput tableThroughput,
            ProvisionedThroughput indexThroughput)
        {
            _logger.LogInformation("Starting CreateTable using table name {TableName}.", tableName);
            var request = new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = "HASH"
                    }
                },
                ProvisionedThroughput = tableThroughput
            };
            try
            {
                var response = await _amazonDynamoDB.CreateTableAsync(request);

                var tableDescription = response.TableDescription;
                _logger.LogInformation("{TableStatus}: {TableName} \t ReadsPerSec: {ReadCapacityUnits} \t WritesPerSec: {WriteCapacityUnits}",
                                tableDescription.TableStatus,
                                tableDescription.TableName,
                                tableDescription.ProvisionedThroughput.ReadCapacityUnits,
                                tableDescription.ProvisionedThroughput.WriteCapacityUnits);

                string status = tableDescription.TableStatus;
                _logger.LogInformation("{TableName} - {Status}", tableName, status);

                if (status != "ACTIVE")
                {
                    //Await for table to be created
                    WaitUntilTableReady(tableName);
                }


            }
            catch (ResourceInUseException rx)
            {
                _logger.LogInformation("The table has already been created. {Message}", rx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("There was a fatal error when provisioning dynamodb. {Exception}", ex);
                //throw exception and kill the service.
                throw ex;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName">The table to be updated.</param>
        /// <param name="tableThroughput">Update provisioned throughput settings for the table.</param>
        /// <param name="indexThroughput">Update provisioned throughput settings for the indexes.</param>
        public async void UpdateTable(
            string tableName,
            ProvisionedThroughput tableThroughput,
            ProvisionedThroughput indexThroughput)
        {
            var request = new UpdateTableRequest()
            {
                TableName = tableName,
                ProvisionedThroughput = tableThroughput
            };
            var response = await _amazonDynamoDB.UpdateTableAsync(request);
            var tableDescription = response.TableDescription;
            _logger.LogInformation("{TableStatus}: {TableName} \t ReadsPerSec: {ReadCapacityUnits} \t WritesPerSec: {WriteCapacityUnits}",
                            tableDescription.TableStatus,
                            tableDescription.TableName,
                            tableDescription.ProvisionedThroughput.ReadCapacityUnits,
                            tableDescription.ProvisionedThroughput.WriteCapacityUnits);

            WaitUntilTableReady(tableName);
        }

        private void WaitUntilTableReady(string tableName)
        {
            string status = null;
            // Let us wait until table is created. Call DescribeTable.
            do
            {
                System.Threading.Thread.Sleep(5000); // Wait 5 seconds.
                try
                {
                    var res = _amazonDynamoDB.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = tableName
                    }).Result;

                    _logger.LogInformation("Table name: {TableName}, status: {TableStatus}",
                                   res.Table.TableName,
                                   res.Table.TableStatus);
                    status = res.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {
                    // DescribeTable is eventually consistent. So you might
                    // get resource not found. So we handle the potential exception.
                }
            } while (status != "ACTIVE");
        }
    }
}
