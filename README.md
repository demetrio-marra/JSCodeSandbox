# JSCodeSandbox

A secure Node.js code execution environment built with .NET 8, providing sandboxed JavaScript execution using SES (Secure EcmaScript) for isolated and safe code execution.

## Overview

JSCodeSandbox is a host application that enables secure execution of JavaScript code in isolated sandbox environments. It leverages SESJS (Secure EcmaScript) behind the scenes to provide sandbox-execution level safety, preventing untrusted code from accessing sensitive resources or affecting the host system.

## Features

- **Secure Sandbox Execution**: Execute JavaScript code in isolated environments using SES
- **Custom API Provisioning**: Define and expose custom APIs to sandboxed code
- **Environment Management**: Create and manage multiple execution environments with different configurations
- **Backend Integration**: Configure backend URLs for API endpoints
- **NPM Package Support**: Specify dependencies via package.json for each environment

## Architecture

The solution consists of multiple projects:

- **JSCodeSandbox**: Core models and interfaces
- **JSCodeSandbox.Application**: Business logic and services
- **JSCodeSandbox.Infrastructure**: Repository implementations and data access
- **JSCodeSandbox.WebAPI**: REST API endpoints

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker (optional, for containerized deployment)

### Installation

1. Clone the repository:
```bash
git clone https://github.com/demetrio-marra/JSCodeSandbox.git
cd JSCodeSandbox
```

2. Build the solution:
```bash
dotnet build
```

3. Run the application:
```bash
dotnet run --project JSCodeSandbox.WebAPI
```

## Usage

### 1. Provisioning a New Execution Environment

Before executing code, you must provision an execution environment with:

- **Environment Name**: A unique identifier for the environment
- **Backend URLs**: Dictionary of backend endpoint configurations
- **Package.json**: NPM dependencies required by your API implementation
- **JavaScript Code**: Implementation of the APIs available to sandboxed code

#### JavaScript Code Requirements

Your API implementation must follow these conventions:

1. **Initialize Function**: Async function with exactly 2 parameters
   - `agentId`: Unique identifier for the agent
   - `backends`: Object containing backend URL configurations

2. **Deinitialize Function**: Async, parameterless cleanup function

3. **API Functions**: Each API must be:
   - Async function
   - Annotated with `@endowment` comment
   - Accept a single parameter: `params = {}`

4. **No Console Statements**: Avoid console.log or similar statements

#### Example API Implementation

```javascript
const axios = require('axios');

let httpClient;

async function Deinitialize() {
    // Cleanup resources
}

class HttpClient {
    constructor(baseURL) {
        this.baseURL = baseURL;
        this.agentId = null;
        this.axiosInstance = axios.create({
            baseURL,
            headers: {
                'Content-Type': 'application/json',
            },
        });
    }

    setAgentId(agentId) {
        this.agentId = agentId;
        this.axiosInstance.defaults.headers.common['X-Agent-Id'] = agentId;
    }

    async callTool(toolName, params = {}) {
        const response = await this.axiosInstance.post('/tools/call', {
            tool: toolName,
            params,
        });
        return response.data;
    }
}

async function Initialize(agentId, backends) {
    if (!backends || typeof backends !== 'object') {
        throw new Error('backends parameter is required and must be an object');
    }

    const serverUrl = backends['my-apis-server'];
    if (!serverUrl) {
        throw new Error('my-apis-server backend URL not found in backends configuration');
    }

    httpClient = new HttpClient(serverUrl);
    httpClient.setAgentId(agentId);
}

/**
 * @endowment
 * @tool MyPlatform_CompanyInfo_FindProductHierarchy
 * @description CompanyInfo - Finds which Company and Family a specific Product belongs to.
 * @inputSchema MyPlatformCompanyInfoFindProductHierarchyParams
 * @outputSchema HttpToolResponse<MyPlatformCompanyInfoFindProductHierarchyResult>
 * @errorSchema HttpToolError
 */
async function MyPlatform_CompanyInfo_FindProductHierarchy(params = {}) {
    try {
        return await httpClient.callTool('MyPlatform_CompanyInfo_FindProductHierarchy', params);
    } catch (error) {
        return {
            isError: true,
            error: error.message,
        };
    }
}

/**
 * @endowment
 * @tool MyPlatform_Statistics_GetRates
 * @description Get provisioning processes statistics rates for a specific product.
 * @inputSchema MyPlatformStatisticsGetRatesParams
 * @outputSchema HttpToolResponse<MyPlatformStatisticsGetRatesResult>
 * @errorSchema HttpToolError
 */
async function MyPlatform_Statistics_GetRates(params = {}) {
    try {
        return await httpClient.callTool('MyPlatform_Statistics_GetRates', params);
    } catch (error) {
        return {
            isError: true,
            error: error.message,
        };
    }
}
```

### 2. Executing Sandboxed Code

Once an environment is provisioned, you can execute JavaScript code in the sandbox by providing:

- **Environment Name**: The name of the provisioned environment
- **JavaScript Code**: The code to execute within the sandbox

The sandboxed code has access to the endowment functions defined in the environment's API implementation.
The first code execution on a new environment is slower due to NodeJS initialization and package installation, but subsequent executions will be faster as the environment remains active.

#### Example Sandboxed Code

```javascript
async function main() {
    let getStatisticsParameters = {
        product: "MySQL",
        dateFrom: "2022-12-01",
        dateTo: "2022-12-31",
        provisioningPhase: "Create"
    };
    
    let statistics = MyPlatform_Statistics_GetRates(getStatisticsParameters);
    if (statistics.length === 0) {
        return "No data was found";
    }
    
    let totalOk = 0;
    let totalKo = 0;

    for (const stat of statistics) {
        totalOk += stat.ok ?? 0;
        totalKo += stat.ko ?? 0;
    }

    return `Number of ok: ${totalOk}, Number of ko: ${totalKo}`;
}
```

## API Endpoints

### Provision Environment
```http
POST /api/environments
Content-Type: application/json

{
  "environmentName": "my-environment",
  "backendUrls": {
    "mcp-server": "https://api.example.com"
  },
  "packageJson": "{ \"dependencies\": { \"axios\": \"^1.0.0\" } }",
  "codeImplementation": "// JavaScript code here"
}
```

### Delete Environment
```http
DELETE /api/environments/{environmentName}
```

### Execute Code
```http
POST /api/execute
Content-Type: application/json

{
  "environmentName": "my-environment",
  "code": "// JavaScript code to execute"
}
```

## Security Considerations

- All JavaScript code runs in isolated SES sandboxes
- No access to Node.js built-in modules unless explicitly provided
- Environment APIs are the only bridge between sandbox and external resources
- Input validation is performed on all provisioning requests

## Docker Support

Build and run using Docker:

```bash
docker build -t jscodesandbox .
docker run -p 8080:80 jscodesandbox
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

- GitHub: [@demetrio-marra](https://github.com/demetrio-marra)
- Repository: [JSCodeSandbox](https://github.com/demetrio-marra/JSCodeSandbox)
