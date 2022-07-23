using System.Diagnostics;
using Serilog;

namespace KernelMakerLibrary;

public class DDImageBuilder
    : IImageBuilder
{
    public void Execute(KernelDefinition kernelDefinition)
    {
        var assembledFilename = kernelDefinition.BootFilename;
        var bootFilename = Path.ChangeExtension(assembledFilename,".flp");
        kernelDefinition.BootFilename = bootFilename;
        
        Process process = new Process();
        process.StartInfo.FileName = "dd";
        process.StartInfo.Arguments = $"status=noxfer conv=notrunc if=\"{assembledFilename}\" of=\"{bootFilename}\"";
        process.StartInfo.ErrorDialog = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
        process.ErrorDataReceived += ProcessOnErrorDataReceived;
        process.OutputDataReceived += ProcessOnOutputDataReceived;
        process.Start();
        process.WaitForExit(1000 * 60 * 5); // Wait up to five minutes. TODO: add image build timeout to user options.
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