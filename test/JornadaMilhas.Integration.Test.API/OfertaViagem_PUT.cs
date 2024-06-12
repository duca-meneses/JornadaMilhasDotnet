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
public class OfertaViagem_PUT : IClassFixture<JornadaMilhasWebApplicationFactory>
{
    private readonly JornadaMilhasWebApplicationFactory app;

    public OfertaViagem_PUT(JornadaMilhasWebApplicationFactory app)
    {
        this.app = app;
    }

    [Fact]
    public async Task AtualizaOfertaViagem()   
    {
        //Arrange
        var ofertaExistente = app.Context.OfertasViagem.FirstOrDefault();
        if (ofertaExistente is null)
        {
            ofertaExistente = new OfertaViagem()
            {
                Preco = 100,
                Rota = new Rota("Teresina", "Fortaleza"),
                Periodo = new Periodo(DateTime.Parse("2024-06-12"), DateTime.Parse("2024-06-20"))
            };
            app.Context.Add(ofertaExistente);
            app.Context.SaveChanges();
        }
        using var client = await app.GetClientWithAccessTokenAsync();

        ofertaExistente.Rota.Origem = "Brasilia";
        ofertaExistente.Rota.Destino = "Orlando";

        //Act
        var response = await client.PutAsJsonAsync($"/ofertas-viagem/", ofertaExistente);
        //Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    }
}
