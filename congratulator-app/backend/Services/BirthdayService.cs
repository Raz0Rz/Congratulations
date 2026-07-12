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

    public async Task<IEnumerable<BirthdayPerson>> GetUpAsync(int day){
        var today = DateOnly.FromDateTime(DateTime.Today);
        var futureDate = today.AddDays(day);
        int daysInYear = DateTime.IsLeapYear(today.Year) ? 366 : 365;

        if (today.Year == futureDate.Year){
            var listUser = await _context.BirthdayPersons
            .Where(p => p.BirthDate.DayOfYear >= today.DayOfYear &&
            p.BirthDate.DayOfYear  <= futureDate.DayOfYear)
            .ToListAsync();
            return listUser;
        }
        else{
            var listUser = await _context.BirthdayPersons
            .Where(p => p.BirthDate.DayOfYear + daysInYear >= today.DayOfYear &&
            p.BirthDate.DayOfYear + daysInYear <= futureDate.DayOfYear + daysInYear)
            .ToListAsync();
            return listUser;
        }
    }

    public async Task<IEnumerable<BirthdayPerson>> GetUpALLAsync(){
        return await _context.BirthdayPersons.ToListAsync();
    }

    public async Task<BirthdayPerson> CreateAsync(BirthdayPerson person){

        var errors = ValidateBirthdayPerson(person);

        if(errors.HasErrors){
            throw new ValidationException(errors);
        }

        _context.BirthdayPersons.Add(person);
        await _context.SaveChangesAsync();
        return person;
    }

    public async Task<BirthdayPerson> ChangeAsync(int id, BirthdayPerson updatedPerson){
        var errors = ValidateBirthdayPerson(updatedPerson);

        if(errors.HasErrors){
            throw new ValidationException(errors);
        }

        var User = await GetByIdAsync(id);

        if(User == null) return null;
        
        User.FirstName = updatedPerson.FirstName;
        User.LastName = updatedPerson.LastName;
        User.BirthDate = updatedPerson.BirthDate;
        User.PhotoPath = updatedPerson.PhotoPath;

        await _context.SaveChangesAsync();

        return User;
    }

    public async Task<bool> DeleteAsync(int id){
        var User = await GetByIdAsync(id);

        if(User == null) return false;

        _context.BirthdayPersons.Remove(User);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<BirthdayPerson?> GetByIdAsync(int id){
        return await _context.BirthdayPersons.FirstOrDefaultAsync(p => p.Id == id);
    }

    private ValidationError ValidateBirthdayPerson(BirthdayPerson person)
    {
        var errors = new ValidationError();

        if (string.IsNullOrWhiteSpace(person.FirstName))
            errors.AddError("Имя обязательно для заполнения");
        else if (person.FirstName.Length > 100)
            errors.AddError("Имя не должно превышать 100 символов");

        if (string.IsNullOrWhiteSpace(person.LastName))
            errors.AddError("Фамилия обязательна для заполнения");
        else if (person.LastName.Length > 100)
            errors.AddError("Фамилия не должна превышать 100 символов");

        if (person.BirthDate == default)
            errors.AddError("Дата рождения обязательна для заполнения");

        if (person.BirthDate > DateOnly.FromDateTime(DateTime.Today))
            errors.AddError("Дата рождения не может быть в будущем");

        if (person.BirthDate.Year < 1900)
            errors.AddError("Год рождения должен быть не ранее 1900");

        return errors;
    }
}