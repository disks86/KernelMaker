using System.Net;

namespace KernelMakerLibrary;

public class BasicRemoteCodeFileProvider
    : IRemoteCodeFileProvider
{
    public async void DownloadRemoteCodeFiles(UserOptions userOptions)
    {
        var remotePackageCachePath = userOptions.RemotePackageCachePath;
        if (remotePackageCachePath != null)
        {
            foreach (var remotePackage in userOptions.RemotePackages)
            {
                var filename = Path.Join(remotePackageCachePath, remotePackage.Name.Replace(':', Path.PathSeparator));
                if (!File.Exists(filename))
                {
                    using HttpClient client = new HttpClient();
                    using var response = await client.GetAsync(remotePackage.Url);
                    await using var urlStream = await response.Content.ReadAsStreamAsync();
                    await using var fileStream = new FileStream(filename,FileMode.Create);
                    await urlStream.CopyToAsync(fileStream);
                }
            }
        }
    }
}