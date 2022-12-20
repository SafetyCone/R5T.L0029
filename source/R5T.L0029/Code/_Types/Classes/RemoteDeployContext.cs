using System;

using R5T.T0142;


namespace R5T.L0029
{
    [DataTypeMarker]
    public class RemoteDeployContext
    {
        public string SourceLocalBinariesDirectoryPath { get; set; }
        public string DestinationRemoteBinariesDirectoryPath { get; set; }
        public string LocalTemporaryDirectoryPath { get; set; }
        public string RemoteTemporaryDirectoryPath { get; set; }
        public string ArchiveFileName { get; set; }
    }
}
