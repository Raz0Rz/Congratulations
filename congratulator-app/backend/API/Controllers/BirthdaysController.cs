using Core;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BirthdaysController : ControllerBase
{
    private readonly IBirthdayService _service;
    private readonly ILogger<BirthdaysController> _logger;

    public BirthdaysController(IBirthdayService service, ILogger<BirthdaysController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // Получить именинника по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var birthday = await _service.GetByIdAsync(id);
            if (birthday == null) return NotFound();
            return Ok(birthday);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении именинника {Id}", id);
            return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
        }
    }

    // Получить всех именинников
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var birthdays = await _service.GetUpALLAsync();
            return Ok(birthdays);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка именинников");
            return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
        }
    }

    // Получить ближайших именинников
    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming([FromQuery] int days = 7)
    {
        try
        {
            var birthdays = await _service.GetUpAsync(days);
            return Ok(birthdays);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении ближайших ДР на {Days} дней", days);
            return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
        }
    }

    // Создать именинника
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] BirthdayPerson person, IFormFile? photo)
    {
        try
        {
            if (photo != null && photo.Length > 0){
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                person.PhotoPath = $"/uploads/{fileName}";
            }

            var created = await _service.CreateAsync(person);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании именинника");
            return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
        }
    }

    // Обновить именинника
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromForm] BirthdayPerson updatedPerson,
        IFormFile? photo)
    {
        try
        {
            // 1. Сначала получаем существующего именинника ИЗ БД
            var user = await _service.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { error = "Именинник не найден" });

            // 2. Обновляем фото (если есть)
            if (photo != null && photo.Length > 0)
            {
                // Удаляем старое фото
                if (!string.IsNullOrEmpty(user.PhotoPath))
                {
                    var oldFilePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        user.PhotoPath.TrimStart('/')
                    );
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                // Сохраняем новое фото
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                updatedPerson.PhotoPath = $"/uploads/{fileName}";
            }

            // 3. Обновляем данные в БД
            var updated = await _service.ChangeAsync(id, updatedPerson);
            if (updated == null)
                return NotFound(new { error = "Именинник не найден" });

            return Ok(updated);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении именинника {Id}", id);
            return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
        }
    }

    // Удалить именинника
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            // 1. Получаем именинника (чтобы узнать путь к фото)
            var user = await _service.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { error = "Именинник не найден" });

            // 2. Удаляем фото (если есть)
            if (!string.IsNullOrEmpty(user.PhotoPath))
            {
                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    user.PhotoPath.TrimStart('/')
                );
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            // 3. Удаляем запись из БД
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound(new { error = "Именинник не найден" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении именинника {Id}", id);
            return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
        }
    }
}