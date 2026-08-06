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

    public Administrador Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        return administrador;
    }

    public List<Administrador> Todos(int? pagina)
    {
        var query = _contexto.Administradores.AsQueryable();

        if (pagina.HasValue && pagina.Value > 0)
        {
            int itensPorPagina = 10;
            query = query.Skip((pagina.Value - 1) * itensPorPagina).Take(itensPorPagina);
        }

        return query.ToList();
    }

    public Administrador? BuscaPorID(int id)
    {
        return _contexto.Administradores.FirstOrDefault(a => a.ID == id);
    }
}
