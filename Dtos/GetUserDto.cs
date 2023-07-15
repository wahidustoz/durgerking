using DurgerKing.Entity;

namespace DurgerKing.Dto;

public class GetUserDto
{
    public GetUserDto(User entity)
    {
        Id = entity.Id;
        Fullname = entity.Fullname;
        Username = entity.Username;
        Language = entity.Language;
        Phone = entity.Phone;
        CreatedAt = entity.CreatedAt;
        ModifiedAt = entity.ModifiedAt;
    }
    public long Id { get; set; }
    public string Fullname { get; set; }
    public string Username { get; set; }
    public string Language { get; set; }
    public string Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}