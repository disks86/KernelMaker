namespace KernelMakerLibrary;

public class ImageBuilderFactory
{
    public static IImageBuilder GetImageBuilder(UserOptions userOptions)
    {
        return new DDImageBuilder();
    }
}