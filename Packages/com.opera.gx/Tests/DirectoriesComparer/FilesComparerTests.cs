using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Opera.FileAdapterFactoryMethods;

namespace Opera
{
    public sealed class FilesComparerTests
    {
        private static readonly object[] testCase =
{
            new object[] { new byte[] { 2, 4, 5 }, new byte[] { 2, 4, 5 }, true },
            new object[] { new byte[] { 2, 5, 5 }, new byte[] { 2, 4, 5 }, false },
            new object[] { new byte[] { 2, 4, 5 }, new byte[] { 2, 4, 5, 6 }, false },

        };

        [TestCaseSource(nameof(testCase))]
        public void AreFileContentsEqual(byte[] bytes1, byte[] bytes2, bool expectedResult)
        {
            var comparer = new FilesContentComparer();
            var file1 = ToFile(bytes1);
            var file2 = ToFile(bytes2);

            var actualResult = comparer.AreFileContentsEqual(file1, file2);

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
