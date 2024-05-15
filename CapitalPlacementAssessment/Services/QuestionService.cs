using System;
using CapitalPlacementAssessment.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CapitalPlacementAssessment.Services
{
	public class QuestionService : IQuestionService
	{
        private readonly Container _questionContainer;
        private readonly IConfiguration configuration;
        private readonly CosmosClient cosmosClient;

        public QuestionService(CosmosClient cosmosClient, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.cosmosClient = cosmosClient;
            var databaseName = configuration["CosmosDbSettings:DatabaseName"];
            var companyProgramContainerName = "Questions";
            _questionContainer = cosmosClient.GetContainer(databaseName, companyProgramContainerName);
        }

        // Create a question
        public async Task<Question> CreateQuestionAsync(Question question)
        {
            var response = await _questionContainer.CreateItemAsync(question);
            return response.Resource;
        }

        // Delete a question
        public async Task DeleteQuestionByIdAsync(string id)
        {
            await _questionContainer.DeleteItemAsync<Question>(id, new PartitionKey(id));
        }

        // Get all questions
        public async Task<IEnumerable<Question>> GetAllQuestionsAsync()
        {
            var query = _questionContainer.GetItemLinqQueryable<Question>()
                .ToFeedIterator();

            var questions = new List<Question>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                questions.AddRange(response);
            }

            return questions;
        }

        // Get all program questions
        public async Task<IEnumerable<Question>> GetAllQuestionsByProgramIdAsync(string programId)
        {
            var query = _questionContainer.GetItemLinqQueryable<Question>()
                .Where(q => q.Program.Id == programId)
                .ToFeedIterator();

            var questions = new List<Question>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                questions.AddRange(response);
            }

            return questions;
        }

        // Get a question by id
        public async Task<Question> GetQuestionByIdAsync(string id)
        {
            var query = _questionContainer.GetItemLinqQueryable<Question>()
                .Where(p => p.Id == id)
                .Take(1)
                .ToFeedIterator();

            var response = await query.ReadNextAsync();
            return response.FirstOrDefault()!;
        }

        // Get a question by id and question text
        public async Task<Question> GetQuestionByProgramIdAndQuestionTextAsync(string programId, string questionText)
        {
            var query = _questionContainer.GetItemLinqQueryable<Question>()
                .Where(p => p.Program.Id == programId)
                .Where(p => p.QuestionText == questionText)
                .Take(1)
                .ToFeedIterator();

            var response = await query.ReadNextAsync();
            return response.FirstOrDefault()!;
        }

        // Update a question
        public async Task<Question> UpdateQuestionByIdAsync(Question question)
        {
            var response = await _questionContainer.ReplaceItemAsync(question, question.Id);
            return response.Resource;
        }
    }
}

