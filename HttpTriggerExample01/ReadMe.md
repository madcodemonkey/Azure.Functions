# Summary
This project contains multiple Azure Functions that use the HttpBinding.  It demonstrates:
- The use of an isloated process (note that in process funcations are NOT supported in .NET 7 or .NET 8)
- FunctionSimpleGet - GET method (two different ways of getting query strings)
- FunctionSimplePost - POST method (two different ways of deserializing with Microsoft's JsonSerializer)
- Exception middleware (see the 4 exception endpoints) 
- The options pattern (see Program.cs and ServiceCollectionExtensions.cs in the HttpTriggerExample.Services project)
- The use of secrets.

# Setup
---
- This project depends on Azurite being installed on your PC.  
- Update your Core tools if necessary.
  - In Visual Studio, you can go to Tools > Options > Projects and Solutions > Azure Functions and press the "Check for Updates" button

# Secret notes
---
I've noticed that you can override local.settings.json values with two different formats in secrets file
## Format 1
```json
{
  "SomeNonNestedSetting": "SomeValue22",
  "ServiceInfo:RunInformation": ".NET 8 [Hello World Dude!]",
  "ServiceInfo:Greeting": "Hello great big World!"
}
```

## Format 2
```json
{
  "SomeNonNestedSetting": "SomeValue22",
  "ServiceInfo": 
  {
    "RunInformation": ".NET 8 [Hello World Dude!]",
    "Greeting": "Hello great big World!"
  }
}
```

# Postman collection
---
There is a postman collection at the root of the repository that has folders for each project.
See the HttpTriggerExample01 folder for all the methods that pertain to this Azure Function.

