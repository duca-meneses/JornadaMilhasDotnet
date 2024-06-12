using JornadaMilhas.Dominio.Entidades;
using JornadaMilhas.Dominio.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace JornadaMilhas.Integration.Test.API;
public class OfertaViagem_POST : IClassFixture<JornadaMilhasWebApplicationFactory>
{
    private readonly JornadaMilhasWebApplicationFactory app;

    public OfertaViagem_POST(JornadaMilhasWebApplicationFactory app)
    {
        this.app = app;
    }

    [Fact]
    public async Task CadastraOfertaViagemComAutorizacao()
    {
        //Arrange
        using var client = await app.GetClientWithAccessTokenAsync();
        var ofertaViagem = new OfertaViagem()
        {
            Preco = 100,
            Rota = new Rota("Teresina", "Fortaleza"),
            Periodo = new Periodo(DateTime.Parse("2024-06-11"), DateTime.Parse("2024-06-20"))
        };
        //Act
        var response = await client.PostAsJsonAsync("/ofertas-viagem", ofertaViagem);
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CadastraOfertaViagemSemAutorizacao()  
    {
        //Arrange
        using var client = app.CreateClient();
        var ofertaViagem = new OfertaViagem()
        {
            Preco = 100,
            Rota = new Rota("Teresina", "Fortaleza"),
            Periodo = new Periodo(DateTime.Parse("2024-06-11"), DateTime.Parse("2024-06-20"))
        };
        //Act
        var response = await client.PostAsJsonAsync("/ofertas-viagem", ofertaViagem);
        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
