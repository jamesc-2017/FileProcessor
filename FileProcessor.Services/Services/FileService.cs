using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileProcessor.Services
{
    public class FileService : IFileService
    {
        private static readonly string UUIDRegex = @"([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}";

        private readonly IDistributedCache _cache;
        private readonly IServer _server;

        public FileService(IDistributedCache cache, IServer server)
        {
            _cache = cache;
            _server = server;
        }

        public async Task<FileProcessInfo> AnalyzeFile(FileDto file)
        {
            var fileProcessInfo = new FileProcessInfo(file.Id);
            await _cache.SetAsync(fileProcessInfo.FileId, fileProcessInfo, new DistributedCacheEntryOptions());

            //arbitrary wait of 15 seconds to simulate extraneous processing
            await Task.Delay(1000 * 15);   

            //simple regex matching for example purposes
            var matches = Regex.Matches(file.Contents, UUIDRegex, RegexOptions.Compiled);
            fileProcessInfo.TaskResult = matches.Select(x => x.Value).ToList();
            fileProcessInfo.TaskStatus = FileProcessStatus.Complete;

            await _cache.SetAsync(fileProcessInfo.FileId, fileProcessInfo, new DistributedCacheEntryOptions());

            return fileProcessInfo;
        }

        public async Task<FileProcessInfo> GetFileProcessInformation(string fileId)
        {
            return await _cache.GetAsync<FileProcessInfo>(fileId);
        }

        public async Task<List<FileProcessInfo>> FindFiles(GetFilesOptions filesOptions)
        {
            var values = new List<FileProcessInfo>();
            var keys = _server.Keys()
                .Select(x => x.ToString())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            foreach (var item in keys)
            {
                var value = await _cache.GetAsync<FileProcessInfo>(item);

                if (!string.IsNullOrEmpty(filesOptions.fileIdToFind)
                    && !value.TaskResult.Contains(filesOptions.fileIdToFind)) continue;
                if (filesOptions.submissionDateCutoff != null
                    && filesOptions.submissionDateCutoff != DateTime.MinValue
                    && (value.TaskCreationDate == DateTime.MinValue 
                        || value.TaskCreationDate < filesOptions.submissionDateCutoff)) continue;

                values.Add(value);
            }

            return values.OrderByDescending(x => x.TaskCreationDate).ToList();
        }
    }
}
