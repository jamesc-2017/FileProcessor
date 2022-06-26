using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileProcessor.Services
{
    [Serializable]
    public class FileProcessInfo
    {
        public string TaskId { get; set; } = Guid.NewGuid().ToString();
        public DateTime TaskCreationDate { get; set; } = DateTime.UtcNow;
        public string FileId { get; set; }
        public FileProcessStatus TaskStatus { get; set; } = FileProcessStatus.InProgress;
        public IEnumerable<string> TaskResult { get; set; }

        public FileProcessInfo(string fileId)
        {
            FileId = fileId;
        }
    }
}
