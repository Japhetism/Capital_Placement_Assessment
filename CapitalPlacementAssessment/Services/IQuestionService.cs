using System;
using CapitalPlacementAssessment.Models;

namespace CapitalPlacementAssessment.Services
{
	public interface IQuestionService
	{

        Task<Question> CreateQuestionAsync(Question question);

        Task<Question> GetQuestionByIdAsync(string id);

        Task<IEnumerable<Question>> GetAllQuestionsByProgramIdAsync(string programId);

        Task<Question> GetQuestionByProgramIdAndQuestionTextAsync(string programId, string questionText);

        Task<IEnumerable<Question>> GetAllQuestionsAsync();

        Task<Question> UpdateQuestionByIdAsync(Question question);

        Task DeleteQuestionByIdAsync(string id);
    }
}

