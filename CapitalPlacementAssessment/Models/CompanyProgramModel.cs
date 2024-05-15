using System;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CapitalPlacementAssessment.Models
{
    public class CompanyProgram
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "dateCreated")]
        public string? DateCreated { get; set; }

        [JsonProperty(PropertyName = "lastModifiedDate")]
        public string? LastModifiedDate { get; set; }

        [JsonProperty(PropertyName = "title")]
        [Required(ErrorMessage = "Program title is required.")]
        public required string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        [Required(ErrorMessage = "Program description is required.")]
        public required string Description { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public List<Field>? Fields { get; set; }
    }

    public class Field
    {
        [JsonProperty(PropertyName = "name")]
        [Required(ErrorMessage = "Field name is required.")]
        public required string Name { get; set; }

        [JsonProperty(PropertyName = "isRequired")]
        [Required(ErrorMessage = "Specify if field is required or not.")]
        public required bool IsRequired { get; set; }
    }
}

