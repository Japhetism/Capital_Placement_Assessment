# .Net Program management

This was developed with ASP.net and has the following features:

- Program management (Create, Update, Delete, Get program).
- Question type management (Create, Update, Delete, Get question type).
- Question management (Create, Update, Delete, Get program).
- Application management (Create, Get).

## Installation

1. Clone the repository.
   ```bash
   git clone https://github.com/Japhetism/Capital_Placement_Assessment
   ```
2. Add Cosmos DB PrimaryKey to CosmosDbSettings in appsettings.json.
   ```bash
   "CosmosDbSettings": {
      "PrimaryKey": "<put_the_primary_key_here>"
   }
   ```
3. Run the application by;
   - Clicking on the play button in Visual Studio Code.
   - Or open your terminal, navigate to the project, and type dotnet run
     ```bash
      $:> dotnet run

## Requests

#### Program 
###### /api/CompanyProgram
```bash
{
  "id": "string",
  "dateCreated": "string",
  "lastModifiedDate": "string",
  "title": "string",
  "description": "string",
  "fields": [
    {
      "name": "string",
      "isRequired": true
    }
  ]
}
```

#### Question Type
###### /api/QuestionType
```bash
{
  "id": "string",
  "dateCreated": "string",
  "lastModifiedDate": "string",
  "name": "string"
}
```

#### Question
###### /api/Question
```bash
{
  "id": "string",
  "programId": "string",
  "typeId": "string",
  "questionText": "string",
  "choice": [
    "string"
  ],
  "maximumChoiceAllowed": 0
}
```

#### Application
###### /api/Application
```bash
{
  "id": "string",
  "programId": "string",
  "questionResponse": [
    {
      "response": "string",
      "questionId": "string"
    }
  ],
  "userInfo": "string"
}
```

## Author

Babatunde Ojo
