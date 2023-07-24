# Durger King (fastfood chain)


## Bot

#### `BotBackroundService.cs`
- bu hosted service `UpdateHanler`ni `BotClient`ga registratsiya qiladi.
- dastur boshida bir marta `ExecuteAsync` methodi Host Server (ya'ni bizni app) tomonidan chaqiriladi

#### `UpdateHandler.cs`
- bu class bot serverdan kelgan `update`larni protses qiladi
- 2 ta asosiy methodi bor: `HandlePollingErrorAsync()` va `HandleUpdateAsync()`
- `HandlePollingErrorAsync()` -> bot serverdan kelgan habarni ishlov berish jarayonida hatolik bo'lsa shu method chaqiriladi
    - uni ichida hatolikni log qilish va kerak bo'lsa client'ga biror habar yuborish mumkin.
- `HandleUpdateAsync()` -> bot serverdan kelgan update'ni turiga qarab protses qiladi. har xil update turlari uchun alohida method yaratganmiz
- UpdateHandler klasi bot server bilan muloqot qiladigan asosiy klas bo'lgani uchun uni hajmi juda kattalashib ketadi.
    - uni oldini olish uchun partial klaslarga ajratganmiz
    - `UpdateHandler.Message.cs` partial klasi Message turidagi habarlarni hal qiladi
    - `UpdateHandler.CallbackQuery.cs` partial klasi CallbackQuery turigadi habarlarni hal qiladi

#### Data va Entity
- ORM sifatida EF Core ishlatamiz
- Entity schemasini Fluent Configuration orqali boshqaramiz
    - entity fluent configuration'ni `AppDbContext` ichida yozamiz
- kelajakda Unit Testing qilishga imkon qoldirish uchun `AppDbContext` ga `IAppDbContext` deb nomlangan interface yaratib o'sha orqali murojat qilamiz

#### API 
- kelajakda adminlar bot faoliyatini boshqara olishi uchun API qurilgan
- u orqali bot yaratgan ma'lumotlarni tekshirsa bo'ladi va o'zgartirsa bo'ladi
- bundan tashqari Productlar qo'shsa boladi
- API ga kelayotgan ma'lumotlarni validatsiya qilish uchun FluentValidatoin ishlatamiz.
- FluentValidation Automatic validatsiya qila olmagani uchun, kichik action filter yozib uni automatic validation qiladigan qilganmiz
    - `/Filters/AsyncFluentAutoValidation.cs` ichiga qarang
    - validator classlarni `/Validators` papkasidan topasiz