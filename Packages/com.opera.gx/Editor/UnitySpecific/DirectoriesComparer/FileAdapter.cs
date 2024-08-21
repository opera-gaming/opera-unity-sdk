using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Opera
{
    public sealed class FileAdapter : IFile
    {
        public long Length => fileInfo.Length;

        public string PathRelativeToProjectRoot { get; }

        private FileInfo fileInfo;

        public FileAdapter(string pathRelativeToProjectRoot)
        {
            fileInfo = new FileInfo(pathRelativeToProjectRoot);
            PathRelativeToProjectRoot = pathRelativeToProjectRoot;
        }

        public byte[] ReadAllBytes() => File.ReadAllBytes(PathRelativeToProjectRoot);

        public string PathRelativeTo(IDirectory directory)
        {
            return Path.GetRelativePath(directory.PathRelativeToProjectRoot, this.PathRelativeToProjectRoot);
        }
    }
}
