using System.IO;
using NLog;
using NzbDrone.Common.EnsureThat;

namespace NzbDrone.Core.MediaFiles
{
    public interface IKeepFileNameHistory
    {
        void KeepFileNameHistory(string newFilePath, string originalFilePath);
    }

    public class KeepFileNameHistoryService : IKeepFileNameHistory
    {
        private readonly Logger _logger;

        public KeepFileNameHistoryService(Logger logger)
        {
            _logger = logger;
        }

        public void KeepFileNameHistory(string newFilePath, string originalFilePath)
        {
            Ensure.That(newFilePath, () => newFilePath).IsValidPath();
            Ensure.That(originalFilePath, () => originalFilePath).IsValidPath();

            var newFileNameHistory = new FileNameHistory(Path.Combine(Path.GetDirectoryName(newFilePath), "file_info"));
            var originalFileNameHistoryPath =
                new FileNameHistory(Path.Combine(Path.GetDirectoryName(originalFilePath), "file_info"));
            var originalName = Path.GetFileName(originalFilePath);
            var newName = Path.GetFileName(newFilePath);
            if (newFileNameHistory.FilePath != originalFileNameHistoryPath.FilePath)
            {
                originalFileNameHistoryPath.AppendEntriesFromFile();
                newFileNameHistory.Append(originalFileNameHistoryPath, originalName);
            }

            newFileNameHistory.AppendEntriesFromFile();
            _logger.Info("Keeping FileName History: {0}", newFileNameHistory.FilePath);
            newFileNameHistory.Append(newName, originalName);
            newFileNameHistory.ToFile();
        }
    }
}