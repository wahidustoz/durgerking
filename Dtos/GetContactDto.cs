using DurgerKing.Entity;

namespace DurgerKing.Dtos;
public class GetContactDto
{
    public GetContactDto(Contact contact)
    {
        Id = contact.Id;
        PhoneNumber = contact.PhoneNumber;
        FirstName = contact.FirstName;
        LastName = contact.LastName;
        Vcard = contact.Vcard;
    }

    public long Id { get; set; }
    public string PhoneNumber { get; set; }
    public string  FirstName { get; set; }
    public string LastName { get; set; }
    public string Vcard { get; set; }
}