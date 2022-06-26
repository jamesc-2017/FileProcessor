A simple file processor application.

In this situation, "process" means to analyze the contents of the file to determine
what other "fileIds" (in GUID format) are referenced within it.

Workflow
    1. POST to /Files, to kick off processing asynchronously
        Params
            Id => the name (as a GUID) of the file
            Contents => the file contents, as a string
                Note that there would be more efficient ways to do this in non-demo/sample situations
        To simulate "extraneous" processing, application delays for 15 seconds
    2. GET to /Files, to retrieve all process results for all files
        Optional query params
            fileIdToFind => searches all results for any references found to fileId
            submissionDateCutoff => filters out results that were processed before the cutoff date
    3. GET to /Files{id}, to retrieve the process result for a specific fileId

Technologies used
    ASP.NET Core Web API
    C#
    Redis
    Docker
    Swagger