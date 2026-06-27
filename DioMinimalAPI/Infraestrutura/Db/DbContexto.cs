using DioMinimalAPI.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DioMinimalAPI.Infraestrutura.Db;

public class DbContexto : DbContext
{
    private readonly IConfiguration _configuracaoAppSettings;

    public DbContexto(IConfiguration configuracaoAppSettings)
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }

    public DbSet<Administrador> Administradores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        var stringConexao = _configuracaoAppSettings.GetConnectionString("SqlServer");

        if (string.IsNullOrEmpty(stringConexao) == false)
            optionsBuilder.UseSqlServer(stringConexao);
    }
}
