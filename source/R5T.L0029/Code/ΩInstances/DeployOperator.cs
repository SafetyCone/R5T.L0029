using System;


namespace R5T.L0029
{
    public class DeployOperator : IDeployOperator
    {
        #region Infrastructure

        public static IDeployOperator Instance { get; } = new DeployOperator();


        private DeployOperator()
        {
        }

        #endregion
    }
}
