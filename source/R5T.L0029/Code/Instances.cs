using System;


namespace R5T.L0029
{
    public static class Instances
    {
        public static L0066.IFileStreamOperator FileStreamOperator => L0066.FileStreamOperator.Instance;
        public static L0066.IFileSystemOperator FileSystemOperator => L0066.FileSystemOperator.Instance;
        public static L0066.IPathOperator PathOperator => L0066.PathOperator.Instance;
        public static F0030.ISftpOperator SftpOperator => F0030.SftpOperator.Instance;
        public static F0030.ISshOperator SshOperator => F0030.SshOperator.Instance;
    }
}