using System;
using System.Collections.Generic;
using System.IO;
using Serilog;
using service.mover.core.models;
using service.mover.Helpers;
using service.mover.Models;

namespace service.mover.Factories
{
    public class StorageFactory
    {
        private readonly Configuration configuration;
        private readonly Helper helper;

        public StorageFactory(Configuration configuration, Helper helper)
        {
            this.configuration = configuration;
            this.helper = helper;
        }

        internal List<CompletedShow> GetCompletedShows()
        {
            try
            {
                List<CompletedShow> completedShows = new List<CompletedShow>();


                // Process Completed Directories:
                foreach (var directory in Directory.GetDirectories(configuration.CompletedDirectory))
                {
                    // is this item a Show?
                    if (helper.IsShow(directory))
                    {
                        // Does this directory contain any files:
                        var completedShowDirectorySubFiles = Directory.GetFiles(directory);
                        if (completedShowDirectorySubFiles.Length > 0)
                        {
                            // Process the files inside this Directory:
                            foreach (var file in completedShowDirectorySubFiles)
                            {
                                if (helper.IsInterestingFile(file))
                                {
                                    FileInfo fileInfo = new FileInfo(file);

                                    CompletedShow newShow = new CompletedShow()
                                    {
                                        FullPath = fileInfo.FullName,
                                        Filename = fileInfo.Name,
                                        Extension = fileInfo.Extension,
                                        ExpectedFilename = helper.GetExpectedFilename(file),
                                        Title = helper.GetShowTitleFromFilename(file),
                                        Archive = helper.IsThisAnArchive(file),
                                        SeasonNumber = helper.GetSeasonNumberFromFilename(fileInfo.Name),
                                        EpisodeNumber = helper.GetEpisodeNumberFromFilename(fileInfo.Name)
                                    };
                                    string expectedFullPath = configuration.ShowsDirectory + "/" + newShow.Title + "/" + "Season " + newShow.SeasonNumber.Replace("S", null) + "/" + newShow.ExpectedFilename;
                                    if (!File.Exists(expectedFullPath))
                                    {
                                        completedShows.Add(newShow);
                                        Log.Information($"Discovered: {newShow.ExpectedFilename}");
                                    }

                                }
                            }
                        }


                        // Does this directory contain SubDirectories:
                        var completedShowDirectorySubDirectories = Directory.GetDirectories(directory);
                        if (completedShowDirectorySubDirectories.Length > 0)
                        {
                            // Process the files inside this SubDirectory:
                            foreach (var subDirectory in completedShowDirectorySubDirectories)
                            {
                                // Does this Subdirectory any Files:
                                var subDirectoryFiles = Directory.GetFiles(subDirectory);
                                if (subDirectoryFiles.Length > 0)
                                {
                                    // Process this subdirectories files:
                                    foreach (var file in subDirectoryFiles)
                                    {
                                        if (helper.IsInterestingFile(file))
                                        {
                                            FileInfo fileInfo = new FileInfo(file);

                                            CompletedShow newShow = new CompletedShow()
                                            {
                                                FullPath = fileInfo.FullName,
                                                Filename = fileInfo.Name,
                                                Extension = fileInfo.Extension,
                                                ExpectedFilename = helper.GetExpectedFilename(file),
                                                Title = helper.GetShowTitleFromFilename(file),
                                                Archive = helper.IsThisAnArchive(file),
                                                SeasonNumber = helper.GetSeasonNumberFromFilename(fileInfo.Name),
                                                EpisodeNumber = helper.GetEpisodeNumberFromFilename(fileInfo.Name)
                                            };
                                            string expectedFullPath = configuration.ShowsDirectory + "/" + newShow.Title + "/" + "Season " + newShow.SeasonNumber.Replace("S", null) + "/" + newShow.ExpectedFilename;
                                            if (!File.Exists(expectedFullPath))
                                            {
                                                completedShows.Add(newShow);
                                                Log.Information($"Discovered: {newShow.ExpectedFilename}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }



                // Process Completed Files:
                foreach (var file in Directory.GetFiles(configuration.CompletedDirectory))
                {
                    // Is this a show:
                    if (helper.IsShow(file))
                    {
                        if (helper.IsInterestingFile(file))
                        {
                            FileInfo fileInfo = new FileInfo(file);

                            CompletedShow newShow = new CompletedShow()
                            {
                                FullPath = fileInfo.FullName,
                                Filename = fileInfo.Name,
                                Extension = fileInfo.Extension,
                                ExpectedFilename = helper.GetExpectedFilename(file),
                                Title = helper.GetShowTitleFromFilename(file),
                                Archive = helper.IsThisAnArchive(file),
                                SeasonNumber = helper.GetSeasonNumberFromFilename(fileInfo.Name),
                                EpisodeNumber = helper.GetEpisodeNumberFromFilename(fileInfo.Name)
                            };
                            string expectedFullPath = configuration.ShowsDirectory + "/" + newShow.Title + "/" + "Season " + newShow.SeasonNumber.Replace("S", null) + "/" + newShow.ExpectedFilename;
                            if (!File.Exists(expectedFullPath))
                            {
                                completedShows.Add(newShow);
                                Log.Information($"Discovered: {newShow.ExpectedFilename}");
                            }
                        }
                    }

                }

                return completedShows;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
    }
}