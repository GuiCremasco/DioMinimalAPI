using DioMinimalAPI.Dominio.Entidades;
using DioMinimalAPI.Dominio.Interfaces;
using DioMinimalAPI.Infraestrutura.Db;

namespace DioMinimalAPI.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{
    private readonly DbContexto _contexto;

    public VeiculoServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var query = _contexto.Veiculos.AsQueryable();

        if (string.IsNullOrEmpty(nome) == false)
        {
            query = query.Where(v => v.Nome.Contains(nome, StringComparison.CurrentCultureIgnoreCase));
        }

        if (!string.IsNullOrEmpty(marca))
        {
            query = query.Where(v => v.Marca.Contains(marca, StringComparison.CurrentCultureIgnoreCase));
        }

        if (pagina.HasValue && pagina.Value > 0)
        {
            int itensPorPagina = 10;
            query = query.Skip((pagina.Value - 1) * itensPorPagina).Take(itensPorPagina);
        }

        return query.ToList();
    }

    public Veiculo? BuscaPorID(int id)
    {
        return _contexto.Veiculos.Where(v => v.ID == id).FirstOrDefault();
    }

    public void Incluir(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        _contexto.Veiculos.Update(veiculo);
        _contexto.SaveChanges();
    }

    public void Apagar(Veiculo veiculo)
    {
        _contexto.Veiculos.Remove(veiculo);
        _contexto.SaveChanges();
    }
}
