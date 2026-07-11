namespace Core;

public class ValidationError
{
    // Список ошибок
    public List<string> Errors { get; set; } = new List<string>();

    // Проверяем, есть ли ошибки (если Count > 0 — значит есть)
    public bool HasErrors => Errors.Count > 0;

    // Метод для добавления ошибки
    public void AddError(string errorMessage)
    {
        Errors.Add(errorMessage);
    }
}