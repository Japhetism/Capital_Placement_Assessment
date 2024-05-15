using System;
using CapitalPlacementAssessment.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CapitalPlacementAssessment.Services
{
	public class ApplicationService : IApplicationService
	{
        private readonly Container _applicationContainer;
        private readonly IConfiguration configuration;
        private readonly CosmosClient cosmosClient;

        public ApplicationService(CosmosClient cosmosClient, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.cosmosClient = cosmosClient;
            var databaseName = configuration["CosmosDbSettings:DatabaseName"];
            var companyProgramContainerName = "Applications";
            _applicationContainer = cosmosClient.GetContainer(databaseName, companyProgramContainerName);
        }

        // Create an application
        public async Task<Application> CreateApplicationAsync(Application application)
        {
            Console.WriteLine(application.UserInfo);
            var response = await _applicationContainer.CreateItemAsync(application);
            return response.Resource;
        }

        // Get all application
        public async Task<IEnumerable<Application>> GetAllApplicationsAsync()
        {
            var query = _applicationContainer.GetItemLinqQueryable<Application>()
                .ToFeedIterator();

            var applications = new List<Application>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                applications.AddRange(response);
            }

            return applications;
        }

        // Get an application by id
        public async Task<Application> GetApplicationByIdAsync(string id)
        {
            var query = _applicationContainer.GetItemLinqQueryable<Application>()
                .Where(p => p.Id == id)
                .Take(1)
                .ToFeedIterator();

            var response = await query.ReadNextAsync();
            return response.FirstOrDefault()!;
        }
    }
}

