using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Opera
{
    public class BuildVersionTests
    {
        [TestCaseSource(nameof(CompareTo_TestData))]
        public void CompareTo_Versions_Result(BuildVersion currentVersion, BuildVersion comparedVersion, int expected)
        {
            var comparison = currentVersion.CompareTo(comparedVersion);

            Assert.AreEqual(expected, comparison);
        }

        [TestCaseSource(nameof(OperatorGreaterThen_TestData))]
        public void OperatorGreaterThen_Versions_Result(BuildVersion a, BuildVersion b, bool expected)
        {
            var isGreater = a > b;

            Assert.AreEqual(expected, isGreater);
        }

        [TestCaseSource(nameof(OperatorLessThen_TestData))]
        public void OperatorLessThen_Versions_Result(BuildVersion a, BuildVersion b, bool expected)
        {
            var isLess = a < b;

            Assert.AreEqual(expected, isLess);
        }

        private readonly static object[] CompareTo_TestData = 
        {
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 100, 1000), 0 },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 100, 1001), -1 },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 100, 999), 1 },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 101, 1000), -1 },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 101, 99), -1 },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 99, 1000), 1 },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 99, 1010), 1 },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 11, 100, 1000), -1 },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 9, 100, 1000), 1 },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(2, 10, 100, 1000), -1 },
            new object[] { new BuildVersion(2, 10, 100, 1000), new BuildVersion(1, 10, 100, 1000), 1 },
            new object[] { new BuildVersion(2, 10, 100, 1000), new BuildVersion(1, 99, 100, 1000), 1 },
        };

        private readonly static object[] OperatorGreaterThen_TestData =
        {
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 100, 1000), false },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 100, 1001), false },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 100, 999), true },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 101, 1000), false },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 101, 99), false },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 99, 1000), true },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 99, 1010), true },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 11, 100, 1000), false },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 9, 100, 1000), true },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(2, 10, 100, 1000), false },
            new object[] { new BuildVersion(2, 10, 100, 1000), new BuildVersion(1, 10, 100, 1000), true },
        };

        private readonly static object[] OperatorLessThen_TestData =
        {
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 100, 1000), false },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 100, 1001), true },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 100, 999), false },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 101, 1000), true },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 101, 99), true },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 99, 1000), false },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 10, 99, 1010), false },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 11, 100, 1000), true },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(1, 9, 100, 1000), false },
            new object[] { new BuildVersion(1, 10, 100, 1000), new BuildVersion(2, 10, 100, 1000), true },
            new object[] { new BuildVersion(2, 10, 100, 1000), new BuildVersion(1, 10, 100, 1000), false },
            new object[] { new BuildVersion(2, 10, 100, 1000), new BuildVersion(1, 99, 100, 1000), false },
        };
    }
}
