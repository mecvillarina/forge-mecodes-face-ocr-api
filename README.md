# API For Face Detect and Image-To-Text Atlassian Forge Apps for Codegeist 2020 hackathon

This project contains an Azure Function written in C#. This project exposes API endpoints for Face Detect and Image-To-Text projects for Codegeist 2020 hackathon. If you're interested what's behind the Face Detect and Image-To-Text Entry Atlassian Forge Apps, you can visit these links below:

- [Face Detect](https://github.com/mecvillarina/forge-mecodes-face-detect)
- [Image-To-Text](https://github.com/mecvillarina/forge-mecodes-face-detect)

## Requirements

- Visual Studio 2019
- Knowledge about C#
- Knowledge about Azure Functions
- Azure Subscription
  - Azure Cognigtive Service Endpoint and Key 
  - Azure Storage Account
  
## Quick start
- To test it locally, after opening the project solution in Visual Studio 2019, you should a file named `local.settings.json`.

`
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "AzureStorageAccountConnectionString",
    "ComputerVisionEndpoint": "CognitiveServiceEndpoint",
    "ComputerVisionKey": "CognitiveServiceKey",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  }
}
`

