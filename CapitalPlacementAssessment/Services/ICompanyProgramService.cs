using System;
using CapitalPlacementAssessment.Models;

namespace CapitalPlacementAssessment.Services
{
	public interface ICompanyProgramService
	{
        Task<CompanyProgram> CreateCompanyProgramAsync(CompanyProgram companyProgram);

        Task<CompanyProgram> GetCompanyProgramByIdAsync(string id);

        Task<CompanyProgram> GetCompanyProgramByTitleAsync(string title);

        Task<CompanyProgram> UpdateCompanyProgramByIdAsync(CompanyProgram companyProgram);

        Task DeleteCompanyProgramByIdAsync(string id);

        Task<IEnumerable<CompanyProgram>> GetAllCompanyProgramsAsync();
    }
}

