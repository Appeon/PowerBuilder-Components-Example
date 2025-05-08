
using FluentFTP;
using PowerBuilderEventInvoker.DotNet;

namespace Appeon.ComponentsApp.FtpClientWrapper
{
    public class FtpClientWrapper
    {
        private readonly AsyncFtpClient client;

        private readonly IList<(string, string)> callbacks;

        internal FtpClientWrapper(AsyncFtpClient client)
        {
            this.client = client;
            callbacks = new List<(string, string)>();
        }


        private void ReportError(string error)
        {
            foreach (var (callbackObject, callbackEvent) in callbacks)
            {
                EventInvoker.InvokeEvent(
                    callbackObject, callbackEvent, error
                    );
            }
        }

        public void Disconnect()
        {
            client.Disconnect().Wait();
        }

        public FileInfo? GetFileInfo(string path, out string? error)
        {
            error = null;
            FtpListItem info;
            try
            {
                info = client.GetObjectInfo(path).Result;
            }
            catch (Exception e)
            {
                error = e.Message;
                return null;
            }

            return new FileInfo
            {
                Created = info.Created,
                Modified = info.Modified,
                Name = info.Name,
                Path = info.FullName,
                Permissions = info.RawPermissions,
                Size = info.Size,
                Type = info.Type switch
                {
                    FtpObjectType.Directory => FileType.Directory,
                    _ => FileType.File,
                },
            };

        }

        public void RegisterErrorCallback(string callbackObject, string callbackEvent)
        {
            callbacks.Add((callbackObject, callbackEvent));
        }

        public async void DownloadFile(string ftpPath,
            string systemPath,
            string callbackObject,
            string finishCallbackEvent,
            string progressCallbackEvent)
        {
            EventInvoker.TestObjectEventInvokation(callbackObject);

            if (ftpPath is null)
            {
                ReportError("Path not specified");
                return;
            }

            if (!await client.FileExists(ftpPath))
            {
                ReportError("Specified path doesn't exist");
                return;
            }

            try
            {
                int? previousProgress = null;
                await client.DownloadFile(systemPath,
                        ftpPath,
                        progress: new Progress<FtpProgress>((progress) =>
                        {
                            int currentProgress = (int)progress.Progress;

                            if (previousProgress == null || currentProgress > previousProgress)
                            {
                                previousProgress = currentProgress;
                                EventInvoker.InvokeEvent(callbackObject, progressCallbackEvent, progress.Progress.ToString());
                            }
                        }
                        ), existsMode: FtpLocalExists.Overwrite);
            }
            catch (Exception e)
            {
                ReportError(e.Message);
                return;
            }

            EventInvoker.InvokeEvent(callbackObject, finishCallbackEvent);
        }

        public void UploadFile(string ftpPath,
            string systemPath,
            string callbackObject,
            string finishCallbackEvent,
            string progressCallbackEvent)
        {
            EventInvoker.TestObjectEventInvokation(callbackObject);

            UploadFileInternal(ftpPath,
                systemPath,
                callbackObject,
                finishCallbackEvent,
                progressCallbackEvent);
        }


        private async void UploadFileInternal(string ftpPath,
            string systemPath,
            string callbackObject,
            string finishCallbackEvent,
            string progressCallbackEvent)
        {
            if (!await client.DirectoryExists(ftpPath))
            {
                ReportError("Specified path is not a directory");
                return;
            }

            if (!File.Exists(systemPath))
            {
                ReportError("Specified file does not exist");
                return;
            }

            try
            {
                var filename = Path.GetFileName(systemPath);
                int? previousProgress = null;
                await client.UploadFile(localPath: systemPath,
                        remotePath: ftpPath + filename,
                        createRemoteDir: true,
                        existsMode: FtpRemoteExists.Overwrite,
                        progress: new Progress<FtpProgress>((progress) =>
                        {
                            int currentProgress = (int)progress.Progress;
                            if (/*(((int)currentProgress) % 10) == 0 && */
                            previousProgress == null || currentProgress > previousProgress)
                            {
                            }
                            previousProgress = currentProgress;
                            try
                            {
                                EventInvoker.InvokeEvent(callbackObject, progressCallbackEvent, currentProgress.ToString());
                            }
                            catch (Exception e)
                            {
                                ReportError(e.Message);
                                return;
                            }
                        }
                        ));
            }
            catch (Exception e)
            {
                ReportError((e.InnerException ?? e).Message);
                return;
            }

            EventInvoker.InvokeEvent(callbackObject, finishCallbackEvent);
        }

        public int RetrieveContentsOfPath(string path,
            out int[]? contentTypes,
            out string[]? contentNames,
            out string? error
        )
        {
            contentTypes = null;
            contentNames = null;
            error = null;

            if (client.DirectoryExists(path).Result)
            {
                FtpListItem[] contents;
                try
                {
                    contents = client.GetListing(path).Result;
                }
                catch (Exception e)
                {
                    error = e.Message;
                    return -1;
                }

                contentTypes = new int[contents.Length];
                contentNames = new string[contents.Length];
                for (int i = 0; i < contents.Length; ++i)
                {
                    contentTypes[i] = contents[i].Type switch
                    {
                        FtpObjectType.Directory => 1,
                        _ => 2
                    };

                    contentNames[i] = contents[i].Name;

                }

                return 1;
            }
            else
            {
                error = "Specified path doesn't exist";
                return -1;
            }
        }

        public int CreateDirectory(string path, string name, out string? error)
        {
            error = null;

            try
            {
                if (!client.DirectoryExists(path).Result)
                {
                    error = "Requested path is not a directory";
                    return -1;
                }

                var created = client.CreateDirectory(Path.Combine(path, name)).Result;
                if (!created)
                {
                    throw new Exception("Directory could not be created. Please check permissions");
                }
            }
            catch (Exception e)
            {
                error = (e.InnerException ?? e).Message;
                return -1;
            }

            return 1;
        }

        public int DeleteFile(string path, out string? error)
        {
            error = null;

            try
            {
                if (!client.FileExists(path).Result)
                {
                    error = "Path is not a file";
                    return -1;
                }

                client.DeleteFile(path)
                    .Wait();
            }
            catch (Exception e)
            {
                error = (e.InnerException ?? e).Message;
                throw;
            }

            return 1;
        }

        public int DeleteFolder(string path, out string? error)
        {
            error = null;

            try
            {
                if (!client.DirectoryExists(path).Result)
                {
                    error = "Path is not a directory";
                    return -1;
                }

                client.DeleteDirectory(path)
                    .Wait();
            }
            catch (Exception e)
            {
                error = (e.InnerException ?? e).Message;
                return -1;
            }

            return 1;
        }
    }
}