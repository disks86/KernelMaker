using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace KernelMakerLibrary;

using Serilog;

public class Compiler
{
    public KernelDefinition Compile()
    {
        var userOptionProvider = UserOptionProviderFactory.GetProvider();
        var userOptions = userOptionProvider.GetUserOptions();

        using var log = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(Path.Join(userOptions.LogPath, "log.txt"), rollingInterval: RollingInterval.Day)
            .CreateLogger();
        
        Log.Logger = log;
        Log.Information("The global logger has been configured");

        ScriptRunner scriptRunner = new(userOptions);
        Parser parser = new(userOptions, scriptRunner);
        AssemblyGenerator assemblyGenerator = new(userOptions, scriptRunner);

        var kernelDefinition = parser.Parse();
        assemblyGenerator.GenerateAssembly(kernelDefinition);

        return kernelDefinition;
    }
}