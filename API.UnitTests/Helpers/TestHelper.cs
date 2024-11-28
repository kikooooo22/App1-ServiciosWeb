namespace API.UnitTests.Helpers;
using API;
using System;
using System.Net.Http;
public class TestHelper
{
    private static readonly Lazy<TestHelper> _lazyInstance =
            new Lazy<TestHelper>(() => new TestHelper());
    public static TestHelper Instance
    {
        get
        {
            return _lazyInstance.Value;
        }
    }
    public HttpClient Client { get; set; }
    public IServiceProvider Services { get; set; }

    public TestHelper()
    {
        // Inicializa el cliente y el contenedor de servicios
        var factory = new APIWebApplicationFactory<Startup>();
        Client = factory.CreateClient();
        Services = factory.Services; // Asigna el contenedor de servicios
    }
}