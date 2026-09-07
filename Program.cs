// ===== USING: Import namespace yang dibutuhkan =====
using appOne.Data;                                     // AppDbContext
using appOne.Services;                                 // IRoleService, IMenuService, IAuthService, dst
using appOne.Middleware;                               // ExceptionHandlingMiddleware
using appOne.Helpers;                                  // JwtSettings
using Microsoft.EntityFrameworkCore;                   // UseNpgsql, UseSnakeCaseNamingConvention
using Microsoft.AspNetCore.Authentication.JwtBearer;    // AddJwtBearer, JwtBearerDefaults
using Microsoft.IdentityModel.Tokens;                  // TokenValidationParameters, SymmetricSecurityKey
using System.Text;                                     // Encoding.UTF8
using System.Text.Json;                                // JsonSerializer, dipakai di custom Events di bawah

// Titik masuk aplikasi: builder dipakai untuk mendaftarkan semua service
// sebelum aplikasi benar-benar dibuat/dijalankan.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Mendaftarkan layanan untuk generate skema OpenAPI (dokumentasi endpoint otomatis)
builder.Services.AddOpenApi();

// Mendaftarkan AppDbContext ke DI Container, supaya bisa "disuntikkan" (inject)
// ke Service manapun yang butuh akses database.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")) // pakai provider PostgreSQL, connection string diambil dari appsettings.json
           .UseSnakeCaseNamingConvention());            // otomatis mapping PascalCase (C#) <-> snake_case (kolom database)

// ===== KONFIGURASI JWT =====

// Mengikat isi section "Jwt" di appsettings.json ke class JwtSettings,
// supaya nanti bisa diambil lewat IOptions<JwtSettings> di Service manapun.
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Ambil nilai JwtSettings sekarang juga (bukan lewat DI), karena dibutuhkan
// langsung di sini untuk konfigurasi validasi token di bawah.
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;

// Mendaftarkan sistem autentikasi aplikasi, dengan skema default JWT Bearer
// (artinya: token dikirim client lewat header "Authorization: Bearer <token>").
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Aturan validasi token yang diterima dari client
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,                          // cek token dibuat oleh issuer yang benar
        ValidateAudience = true,                         // cek token ditujukan untuk audience yang benar
        ValidateLifetime = true,                         // cek token belum kadaluarsa
        ValidateIssuerSigningKey = true,                 // cek tanda tangan token cocok dengan Key rahasia kita
        ValidIssuer = jwtSettings.Issuer,                 // nilai issuer yang valid (dari appsettings.json)
        ValidAudience = jwtSettings.Audience,             // nilai audience yang valid
        IssuerSigningKey = new SymmetricSecurityKey(      // key rahasia untuk verifikasi tanda tangan token
            Encoding.UTF8.GetBytes(jwtSettings.Key))
    };

    // Tambahan: secara default, kalau token tidak ada/invalid/expired atau role tidak sesuai,
    // ASP.NET Core cuma balikin status code doang tanpa body JSON. Di sini kita timpa
    // supaya client selalu dapat pesan error yang jelas dalam bentuk JSON.
    options.Events = new JwtBearerEvents
    {
        // Dipanggil saat token tidak ada / tidak valid / sudah kadaluarsa (401)
        OnChallenge = context =>
        {
            context.HandleResponse(); // matikan behavior default (yang bikin body kosong)
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new { error = "Token tidak valid, kadaluarsa, atau tidak disertakan." });
            return context.Response.WriteAsync(result);
        },
        // Dipanggil saat token valid tapi role/klaim tidak memenuhi syarat [Authorize(Roles = "...")] (403)
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new { error = "Anda tidak memiliki akses untuk resource ini." });
            return context.Response.WriteAsync(result);
        }
    };
});

// Mengaktifkan sistem otorisasi (dipakai oleh atribut [Authorize] di Controller)
builder.Services.AddAuthorization();

// Mendaftarkan dukungan Controller (MVC-style routing, dipakai oleh semua file di folder Controllers/)
builder.Services.AddControllers();

// Mendaftarkan tiap pasangan interface+implementasi Service ke DI Container.
// Artinya: kalau ada Controller yang minta IRoleService lewat constructor,
// DI Container otomatis kasih instance RoleService.
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IAuthService, AuthService>();   // service untuk login & register JWT

// Setelah semua service didaftarkan, bangun aplikasi (app) dari builder.
// Setelah baris ini, tidak bisa lagi menambah service baru ke builder.Services.
var app = builder.Build();

// Middleware pertama dalam pipeline: menangkap semua exception dari middleware/Controller
// di bawahnya, supaya error selalu balik sebagai JSON yang rapi, bukan stack trace mentah.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
// Hanya aktifkan endpoint dokumentasi OpenAPI kalau environment-nya Development
// (tidak di-expose saat aplikasi jalan di production).
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Redirect otomatis dari HTTP ke HTTPS
app.UseHttpsRedirection();

// Mengaktifkan proses autentikasi: tiap request dicek apakah bawa token JWT yang valid
// di header Authorization. HARUS sebelum UseAuthorization().
app.UseAuthentication();

// Mengaktifkan proses otorisasi: menentukan apakah user (dari token yang sudah divalidasi
// di atas) diizinkan mengakses endpoint tertentu, berdasarkan atribut [Authorize] di Controller.
app.UseAuthorization();

// Mengaktifkan routing ke semua Controller yang sudah didaftarkan (RolesController,
// MenuController, AuthController, dst) sesuai atribut [Route] masing-masing.
app.MapControllers();

// ===== Kode bawaan template (boleh dihapus kapan saja, tidak dipakai) =====

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Contoh endpoint minimal API bawaan template .NET, di luar Controller.
app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Menjalankan aplikasi, mulai "mendengarkan" request masuk di port yang sudah dikonfigurasi.
app.Run();

// Record bawaan template untuk bentuk data weather forecast di atas.
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}