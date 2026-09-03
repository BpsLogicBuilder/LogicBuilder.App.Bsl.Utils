using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace LogicBuilder.App.Bsl.Utils.Tests
{
    public class CustomActionsTest
    {
        private readonly Mock<ILogger<CustomActions>> _logger;
        private readonly CustomActions _itemToTest;

        public CustomActionsTest()
        {
            _logger = new Mock<ILogger<CustomActions>>();
            _itemToTest = new CustomActions(_logger.Object);
        }

        [Fact]
        public void InformationLoggedWhenEnabled()
        {
            //arrange
            _logger.Setup(l => l.IsEnabled(LogLevel.Information)).Returns(true);

            //act
            _itemToTest.WriteToLog("Log Message");

            //assert
#pragma warning disable CA1873 // Avoid potentially expensive logging
            _logger.Verify
            (
                l => l.Log
                (
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Write to log from workflow Log Message")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!
                ), 
                Times.Once
            );
#pragma warning restore CA1873 // Avoid potentially expensive logging
        }

        [Fact]
        public void InformationNotLoggedWhenDiabled()
        {
            //arrange
            _logger.Setup(l => l.IsEnabled(LogLevel.Information)).Returns(false);

            //act
            _itemToTest.WriteToLog("Log Message");

            //assert
#pragma warning disable CA1873 // Avoid potentially expensive logging
            _logger.Verify
            (
                l => l.Log
                (
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Write to log from workflow Log Message")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!
                ),
                Times.Never
            );
#pragma warning restore CA1873 // Avoid potentially expensive logging
        }
    }
}
