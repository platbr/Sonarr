using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NzbDrone.Core.MediaFiles
{
    public class FileNameHistory
    {
        public FileNameHistory(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
        private List<Entry> Entries { get; } = new List<Entry>();

        public void Append(string newName, string originalName)
        {
            if (newName == originalName)
            {
                return;
            }

            var oldestFileName = FindOldestFileName(originalName);
            foreach (var entry in Entries.Where(entry => entry.NewName == newName || entry.NewName == originalName).ToList())
            {
                Entries.Remove(entry);
            }

            Entries.Add(new Entry(newName, oldestFileName));
        }

        private void Append(Entry entry)
        {
            Append(entry.NewName, entry.OriginalName);
        }

        public void AppendEntriesFromFile()
        {
            try
            {
                using (var file =
                    new StreamReader(FilePath))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        const string pattern = @"(.+)=""(.+)""";
                        var matches = Regex.Matches(line, pattern);
                        if (matches.Count > 0 && matches[0].Groups.Count > 1)
                        {
                            Entries.Add(new Entry(matches[0].Groups[1].Value, matches[0].Groups[2].Value));
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // ignored
            }
        }

        public void ToFile()
        {
            using (var file =
                new StreamWriter(FilePath, false, Encoding.UTF8))
            {
                foreach (var entry in Entries)
                {
                    file.WriteLine("{0}=\"{1}\"", entry.NewName, entry.OriginalName);
                }
            }
        }

        private string FindOldestFileName(string fileName)
        {
            foreach (var entry in Entries.Where(entry => entry.NewName == fileName))
            {
                return entry.OriginalName;
            }

            return fileName;
        }

        public void Append(FileNameHistory originalFileNameHistoryPath, string nameFilter)
        {
            foreach (var entry in originalFileNameHistoryPath.Entries.Where(entry => entry.NewName == nameFilter || entry.OriginalName == nameFilter))
            {
                Append(entry);
            }
        }

        private class Entry
        {
            public Entry(string newName, string originalName)
            {
                NewName = newName;
                OriginalName = originalName;
            }

            public string NewName { get; }
            public string OriginalName { get; }
        }
    }
}
