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

public async Task <IEnumerable<BirthdayPerson>> GetUpAsync (int day){
    var today = DateOnly.FromDateTime(DateTime.Today);
    var futureDate = today.AddDays(days);
    int daysInYear = DateTime.IsLeapYear(today.Year) ? 366 : 365;

    if (today.Year == futureDate.Year){
        var listUser = await _context.BirthdayPersons
        .Where(p => p.BirthDate.DayOfYear >= today.DayOfYear &&
        p.BirthDate.DayOfYear  <= futureDate.DayOfYear)
        .ToListAsync();
    }
    else{
        var listUser = await _context.BirthdayPersons
        .Where(p => p.BirthDate.DayOfYear + daysInYear >= today.DayOfYear &&
        p.BirthDate.DayOfYear + daysInYear <= futureDate.DayOfYear + daysInYear)
        .ToListAsync();
    }

    return listUser;
}