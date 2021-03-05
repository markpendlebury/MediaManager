using System.Collections.Generic;

namespace service.mover.Models
{
    public class Show
    {
        public string Title { get; set; }
        public List<Season> Seasons { get; set; }
        
    }

    public class Season
    {
        public string SeasonNumber { get; set; }
        public List<Episode> Episodes { get; set; }
    }

    public class Episode
    {
        // TODO: Need to rename all episodes so they contain the episode title (TMDB Intergration)
        // public string Title { get; set; } 
        public string FullPath { get; set; }
        public string Extension { get; set; }
        public string EpisodeNumber { get; set; }
    }
}