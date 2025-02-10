// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TestDDDConsole.Application.Service;
using TestDDDConsole.Application.Usecase;
using TestDDDConsole.Domain.Repository;
using TestDDDConsole.Infrastructure.Database;
using TestDDDConsole.Infrastructure.Database.Repository;
using TestDDDConsole.ORM;

var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsetting.json");
var config = builder.Build();

var context = new DataContext(config);

var serviceProvider = new ServiceCollection()
    .AddSingleton<IExcelToDatabase, ExcelToDatabase>() // Register IExcelToDatabase
    .AddSingleton<IImportData,ImportData>() // Register ImportData
    .AddSingleton<IDatabaseSystem, DatabaseSystem>() // Register ImportData
    .BuildServiceProvider();

IImportDataService importDataService = new ImportDataService();

string? basePath = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;

string relativePath = @"File\FileTest\FileImportPerson.xlsx";
string fullPath = Path.Combine(basePath??"", relativePath);

string base64 = importDataService.GetBase64FromExcel(fullPath);

IDatabaseSystem repositoryDataBaseSystem = new DatabaseSystem(context);
IDatabaseInformation databaseInformation = new DatabaseInformation(repositoryDataBaseSystem);
ResultData data = await importDataService.GetDataFromExcel(base64, databaseInformation);

if (string.IsNullOrEmpty(data.Error))
{
    IExcelToDatabase repository = new ExcelToDatabase(context);
    ImportData importData = new ImportData(repository);
    await importData.InsertUpdateDeleteData(data.Data);
}
else
{
    Console.WriteLine(data.Error);
}

