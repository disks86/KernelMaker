test = "hello"

def download_remote_files():
    print(RootPath)
    print(TempPath)
    print(OutputPath)
    print(LogPath)
    print(RemotePackageCachePath)
    print(TargetArchitecture)

if 'ScriptMode' in locals() and ScriptMode=="DownloadRemoteCodeFiles":
    download_remote_files()
else:
    print("Didn't work")
