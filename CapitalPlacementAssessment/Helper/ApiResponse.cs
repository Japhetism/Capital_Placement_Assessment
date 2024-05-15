using System;
namespace CapitalPlacementAssessment.Helper
{
	public class ApiResponse<T>
	{
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }
        public T? Data { get; set; }

        public ApiResponse()
        {
            Success = true;
            ErrorMessage = null;
            Data = default;
        }
    }
}

