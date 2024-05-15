using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapitalPlacementAssessment.DTOs;
using CapitalPlacementAssessment.Helper;
using CapitalPlacementAssessment.Models;
using CapitalPlacementAssessment.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CapitalPlacementAssessment.Controllers
{
    [Route("api/[controller]")]
    public class CompanyProgramController : Controller
    {
        ApiResponse<CompanyProgramDTO> response = new ApiResponse<CompanyProgramDTO>();
        ApiResponse<List<CompanyProgramDTO>> responseList = new ApiResponse<List<CompanyProgramDTO>>();

        private readonly ICompanyProgramService _companyProgramService;

        public CompanyProgramController(ICompanyProgramService companyProgramService)
        {
            _companyProgramService = companyProgramService;
        }

        // GET api/CompanyProgram/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyProgram>> GetCompanyProgramById(string id)
        {
            var companyProgram = await _companyProgramService.GetCompanyProgramByIdAsync(id);

            if (companyProgram == null)
            {
                response.StatusCode = 404;
                response.ErrorMessage = $"Program with id {id} does not exist";
                return NotFound(response);
            }
            else
            {
                var companyProgramDTO = new CompanyProgramDTO
                {
                    Id = companyProgram.Id,
                    Title = companyProgram.Title,
                    Description = companyProgram.Description,
                    Fields = companyProgram.Fields!
                };
                response.StatusCode = 200;
                response.Data = companyProgramDTO;
                return Ok(response);
            }
        }

        // POST api/CompanyProgram
        [HttpPost]
        public async Task<ActionResult<CompanyProgram>> CreateCompanyProgram([FromBody] CompanyProgram companyProgram)
        {
            if (ModelState.IsValid)
            {
                var exisitingCompanyProgram = await _companyProgramService.GetCompanyProgramByTitleAsync(companyProgram.Title);
                if (exisitingCompanyProgram != null)
                {
                    response.StatusCode = 409;
                    response.ErrorMessage = $"Program with this title {companyProgram.Title} already exist";
                    return Conflict(response);
                }
                else
                {
                    companyProgram.Id = Guid.NewGuid().ToString();
                    companyProgram.DateCreated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    companyProgram.LastModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var createdCompanyProgram = await _companyProgramService.CreateCompanyProgramAsync(companyProgram);
                    var resourceUrl = Path.Combine(Request.Path.ToString(), Uri.EscapeDataString(createdCompanyProgram.Title));
                    var companyProgramDTO = new CompanyProgramDTO
                    {
                        Id = createdCompanyProgram.Id,
                        Title = createdCompanyProgram.Title,
                        Description = createdCompanyProgram.Description,
                        Fields = createdCompanyProgram.Fields!
                    };

                    response.StatusCode = 201;
                    response.Data = companyProgramDTO;
                    return Created(resourceUrl, response);
                }
            }
            else
            {
                string errorMessage = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                response.Success = false;
                response.ErrorMessage = $"Validation error occurred for {errorMessage}";
                response.StatusCode = 400;
                return BadRequest(response);
            }
        }

        // PUT api/CompanyProgram/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<CompanyProgram>> UpdateCompanyProgram(string id, [FromBody] CompanyProgram companyProgram)
        {
            if (ModelState.IsValid)
            {
                var exisitingCompanyProgram = await _companyProgramService.GetCompanyProgramByIdAsync(id);
                if (exisitingCompanyProgram == null)
                {
                    response.StatusCode = 400;
                    response.ErrorMessage = $"Program with id {id} does not exist";
                    return NotFound(response);
                }
                else
                {
                    companyProgram.Id = exisitingCompanyProgram.Id;
                    companyProgram.LastModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    var updatedCompanyProgram = await _companyProgramService.UpdateCompanyProgramByIdAsync(companyProgram);

                    var companyProgramDTO = new CompanyProgramDTO
                    {
                        Id = updatedCompanyProgram.Id,
                        Title = updatedCompanyProgram.Title,
                        Description = updatedCompanyProgram.Description,
                        Fields = updatedCompanyProgram.Fields!
                    };

                    response.StatusCode = 200;
                    response.Data = companyProgramDTO;

                    return Ok(response);
                }
            }
            else
            {
                string errorMessage = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                response.Success = false;
                response.ErrorMessage = $"Validation error occurred for {errorMessage}";
                response.StatusCode = 400;
                return BadRequest(response);
            }
        }

        // DELETE api/CompanyProgram/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyProgram(string id)
        {
            var exisitingCompanyProgram = await _companyProgramService.GetCompanyProgramByIdAsync(id);

            if (exisitingCompanyProgram == null)
            {
                response.StatusCode = 404;
                response.ErrorMessage = $"Program with id {id} does not exist.";
                return NotFound(response);
            }
            else
            {
                await _companyProgramService.DeleteCompanyProgramByIdAsync(id);

                return NoContent();

            }
        }

        //GET api/CompanyProgram
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyProgram>>> GetAllCompanyPrograms()
        {
            var companyPrograms = await _companyProgramService.GetAllCompanyProgramsAsync();

            if (companyPrograms == null)
            {
                responseList.StatusCode = 404;
                responseList.ErrorMessage = "No program has been added to the application";
                return NotFound(responseList);
            }
            else
            {
                var companyProgramDTOs = companyPrograms.Select(companyProgram => new CompanyProgramDTO
                {
                    Id = companyProgram.Id,
                    Title = companyProgram.Title,
                    Description = companyProgram.Description,
                    Fields = companyProgram.Fields!
                }).ToList();

                responseList.StatusCode = 200;
                responseList.Data = companyProgramDTOs;
                return Ok(responseList);
            }
        }
    }
}

