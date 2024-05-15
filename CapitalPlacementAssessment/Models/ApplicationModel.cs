using System;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CapitalPlacementAssessment.Models
{
    public class Application
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "program")]
        [Required(ErrorMessage = "Program is required.")]
        public required CompanyProgram Program { get; set; }

        [JsonProperty(PropertyName = "questionResponse")]
        [Required(ErrorMessage = "Question response is required.")]
        public required List<ProgramQuestionResponseDetails> QuestionResponse { get; set; }

        [JsonProperty(PropertyName = "userInfo")]
        [Required(ErrorMessage = "User info is required.")]
        public required string UserInfo { get; set; }

        [JsonProperty(PropertyName = "dateCreated")]
        public string? DateCreated { get; set; }

        [JsonProperty(PropertyName = "lastModifiedDate")]
        public string? LastModifiedDate { get; set; }

        [SetsRequiredMembers]
        public Application(CompanyProgram program, List<ProgramQuestionResponseDetails> questionResponse, string userInfo)
        {
            Program = program;
            QuestionResponse = questionResponse;
            UserInfo = userInfo;
        }
    }

    public class ApplicationRequest
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "programId")]
        [Required(ErrorMessage = "Program is required.")]
        public required string ProgramId { get; set; }

        [JsonProperty(PropertyName = "questionResponse")]
        [Required(ErrorMessage = "Question response is required.")]
        public required List<ProgramQuestionResponseRequest> QuestionResponse { get; set; }

        [JsonProperty(PropertyName = "userInfo")]
        [Required(ErrorMessage = "User information is required")]
        public required object UserInfo { get; set; }
    }

    public class ProgramQuestionResponseRequest
    {
        [JsonProperty(PropertyName = "response")]
        [Required(ErrorMessage = "Program question response is required.")]
        public required string Response { get; set; }

        [JsonProperty(PropertyName = "questionId")]
        [Required(ErrorMessage = "Question is required.")]
        public required string QuestionId { get; set; }
    }

    public class ProgramQuestionResponseDetails
    {
        [JsonProperty(PropertyName = "question")]
        [Required(ErrorMessage = "question is required.")]
        public required Question Details { get; set; }

        [JsonProperty(PropertyName = "response")]
        [Required(ErrorMessage = "Program question response is required.")]
        public required string Response { get; set; }

        [SetsRequiredMembers]
        public ProgramQuestionResponseDetails(Question questionDetails, string response)
        {
            Details = questionDetails;
            Response = response;
        }
    }
}

