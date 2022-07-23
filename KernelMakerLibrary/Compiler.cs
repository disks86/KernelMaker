using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Serilog.Exceptions;

namespace KernelMakerLibrary;

using Serilog;

public class Compiler
{
    public KernelDefinition? Run(bool executeAfterBuild = true)
    {
        KernelDefinition? kernelDefinition = null;
        
        var userOptionProvider = UserOptionProviderFactory.GetProvider();
        var userOptions = userOptionProvider.GetUserOptions();

        using var log = new LoggerConfiguration()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .WriteTo.File(Path.Join(userOptions.LogPath, "log.txt"), rollingInterval: RollingInterval.Day)
            .CreateLogger();
        
        Log.Logger = log;
        Log.Information("The global logger has been configured");

        try
        {
            ScriptRunner scriptRunner = new(userOptions);
            Parser parser = new(userOptions, scriptRunner);
            AssemblyGenerator assemblyGenerator = new(userOptions, scriptRunner);

            kernelDefinition = parser.Parse();
            assemblyGenerator.GenerateAssembly(kernelDefinition);

            var assembler = AssemblerFactory.GetAssembler(userOptions);
            assembler.Execute(kernelDefinition);
            
            var imageBuilder = ImageBuilderFactory.GetImageBuilder(userOptions);
            imageBuilder.Execute(kernelDefinition);
        }
        catch (Exception e)
        {
            Log.Error(e,"Compile failed");
        }
        
        return kernelDefinition;
    }

}