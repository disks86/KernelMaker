// See https://aka.ms/new-console-template for more information

using KernelMakerLibrary;
using Serilog;

var userOptionProvider = UserOptionProviderFactory.GetProvider();
var userOptions = userOptionProvider.GetUserOptions();

using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
Log.Logger = log;

//TODO: use log file path from user options for output log files.

Log.Information("The global logger has been configured");

Parser parser = new(userOptions);

var kernelDefinition = parser.Parse();

var assemblyGenerator = new AssemblyGenerator();

assemblyGenerator.GenerateAssembly(userOptions, kernelDefinition);

Console.ReadLine();