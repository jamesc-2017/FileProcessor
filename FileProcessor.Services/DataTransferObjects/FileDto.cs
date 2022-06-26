using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileProcessor.Services
{
    public class FileDto
    {
        public string Id { get; set; }
        //for example purposes, file contents are simply passed in as a string using this property
        public string Contents { get; set; }
    }
}
