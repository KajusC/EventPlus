using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EventPlus.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRequestAnswerController : ControllerBase
    {
        private readonly IUserRequestAnswerLogic _UserRequestAnswerLogic;

        public UserRequestAnswerController(IUserRequestAnswerLogic UserRequestAnswerLogic)
        {
            _UserRequestAnswerLogic = UserRequestAnswerLogic ?? throw new ArgumentNullException(nameof(UserRequestAnswerLogic));
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, Organiser")]
        public async Task<IActionResult> GetAllUserRequestAnswers()
        {
            var UserRequestAnswers = await _UserRequestAnswerLogic.GetAllUserRequestAnswersAsync();
            return Ok(UserRequestAnswers);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserRequestAnswerById(int id)
        {
            try
            {
                var UserRequestAnswer = await _UserRequestAnswerLogic.GetUserRequestAnswerByIdAsync(id);
                if (UserRequestAnswer == null)
                {
                    return NotFound();
                }
                return Ok(UserRequestAnswer);
            }
            catch (Exception ex)
            {
                return BadRequest($"Klaida gaunant vartotojo užklausą: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserRequestAnswersByUserId(int userId)
        {
            try
            {
                var UserRequestAnswers = await _UserRequestAnswerLogic.GetUserRequestAnswersByUserIdAsync(userId);
                if (UserRequestAnswers == null || UserRequestAnswers.Count == 0)
                {
                    return NotFound("Šiam vartotojui užklausų nerasta.");
                }
                return Ok(UserRequestAnswers);
            }
            catch (Exception ex)
            {
                return BadRequest($"Klaida gaunant vartotojo užklausas: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUserRequestAnswer([FromBody] UserRequestAnswerViewModel UserRequestAnswer)
        {
            if (UserRequestAnswer == null)
            {
                return BadRequest("Vartotojo užklausa negali būti tuščia.");
            }

            try
            {
                // Validuojame duomenis prieš išsaugojimą
                var isValid = await _UserRequestAnswerLogic.CheckUserRequestAnswerDataAsync(UserRequestAnswer);
                if (!isValid)
                {
                    return BadRequest("Pateikti duomenys yra neteisingi.");
                }

                var result = await _UserRequestAnswerLogic.CreateUserRequestAnswerAsync(UserRequestAnswer);
                
                if (result)
                {
                    return CreatedAtAction(nameof(GetUserRequestAnswerById), new { id = UserRequestAnswer.IdUserRequestAnswer }, UserRequestAnswer);
                }
                
                return BadRequest("Nepavyko sukurti vartotojo užklausos.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Klaida kuriant vartotojo užklausą: {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrator, Organiser")]
        public async Task<IActionResult> UpdateUserRequestAnswer([FromBody] UserRequestAnswerViewModel UserRequestAnswer)
        {
            if (UserRequestAnswer == null)
            {
                return BadRequest("Vartotojo užklausa negali būti tuščia.");
            }

            try
            {
                var result = await _UserRequestAnswerLogic.UpdateUserRequestAnswerAsync(UserRequestAnswer);
                
                if (result)
                {
                    return NoContent();
                }
                
                return NotFound("Užklausa nerasta arba nepavyko atnaujinti.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Klaida atnaujinant vartotojo užklausą: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteUserRequestAnswer(int id)
        {
            try
            {
                var result = await _UserRequestAnswerLogic.DeleteUserRequestAnswerAsync(id);
                
                if (result)
                {
                    return NoContent();
                }
                
                return NotFound("Užklausa nerasta arba nepavyko ištrinti.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Klaida trinant vartotojo užklausą: {ex.Message}");
            }
        }
    }
}

namespace EventPlus.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionLogic _questionLogic;

        public QuestionController(IQuestionLogic questionLogic)
        {
            _questionLogic = questionLogic ?? throw new ArgumentNullException(nameof(questionLogic));
        }

        [HttpGet]
        public async Task<ActionResult<List<QuestionViewModel>>> GetAllQuestions()
        {
            try
            {
                var questions = await _questionLogic.GetAllQuestionsAsync();
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return BadRequest($"Klaida gaunant klausimus: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionViewModel>> GetQuestionById(int id)
        {
            try
            {
                var question = await _questionLogic.GetQuestionByIdAsync(id);
                if (question == null)
                {
                    return NotFound();
                }
                return Ok(question);
            }
            catch (Exception ex)
            {
                return BadRequest($"Klaida gaunant klausimą: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionViewModel question)
        {
            if (question == null)
            {
                return BadRequest("Klausimas negali būti tuščias.");
            }

            try
            {
                var result = await _questionLogic.CreateQuestionAsync(question);
                
                if (result)
                {
                    return CreatedAtAction(nameof(GetQuestionById), new { id = question.IdQuestion }, question);
                }
                
                return BadRequest("Nepavyko sukurti klausimo.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Klaida kuriant klausimą: {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateQuestion([FromBody] QuestionViewModel question)
        {
            if (question == null)
            {
                return BadRequest("Klausimas negali būti tuščias.");
            }

            try
            {
                var result = await _questionLogic.UpdateQuestionAsync(question);
                
                if (result)
                {
                    return NoContent();
                }
                
                return NotFound("Klausimas nerastas arba nepavyko atnaujinti.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Klaida atnaujinant klausimą: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            try
            {
                var result = await _questionLogic.DeleteQuestionAsync(id);
                
                if (result)
                {
                    return NoContent();
                }
                
                return NotFound("Klausimas nerastas arba nepavyko ištrinti.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Klaida trinant klausimą: {ex.Message}");
            }
        }
    }
}