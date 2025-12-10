using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS İZNİ (HTML dosyanın buraya erişebilmesi için şart)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        b => b.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// CORS'u aktif et
app.UseCors("AllowAll");

// 2. SANAL VERİTABANI (RAM üzerinde çalışır)
// Gerçek projede burası SQL veritabanı olacak.
var users = new List<User>(); 

// --- ENDPOINTLER (API Uçları) ---

// KAYIT OLMA (Register)
app.MapPost("/api/auth/register", ([FromBody] User newUser) =>
{
    // Kullanıcı zaten var mı kontrol et
    var exists = users.Any(u => u.Username == newUser.Username);
    if (exists) return Results.BadRequest(new { message = "Bu kullanıcı adı zaten alınmış." });

    // Listeye ekle
    users.Add(newUser);
    
    Console.WriteLine($"Yeni Kayıt: {newUser.Username}"); // Terminale log bas
    return Results.Ok(new { message = "Kayıt başarılı!" });
});

// GİRİŞ YAPMA (Login)
app.MapPost("/api/auth/login", ([FromBody] User loginUser) =>
{
    // Kullanıcıyı ve şifreyi kontrol et
    var user = users.FirstOrDefault(u => u.Username == loginUser.Username && u.Password == loginUser.Password);

    if (user == null)
    {
        return Results.Unauthorized(); // 401 Hatası döner
    }

    // Basit bir Token üret (Gerçekte burası JWT olur)
    var fakeToken = Guid.NewGuid().ToString();

    return Results.Ok(new { token = fakeToken, username = user.Username });
});

app.Run();

// 3. KULLANICI MODELİ (C'deki struct gibi düşün)
class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}