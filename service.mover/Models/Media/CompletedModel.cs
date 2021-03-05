namespace service.mover.Models
{
    public class CompletedShow
    {
        public string FullPath { get; set; }
        public string Filename { get; set; }
        public string ExpectedFilename { get; set; }
        public string Extension { get; set; }
        public bool Archive { get; set; }
        public string Title { get; set; }
        public string SeasonNumber { get; set; }
        public string EpisodeNumber { get; set; }
    }
}