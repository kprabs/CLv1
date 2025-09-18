using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CoreLib.Application.Common.Utility
{
    static partial class BlockStopwatchLogMessages
    {
        [LoggerMessage(Level = LogLevel.Information, EventId = 1, Message = "Block Duration: blockName = {blockName} elapsedMs = {elapsedMs}")]
        internal static partial void BlockDuration(this ILogger logger, string blockName, double elapsedMs);
    }

    internal class BlockStopwatch : IDisposable
    {
        bool disposedValue;

        readonly string _block;
        readonly ILogger _logger;
        readonly Stopwatch _stopwatch;

        public BlockStopwatch(string block, ILogger logger)
        {
            _block = block;
            _logger = logger;
            _stopwatch = Stopwatch.StartNew();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _stopwatch.Stop();
                _logger.BlockDuration(_block, _stopwatch.ElapsedMilliseconds);

                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}