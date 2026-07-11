using Core;

namespace Services;

public interface IBirthdayService
{
    Task<IEnumerable<BirthdayPerson>> GetAllAsync();
    Task<BirthdayPerson> CreateAsync(BirthdayPerson person);
}