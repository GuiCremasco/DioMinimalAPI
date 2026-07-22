#region Usings

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DioMinimalAPI.Dominio.DTO;
using DioMinimalAPI.Dominio.Interfaces;
using DioMinimalAPI.Dominio.ModelViews;
using DioMinimalAPI.Dominio.Servicos;
using DioMinimalAPI.Infraestrutura.Db;

#endregion Usings

#region Builder

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer")
    );
});

var app = builder.Build();

#endregion Builder

#region Home

app.MapGet("/", () => Results.Json(new Home()));

#endregion Home

#region Administradores

app.MapPost("/administradores/login",
(
    [FromBody] LoginDTO loginDTO,
    [FromServices] IAdministradorServico administradorServico
) =>
{
    if (administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso!");

    return Results.Unauthorized();
});

#endregion Administradores

#region Veículos


#endregion Veículos

#region App

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

#endregion App