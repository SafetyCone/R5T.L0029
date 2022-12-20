using System;


namespace R5T.L0029
{
    public class RemoteDeployActions : IRemoteDeployActions
    {
        #region Infrastructure

        public static IRemoteDeployActions Instance { get; } = new RemoteDeployActions();


        private RemoteDeployActions()
        {
        }

        #endregion
    }
}
