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

public async Task<IEnumerable<BirthdayPerson>> GetUpAsync(int day){
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

public async Task<IEnumerable<BirthdayPerson>> GetUpALLAsync(){
    return await _context.BirthdayPersons.ToListAsync();
}

public async Task<BirthdayPerson> CreateAsync(BirthdayPerson person){
    _context.BirthdayPersons.Add(person);
    await _context.SaveChangesAsync();
    return person;
}

public async Task<BirthdayPerson> ChangeAsync(int id, BirthdayPerson updatedPerson){
    var User = await _context.BirthdayPersons
    .FirstOrDefaultAsync(p => p.Id == id);

    if(User == null) return null;
    
    User.FirstName = updatedPerson.FirstName;
    User.LastName = updatedPerson.LastName;
    User.BirthDate = updatedPerson.BirthDate;
    User.PhotoPath = updatedPerson.PhotoPath;
    User.CreateTime = updatedPerson.CreateTime;

    await _context.SaveChangesAsync();

    return User;
}

public async Task<bool> DeleteAsync(int id){
    var User = await _context.BirthdayPersons
    .FirstOrDefaultAsync(p => p.Id == id);

    if(User == null) return false;

    _context.BirthdayPersons.Remove(user);
    await _context.SaveChangesAsync();

    return true;
}