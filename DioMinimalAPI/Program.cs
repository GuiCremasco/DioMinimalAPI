#region Usings

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using DioMinimalAPI.Dominio.DTO;
using DioMinimalAPI.Dominio.Entidades;
using DioMinimalAPI.Dominio.Enums;
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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no campo abaixo:"
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement()
    {
        [new OpenApiSecuritySchemeReference("Bearer", doc)] = []
    });
});

// Add JWT Bearer authentication.
string jwtKey = builder.Configuration.GetSection("Jwt")["Key"]?.ToString() ?? "123456";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

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
   .AllowAnonymous()
   .WithTags("Home");

#endregion Home

#region Administradores

string GerarTokenJwt(Administrador administrador)
{
    if (string.IsNullOrWhiteSpace(jwtKey))
        return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new("Email", administrador.Email),
        new("Perfil", administrador.Perfil)
    };

    var token = new JwtSecurityToken(expires: DateTime.Now.AddDays(1),
                                     signingCredentials: credentials,
                                     claims: claims);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/administradores/login",
(
    [FromBody] LoginDTO loginDTO,
    [FromServices] IAdministradorServico administradorServico
) =>
{
    var administrador = administradorServico.Login(loginDTO);

    if (administrador != null)
    {
        string token = GerarTokenJwt(administrador);

        return Results.Ok(new AdministradorLogado()
        {
            Email = administrador.Email,
            Perfil = administrador.Perfil,
            Token = token
        });
    }

    return Results.Unauthorized();
})
.AllowAnonymous()
.WithTags("Administradores");

ErrosValidacao ValidaAdministradorDTO(AdministradorDTO administradorDTO)
{
    var validacao = new ErrosValidacao()
    {
        Mensagens = []
    };

    if (string.IsNullOrWhiteSpace(administradorDTO.Email))
        validacao.Mensagens.Add("O email não pode ficar em branco.");

    if (string.IsNullOrWhiteSpace(administradorDTO.Senha))
        validacao.Mensagens.Add("A senha não pode ficar em branco.");

    if (administradorDTO.Perfil == null)
        validacao.Mensagens.Add("O perfil não pode ficar em branco.");

    return validacao;
}

app.MapPost("/administradores",
(
    [FromBody] AdministradorDTO administradorDTO,
    [FromServices] IAdministradorServico administradorServico
) =>
{
    var validacao = ValidaAdministradorDTO(administradorDTO);

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var administrador = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };

    administradorServico.Incluir(administrador);

    var administradorMV = new AdministradorModelView
    {
        ID = administrador.ID,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    };

    return Results.Created($"/administrador/{administrador.ID}", administradorMV);
})
.RequireAuthorization()
.WithTags("Administradores");

app.MapGet("/administradores",
(
    [FromQuery] int? pagina,
    [FromServices] IAdministradorServico administradorServico
) =>
{
    var administradoresMV = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);

    foreach (var administrador in administradores)
    {
        administradoresMV.Add(new AdministradorModelView
        {
            ID = administrador.ID,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
    }

    return Results.Ok(administradoresMV);
})
.RequireAuthorization()
.WithTags("Administradores");

app.MapGet("/administradores/{ID}",
(
    [FromRoute] int ID,
    [FromServices] IAdministradorServico administradorServico
) =>
{
    var administrador = administradorServico.BuscaPorID(ID);

    if (administrador == null)
        return Results.NotFound();

    var administradorMV = new AdministradorModelView
    {
        ID = administrador.ID,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    };

    return Results.Ok(administradorMV);
})
.RequireAuthorization()
.WithTags("Administradores");

#endregion Administradores

#region Veículos

ErrosValidacao ValidaVeiculoDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosValidacao()
    {
        Mensagens = []
    };

    if (string.IsNullOrWhiteSpace(veiculoDTO.Nome))
        validacao.Mensagens.Add("O nome não pode ficar em branco.");

    if (string.IsNullOrWhiteSpace(veiculoDTO.Marca))
        validacao.Mensagens.Add("A marca não pode ficar em branco.");

    if (veiculoDTO.Ano <= 1950)
        validacao.Mensagens.Add("Veículo muito antigo, aceito somente anos superiores a 1950.");

    return validacao;
}

app.MapPost("/veiculos",
(
    [FromBody] VeiculoDTO veiculoDTO,
    [FromServices] IVeiculoServico veiculoServico
) =>
{
    var validacao = ValidaVeiculoDTO(veiculoDTO);

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.ID}", veiculo);
})
.RequireAuthorization()
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
.RequireAuthorization()
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
.RequireAuthorization()
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

    var validacao = ValidaVeiculoDTO(veiculoDTO);

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
})
.RequireAuthorization()
.WithTags("Veículos");

app.MapDelete("/veiculos/{ID}",
(
    [FromRoute] int ID,
    [FromServices] IVeiculoServico veiculoServico
) =>
{
    var veiculo = veiculoServico.BuscaPorID(ID);
    if (veiculo == null)
        return Results.NotFound();

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
})
.RequireAuthorization()
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

app.UseAuthentication();
app.UseAuthorization();

app.Run();

#endregion App