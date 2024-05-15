using System;
using CapitalPlacementAssessment.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CapitalPlacementAssessment.Services
{
	public class CompanyProgramService : ICompanyProgramService
	{
        private readonly Container _companyProgramContainer;
        private readonly IConfiguration configuration;
        private readonly CosmosClient cosmosClient;

        public CompanyProgramService(CosmosClient cosmosClient, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.cosmosClient = cosmosClient;
            var databaseName = configuration["CosmosDbSettings:DatabaseName"];
            var companyProgramContainerName = "Programs";
            _companyProgramContainer = cosmosClient.GetContainer(databaseName, companyProgramContainerName);
        }

        // Create a new program
        public async Task<CompanyProgram> CreateCompanyProgramAsync(CompanyProgram companyProgram)
        {
            var response = await _companyProgramContainer.CreateItemAsync(companyProgram);
            return response.Resource;
        }

        // Delete program
        public async Task DeleteCompanyProgramByIdAsync(string id)
        {
            await _companyProgramContainer.DeleteItemAsync<CompanyProgram>(id, new PartitionKey(id));
        }

        // Get all programs
        public async Task<IEnumerable<CompanyProgram>> GetAllCompanyProgramsAsync()
        {
            var query = _companyProgramContainer.GetItemLinqQueryable<CompanyProgram>()
                .ToFeedIterator();

            var companyPrograms = new List<CompanyProgram>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                companyPrograms.AddRange(response);
            }

            return companyPrograms;
        }

        // Get a program by id
        public async Task<CompanyProgram> GetCompanyProgramByIdAsync(string id)
        {
            var query = _companyProgramContainer.GetItemLinqQueryable<CompanyProgram>()
                .Where(p => p.Id == id)
                .Take(1)
                .ToFeedIterator();

            var response = await query.ReadNextAsync();
            return response.FirstOrDefault()!;
        }

        // Get a program by title
        public async Task<CompanyProgram> GetCompanyProgramByTitleAsync(string title)
        {
            var query = _companyProgramContainer.GetItemLinqQueryable<CompanyProgram>()
                .Where(p => p.Title.ToLower() == title.ToLower())
                .Take(1)
                .ToFeedIterator();

            var response = await query.ReadNextAsync();
            return response.FirstOrDefault()!;
        }

        // Update a program
        public async Task<CompanyProgram> UpdateCompanyProgramByIdAsync(CompanyProgram companyProgram)
        {
            var response = await _companyProgramContainer.ReplaceItemAsync(companyProgram, companyProgram.Id);
            return response.Resource;
        }
    }
}

