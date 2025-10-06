using TellusAPI.Application.Interfaces;

namespace TellusAPI.Application.Services
{
    public class HelloWorldService : IHelloWorldService
    {
        public string GetHelloWorldMessage()
        {
            return "Hello World from Tellus API!";
        }
    }
}

