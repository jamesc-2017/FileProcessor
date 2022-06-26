using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileProcessor.Services
{
    public interface IFileService
    {

        //Send a file to analysis that is specified via the ID
        public Task<FileProcessInfo> AnalyzeFile(FileDto file);

        //Get information about an analysis (execution status and results).
        public Task<FileProcessInfo> GetFileProcessInformation(string fileId);

        //Search for the files that contain a particular file ID
        //or
        //Given a file ID, return all files that contain it and were submitted after a given date,
        //ordered by submission date.
        public Task<List<FileProcessInfo>> FindFiles(GetFilesOptions filesOptions);

    }
}
