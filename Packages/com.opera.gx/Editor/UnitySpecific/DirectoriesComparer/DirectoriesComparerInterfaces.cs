using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Opera
{
    public interface IDirectory
    {
        string PathRelativeToProjectRoot { get; }

        /// <summary>
        /// Paths of all the files relative to the project root.
        /// Includes files in subdirectories
        /// Does not include *.meta files.
        /// Files are guaranteed to be sorted alphabetically.
        /// </summary>
        IFile[] AllAssetFiles { get; }

        /// <summary>
        /// Gets a value which indicates whether the directory exists.
        /// </summary>
        bool Exists { get; }
    }

    public interface IFile
    {
        /// <summary>
        /// Gets the path of the file relative to the folder.
        /// </summary>
        string PathRelativeTo(IDirectory directory);

        /// <summary>
        /// Gets the size, in bytes, of the current file.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Opens a file, reads the contents of the file into a byte array, and then closes the file.
        /// </summary>
        /// <param name="path">Path relative to the project root</param>
        /// <returns></returns>
        byte[] ReadAllBytes();
    }

    public interface IFilesContentComparer
    {
        /// <summary>
        /// Compares two files by their content. Does not take into account their names, sizes etc.
        /// </summary>
        bool AreFileContentsEqual(IFile file1, IFile file2);
    }
}
