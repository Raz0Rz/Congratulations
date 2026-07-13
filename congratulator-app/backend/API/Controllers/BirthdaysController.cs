using Core;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BirthdaysController : ControllerBase
{
    private readonly IBirthdayService _service;

    public BirthdaysController(IBirthdayService service)
    {
        _service = service;
    }

    // Получить айди именинника
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try{
            var birthday = await _service.GetByIdAsync(id);
            if (birthday == null) return NotFound();
            return Ok(birthday);
        }
        catch(Exception){
            _logger.LogError(ex, "Ошибка при получении id именинника");
            return StatusCode(500, new { error = "Внутренняя ошибка сервера"});
        }
    }

    // Получить всех именинников
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try{
            var birthdays = await _service.GetUpALLAsync();
            return Ok(birthdays);
        }
        catch(Exception){
            _logger.LogError(ex, "Ошибка при получении всех именинников");
            return StatusCode(500, new { error = "Внутренняя ошибка сервера"});
        }
    }

    // Получить ближайших именинников
    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming([FromQuery] int days = 7)
    {
        try{
            var birthdays = await _service.GetUpAsync(days);
            return Ok(birthdays);
        }
        catch(Exception){
            _logger.LogError(ex, "Ошибка при получении ближайших именинников");
            return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
        }
    }

    // Создать именинника
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BirthdayPerson person)
    {
        try
        {
            var created = await _service.CreateAsync(person);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors.Errors });
        }
        catch (Exception){
            _logger.LogError(ex, "Ошибка при создании именинника");
            return (500, new { error = "Внутрення ошибка сервера" });
        }

    }

    // Обновить именинника
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] BirthdayPerson updatedPerson)
    {
        try
        {
            var updated = await _service.ChangeAsync(id, updatedPerson);
            if (updated == null)
                return NotFound(new { error = "Именинник не найден" });
            return Ok(updated);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors.Errors });
        }
        catch (Exception)
        {
            _logger.LogError(ex, "Ошибка при обновлении именинника");
            return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
        }
    }

    // Удалить именинника
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound(new { error = "Именинник не найден" });
            return NoContent();
        }
        catch (Exception)
        {   
            _logger.LogError(ex, "Ошибка при удалении именинника");
            return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
        }
    }
}