using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Opera.DirectoryAdapterFactoryMethods;

namespace Opera
{
    public sealed class DirectoriesComparerTests
    {
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void AreDirectoriesEqual_AnyDirectoryDoesNotExist_False(
            bool isFirstDirectoryExists, bool isSecondDirectoryExists) =>
            Test(
                comparer: NewDirectoriesComparerSkippingContent(),
                directory1: new DirectoryAdapterMock(new IFile[0], isFirstDirectoryExists),
                directory2: new DirectoryAdapterMock(new IFile[0], isSecondDirectoryExists),

                expectedResult: false
            );

        [Test]
        public void AreDirectoriesEqual_BothExistAndEmpty_True() =>
            Test(
                comparer: NewDirectoriesComparerSkippingContent(),
                directory1: new DirectoryAdapterMock(new IFile[0]),
                directory2: new DirectoryAdapterMock(new IFile[0]),

                expectedResult: true
            );

        private static readonly object[] filesCountsTestCase =
        {
            new object[] { new[] { "file1", "file2" }, new[] { "file1", "file2" }, true },

            new object[] { new[] { "file1"}, new[] { "file1", "file2" }, false },
            new object[] { new[] { "file1", "file2" }, new[] { "file1" }, false },
        };

        [TestCaseSource(nameof(filesCountsTestCase))]
        public void AreDirectoriesEqual_FilesCounts_Result(
            string[] fileNames1, string[] fileNames2, bool expectedResult) =>
            Test(
                comparer: NewDirectoriesComparerSkippingContent(),
                directory1: ToDirectory(fileNames1),
                directory2: ToDirectory(fileNames2),

                expectedResult
            );
        

        private static readonly object[] filesNamesTestCase =
        {
            new object[] { new[] { "file1", "file2" }, new[] { "file1", "file2" }, true },
            
            new object[] { new[] { "file1DIFF", "file2" }, new[] { "file1", "file2" }, false },
            new object[] { new[] { "file1", "file2" }, new[] { "file1", "file2DIFF" }, false },

        };

        [TestCaseSource(nameof(filesNamesTestCase))]
        public void AreDirectoriesEqual_FileNames_Result(
            string[] fileNames1, string[] fileNames2, bool expectedResult) =>
            Test(
                comparer: NewDirectoriesComparerSkippingContent(),
                directory1: ToDirectory(fileNames1),
                directory2: ToDirectory(fileNames2),

                expectedResult
            );

        // This test case is suitable for both sizes and first bytes comparison test.
        private static readonly object[] filesIntegerParameterTestCase =
        {
            new object[] { new[] { 2, 4 }, new[] { 2, 4 }, true },

            new object[] { new[] { 2, 5 }, new[] { 2, 4 }, false },
            new object[] { new[] { 2, 5 }, new[] { 1, 4 }, false },
        };

        [TestCaseSource(nameof(filesIntegerParameterTestCase))]
        public void AreDirectoriesEqual_FileSizes_Results
            (int[] fileSizes1, int[] fileSizes2, bool expectedResult) =>
            Test(
                comparer: NewDirectoriesComparerSkippingContent(),
                directory1: ToDirectory(fileSizes1),
                directory2: ToDirectory(fileSizes2),

                expectedResult
            );


        [TestCaseSource(nameof(filesIntegerParameterTestCase))]
        public void AreDirectoriesEqual_FileBytes_Results
            (int[] fileFirstBytes1, int[] fileFirstBytes2, bool expectedResult) =>
            Test(
                comparer: NewDirectoriesComparer(new CompareFirstByteMockFilesComparer()),
                directory1: ToDirectory(fileFirstBytes1.Select(Convert.ToByte).ToArray()),
                directory2: ToDirectory(fileFirstBytes2.Select(Convert.ToByte).ToArray()),

                expectedResult
            );

        /// <summary>
        /// Common testing function for all the tests
        /// </summary>
        private void Test(
            DirectoriesComparer comparer,
            IDirectory directory1,
            IDirectory directory2,
            bool expectedResult)
        {
            // Act
            var actualResult = comparer.AreDirectoriesEqual(directory1, directory2);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        private DirectoriesComparer NewDirectoriesComparerSkippingContent()
        {
            return new DirectoriesComparer(new AlwaysTrueFilesComparer());
        }

        private DirectoriesComparer NewDirectoriesComparer(IFilesContentComparer filesComparer)
        {
            return new DirectoriesComparer(filesComparer);
        }
    }

    public sealed class AlwaysTrueFilesComparer : IFilesContentComparer
    {
        public bool AreFileContentsEqual(IFile file1, IFile file2) => true;
    }

    public sealed class CompareFirstByteMockFilesComparer : IFilesContentComparer
    {
        public bool AreFileContentsEqual(IFile file1, IFile file2)
        {
            // Compare only the first byte of each file
            byte[] file1Bytes = file1.ReadAllBytes();
            byte[] file2Bytes = file2.ReadAllBytes();

            return file1Bytes[0] == file2Bytes[0];
        }
    }
}
