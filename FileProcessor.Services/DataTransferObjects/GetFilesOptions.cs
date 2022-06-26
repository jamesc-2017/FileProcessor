using System;

namespace FileProcessor.Services
{
    public class GetFilesOptions
    {
        public string fileIdToFind { get; set; }
        public DateTime? submissionDateCutoff { get; set; }
    }
}
