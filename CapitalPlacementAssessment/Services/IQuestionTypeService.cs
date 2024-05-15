using System;
using CapitalPlacementAssessment.Models;

namespace CapitalPlacementAssessment.Services
{
	public interface IQuestionTypeService
	{
        Task<QuestionType> CreateQuestionTypeAsync(QuestionType questionType);

        Task<QuestionType> GetQuestionTypeByIdAsync(string id);

        Task<QuestionType> GetQuestionTypeByNameAsync(string name);

        Task<QuestionType> UpdateQuestionTypeByIdAsync(QuestionType questionType);

        Task DeleteQuestionTypeByIdAsync(string id);

        Task<IEnumerable<QuestionType>> GetAllQuestionTypesAsync();
    }
}

