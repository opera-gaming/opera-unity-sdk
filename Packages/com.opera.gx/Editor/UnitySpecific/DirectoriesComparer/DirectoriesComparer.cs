using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text;

namespace Opera
{
    public sealed class DirectoriesComparer
    {
        private readonly IFilesContentComparer filesComparer;

        public DirectoriesComparer(IFilesContentComparer filesComparer)
        {
            this.filesComparer = filesComparer;
        }

        /// <summary>
        /// Compares two files by the content of all their files. Takes into account other
        /// parameters (like files count, their names, sizes etc) before checking the
        /// binary content.
        /// </summary>
        /// <param name="path1">Path to the first directory relative to the project root.</param>
        /// <param name="path2">Path to the second directory relative to the project root.</param>
        /// <returns></returns>
        public bool AreDirectoriesEqual(string path1, string path2)
        {
            return AreDirectoriesEqual(new DirectoryAdapter(path1), new DirectoryAdapter(path2));
        }

        public bool AreDirectoriesEqual(IDirectory directory1, IDirectory directory2)
        {
            if (!directory1.Exists || !directory2.Exists)
            {
                return false;
            }

            var files1 = directory1.AllAssetFiles;
            var files2 = directory2.AllAssetFiles;

            if (files1.Length != files2.Length)
            {
                return false;
            }

            return files1.Zip(files2, (a, b) => (a, b))
                         .Select(files => TwoFilesAreEqual(files.a, files.b, directory1, directory2))
                         .All(x => x);
        }

        private bool TwoFilesAreEqual(IFile file1, IFile file2, IDirectory directory1, IDirectory directory2)
        {
            var result = file1.PathRelativeTo(directory1) == file2.PathRelativeTo(directory2) &&
                   file1.Length == file2.Length &&
                   filesComparer.AreFileContentsEqual(file1, file2);

            return result;
        }
    }
}
