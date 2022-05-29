namespace KernelMakerLibrary;

public interface ICodeFileProvider
{
    CodeFileSet GetCodeFiles(UserOptions userOptions);
}