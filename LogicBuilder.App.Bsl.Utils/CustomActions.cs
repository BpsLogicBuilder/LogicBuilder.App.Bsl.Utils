using LogicBuilder.App.Bsl.Utils.Interfaces;
using Microsoft.Extensions.Logging;

namespace LogicBuilder.App.Bsl.Utils
{
    public class CustomActions(ILogger<CustomActions> logger) : ICustomActions
    {
        private readonly ILogger<CustomActions> _logger = logger;

        public void WriteToLog(string message)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Write to log from workflow {Message}", message);
        }
    }
}
