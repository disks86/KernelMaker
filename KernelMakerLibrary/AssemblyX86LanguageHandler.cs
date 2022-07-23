using System.Text;
using Serilog;

namespace KernelMakerLibrary;

public class AssemblyX86LanguageHandler
    : ILanguageHandler
{
    public string GenerateAssembly(UserOptions userOptions, KernelDefinition kernelDefinition,
        FunctionDefinition functionDefinition)
    {
        string filename = string.Empty;
        string output = string.Empty;
        
        if (string.IsNullOrWhiteSpace(userOptions.TempPath))
        {
            throw new Exception("Missing temporary directory");
        }
        
        if (!Directory.Exists(userOptions.TempPath))
        {
            Directory.CreateDirectory(userOptions.TempPath);
        }
        
        if (userOptions.TargetArchitecture is "x86" && functionDefinition.FunctionLanguage.Equals("Assembly-x86"))
        {
            if (functionDefinition.FunctionSignature.Name.Equals("Boot"))
            {
                filename = Path.Join(userOptions.TempPath,"Boot.asm");

                kernelDefinition.BootFilename = filename;
                
                var sb = new StringBuilder(functionDefinition.RawMethodBody.Length + 40);
                using var stringReader = new StringReader(functionDefinition.RawMethodBody);
                while (true)
                {
                    var line = stringReader.ReadLine();
                    if (line!=null)
                    {
                        sb.Append(line/*.Trim()*/);
                        sb.Append('\n');
                    }
                    else
                    {
                        break;
                    }
                }
                output = sb.ToString();
            }
            else
            {
                filename = Path.Join(userOptions.TempPath,
                    string.Concat(functionDefinition.AssemblyLabel, ".asm"));
                
                var sb = new StringBuilder(functionDefinition.RawMethodBody.Length + 40);
                sb.Append(".section .text\n");
                sb.Append(".globl ");
                sb.Append(functionDefinition.AssemblyLabel);
                sb.Append('\n');
                sb.Append(functionDefinition.AssemblyLabel);
                sb.Append(':');
                sb.Append('\n');
                sb.Append(functionDefinition.RawMethodBody);
                
                output = sb.ToString();
            }
        }

        if (!string.IsNullOrWhiteSpace(output))
        {
            File.WriteAllText(filename,output); 
        }

        return filename;
    }
}