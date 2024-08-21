using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Opera.DirectoryAdapterFactoryMethods;

namespace Opera
{
    /// <summary>
    /// Integration test for both DirectoriesComparer and FilesComparer.
    /// Testing only comparison based on file contents because all other checks
    /// are sufficiently tested by unit tests of DirectoriesComparer.
    /// </summary>
    public sealed class DirectoriesAndFilesComparerTests
    {
        private static readonly object[] bytesListsTestCase =
        {
            // All files have the same contents:
            new object[] {
                new[] {
                    new byte[] {1, 2, 3, 4},
                    new byte[] {5, 6, 7, 8, 9, 10, 11, 12},
                },
                new[] {
                    new byte[] {1, 2, 3, 4},
                    new byte[] {5, 6, 7, 8, 9, 10, 11, 12},
                },
                true
            },

            // Files have different contents:
            new object[] {
                new[] {
                    new byte[] {1, 2, 3, 4, 5},
                    new byte[] {5, 6, 7, 8, 9, 10, 11, 12},
                },
                new[] {
                    new byte[] {1, 2, 3, 4},
                    new byte[] {5, 6, 7, 8, 9, 10, 11, 12},
                },
                false
            },
            new object[] {
                new[] {
                    new byte[] {9, 2, 3, 4},
                    new byte[] {5, 6, 7, 8, 9, 10, 11, 12},
                },
                new[] {
                    new byte[] {1, 2, 3, 4},
                    new byte[] {5, 6, 7, 8, 9, 10, 11, 12},
                },
                false
            },
            new object[] {
                new[] {
                    new byte[] {1, 2, 3, 4},
                    new byte[] {5, 6, 7, 8, 9, 10, 11, 12},
                },
                new[] {
                    new byte[] {1, 2, 3, 4},
                    new byte[] {9, 6, 7, 8, 9, 10, 11, 12},
                },
                false
            },

        };

        [TestCaseSource(nameof(bytesListsTestCase))]
        public void AreDirectoriesEqual_FileBytes_Results(
            byte[][] contents1, byte[][] contents2, bool expectedResult)
        {
            var comparer = new DirectoriesComparer(new FilesContentComparer());
            var directory1 = ToDirectory(contents1);
            var directory2 = ToDirectory(contents2);

            var actualResult = comparer.AreDirectoriesEqual(directory1, directory2);

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}

