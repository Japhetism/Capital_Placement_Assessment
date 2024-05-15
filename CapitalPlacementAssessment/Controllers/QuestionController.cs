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
    public class QuestionController : Controller
    {

        ApiResponse<QuestionDTO> response = new ApiResponse<QuestionDTO>();
        ApiResponse<List<QuestionDTO>> responseList = new ApiResponse<List<QuestionDTO>>();

        private readonly IQuestionService _questionService;
        private readonly IQuestionTypeService _questionTypeService;
        private readonly ICompanyProgramService _companyProgramService;

        public QuestionController(IQuestionService questionService, IQuestionTypeService questionTypeService, ICompanyProgramService companyProgramService)
        {
            _questionService = questionService;
            _questionTypeService = questionTypeService;
            _companyProgramService = companyProgramService;
        }

        // GET api/Question/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestionById(string id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);

            if (question == null)
            {
                response.StatusCode = 404;
                response.ErrorMessage = $"Question with id {id} does not exist";
                return NotFound(response);
            }
            else
            {
                var companyProgramDTO = new CompanyProgramDTO
                {
                    Id = question.Program.Id,
                    Title = question.Program.Title,
                    Description = question.Program.Description,
                    Fields = question.Program.Fields!
                };

                var questionTypeDTO = new QuestionTypeDTO
                {
                    Id = question.Type.Id,
                    Name = question.Type.Name
                };

                var questionDTO = new QuestionDTO
                {
                    Id = question.Id,
                    Program = companyProgramDTO,
                    Type = questionTypeDTO,
                    QuestionText = question.QuestionText,
                    Choice = question.Choice,
                    MaximumChoiceAllowed = question.MaximumChoiceAllowed
                };

                response.StatusCode = 200;
                response.Data = questionDTO;
                return Ok(response);
            }
        }

        // GET api/Question/Program/{programId}
        [HttpGet("/Program/{programId}")]
        public async Task<ActionResult<Question>> GetQuestionByProgramId(string programId)
        {
            var questions = await _questionService.GetAllQuestionsByProgramIdAsync(programId);

            if (questions == null)
            {
                responseList.StatusCode = 404;
                responseList.ErrorMessage = $"No question with program id {programId} has been added to the application";
                return NotFound(responseList);
            }
            else
            {
                var questionDTOs = questions.Select(question => new QuestionDTO
                {
                    Id = question.Id,
                    Program = GetCompanyProgramDTO(question.Program),
                    Type = GetQuestionTypeDTO(question.Type),
                    QuestionText = question.QuestionText,
                    Choice = question.Choice,
                    MaximumChoiceAllowed = question.MaximumChoiceAllowed
                }).ToList();

                responseList.StatusCode = 200;
                responseList.Data = questionDTOs;

                return Ok(responseList);
            }
        }

        // POST api/Question
        [HttpPost]
        public async Task<ActionResult<Question>> CreateQuestionAsync([FromBody] QuestionRequest question)
        {
            if (ModelState.IsValid)
            {
                var existingQuestion = await _questionService.GetQuestionByProgramIdAndQuestionTextAsync(question.ProgramId, question.QuestionText);
                if (existingQuestion != null)
                {
                    response.StatusCode = 409;
                    response.ErrorMessage = $"Question with this question text {question.QuestionText} and program id {question.ProgramId} already exist";
                    return Conflict(response);
                }
                else
                {
                    var existingCompanyProgram = await _companyProgramService.GetCompanyProgramByIdAsync(question.ProgramId);
                    var existingQuestionType = await _questionTypeService.GetQuestionTypeByIdAsync(question.TypeId);

                    if (existingCompanyProgram == null && existingQuestionType == null)
                    {
                        response.StatusCode = 409;
                        response.ErrorMessage = $"Program id {question.ProgramId} and question type id {question.TypeId} does not exist";
                        return Conflict(response);
                    }
                    else if (existingCompanyProgram == null || existingQuestionType == null)
                    {
                        string errorMessage = existingCompanyProgram == null ? $"Program id {question.ProgramId}" : $"Question type id {question.TypeId}";
                        response.StatusCode = 409;
                        response.ErrorMessage = $"{errorMessage} does not exist";
                        return Conflict(response);
                    }
                    else
                    {
                        Question newQuestion = new Question(existingCompanyProgram, existingQuestionType, question.QuestionText);
                        newQuestion.Id = Guid.NewGuid().ToString();
                        newQuestion.Choice = question.Choice;
                        newQuestion.MaximumChoiceAllowed = question.MaximumChoiceAllowed;
                        newQuestion.DateCreated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        newQuestion.LastModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        var createdQuestion = await _questionService.CreateQuestionAsync(newQuestion);

                        var resourceUrl = Path.Combine(Request.Path.ToString(), Uri.EscapeDataString(createdQuestion.QuestionText));

                        var companyProgramDTO = new CompanyProgramDTO
                        {
                            Id = createdQuestion.Program.Id,
                            Title = createdQuestion.Program.Title,
                            Description = createdQuestion.Program.Description,
                            Fields = createdQuestion.Program.Fields!
                        };

                        var questionTypeDTO = new QuestionTypeDTO
                        {
                            Id = createdQuestion.Type.Id,
                            Name = createdQuestion.Type.Name
                        };

                        var questionDTO = new QuestionDTO
                        {
                            Id = createdQuestion.Id,
                            Program = companyProgramDTO,
                            Type = questionTypeDTO,
                            QuestionText = createdQuestion.QuestionText,
                            Choice = createdQuestion.Choice,
                            MaximumChoiceAllowed = createdQuestion.MaximumChoiceAllowed
                        };

                        response.StatusCode = 201;
                        response.Data = questionDTO;

                        return Created(resourceUrl, response);
                    }

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

        //GET api/Question
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Question>>> GetAllQuestions()
        {
            var questions = await _questionService.GetAllQuestionsAsync();

            if (questions == null)
            {
                responseList.StatusCode = 404;
                responseList.ErrorMessage = "No question has been added to the application";
                return NotFound(responseList);
            }
            else
            {
                var questionDTOs = questions.Select(question => new QuestionDTO
                {
                    Id = question.Id,
                    Program = GetCompanyProgramDTO(question.Program),
                    Type = GetQuestionTypeDTO(question.Type),
                    QuestionText = question.QuestionText,
                    Choice = question.Choice,
                    MaximumChoiceAllowed = question.MaximumChoiceAllowed
                }).ToList();

                responseList.StatusCode = 200;
                responseList.Data = questionDTOs;

                return Ok(responseList);
            }
        }

        // PUT api/Question/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Question>> UpdateQuestion(string id, [FromBody] QuestionRequest question)
        {
            if (ModelState.IsValid)
            {
                var existingQuestion = await _questionService.GetQuestionByIdAsync(id);
                if (existingQuestion == null)
                {
                    response.StatusCode = 409;
                    response.ErrorMessage = $"Question with id {id} does not exist";
                    return Conflict(response);
                }
                else
                {
                    var existingCompanyProgram = await _companyProgramService.GetCompanyProgramByIdAsync(question.ProgramId);
                    var existingQuestionType = await _questionTypeService.GetQuestionTypeByIdAsync(question.TypeId);

                    if (existingCompanyProgram == null && existingQuestionType == null)
                    {
                        response.StatusCode = 409;
                        response.ErrorMessage = $"Program id {question.ProgramId} and question type id {question.TypeId} does not exist";
                        return Conflict(response);
                    }
                    else if (existingCompanyProgram == null || existingQuestionType == null)
                    {
                        string errorMessage = existingCompanyProgram == null ? $"Program id {question.ProgramId}" : $"Question type id {question.TypeId}";
                        response.StatusCode = 409;
                        response.ErrorMessage = $"{errorMessage} does not exist";
                        return Conflict(response);
                    }
                    else
                    {
                        Question questionDetails = new Question(existingCompanyProgram, existingQuestionType, question.QuestionText);
                        questionDetails.Id = existingQuestion.Id;
                        questionDetails.Choice = question.Choice;
                        questionDetails.MaximumChoiceAllowed = question.MaximumChoiceAllowed;
                        questionDetails.LastModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        var updatedQuestion = await _questionService.UpdateQuestionByIdAsync(questionDetails);

                        var resourceUrl = Path.Combine(Request.Path.ToString(), Uri.EscapeDataString(updatedQuestion.QuestionText));

                        var companyProgramDTO = new CompanyProgramDTO
                        {
                            Id = updatedQuestion.Program.Id,
                            Title = updatedQuestion.Program.Title,
                            Description = updatedQuestion.Program.Description,
                            Fields = updatedQuestion.Program.Fields!
                        };

                        var questionTypeDTO = new QuestionTypeDTO
                        {
                            Id = updatedQuestion.Type.Id,
                            Name = updatedQuestion.Type.Name
                        };

                        var questionDTO = new QuestionDTO
                        {
                            Id = updatedQuestion.Id,
                            Program = companyProgramDTO,
                            Type = questionTypeDTO,
                            QuestionText = updatedQuestion.QuestionText,
                            Choice = updatedQuestion.Choice,
                            MaximumChoiceAllowed = updatedQuestion.MaximumChoiceAllowed
                        };

                        response.StatusCode = 200;
                        response.Data = questionDTO;

                        return Created(resourceUrl, response);
                    }

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

        // DELETE api/Question/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(string id)
        {
            var exisitingQuestion = await _questionService.GetQuestionByIdAsync(id);

            if (exisitingQuestion == null)
            {
                response.StatusCode = 404;
                response.ErrorMessage = $"Question with id {id} does not exist.";
                return NotFound(response);
            }
            else
            {
                await _questionService.DeleteQuestionByIdAsync(id);

                return NoContent();

            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public CompanyProgramDTO GetCompanyProgramDTO(CompanyProgram companyProgram)
        {
            var companyProgramDTO = new CompanyProgramDTO
            {
                Id = companyProgram.Id,
                Title = companyProgram.Title,
                Description = companyProgram.Description,
                Fields = companyProgram.Fields!
            };

            return companyProgramDTO;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public QuestionTypeDTO GetQuestionTypeDTO(QuestionType questionType)
        {
            var questionTypeDTO = new QuestionTypeDTO
            {
                Id = questionType.Id,
                Name = questionType.Name
            };

            return questionTypeDTO;
        }
    }
}

