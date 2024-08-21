using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Opera
{
    public sealed class FilesContentComparer : IFilesContentComparer
    {
        public bool AreFileContentsEqual(IFile file1, IFile file2)
        {
            return Enumerable.SequenceEqual(file1.ReadAllBytes(), file2.ReadAllBytes());
        }
    }
}
