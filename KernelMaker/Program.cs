// See https://aka.ms/new-console-template for more information

using KernelMakerLibrary;
using Serilog;

using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
Log.Logger = log;
Log.Information("The global logger has been configured");

var userOptionProvider = UserOptionProviderFactory.GetProvider();
var userOptions = userOptionProvider.GetUserOptions();
Parser parser = new(userOptions);

parser.Parse();

Console.ReadLine();