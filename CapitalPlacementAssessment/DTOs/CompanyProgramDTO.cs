using System;
using CapitalPlacementAssessment.Models;

namespace CapitalPlacementAssessment.DTOs
{
    public class CompanyProgramDTO
    {
        public string? Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required List<Field> Fields { get; set; }
    }
}

