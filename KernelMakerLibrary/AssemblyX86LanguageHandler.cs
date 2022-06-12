using System.Text;

namespace KernelMakerLibrary;

public class AssemblyX86LanguageHandler
    : ILanguageHandler
{
    public void GenerateAssembly(UserOptions userOptions, KernelDefinition kernelDefinition,
        FunctionDefinition functionDefinition)
    {
        if (string.IsNullOrWhiteSpace(userOptions.TempPath))
        {
            throw new Exception("Missing temporary directory");
        }
        
        if (userOptions.TargetArchitecture is "x86" && functionDefinition.FunctionLanguage.Equals("Assembly-x86"))
        {
            if (functionDefinition.FunctionSignature.Name.Equals("Boot"))
            {
                if (!Directory.Exists(userOptions.TempPath))
                {
                    Directory.CreateDirectory(userOptions.TempPath);
                }
                
                var filename = Path.Join(userOptions.TempPath,
                    string.Concat(functionDefinition.FunctionSignature.Name, ".s"));
                
                File.WriteAllText(filename, functionDefinition.RawMethodBody);
            }
            else
            {
                if (!Directory.Exists(userOptions.TempPath))
                {
                    Directory.CreateDirectory(userOptions.TempPath);
                }
                
                var filename = Path.Join(userOptions.TempPath,
                    string.Concat(functionDefinition.FunctionSignature.Name, ".s"));

                //TODO revisit
                
                var labelBuilder = new StringBuilder(functionDefinition.FunctionSignature.Name.Length +
                                                     functionDefinition.FunctionSignature.ReturnType.Length +
                                                     (functionDefinition.FunctionSignature.FunctionArguments.Count *
                                                      20));
                labelBuilder.Append('_');
                labelBuilder.Append(functionDefinition.FunctionSignature.Name);
                labelBuilder.Append('_');
                labelBuilder.Append(functionDefinition.FunctionSignature.ReturnType);
                foreach (var functionArgument in functionDefinition.FunctionSignature.FunctionArguments)
                {
                    labelBuilder.Append('_');
                    labelBuilder.Append(functionArgument.ArgumentType);                    
                }
                labelBuilder.Append(functionDefinition.FunctionSignature.ReturnType);
                functionDefinition.AssemblyLabel = labelBuilder.ToString();

                var sb = new StringBuilder(functionDefinition.RawMethodBody.Length + 40);
                sb.Append(".section .text\n");
                sb.Append(".globl ");
                sb.Append(functionDefinition.AssemblyLabel);
                sb.Append('\n');
                sb.Append(functionDefinition.AssemblyLabel);
                sb.Append(':');
                sb.Append('\n');
                sb.Append(functionDefinition.RawMethodBody);
                File.WriteAllText(filename,sb.ToString());
            }
        }
    }
}