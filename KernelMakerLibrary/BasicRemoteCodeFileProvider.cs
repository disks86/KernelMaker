using System.Net;

namespace KernelMakerLibrary;

public class BasicRemoteCodeFileProvider
    : IRemoteCodeFileProvider
{
    public void DownloadRemoteCodeFiles(UserOptions userOptions)
    {
        var remotePackageCachePath = userOptions.RemotePackageCachePath;
        if (remotePackageCachePath != null)
        {
            foreach (var remotePackage in userOptions.RemotePackages)
            {
                var filename = Path.Join(remotePackageCachePath, remotePackage.Name.Replace(':', Path.PathSeparator));
                if (!File.Exists(filename))
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile("http://example.com/file/song/a.mpeg", filename);
                    }
                }
            }
        }
    }
}