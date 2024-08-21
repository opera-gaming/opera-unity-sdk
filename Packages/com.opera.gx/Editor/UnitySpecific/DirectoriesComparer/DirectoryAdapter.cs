using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Opera
{
    public sealed class DirectoryAdapter : IDirectory
    {
        /// <summary>
        /// Paths of all the files relative to the project root.
        /// Includes files in subdirectories
        /// Does not include *.meta files.
        /// Files are guaranteed to be sorted alphabetically.
        /// </summary>
        public IFile[] AllAssetFiles
        {
            get
            {
                var allFileNames = Directory.GetFiles(PathRelativeToProjectRoot, "*", SearchOption.AllDirectories)
                                        .Where(fileName => !fileName.EndsWith(".meta"))
                                        .ToList();
                allFileNames.Sort();

                return allFileNames.Select(name => new FileAdapter(name)).ToArray();
            }
        }

        /// <summary>
        /// Gets a value which indicates whether the directory exists.
        /// </summary>
        public bool Exists => Directory.Exists(PathRelativeToProjectRoot);

        public string PathRelativeToProjectRoot { get; }

        public DirectoryAdapter(string pathRelativeToProjectRoot)
        {
            PathRelativeToProjectRoot = pathRelativeToProjectRoot;
        }
    }
}
