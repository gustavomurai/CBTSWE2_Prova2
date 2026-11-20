
// ============================================================
// PROJETO: PROVA II - CBTSWE2
// INSTITUIÇÃO: IFSP - CAMPUS CUBATÃO
//
// INTEGRANTES:
// Gustavo Cerqueira Murai - CB3033261
// Igor Cerqueira Murai - CB3033295
//
// ============================================================



using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Prova2.Api.Data;
using Prova2.Api.Models;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CONEXÃO COM O BANCO DE DADOS
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "A conexão DefaultConnection não foi encontrada.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// CONFIGURAÇÕES JWT
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key não encontrada.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer não encontrado.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT Audience não encontrado.");

// SERVIÇOS
builder.Services.AddControllers();

builder.Services.AddScoped<
    IPasswordHasher<Usuario>,
    PasswordHasher<Usuario>>();

builder.Services.AddOpenApi();

// AUTENTICAÇÃO JWT
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

                ClockSkew = TimeSpan.Zero
            };
    });

// AUTORIZAÇÃO
builder.Services.AddAuthorization();

var app = builder.Build();

// CONFIGURAÇÃO HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// IMPORTANTE: AUTENTICAÇÃO ANTES DA AUTORIZAÇÃO
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
