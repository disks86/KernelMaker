using System.Security;
using System.Text;
using Serilog;

namespace KernelMakerLibrary;

public class Parser
{
    /// <summary>
    /// The options provided by the user.
    /// </summary>
    public UserOptions UserOptions { get; set; }

    /// <summary>
    /// The code file provider which is used to fetch local code files.
    /// </summary>
    public ICodeFileProvider CodeFileProvider { get; set; }

    /// <summary>
    /// The remote code file provider which is used to download remote code files.
    /// </summary>
    public IRemoteCodeFileProvider RemoteCodeFileProvider { get; set; }

    /// <summary>
    /// The script runner for executing user provided scripts.
    /// </summary>
    public ScriptRunner ScriptRunner { get; set; }
    
    public Parser(UserOptions userOptions, ScriptRunner scriptRunner)
    {
        UserOptions = userOptions;
        ScriptRunner = scriptRunner;
        CodeFileProvider = CodeFileProviderFactory.GetProvider(userOptions);
        RemoteCodeFileProvider = RemoteCodeFileProviderFactory.GetProvider(userOptions);
    }

    enum FunctionCodeLocation
    {
        None,
        BeforeReturnType,
        ReturnType,
        BeforeFunctionName,
        FunctionName,
        BeforeFunctionArgumentType,
        FunctionArgumentType,
        BeforeFunctionArgumentName,
        FunctionArgumentName,
        BeforeLanguageType,
        LanguageType,
        BeforeFunctionBody,
        FunctionBody,
    }

    public KernelDefinition Parse()
    {
        Log.Information("Downloading remote code files");
        RemoteCodeFileProvider.DownloadRemoteCodeFiles(UserOptions);
        ScriptRunner.Run("DownloadRemoteCodeFiles");
        
        var codeFiles = CodeFileProvider.GetCodeFiles(UserOptions);
        Log.Information("{CodeFiles}", codeFiles);

        KernelDefinition kernelDefinition = new();

        Log.Information("Parsing function code files");
        Parallel.ForEach(codeFiles.FunctionCodeFiles,
            functionCodeFile => { ParseFunctionCodeFile(kernelDefinition, functionCodeFile); });

        Log.Information("Parsing object code files");
        Parallel.ForEach(codeFiles.TypeCodeFiles,
            typeCodeFile => { ParseTypeCodeFile(kernelDefinition, typeCodeFile); });

        return kernelDefinition;
    }

