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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var birthdays = await _service.GetAllAsync();
        return Ok(birthdays);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BirthdayPerson person)
    {
        var created = await _service.CreateAsync(person);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }
}