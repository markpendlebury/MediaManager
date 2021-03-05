
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using service.mover.core.models;
using service.mover.Factories;
using service.mover.Helpers;
using service.mover.Models;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

namespace service.mover.core
{
    public class Worker
    {
        private readonly Configuration configuration;
        private readonly StorageFactory storageFactory;
        private readonly Helper helper;

        public Worker(Configuration configuration, StorageFactory storageFactory, Helper helper)
        {
            this.configuration = configuration;
            this.storageFactory = storageFactory;
            this.helper = helper;
        }
        public void DoWork()
        {
            try
            {
                List<CompletedShow> completedShows = storageFactory.GetCompletedShows();


                int totalShows = completedShows.Count;
                int progress = 1;

                Log.Information($"Found {totalShows} shows to process");

                foreach (var completedShow in completedShows)
                {
                    switch (completedShow.Archive)
                    {
                        case true:
                            {
                                using (var archive = RarArchive.Open(completedShow.FullPath))
                                {
                                    foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                                    {

                                        if (helper.IsInterestingFile(entry.Key))
                                        {
                                            string showDirectory = configuration.ShowsDirectory + "/" + completedShow.Title;
                                            string seasonDirectory = showDirectory + "/Season " + completedShow.SeasonNumber.ToLower().Replace("s", "").Trim();

                                            // Does this file exist?
                                            FileInfo fileInfo = new FileInfo(seasonDirectory + "/" + completedShow.ExpectedFilename);

                                            if (!fileInfo.Exists)
                                            {
                                                if (!Directory.Exists(showDirectory))
                                                {
                                                    Directory.CreateDirectory(showDirectory);
                                                }

                                                if (!Directory.Exists(seasonDirectory))
                                                {
                                                    Directory.CreateDirectory(seasonDirectory);
                                                }

                                                Log.Information($"[{progress}/{totalShows}] Extracting {completedShow.Filename} => {seasonDirectory + "/"}");

                                                entry.WriteToDirectory(seasonDirectory + "/", new ExtractionOptions()
                                                {
                                                    ExtractFullPath = false,
                                                    Overwrite = true
                                                });

                                                Log.Information($"[{progress}/{totalShows}] Renaming {completedShow.Filename} => {seasonDirectory + "/" + completedShow.ExpectedFilename}");
                                                File.Move(seasonDirectory + "/" + entry.Key, seasonDirectory + "/" + completedShow.ExpectedFilename);
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        case false:
                            {
                                string showDirectory = configuration.ShowsDirectory + "/" + completedShow.Title;
                                string seasonDirectory = showDirectory + "/Season " + completedShow.SeasonNumber.ToLower().Replace("s", "").Trim();

                                // Does this file exist?
                                FileInfo fileInfo = new FileInfo(seasonDirectory + "/" + completedShow.ExpectedFilename);

                                if (!fileInfo.Exists)
                                {
                                    if (!Directory.Exists(showDirectory))
                                    {
                                        Directory.CreateDirectory(showDirectory);
                                    }

                                    if (!Directory.Exists(seasonDirectory))
                                    {
                                        Directory.CreateDirectory(seasonDirectory);
                                    }

                                    Log.Information($"[{progress}/{totalShows}] Moving {completedShow.Filename} => {seasonDirectory + "/" + completedShow.ExpectedFilename}");

                                    File.Copy(completedShow.FullPath, seasonDirectory + "/" + completedShow.ExpectedFilename);

                                    Log.Information($"Done!");
                                    progress = progress + 1;
                                }
                                break;
                            }
                        default:
                            {
                                throw new Exception($"Not sure what to do with this file: {completedShow.FullPath}");
                            }
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
    }
}
