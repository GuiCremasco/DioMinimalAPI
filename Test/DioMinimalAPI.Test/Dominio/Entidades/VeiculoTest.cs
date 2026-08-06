using DioMinimalAPI.Dominio.Entidades;

namespace DioMinimalAPI.Test.Dominio.Entidades;

[TestClass]
public class VeiculoTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        // Arrange
        var veiculo = new Veiculo();

        // Act
        veiculo.ID = 1;
        veiculo.Nome = "Carro Teste";
        veiculo.Marca = "Marca Teste";
        veiculo.Ano = 2026;

        // Assert
        Assert.AreEqual(1, veiculo.ID);
        Assert.AreEqual("Carro Teste", veiculo.Nome);
        Assert.AreEqual("Marca Teste", veiculo.Marca);
        Assert.AreEqual(2026, veiculo.Ano);
    }
}