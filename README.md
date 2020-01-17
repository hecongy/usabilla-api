# Usabilla API - C# Client
The Usabilla C# client allows users to access data from their Usabilla accounts. This repo refers to the [official python api](https://github.com/usabilla/api-python) and the codes/usage are similar except [Easier Way](#Easier_Way).

This API makes use of the API to request the following products and resources:

### Usabilla for Websites

- Buttons
- Feedback items
- Campaigns
- Campaign results
- Campaign statistics
- In-Page widgets
- In-Page feedback

### Usabilla for Email

- Buttons
- Feedback items

### Usabilla for Apps

- Apps
- Feedback items
- Campaigns
- Campaign results

For more information on resources, authorization and available API calls, please visit the [documentation](https://developers.usabilla.com).

## Installing [NuGet Package](https://www.nuget.org/packages/Usabilla.NET/)
Requires .NET Core 2.2
```
PM> Install-Package Usabilla.NET -Version 1.0.0
```

## How to Use
Please refer to the [examples](https://github.com/hecongy/usabilla-api/tree/master/Examples).

## Easier Way
This repo also provides an easier way to get resources.
```
var fastUsabilla = new FastUsabilla("ACCESS-KEY", "SECRET-KEY");
var url = "";
var resources = fastUsabilla.GetResource(url);
```
