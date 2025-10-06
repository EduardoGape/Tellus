using Xunit;
using FluentAssertions;
using TellusAPI.Application.Services;
using System.Data;
using Moq;
using Dapper;
using TellusAPI.Domain.Entities;
using TellusAPI.Application.DTOs;

namespace TellusAPI.UnitTests.Services
{
    public class HelloWorldServiceTests
    {
        private readonly Mock<IDbConnection> _dbConnectionMock;
        private readonly HelloWorldService _helloWorldService;

        public HelloWorldServiceTests()
        {
            _dbConnectionMock = new Mock<IDbConnection>();
            _helloWorldService = new HelloWorldService(_dbConnectionMock.Object);
        }

        [Fact]
        public async Task GetHelloWorldMessage_ShouldReturnCorrectMessage()
        {
            // Act
            var result = await _helloWorldService.GetHelloWorldMessage();

            // Assert
            result.Should().Be("Hello World from Tellus API!");
        }
    }
}