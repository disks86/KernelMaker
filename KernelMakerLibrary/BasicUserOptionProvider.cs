using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace KernelMakerLibrary;

using System.IO;

public class BasicUserOptionProvider
    : IUserOptionProvider
{
    public UserOptions GetUserOptions()
    {
        var userOptions = new UserOptions
        {
            RootPath = Environment.CurrentDirectory,
            TempPath = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString()),
            OutputPath = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString()),
            LogPath = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString()),
            RemotePackageCachePath = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString()),
            TargetArchitecture = "x86"
        };

        var optionType = typeof(UserOptions);
        
        foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            var name = entry.Key?.ToString() ?? string.Empty;
            if (name.StartsWith("KernelMaker") && name.Contains('_'))
            {
                var splits = name.Split('_');
                var propertyName = splits[1];
                var propertyInfo = optionType.GetProperty(propertyName);
                if (propertyInfo == null || !propertyInfo.CanRead) continue;
                var value = splits[1];
                propertyInfo.SetValue(userOptions, value);              
            }
        }
        
        foreach (var argument in Environment.GetCommandLineArgs())
        {
            if (argument.Contains('='))
            {
                var splits = argument.Split('=');
                var propertyName = splits[0];
                var propertyInfo = optionType.GetProperty(propertyName);
                if (propertyInfo == null || !propertyInfo.CanRead) continue;
                var value = splits[1];
                propertyInfo.SetValue(userOptions, value);
            }
        }
        
        return userOptions;
    }
}