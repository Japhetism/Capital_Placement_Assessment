using System;
namespace CapitalPlacementAssessment.DTOs
{
	public class QuestionDTO
	{
        public string? Id { get; set; }

        public required CompanyProgramDTO Program { get; set; }

        public required string QuestionText { get; set; }

        public required QuestionTypeDTO Type { get; set; }

        public List<string>? Choice { get; set; }

        public int? MaximumChoiceAllowed { get; set; }
    }
}

