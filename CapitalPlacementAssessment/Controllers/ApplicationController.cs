using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
    public class ApplicationController : Controller
    {
        ApiResponse<ApplicationDTO> response = new ApiResponse<ApplicationDTO>();
        ApiResponse<List<ApplicationDTO>> responseList = new ApiResponse<List<ApplicationDTO>>();

        private readonly IApplicationService _applicationService;
        private readonly IQuestionService _questionService;
        private readonly ICompanyProgramService _companyProgramService;

        public ApplicationController(IApplicationService applicationService, IQuestionService questionService, ICompanyProgramService companyProgramService)
        {
            _applicationService = applicationService;
            _questionService = questionService;
            _companyProgramService = companyProgramService;
        }

        // GET api/Application/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Application>> GetApplicationById(string id)
        {
            var application = await _applicationService.GetApplicationByIdAsync(id);

            if (application == null)
            {
                response.StatusCode = 404;
                response.ErrorMessage = $"Application with id {id} does not exist";
                return NotFound(response);
            }
            else
            {

                JsonDocument parsedUserInfo = JsonDocument.Parse(application.UserInfo);

                var companyProgramDTO = new CompanyProgramDTO
                {
                    Id = application.Program.Id,
                    Title = application.Program.Title,
                    Description = application.Program.Description,
                    Fields = application.Program.Fields!
                };

                var applicationDTO = new ApplicationDTO
                {
                    Id = application.Id,
                    Program = companyProgramDTO,
                    UserInfo = parsedUserInfo,
                    QuestionResponse = application.QuestionResponse,
                };

                response.StatusCode = 200;
                response.Data = applicationDTO;
                return Ok(response);
            }
        }

        // POST api/application
        [HttpPost]
        public async Task<ActionResult<Application>> CreateApplicationAsync([FromBody] ApplicationRequest application)
        {
            Console.WriteLine(application.UserInfo);
            if (ModelState.IsValid)
            {
                var existingCompanyProgram = await _companyProgramService.GetCompanyProgramByIdAsync(application.ProgramId);
                if (existingCompanyProgram == null)
                {
                    response.StatusCode = 409;
                    response.ErrorMessage = $"Program id {application.ProgramId} does not exist";
                    return Conflict(response);
                }
                else
                {
                    List<ProgramQuestionResponseDetails> updatedAllQuestionDetails = new List<ProgramQuestionResponseDetails>();

                    foreach (ProgramQuestionResponseRequest questionResponse in application.QuestionResponse)
                    {
                        var question = await _questionService.GetQuestionByIdAsync(questionResponse.QuestionId);
                        if (question != null)
                        {
                            ProgramQuestionResponseDetails questionDetails = new ProgramQuestionResponseDetails(question, questionResponse.Response);
                            updatedAllQuestionDetails.Add(questionDetails);
                        }
                    }

                    string userInfoString = application.UserInfo.ToString()!;

                    Application newApplication = new Application(existingCompanyProgram, updatedAllQuestionDetails, userInfoString);
                    newApplication.Id = Guid.NewGuid().ToString();
                    newApplication.DateCreated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    newApplication.LastModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    var createdApplication = await _applicationService.CreateApplicationAsync(newApplication);

                    var resourceUrl = Path.Combine(Request.Path.ToString(), Uri.EscapeDataString(createdApplication.Program.Id!));

                    var companyProgramDTO = new CompanyProgramDTO
                    {
                        Id = createdApplication.Program.Id,
                        Title = createdApplication.Program.Title,
                        Description = createdApplication.Program.Description,
                        Fields = createdApplication.Program.Fields!
                    };

                    JsonDocument parsedUserInfo = JsonDocument.Parse(createdApplication.UserInfo);

                    var applicationDTO = new ApplicationDTO
                    {
                        Id = createdApplication.Id,
                        Program = companyProgramDTO,
                        UserInfo = parsedUserInfo,
                        QuestionResponse = createdApplication.QuestionResponse,
                    };

                    response.StatusCode = 201;
                    response.Data = applicationDTO;
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
    }
}

