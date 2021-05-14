using Disqord.Logging;
using Microsoft.Extensions.Logging;

namespace Alexandra.Services
{
    public abstract class LexService : ILogging
    {
        public ILogger Logger { get; }
        
        protected LexService(ILogger logger)
        {
            Logger = logger;
        }
    }
}