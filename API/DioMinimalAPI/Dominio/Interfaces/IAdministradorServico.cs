using DioMinimalAPI.Dominio.DTO;
using DioMinimalAPI.Dominio.Entidades;

namespace DioMinimalAPI.Dominio.Interfaces;

public interface IAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);
    Administrador Incluir(Administrador administrador);
    List<Administrador> Todos(int? pagina);
    Administrador? BuscaPorID(int id);
}