    private void ParseFunctionCodeFile(KernelDefinition kernelDefinition, CodeFile functionCodeFile)
    {
        FunctionCodeLocation functionCodeLocation = FunctionCodeLocation.BeforeReturnType;
        FunctionDefinition functionDefinition = new();

        FunctionArgument functionArgument = new();

        ulong position = 0;
        ulong lineNumber = 0;
        long nestCount = 0;
        foreach (var character in functionCodeFile.Content)
        {
            switch (character)
            {
                case char c when (Char.IsLetterOrDigit(character) || character is '_' or '-'):
                    switch (functionCodeLocation)
                    {
                        case FunctionCodeLocation.BeforeReturnType:
                            functionCodeLocation = FunctionCodeLocation.ReturnType;
                            functionDefinition.FunctionSignature.ReturnType += character;
                            break;
                        case FunctionCodeLocation.ReturnType:
                            functionDefinition.FunctionSignature.ReturnType += character;
                            break;
                        case FunctionCodeLocation.BeforeFunctionName:
                            functionCodeLocation = FunctionCodeLocation.FunctionName;
                            functionDefinition.FunctionSignature.Name += character;
                            break;
                        case FunctionCodeLocation.FunctionName:
                            functionDefinition.FunctionSignature.Name += character;
                            break;
                        case FunctionCodeLocation.BeforeFunctionArgumentType:
                            functionCodeLocation = FunctionCodeLocation.FunctionArgumentType;
                            functionArgument.ArgumentType += character;
                            break;
                        case FunctionCodeLocation.FunctionArgumentType:
                            functionArgument.ArgumentType += character;
                            break;
                        case FunctionCodeLocation.BeforeFunctionArgumentName:
                            functionCodeLocation = FunctionCodeLocation.FunctionArgumentName;
                            functionArgument.ArgumentName += character;
                            break;
                        case FunctionCodeLocation.FunctionArgumentName:
                            functionArgument.ArgumentName += character;
                            break;
                        case FunctionCodeLocation.BeforeLanguageType:
                            functionCodeLocation = FunctionCodeLocation.LanguageType;
                            functionDefinition.FunctionLanguage += character;
                            break;
                        case FunctionCodeLocation.LanguageType:
                            functionDefinition.FunctionLanguage += character;
                            break;
                        case FunctionCodeLocation.BeforeFunctionBody:
                            functionCodeLocation = FunctionCodeLocation.FunctionBody;
                            functionDefinition.RawMethodBody += character;
                            break;
                        case FunctionCodeLocation.FunctionBody:
                            functionDefinition.RawMethodBody += character;
                            break;
                        default:
                            Log.Warning(
                                "Unhandled token {Character} when location type was {FunctionCodeLocation} as position {Position} on line {LineNumber}",
                                character, functionCodeLocation, position, lineNumber);
                            break;
                    }

                    break;
                case ' ':
                    switch (functionCodeLocation)
                    {
                        case FunctionCodeLocation.ReturnType:
                            functionCodeLocation = FunctionCodeLocation.BeforeFunctionName;
                            break;
                        case FunctionCodeLocation.FunctionArgumentType:
                            functionCodeLocation = FunctionCodeLocation.BeforeFunctionArgumentName;
                            break;
                        case FunctionCodeLocation.BeforeFunctionBody:
                            functionCodeLocation = FunctionCodeLocation.FunctionBody;
                            functionDefinition.RawMethodBody += character;
                            break;
                        case FunctionCodeLocation.FunctionBody:
                            functionDefinition.RawMethodBody += character;
                            break;
                        case FunctionCodeLocation.BeforeLanguageType:
                            break;
                        case FunctionCodeLocation.LanguageType:
                            break;
                        default:
                            Log.Warning(
                                "Unhandled token {Character} when location type was {FunctionCodeLocation} as position {Position} on line {LineNumber}",
                                character, functionCodeLocation, position, lineNumber);
                            break;
                    }

                    break;
                case '(':
                    switch (functionCodeLocation)
                    {
                        case FunctionCodeLocation.FunctionName:
                            functionArgument = new();
                            functionCodeLocation = FunctionCodeLocation.BeforeFunctionArgumentType;
                            break;
                        case FunctionCodeLocation.BeforeFunctionBody:
                            functionCodeLocation = FunctionCodeLocation.FunctionBody;
                            functionDefinition.RawMethodBody += character;
                            break;
                        case FunctionCodeLocation.FunctionBody:
                            functionDefinition.RawMethodBody += character;
                            break;
                        default:
                            Log.Warning(
                                "Unhandled token {Character} when location type was {FunctionCodeLocation} as position {Position} on line {LineNumber}",
                                character, functionCodeLocation, position, lineNumber);
                            break;
                    }

                    break;
                case ')':
                    switch (functionCodeLocation)
                    {
                        case FunctionCodeLocation.BeforeFunctionArgumentType:
                            functionCodeLocation = FunctionCodeLocation.BeforeLanguageType;
                            break;                        
                        case FunctionCodeLocation.FunctionArgumentName:
                            if (!string.IsNullOrWhiteSpace(functionArgument.ArgumentName) &&
                                !string.IsNullOrWhiteSpace(functionArgument.ArgumentType))
                            {
                                functionDefinition.FunctionSignature.FunctionArguments.Add(functionArgument);
                            }

                            functionArgument = new();
                            functionCodeLocation = FunctionCodeLocation.BeforeLanguageType;
                            break;
                        case FunctionCodeLocation.BeforeFunctionBody:
                            functionCodeLocation = FunctionCodeLocation.FunctionBody;
                            functionDefinition.RawMethodBody += character;
                            break;
                        case FunctionCodeLocation.FunctionBody:
                            functionDefinition.RawMethodBody += character;
                            break;
                        default:
                            Log.Warning(
                                "Unhandled token {Character} when location type was {FunctionCodeLocation} as position {Position} on line {LineNumber}",
                                character, functionCodeLocation, position, lineNumber);
                            break;
                    }

                    break;
                case ',':
                    switch (functionCodeLocation)
                    {
                        case FunctionCodeLocation.FunctionArgumentName:
                            if (!string.IsNullOrWhiteSpace(functionArgument.ArgumentName) &&
                                !string.IsNullOrWhiteSpace(functionArgument.ArgumentType))
                            {
                                functionDefinition.FunctionSignature.FunctionArguments.Add(functionArgument);
                            }

                            functionArgument = new();
                            functionCodeLocation = FunctionCodeLocation.BeforeFunctionArgumentType;
                            break;
                        case FunctionCodeLocation.BeforeFunctionBody:
                            functionCodeLocation = FunctionCodeLocation.FunctionBody;
                            functionDefinition.RawMethodBody += character;
                            break;
                        case FunctionCodeLocation.FunctionBody:
                            functionDefinition.RawMethodBody += character;
                            break;
                        default:
                            Log.Warning(
                                "Unhandled token {Character} when location type was {FunctionCodeLocation} as position {Position} on line {LineNumber}",
                                character, functionCodeLocation, position, lineNumber);
                            break;
                    }

                    break;
                case '{':
                    switch (functionCodeLocation)
                    {
                        case FunctionCodeLocation.LanguageType:
                            if (nestCount > 0)
                            {
                                functionDefinition.RawMethodBody += character;
                            }
                            else
                            {
                                functionCodeLocation = FunctionCodeLocation.BeforeFunctionBody;
                            }

                            nestCount++;
                            break;
                        case FunctionCodeLocation.BeforeFunctionBody:
                            if (nestCount > 0)
                            {
                                functionDefinition.RawMethodBody += character;
                            }

                            nestCount++;
                            break;
                        case FunctionCodeLocation.FunctionBody:
                            if (nestCount > 0)
                            {
                                functionDefinition.RawMethodBody += character;
                            }

                            nestCount++;
                            break;
                        default:
                            Log.Warning(
                                "Unhandled token {Character} when location type was {FunctionCodeLocation} as position {Position} on line {LineNumber}",
                                character, functionCodeLocation, position, lineNumber);
                            break;
                    }

                    break;
                case '}':
                    switch (functionCodeLocation)
                    {
                        case FunctionCodeLocation.FunctionBody:
                            nestCount--;
                            if (nestCount > 0)
                            {
                                functionDefinition.RawMethodBody += character;
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(functionDefinition.RawMethodBody))
                                {
                                    var labelBuilder = new StringBuilder(functionDefinition.FunctionSignature.Name.Length +
                                                                         functionDefinition.FunctionSignature.ReturnType.Length +
                                                                         (functionDefinition.FunctionSignature.FunctionArguments.Count *
                                                                          20));
                                    labelBuilder.Append('_');
                                    labelBuilder.Append(functionDefinition.FunctionSignature.Name);
                                    labelBuilder.Append('_');
                                    labelBuilder.Append(functionDefinition.FunctionSignature.ReturnType);
                                    foreach (var af in functionDefinition.FunctionSignature.FunctionArguments)
                                    {
                                        labelBuilder.Append('_');
                                        labelBuilder.Append(af.ArgumentType);                    
                                    }
                                    labelBuilder.Append(functionDefinition.FunctionSignature.ReturnType);
                                    functionDefinition.AssemblyLabel = labelBuilder.ToString();
                                    
                                    kernelDefinition.FunctionDefinitions.Add(functionDefinition);
                                }
                                functionDefinition = new();
                                functionCodeLocation = FunctionCodeLocation.BeforeReturnType;
                            }

                            break;
                        case FunctionCodeLocation.BeforeFunctionBody:
                            nestCount--;
                            if (nestCount > 0)
                            {
                                functionDefinition.RawMethodBody += character;
                            }
                            else
                            {
                                functionCodeLocation = FunctionCodeLocation.BeforeReturnType;
                            }

                            break;
                        default:
                            Log.Warning(
                                "Unhandled token {Character} when location type was {FunctionCodeLocation} as position {Position} on line {LineNumber}",
                                character, functionCodeLocation, position, lineNumber);
                            break;
                    }

                    break;
                case '\r':
                    break;
                case '\n':
                    switch (functionCodeLocation)
                    {
                        case FunctionCodeLocation.BeforeFunctionBody:
                            functionCodeLocation = FunctionCodeLocation.FunctionBody;
                            functionDefinition.RawMethodBody += character;
                            break;
                        case FunctionCodeLocation.FunctionBody:
                            functionDefinition.RawMethodBody += character;
                            break;
                        case FunctionCodeLocation.LanguageType:
                            //functionDefinition.RawMethodBody += character;
                            break;
                        case FunctionCodeLocation.BeforeReturnType:
                            break;
                        default:
                            Log.Warning(
                                "Unhandled token {Character} when location type was {FunctionCodeLocation} as position {Position} on line {LineNumber}",
                                character, functionCodeLocation, position, lineNumber);
                            break;
                    }                   
                    lineNumber++;
                    break;
                default:
                    switch (functionCodeLocation)
                    {
                        case FunctionCodeLocation.BeforeFunctionBody:
                            functionCodeLocation = FunctionCodeLocation.FunctionBody;
                            functionDefinition.RawMethodBody += character;
                            break;
                        case FunctionCodeLocation.FunctionBody:
                            functionDefinition.RawMethodBody += character;
                            break;
                        default:
                            Log.Warning(
                                "Unhandled token {Character} when location type was {FunctionCodeLocation} as position {Position} on line {LineNumber}",
                                character, functionCodeLocation, position, lineNumber);
                            break;
                    }
                    break;
            }

            position += 1;
        }
    }

    private void ParseTypeCodeFile(KernelDefinition kernelDefinition, CodeFile typeCodeFile)
    {
    }
}