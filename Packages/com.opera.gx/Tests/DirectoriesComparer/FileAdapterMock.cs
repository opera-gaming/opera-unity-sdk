using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Opera
{
    public sealed class FileAdapterMock : IFile
    {
        public long Length { get; }

        private readonly string pathRelativeToDirectory;
        private byte[] bytes;

        public FileAdapterMock(string pathRelativeToDirectory, long length, byte[] bytes)
        {
            this.pathRelativeToDirectory = pathRelativeToDirectory;
            Length = length;
            this.bytes = bytes;
        }

        public byte[] ReadAllBytes() => bytes;

        public string PathRelativeTo(IDirectory directory)
        {
            return pathRelativeToDirectory;
        }
    }

    public sealed class FileAdapterFactoryMethods
    {
        public static IFile ToFile(string path) => new FileAdapterMock(path, 0, new byte[0]);
        public static IFile ToFile(int size) => new FileAdapterMock("path", size, new byte[0]);
        public static IFile ToFile(byte firstByte) => new FileAdapterMock("path", 1, new[] { firstByte });
        public static IFile ToFile(byte[] bytes) => new FileAdapterMock("path", bytes.Length, bytes);
    }
}
