using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DioMinimalAPI.Dominio.DTO;
using DioMinimalAPI.Dominio.Interfaces;
using DioMinimalAPI.Dominio.Servicos;
using DioMinimalAPI.Infraestrutura.Db;

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/login", (
    [FromBody] LoginDTO loginDTO,
    [FromServices] IAdministradorServico administradorServico) =>
{
    if (administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso!");

    return Results.Unauthorized();
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();