using System;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CapitalPlacementAssessment.Models
{
    public class Question
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "program")]
        [Required(ErrorMessage = "Program is required.")]
        public required CompanyProgram Program { get; set; }

        [JsonProperty(PropertyName = "type")]
        [Required(ErrorMessage = "Question type is required.")]
        public required QuestionType Type { get; set; }

        [JsonProperty(PropertyName = "question")]
        [Required(ErrorMessage = "Question is required.")]
        public required string QuestionText { get; set; }

        [JsonProperty(PropertyName = "choice")]
        public List<string>? Choice { get; set; }

        [JsonProperty(PropertyName = "maximumChoiceAllowed")]
        public int? MaximumChoiceAllowed { get; set; }

        [JsonProperty(PropertyName = "dateCreated")]
        public string? DateCreated { get; set; }

        [JsonProperty(PropertyName = "lastModifiedDate")]
        public string? LastModifiedDate { get; set; }

        [SetsRequiredMembers]
        public Question(CompanyProgram companyProgram, QuestionType questionType, string questionText)
        {
            Program = companyProgram;
            Type = questionType;
            QuestionText = questionText;
        }

    }

    public class QuestionRequest
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "programId")]
        [Required(ErrorMessage = "Program is required.")]
        public required string ProgramId { get; set; }

        [JsonProperty(PropertyName = "typeId")]
        [Required(ErrorMessage = "Question type is required.")]
        public required string TypeId { get; set; }

        [JsonProperty(PropertyName = "question")]
        [Required(ErrorMessage = "Question is required.")]
        public required string QuestionText { get; set; }

        [JsonProperty(PropertyName = "choice")]
        public List<string>? Choice { get; set; }

        [JsonProperty(PropertyName = "maximumChoiceAllowed")]
        public int? MaximumChoiceAllowed { get; set; }
    }
}

