using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuanLyNhaSachAPI.Models;
using QuanLyNhaSachAPI.Repositories;
using QuanLyNhaSachAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<QuanLyNhaSachContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngular", policy => {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // ép JSON trả về camelCase để khớp với Angular
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Nhập Token theo cú pháp: Bearer {chuỗi_token_của_bạn}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddScoped<IDonHangRepository, DonHangRepository>();
builder.Services.AddScoped<IDonHangService, DonHangService>();
builder.Services.AddScoped<IThongKeRepository, ThongKeRepository>();
builder.Services.AddScoped<IThongKeService, ThongKeService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IKhachHangRepository, KhachHangRepository>();
builder.Services.AddScoped<IKhachHangService, KhachHangService>();
builder.Services.AddScoped<ISachRepository, SachRepository>();
builder.Services.AddScoped<ISachService, SachService>();
builder.Services.AddScoped<ITheLoaiRepository, TheLoaiRepository>();
builder.Services.AddScoped<ITheLoaiService, TheLoaiService>();
builder.Services.AddScoped<IGioHangRepository, GioHangRepository>();
builder.Services.AddScoped<IGioHangService, GioHangService>();
builder.Services.AddScoped<IConfigRepository, ConfigRepository>();
builder.Services.AddScoped<IConfigService, ConfigService>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();