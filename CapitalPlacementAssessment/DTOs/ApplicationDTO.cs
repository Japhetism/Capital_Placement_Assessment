using System;
using CapitalPlacementAssessment.Models;

namespace CapitalPlacementAssessment.DTOs
{
	public class ApplicationDTO
	{
        public string? Id { get; set; }

        public required CompanyProgramDTO Program { get; set; }

        public required List<ProgramQuestionResponseDetails> QuestionResponse { get; set; }

        public required object UserInfo { get; set; }
    }
}

