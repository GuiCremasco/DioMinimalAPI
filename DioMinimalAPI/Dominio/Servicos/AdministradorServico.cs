using DioMinimalAPI.Dominio.DTO;
using DioMinimalAPI.Dominio.Entidades;
using DioMinimalAPI.Dominio.Interfaces;
using DioMinimalAPI.Infraestrutura.Db;

namespace DioMinimalAPI.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;

    public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        return _contexto.Administradores
                        .FirstOrDefault(
                            a => a.Email == loginDTO.Email
                                 &&
                                 a.Senha == loginDTO.Senha
                        );
    }
}
