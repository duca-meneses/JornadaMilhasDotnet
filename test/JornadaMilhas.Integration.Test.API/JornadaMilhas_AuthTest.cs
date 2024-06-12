using JornadaMilhas.API.DTO.Auth;
using System.Net;
using System.Net.Http.Json;

namespace JornadaMilhas.Integration.Test.API;

public class JornadaMilhas_AuthTest
{
    [Fact]
    public async Task POSTEfetuaLoginComSucesso()
    {

        //Arrange
        var app = new JornadaMilhasWebApplicationFactory();
        var user = new UserDTO { Email = "tester@email.com", Password = "Senha123@" };
        using var client = app.CreateClient();
        //Act
        var resultado = await client.PostAsJsonAsync("/auth-login", user);
        //Assert
        Assert.Equal(HttpStatusCode.OK, resultado.StatusCode);

    }

    [Fact]
    public async Task POSTEfetuaLoginComUserEPasswordInvalidos()
    {
        //Arrange
        var app = new JornadaMilhasWebApplicationFactory();
        var userInvalido = new UserDTO { Email = "", Password = "" };
        using var client = app.CreateClient();
        //Act
        var resultado = await client.PostAsJsonAsync("/auth-login", userInvalido);
        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, resultado.StatusCode);
    }
}