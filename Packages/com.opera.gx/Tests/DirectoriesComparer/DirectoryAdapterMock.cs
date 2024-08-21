using System.Linq;
using static Opera.FileAdapterFactoryMethods;

namespace Opera
{
    public sealed class DirectoryAdapterMock : IDirectory
    {
        public IFile[] AllAssetFiles { get; }
        public bool Exists { get; }

        public string PathRelativeToProjectRoot => throw new System.NotImplementedException();

        public DirectoryAdapterMock(IFile[] allFilePaths, bool exists)
        {
            AllAssetFiles = allFilePaths;
            Exists = exists;
        }

        public DirectoryAdapterMock(IFile[] allFilePaths)
        {
            AllAssetFiles = allFilePaths;
            Exists = true;
        }
    }

    public sealed class DirectoryAdapterFactoryMethods
    {
        public static IDirectory ToDirectory(string[] paths) => new DirectoryAdapterMock(paths.Select(ToFile).ToArray());
        public static IDirectory ToDirectory(int[] sizes) => new DirectoryAdapterMock(sizes.Select(ToFile).ToArray());
        public static IDirectory ToDirectory(byte[] firstBytes) => new DirectoryAdapterMock(firstBytes.Select(ToFile).ToArray());
        public static IDirectory ToDirectory(byte[][] contents) => new DirectoryAdapterMock(contents.Select(ToFile).ToArray());
    }
}
