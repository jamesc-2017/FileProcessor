using FileProcessor.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DalSoft.Hosting.BackgroundQueue;

namespace FileProcessor
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly BackgroundQueue _backgroundQueue;

        public FilesController(IFileService fileService, BackgroundQueue backgroundQueue)
        {
            _backgroundQueue = backgroundQueue;
            _fileService = fileService;
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<ActionResult<FileProcessInfo>> Post([FromBody] FileDto fileDto)
        {
            if (string.IsNullOrEmpty(fileDto?.Id) || string.IsNullOrEmpty(fileDto?.Contents))
                return BadRequest();

            try
            {
                _backgroundQueue.Enqueue(async cancellationToken =>
                {
                    await _fileService.AnalyzeFile(fileDto);
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("{fileId}")]
        [ProducesResponseType(typeof(FileProcessInfo), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<ActionResult<FileProcessInfo>> Get([FromRoute] string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
                return BadRequest();

            try
            {
                var result = await _fileService.GetFileProcessInformation(fileId);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<FileProcessInfo>), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<ActionResult<List<FileProcessInfo>>> GetAll([FromQuery] GetFilesOptions filesOptions)
        {
            try
            {
                var result = await _fileService.FindFiles(filesOptions);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
