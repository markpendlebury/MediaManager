
using Serilog;
using service.mover.core.models;

namespace service.mover.core
{
    public class Worker
    {
        private readonly Configuration configuration;
        public Worker(Configuration configuration)
        {
            this.configuration = configuration;
        }
        public void DoWork()
        {
            Log.Information("Hello from Worker");
        }
    }
}
