using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Renci.SshNet;

using R5T.F0000;
using R5T.F0030;
using R5T.T0132;


namespace R5T.L0029
{
    [FunctionalityMarker]
    public partial interface IRemoteDeployActions : IFunctionalityMarker
    {
        /// <summary>
        /// Allows waiting a while during a deployment.
        /// </summary>
        public Func<RemoteDeployContext, SshClient, Task> Wait(
            int milliseconds = 3000)
        {
            return async (remoteDeployContext, sshClient) =>
            {
                await Task.Delay(milliseconds);
            };
        }

        public Func<RemoteDeployContext, SshClient, Task> CheckServiceStatus(
            string serviceName,
            ILogger logger)
        {
            return (remoteDeployContext, sshClient) =>
            {
                logger.LogInformation($"Checking status of service '{serviceName}'...");

                var statusCommand = sshClient.RunCommand($"sudo systemctl status {serviceName}");

                RemoteCommandOperator.Instance.LogCommandResult(statusCommand, logger);

                return Task.CompletedTask;
            };
        }

        public Func<RemoteDeployContext, SshClient, Task> RestartService(
            string serviceName,
            ILogger logger)
        {
            return (remoteDeployContext, sshClient) =>
            {
                logger.LogInformation($"Restarting the service '{serviceName}'...");

                var restartCommand = sshClient.RunCommand($"sudo systemctl restart {serviceName}");

                RemoteCommandOperator.Instance.LogCommandResult(restartCommand, logger);

                return Task.CompletedTask;
            };
        }

        public Func<RemoteDeployContext, SshClient, Task> ChangePermissionsOnRemoteDirectory(ILogger logger)
        {
            return (remoteDeployContext, sshClient) =>
            {
                logger.LogInformation($"Changing permissions on remote deploy directory...\n\t{remoteDeployContext.DestinationRemoteBinariesDirectoryPath}");

                var changePermissionsCommand = sshClient.RunCommand($"sudo chmod -R +777 {remoteDeployContext.DestinationRemoteBinariesDirectoryPath}");

                RemoteCommandOperator.Instance.LogCommandResult(changePermissionsCommand, logger);

                return Task.CompletedTask;
            };
        }

        public IEnumerable<Func<RemoteDeployContext, SshClient, Task>> NoActions()
        {
            return EnumerableOperator.Instance.Empty<Func<RemoteDeployContext, SshClient, Task>>();
        }

        public Task None(RemoteDeployContext remoteDeployContext, SshClient sshClient)
        {
            // Do nothing.

            return Task.CompletedTask;
        }
    }
}
