namespace API.UnitTests.Controllers;
using API.DTOs;
using API.UnitTests.Helpers;
using Newtonsoft.Json;

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class AccountControllerTests : IClassFixture<TestHelper>
{
    private readonly HttpClient _client;

    public AccountControllerTests(TestHelper helper)
    {
        _client = helper.Client;
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenUsernameExists()
    {
         // Hacer una petición de registro con un nombre de usuario ya existente
        var registerRequest = new RegisterRequest
        {
            Username = "arenita",
            Password = "123456"
        };

        var content = new StringContent(JsonConvert.SerializeObject(registerRequest), Encoding.UTF8, "application/json");

        // Act: Hacer la solicitud POST
        var response = await _client.PostAsync("/api/account/register", content);

        // Assert: Verificar que el estado de la respuesta sea 400 y que el mensaje sea "Username already exist"
        Assert.Equal("BadRequest", response.StatusCode.ToString());
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Username already exist", responseContent);
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenUsernameIsNew()
    {
        var registerRequest = new RegisterRequest
        {
            Username = "ElAlextar",
            Password = "123456"
        };

        var content = new StringContent(JsonConvert.SerializeObject(registerRequest), Encoding.UTF8, "application/json");

        // Act: Hacer la solicitud POST
        var response = await _client.PostAsync("/api/account/register", content);

        // Assert: Verificar que el estado de la respuesta sea 200 (OK)
        Assert.Equal("OK", response.StatusCode.ToString());
    }

    [Fact]
    public async Task LoginAsync_ReturnsUnauthorized_WhenUserNotFound()
    {
        // Arrange: Simular una solicitud de login con un nombre de usuario que no existe en la base de datos
        var loginRequest = new LoginRequest
        {
            Username = "nonexistentuser",
            Password = "123456"
        };

        var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

        // Act: Hacer la solicitud POST al endpoint /api/account/login
        var response = await _client.PostAsync("/api/account/login", content);

        // Assert: Verificar que el estado de la respuesta sea 401 Unauthorized
        Assert.Equal("Unauthorized", response.StatusCode.ToString());

        // Verificar que el mensaje de error sea el esperado
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid username or password", responseContent);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenPasswordIsIncorrect()
    {
        // Arrange: Crear una solicitud de login con una contraseña incorrecta
        var loginRequest = new LoginRequest
        {
            Username = "arenita",
            Password = "wrongpassword"  // Contraseña incorrecta
        };

        var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

        // Act: Hacer la solicitud POST
        var response = await _client.PostAsync("/api/account/login", content);

        // Assert: Verificar que el estado de la respuesta sea 401 (Unauthorized)
        Assert.Equal("Unauthorized", response.StatusCode.ToString());

        // Verifica que el mensaje de error contenga "Invalid username or password"
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid username or password", responseContent);
    }

}

