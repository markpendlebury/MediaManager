using System;

namespace service.mover.core.models
{
    public class Configuration
    {
        private string _completedDirectory = Environment.GetEnvironmentVariable("COMPLETED_DIRECTORY");
        private string _showsDirectory = Environment.GetEnvironmentVariable("SHOWS_DIRECTORY");
        private string _moviesDirectory = Environment.GetEnvironmentVariable("MOVIES_DIRECTORY");
        private int _sleep = Convert.ToInt16(Environment.GetEnvironmentVariable("SLEEP_DURATION"));
        public string CompletedDirectory 
        {
            get
            {
                if(!string.IsNullOrEmpty(_completedDirectory))
                {
                    return _completedDirectory;
                }
                throw new ArgumentException("COMPLETED_DIRECTORY environment variable is not set");
            }
        }
        public string ShowsDirectory 
        {
            get
            {
                if(!string.IsNullOrEmpty(_showsDirectory))
                {
                    return _showsDirectory;
                }
                throw new ArgumentException("SHOWS_DIRECTORY environment variable is not set");
            }
        }
        public string MoviesDirectory 
        {
            get
            {
                if(!string.IsNullOrEmpty(_moviesDirectory))
                {
                    return _moviesDirectory;
                }
                throw new ArgumentException("MOVIES_DIRECTORY environment variable is not set");
            }
        }
        public int Sleep
        {
            get
            {
                if(_sleep > 0)
                {
                    return _sleep;
                }
                throw new ArgumentException("SLEEP_DURATION must be greater than 0. Integer required to represent number of minutes to wait between scans..");
            }
        }
    }
}