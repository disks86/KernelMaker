using System.Reflection;
using System.Text;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Serilog;

namespace KernelMakerLibrary;

public class ScriptRunner
{
    /// <summary>
    /// The options provided by the user.
    /// </summary>
    public UserOptions UserOptions { get; set; }

    /// <summary>
    /// Engine for python scripts.
    /// </summary>
    public ScriptEngine PythonScriptEngine { get; set; }

    /// <summary>
    /// scrope for python scripts.
    /// </summary>
    public ScriptScope PythonScriptScope { get; set; }


    public ScriptRunner(UserOptions userOptions)
    {
        UserOptions = userOptions;
        PythonScriptEngine = Python.CreateEngine();
        PythonScriptScope = PythonScriptEngine.CreateScope();
        
        var optionType = typeof(UserOptions);
        var optionProperties = optionType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var optionProperty in optionProperties)
        {
            if (optionProperty.CanRead)
            {
                PythonScriptScope.SetVariable(optionProperty.Name, optionProperty.GetValue(UserOptions));
            }
        }
    }

    public void Run(string scriptMode)
    {
        if (string.IsNullOrWhiteSpace(UserOptions.RootPath))
        {
            return;
        }

        PythonScriptScope.SetVariable("ScriptMode", scriptMode);

        var streamOut = new MemoryStream();
        var streamErr = new MemoryStream();
        PythonScriptEngine.Runtime.IO.SetOutput(streamOut, Encoding.Default);
        PythonScriptEngine.Runtime.IO.SetErrorOutput(streamErr, Encoding.Default);

        var pythonFiles = Directory.GetFiles(UserOptions.RootPath, "*.py", SearchOption.AllDirectories).Union(
            Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), "*.py",
                SearchOption.AllDirectories));
        foreach (var pythonFile in pythonFiles)
        {
            try
            {
                PythonScriptScope = PythonScriptEngine.ExecuteFile(pythonFile, PythonScriptScope);
            }
            catch (Exception e)
            {
               Log.Error(e,"Python script failed");
            }
        }

        var output = Encoding.Default.GetString(streamOut.ToArray());
        if (!string.IsNullOrWhiteSpace(output))
        {
            Log.Information(output);
        }
        
        var errorOutput = Encoding.Default.GetString(streamErr.ToArray());
        if (!string.IsNullOrWhiteSpace(errorOutput))
        {
            Log.Error(errorOutput);
        }
    }
}