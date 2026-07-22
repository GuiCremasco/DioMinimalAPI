using DioMinimalAPI.Dominio.Entidades;

namespace DioMinimalAPI.Dominio.Interfaces;

public interface IVeiculoServico
{
    List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null);
    Veiculo? BuscaPorID(int id);
    void Incluir(Veiculo veiculo);
    void Atualizar(Veiculo veiculo);
    void Apagar(Veiculo veiculo);
}