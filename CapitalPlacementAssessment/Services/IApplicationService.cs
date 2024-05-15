using System;
using CapitalPlacementAssessment.Models;

namespace CapitalPlacementAssessment.Services
{
	public interface IApplicationService
	{
        Task<Application> CreateApplicationAsync(Application application);

        Task<Application> GetApplicationByIdAsync(string id);

        Task<IEnumerable<Application>> GetAllApplicationsAsync();
    }
}

