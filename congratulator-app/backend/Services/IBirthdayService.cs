using Core;

namespace Services;

public interface IBirthdayService
{
    Task<IEnumerable<BirthdayPerson>> GetUpAsync(int day);
    Task<IEnumerable<BirthdayPerson>> GetUpALLAsync();
    Task<BirthdayPerson> CreateAsync(BirthdayPerson person);
    Task<BirthdayPerson> ChangeAsync(int id, BirthdayPerson updatedPerson);
    Task<bool> DeleteAsync(int id);
}