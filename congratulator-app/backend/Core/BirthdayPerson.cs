namespace Core;

public class BirthdayPerson
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
    public string? Comment { get; set; }
}