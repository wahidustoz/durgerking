using DurgerKing.Entity;

namespace DurgerKing.Dtos;
public class GetContactDto
{

    public GetContactDto(Contact getContactDto)
    {
        Id = getContactDto.Id;
        PhoneNumber = getContactDto.PhoneNumber;
        FirstName = getContactDto.FirstName;
        LastName = getContactDto.LastName;
        Vcard = getContactDto.Vcard;
    }
    public long Id { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Vcard { get; set; }
}