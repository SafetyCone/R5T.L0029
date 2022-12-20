using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Renci.SshNet;

using R5T.F0000;
using R5T.F0030;
using R5T.T0132;
using R5T.T0144;


namespace R5T.L0029
{
    [FunctionalityMarker]
    public partial interface IDeployOperator : IFunctionalityMarker
    {
        public async Task DeployToRemote(
            RemoteServerAuthentication remoteServerAuthentication,
            RemoteDeployContext remoteDeployContext,
            IEnumerable<Func<RemoteDeployContext, SshClient, Task>> preDeployActions,
            IEnumerable<Func<RemoteDeployContext, SshClient, Task>> postDeployActions,
            ILogger logger)
        {
            FileSystemOperator.Instance.EnsureDirectoryExists(remoteDeployContext.SourceLocalBinariesDirectoryPath);

            // Archive locally.
            var localArchiveFilePath = F0002.PathOperator.Instance.GetFilePath(
                remoteDeployContext.LocalTemporaryDirectoryPath,
                remoteDeployContext.ArchiveFileName);

            FileSystemOperator.Instance.DeleteFile_OkIfNotExists(localArchiveFilePath);

            logger.LogInformation($"Archiving to local file...\n\t{localArchiveFilePath}");

            ZipFileOperator.Instance.CreateFromDirectory(
                remoteDeployContext.SourceLocalBinariesDirectoryPath,
                localArchiveFilePath);

            logger.LogInformation($"Archived to local file.\n\t{localArchiveFilePath}");

            // SFTP archive to remote.
            var remoteArchiveFilePath = F0002.PathOperator.Instance.GetFilePath(
                remoteDeployContext.RemoteTemporaryDirectoryPath,
                remoteDeployContext.ArchiveFileName);

            await SshOperator.Instance.InConnectionContext(
                remoteServerAuthentication,
                async connection =>
                {
                    // Upload the file.
                    logger.LogInformation($"Uploading archive file to remote server...\n\t{remoteArchiveFilePath}");

                    SftpOperator.Instance.InSftpContext_Connected_Synchronous(
                        connection,
                        sftpClient =>
                        {
                            using var fileStream = FileStreamOperator.Instance.OpenRead(localArchiveFilePath);
                                
                            sftpClient.UploadFile(fileStream, remoteArchiveFilePath, true);
                        });

                    logger.LogInformation($"Uploaded archive file to remote server.\n\t{remoteArchiveFilePath}");

                    await SshOperator.Instance.InSshContext_Connected(
                        connection,
                        async sshClient =>
                        {
                            logger.LogInformation("Performing remote server commands...");

                            // Run pre-deploy actions.
                            logger.LogInformation("Performing pre-deploy remote server commands...");

                            await ActionOperator.Instance.Run(
                                remoteDeployContext, sshClient,
                                preDeployActions);

                            // Delete the directory, using SSH command.
                            logger.LogInformation($"Deleting remote deploy directory...\n\t{remoteDeployContext.DestinationRemoteBinariesDirectoryPath}");

                            var deleteDirectoryCommand = sshClient.RunCommand($"sudo rm -rf \"{remoteDeployContext.DestinationRemoteBinariesDirectoryPath}\"");

                            RemoteCommandOperator.Instance.LogCommandResult(deleteDirectoryCommand, logger);

                            // Unzip to the directory.
                            logger.LogInformation($"Unzipping archive to remote deploy directory...\n\t{remoteArchiveFilePath}\n\t{remoteDeployContext.DestinationRemoteBinariesDirectoryPath}");

                            var unzipCommand = sshClient.RunCommand($"sudo unzip -o \"{remoteArchiveFilePath}\" -d \"{remoteDeployContext.DestinationRemoteBinariesDirectoryPath}\"");

                            RemoteCommandOperator.Instance.LogCommandResult(unzipCommand, logger);

                            // Run post-deploy actions.
                            logger.LogInformation("Performing post-deploy remote server commands...");

                            await ActionOperator.Instance.Run(
                                remoteDeployContext, sshClient,
                                postDeployActions);
                        });
                });
        }
    }
}
