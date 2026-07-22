#region Usings

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DioMinimalAPI.Dominio.DTO;
using DioMinimalAPI.Dominio.Entidades;
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
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer")
    );
});

var app = builder.Build();

#endregion Builder

#region Home

app.MapGet("/", () => Results.Json(new Home()))
   .WithTags("Home");

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
})
.WithTags("Administradores");

#endregion Administradores

#region Veículos

app.MapPost("/veiculos",
(
    [FromBody] VeiculoDTO veiculoDTO,
    [FromServices] IVeiculoServico veiculoServico
) =>
{
    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.ID}", veiculo);
})
.WithTags("Veículos");

app.MapGet("/veiculos",
(
    [FromQuery] int? pagina,
    [FromServices] IVeiculoServico veiculoServico
) =>
{
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
})
.WithTags("Veículos");

app.MapGet("/veiculos/{ID}",
(
    [FromRoute] int ID,
    [FromServices] IVeiculoServico veiculoServico
) =>
{
    var veiculo = veiculoServico.BuscaPorID(ID);

    if (veiculo == null)
        return Results.NotFound();

    return Results.Ok(veiculo);
})
.WithTags("Veículos");

app.MapPut("/veiculos/{ID}",
(
    [FromRoute] int ID,
    [FromBody] VeiculoDTO veiculoDTO,
    [FromServices] IVeiculoServico veiculoServico
) =>
{
    var veiculo = veiculoServico.BuscaPorID(ID);
    if (veiculo == null)
        return Results.NotFound();

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
})
.WithTags("Veículos");

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