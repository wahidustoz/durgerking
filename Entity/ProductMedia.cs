public class ProductMedia
{
    public Guid Id { get; set; }
    public string MimeType { get; set; } // mp4 yoki shunga o'xshash file turi haqida malumot
    public string Filename { get; set; } // user tomonidan berilgan fayl nomi
    public string Extension{ get; set; } // user tomonidan berilgan fayl nomi
    public int Order { get; set; }  // maxsulot rasmi nechinchi ochertda ko'rsatilishi 0-based index
    public byte[] Data { get; set; } // byte array malumot. Max 5MB

    public virtual Product Product { get; set; } // qaysi maxsulot tegishli ekani haqida
}