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
    public class QuestionTypeController : Controller
    {
        ApiResponse<QuestionTypeDTO> response = new ApiResponse<QuestionTypeDTO>();
        ApiResponse<List<QuestionTypeDTO>> responseList = new ApiResponse<List<QuestionTypeDTO>>();

        private readonly IQuestionTypeService _questionTypeService;

        public QuestionTypeController(IQuestionTypeService questionTypeService)
        {
            _questionTypeService = questionTypeService;
        }

        // GET api/QuestionType/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionType>> GetQuestionTypeById(string id)
        {
            var questionType = await _questionTypeService.GetQuestionTypeByIdAsync(id);

            if (questionType == null)
            {
                response.StatusCode = 404;
                response.ErrorMessage = $"Question type with id {id} does not exist";
                return NotFound(response);
            }
            else
            {
                var questionTypeDTO = new QuestionTypeDTO
                {
                    Id = questionType.Id,
                    Name = questionType.Name
                };
                response.StatusCode = 200;
                response.Data = questionTypeDTO;
                return Ok(response);
            }
        }

        // POST api/QuestionType
        [HttpPost]
        public async Task<ActionResult<QuestionType>> CreateQuestionType([FromBody] QuestionType questionType)
        {
            if (ModelState.IsValid)
            {
                var existingQuestionType = await _questionTypeService.GetQuestionTypeByNameAsync(questionType.Name);
                if (existingQuestionType != null)
                {
                    response.StatusCode = 409;
                    response.ErrorMessage = $"Question type with this name {questionType.Name} already exist";
                    return Conflict(response);
                }
                else
                {
                    questionType.Id = Guid.NewGuid().ToString();
                    questionType.DateCreated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    questionType.LastModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    var createdQuestionType = await _questionTypeService.CreateQuestionTypeAsync(questionType);

                    var resourceUrl = Path.Combine(Request.Path.ToString(), Uri.EscapeDataString(createdQuestionType.Name));

                    var questionTypeDTO = new QuestionTypeDTO
                    {
                        Id = createdQuestionType.Id,
                        Name = createdQuestionType.Name
                    };

                    response.StatusCode = 201;
                    response.Data = questionTypeDTO;
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

        // PUT api/QuestionType/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<QuestionType>> UpdateQuestionType(string id, [FromBody] QuestionType questionType)
        {
            if (ModelState.IsValid)
            {
                var exisitingQuestionType = await _questionTypeService.GetQuestionTypeByIdAsync(id);
                if (exisitingQuestionType == null)
                {
                    response.StatusCode = 400;
                    response.ErrorMessage = $"Question type with id {id} does not exist";
                    return NotFound(response);
                }
                else
                {
                    questionType.Id = exisitingQuestionType.Id;
                    questionType.LastModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    var updatedQuestionType = await _questionTypeService.UpdateQuestionTypeByIdAsync(questionType);

                    var questionTypeDTO = new QuestionTypeDTO
                    {
                        Id = updatedQuestionType.Id,
                        Name = updatedQuestionType.Name
                    };

                    response.StatusCode = 200;
                    response.Data = questionTypeDTO;

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

        // DELETE api/QuestionType/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestionType(string id)
        {
            var exisitingQuestionType = await _questionTypeService.GetQuestionTypeByIdAsync(id);

            if (exisitingQuestionType == null)
            {
                response.StatusCode = 404;
                response.ErrorMessage = $"Question type with id {id} does not exist.";
                return NotFound(response);
            }
            else
            {
                await _questionTypeService.DeleteQuestionTypeByIdAsync(id);

                return NoContent();

            }
        }

        //GET api/QuestionType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionType>>> GetAllQuestionTypes()
        {
            var questionTypes = await _questionTypeService.GetAllQuestionTypesAsync();

            if (questionTypes == null)
            {
                responseList.StatusCode = 404;
                responseList.ErrorMessage = "No question type has been added to the application";
                return NotFound(responseList);
            }
            else
            {
                var questionTypeDTOs = questionTypes.Select(questionType => new QuestionTypeDTO
                {
                    Id = questionType.Id,
                    Name = questionType.Name
                }).ToList();

                responseList.StatusCode = 200;
                responseList.Data = questionTypeDTOs;

                return Ok(responseList);
            }
        }
    }
}

