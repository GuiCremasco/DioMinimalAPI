using DioMinimalAPI.Dominio.DTO;
using DioMinimalAPI.Dominio.Entidades;

namespace DioMinimalAPI.Dominio.Interfaces;

public interface IAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);
}
