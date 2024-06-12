using JornadaMilhas.Dominio.Entidades;
using JornadaMilhas.Dominio.ValueObjects;
using JornadaMilhas.Integration.Test.API.DataBuilders;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace JornadaMilhas.Integration.Test.API;
public class OfertaViagem_GET : IClassFixture<JornadaMilhasWebApplicationFactory>
{
    private readonly JornadaMilhasWebApplicationFactory app;

    public OfertaViagem_GET(JornadaMilhasWebApplicationFactory app)
    {
        this.app = app;
    }

    [Fact]
    public async Task RecuperaOfertaViagemPorId()
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

        //Act
        var response = await client.GetFromJsonAsync<OfertaViagem>("/ofertas-viagem/" + ofertaExistente.Id);
        //Assert
        Assert.NotNull(response);
        Assert.Equal(ofertaExistente.Preco, response.Preco, 0.001);
        Assert.Equal(ofertaExistente.Rota.Origem, response.Rota.Origem);
        Assert.Equal(ofertaExistente.Rota.Destino, response.Rota.Destino);
    }

    [Fact]
    public async Task RecuperarOfertaViagemNaConsultaPaginada()
    {
        //Arrange
        var ofertaDataBuilder = new OfertaViagemDataBuilder();
        var listaDeOfertas = ofertaDataBuilder.Generate(80);
        app.Context.OfertasViagem.AddRange(listaDeOfertas);
        app.Context.SaveChanges();
        using var client = await app.GetClientWithAccessTokenAsync();

        int pagina = 1;
        int tamanhoPorPagina = 80;

        //Act
        var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem?pagina={pagina}&tamanhoPorPagina={tamanhoPorPagina}");

        //Assert
        Assert.True(response != null );
        Assert.Equal(tamanhoPorPagina, response.Count);
    }

    [Fact]
    public async Task RecuperarOfertaViagemNaConsultaNaUltimaPagina()
    {
        //Arrange
            //Limpando a base de dados
        app.Context.Database.ExecuteSqlRaw("Delete From OfertasViagem");

        var ofertaDataBuilder = new OfertaViagemDataBuilder();
        var listaDeOfertas = ofertaDataBuilder.Generate(80);
        app.Context.OfertasViagem.AddRange(listaDeOfertas);
        app.Context.SaveChanges();
        using var client = await app.GetClientWithAccessTokenAsync();

        int pagina = 4;
        int tamanhoPorPagina = 25;

        //Act
        var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem?pagina={pagina}&tamanhoPorPagina={tamanhoPorPagina}");

        //Assert
        Assert.NotNull(response);
        Assert.Equal(5, response.Count());
    }

    [Fact]
    public async Task RecuperarOfertaViagemNaConsultaComPaginaInexistente()
    {
        //Arrange
        //Limpando a base de dados
        app.Context.Database.ExecuteSqlRaw("Delete From OfertasViagem");

        var ofertaDataBuilder = new OfertaViagemDataBuilder();
        var listaDeOfertas = ofertaDataBuilder.Generate(80);
        app.Context.OfertasViagem.AddRange(listaDeOfertas);
        app.Context.SaveChanges();
        using var client = await app.GetClientWithAccessTokenAsync();

        int pagina = 5;
        int tamanhoPorPagina = 25;

        //Act
        var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem?pagina={pagina}&tamanhoPorPagina={tamanhoPorPagina}");

        //Assert
        Assert.NotNull(response);
        Assert.Equal(0, response.Count());
    }

    [Fact]
    public async Task RecuperarOfertaViagemNaConsultaComPaginaComValorNegativo()
    {
        //Arrange
        //Limpando a base de dados
        app.Context.Database.ExecuteSqlRaw("Delete From OfertasViagem");

        var ofertaDataBuilder = new OfertaViagemDataBuilder();
        var listaDeOfertas = ofertaDataBuilder.Generate(80);
        app.Context.OfertasViagem.AddRange(listaDeOfertas);
        app.Context.SaveChanges();
        using var client = await app.GetClientWithAccessTokenAsync();

        int pagina = -5;
        int tamanhoPorPagina = 25;

        //Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem?pagina={pagina}&tamanhoPorPagina={tamanhoPorPagina}");
        });
        
    }
}

