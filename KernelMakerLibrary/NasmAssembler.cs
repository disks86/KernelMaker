using System.Diagnostics;
using Serilog;

namespace KernelMakerLibrary;

public class NasmAssembler
    : IAssembler
{
    public void Execute(KernelDefinition kernelDefinition)
    {
        Parallel.ForEach(kernelDefinition.AssemblyFiles,
        assemblyFile =>
        {
            var outputFilename = Path.ChangeExtension(assemblyFile, ".bin");
            
            Process process = new Process();
            process.StartInfo.FileName = "nasm";
            process.StartInfo.Arguments = $"-f bin -o \"{outputFilename}\" \"{assemblyFile}\"";
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process.ErrorDataReceived += ProcessOnErrorDataReceived;
            process.OutputDataReceived += ProcessOnOutputDataReceived;
            process.Start();
            process.WaitForExit(1000 * 60 * 5); // Wait up to five minutes. TODO: add assemble timeout to user options.

            if (!process.HasExited)
            {
                process.Kill();
                throw new Exception($"Assembler timed out when running for '{assemblyFile}'.");
            }

            if (process.ExitCode == 0)
            {
                if (File.Exists(outputFilename))
                {
                    kernelDefinition.AssembledFiles.Add(outputFilename);
                }
                else
                {
                    throw new Exception($"No output was generated when running for '{assemblyFile}'.");
                }
            }
            else
            {
                throw new Exception($"Assembler encountered an error when running for '{assemblyFile}'.");
            }
        });        
        
    }

    private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            Log.Information("{@Data}",e.Data);
        }
    }

    private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            Log.Error("{@Data}",e.Data);
        }
    }
}