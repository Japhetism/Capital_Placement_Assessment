using System;
using CapitalPlacementAssessment.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CapitalPlacementAssessment.Services
{
	public class QuestionTypeService : IQuestionTypeService
	{
        private readonly Container _questionTypeContainer;
        private readonly IConfiguration configuration;
        private readonly CosmosClient cosmosClient;

        public QuestionTypeService(CosmosClient cosmosClient, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.cosmosClient = cosmosClient;
            var databaseName = configuration["CosmosDbSettings:DatabaseName"];
            var companyProgramContainerName = "QuestionTypes";
            _questionTypeContainer = cosmosClient.GetContainer(databaseName, companyProgramContainerName);
        }

        // Create a question type
        public async Task<QuestionType> CreateQuestionTypeAsync(QuestionType questionType)
        {
            var response = await _questionTypeContainer.CreateItemAsync(questionType);
            return response.Resource;
        }

        // Delete a question type
        public async Task DeleteQuestionTypeByIdAsync(string id)
        {
            await _questionTypeContainer.DeleteItemAsync<QuestionType>(id, new PartitionKey(id));
        }

        // Get all question types
        public async Task<IEnumerable<QuestionType>> GetAllQuestionTypesAsync()
        {
            var query = _questionTypeContainer.GetItemLinqQueryable<QuestionType>()
                .ToFeedIterator();

            var questionTypes = new List<QuestionType>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                questionTypes.AddRange(response);
            }

            return questionTypes;
        }

        // Get a question type by id
        public async Task<QuestionType> GetQuestionTypeByIdAsync(string id)
        {
            var query = _questionTypeContainer.GetItemLinqQueryable<QuestionType>()
                .Where(p => p.Id == id)
                .Take(1)
                .ToFeedIterator();

            var response = await query.ReadNextAsync();
            return response.FirstOrDefault()!;
        }

        // Get a question type by name
        public async Task<QuestionType> GetQuestionTypeByNameAsync(string name)
        {
            var query = _questionTypeContainer.GetItemLinqQueryable<QuestionType>()
                .Where(p => p.Name.ToLower() == name.ToLower())
                .Take(1)
                .ToFeedIterator();

            var response = await query.ReadNextAsync();
            return response.FirstOrDefault()!;
        }

        // Update a question type
        public async Task<QuestionType> UpdateQuestionTypeByIdAsync(QuestionType questionType)
        {
            var response = await _questionTypeContainer.ReplaceItemAsync(questionType, questionType.Id);
            return response.Resource;
        }
    }
}

