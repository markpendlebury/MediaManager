using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;
using service.mover.core.models;
using SharpCompress.Archives.Rar;

namespace service.mover.Helpers
{
    public class Helper
    {
        private readonly Configuration configuration;
        public Helper(Configuration configuration)
        {
            this.configuration = configuration;
        }
        internal string GetEpisodeNumberFromFilename(string fullName)
        {
            try
            {
                // Using regex here to find the Episode number from the filename:
                // This regex attempts to match something similar to e01 (e  plus two digits) Ignoring the caseing
                Regex rEpisode = new Regex(@"(e\d\d)", RegexOptions.IgnoreCase);
                Match mEpisode = rEpisode.Match(fullName);
                if (mEpisode.Success)
                {
                    return mEpisode.Value.ToUpper();
                }
                throw new Exception($"Unable to find episode in string {fullName}");

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        internal string GetSeasonNumberFromFilename(string fullName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fullName);

                // Using regex here to find the Seasopn number from the filename:
                // This regex attempts to match something similar to e01 (e  plus two digits) Ignoring the caseing
                Regex rSeason = new Regex(@"(s\d\d)", RegexOptions.IgnoreCase);
                Match mSeason = rSeason.Match(fileInfo.Name);
                if (mSeason.Success)
                {
                    return mSeason.Value.ToUpper();
                }
                throw new Exception($"Unable to find seaon in string {fullName}");

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        internal bool IsInterestingFile(string file)
        {
            string[] interestingFileExtensions = { ".mkv", ".rar", ".avi", ".mp4", ".zip" };
            if (file.ToLower().Contains("sample"))
            {
                return false;
            }
            FileInfo fileInfo = new FileInfo(file);
            if (interestingFileExtensions.Contains(fileInfo.Extension))
            {
                return true;
            }
            return false;
        }

        internal bool IsShow(string item)
        {
            try
            {
                Regex rShow = new Regex(@"(s\d\d)", RegexOptions.IgnoreCase);
                Match mShow = rShow.Match(item);
                if (mShow.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                throw new Exception($"Unable to find show in string {item}");

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        internal string GetExpectedFilename(string file)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(file);
                string extension = string.Empty;

                if (IsThisAnArchive(fileInfo.Name))
                {
                    // Log.Information($"Inspecting {fileInfo.FullName} for it's file Extension");
                    // Get the actual filename from within the archive: 
                    using (var archive = RarArchive.Open(fileInfo.FullName))
                    {
                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                        {

                            if (IsInterestingFile(entry.Key))
                            {
                                FileInfo archivedFileInfo = new FileInfo(entry.Key);
                                extension = archivedFileInfo.Extension;
                            }
                        }
                    }
                }
                else
                {
                    extension = GetExtensionFromFilename(fileInfo.Name);
                }

                string season = GetSeasonNumberFromFilename(fileInfo.Name);
                string episode = GetEpisodeNumberFromFilename(fileInfo.Name);
                
                string title = GetShowTitleFromFilename(fileInfo.Name);

                string expectedFilename = title + " - " + season + episode + extension;

                return expectedFilename;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());

                throw;
            }
        }

        private string GetExtensionFromFilename(string file)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension != null)
                {
                    return fileInfo.Extension;
                }
                throw new Exception($"Couldn't get extension from {file}");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }

        }

        internal string GetShowTitleFromFilename(string file)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(file);

                string season = GetSeasonNumberFromFilename(file);
                string episode = GetEpisodeNumberFromFilename(file);
                string extension = GetExtensionFromFilename(file);
                string seasonAndEpisode = season.ToUpper() + episode.ToUpper();
                string title = fileInfo.Name.Substring(0, fileInfo.Name.ToUpper().IndexOf(seasonAndEpisode)).Replace(".", " ");

                Regex rSeason = new Regex(@"(season \d\d)|(season \d)|(season.\d)|(season.\d\d)", RegexOptions.IgnoreCase);
                Match mSeason = rSeason.Match(title);
                if (mSeason.Success)
                {
                    title = title.Replace(mSeason.Value, null);
                }

                Regex rYear = new Regex(@"(\d\d\d\d)", RegexOptions.IgnoreCase);
                Match mYear = rYear.Match(title);
                if (mYear.Success)
                {
                    title = title.Replace(mYear.Value, null);
                    title = title.Replace("(", null).Replace(")", null).Replace("[", null).Replace("]", null).Replace("{", null).Replace("}", null);
                    var seasonNumber = Convert.ToInt16(season.ToLower().Replace("s", ""));
                    title = title.Replace(seasonNumber.ToString(), null);
                }
                title = upperCaseFirstCharacter(title);
                return title.Trim();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        internal string upperCaseFirstCharacter(string input)
        {
            if (input.ToLower().Contains("qi"))
            {
                return "QI";
            }
            TextInfo ti = new CultureInfo("en-GB", false).TextInfo;
            return ti.ToTitleCase(input);
        }

        internal bool IsThisAnArchive(string file)
        {
            try
            {
                string[] archiveExtensions = { ".rar", ".zip" };

                FileInfo fileInfo = new FileInfo(file);
                if (archiveExtensions.Contains(fileInfo.Extension.ToLower()))
                {
                    return true;
                }
                else
                {
                    return false;
                }

                throw new Exception($"Couldn not determine if {file} is an archive or not");

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
    }
}