using System;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CapitalPlacementAssessment.Models
{
	public class QuestionType
	{
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "dateCreated")]
        public string? DateCreated { get; set; }

        [JsonProperty(PropertyName = "lastModifiedDate")]
        public string? LastModifiedDate { get; set; }

        [JsonProperty(PropertyName = "Name")]
        [Required(ErrorMessage = "Question type name is required.")]
        public required string Name { get; set; }
    }
}

