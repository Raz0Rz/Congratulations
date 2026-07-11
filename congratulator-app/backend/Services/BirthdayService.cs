using Core;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class BirthdayService : IBirthdayService
{
    private readonly AppDbContext _context;

    public BirthdayService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BirthdayPerson>> GetAllAsync()
    {
        return await _context.BirthdayPersons.ToListAsync();
    }

    public async Task<BirthdayPerson> CreateAsync(BirthdayPerson person)
    {
        person.CreatedAt = DateTime.UtcNow;
        _context.BirthdayPersons.Add(person);
        await _context.SaveChangesAsync();
        return person;
    }
}