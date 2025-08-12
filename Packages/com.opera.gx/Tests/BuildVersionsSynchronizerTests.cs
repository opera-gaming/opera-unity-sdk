using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Opera
{
    public class BuildVersionsSynchronizerTests
    {
        // with forceMinimalNextVersion
        [TestCase(null, "0.0.0.0", "0.0.0.1", true, "0.0.0.0")]
        [TestCase(null, "0.0.0.2", "0.0.0.3", true, "0.0.0.0")]
        [TestCase("some-id", "0.0.0.0", "0.0.0.3", true, "0.0.0.1")]
        [TestCase("some-id", "0.0.0.2", "0.0.0.3", true, "0.0.0.3")]
        [TestCase("some-id", "0.0.0.2", "0.0.0.9", true, "0.0.0.3")]

        // when the next version is greater than the current one
        [TestCase(null, "0.0.0.2", "0.0.0.3", false, "0.0.0.3")]
        [TestCase(null, "0.0.0.2", "0.0.0.9", false, "0.0.0.9")]
        [TestCase("some-id", "0.0.0.2", "0.0.0.3", false, "0.0.0.3")]
        [TestCase("some-id", "0.0.0.2", "0.0.0.9", false, "0.0.0.9")]

        // when the next version is less or equal than the current one
        [TestCase(null, "0.0.0.2", "0.0.0.2", false, "0.0.0.3")]
        [TestCase(null, "0.0.0.2", "0.0.0.1", false, "0.0.0.3")]
        [TestCase(null, "0.0.0.9", "0.0.0.1", false, "0.0.0.10")]
        [TestCase("some-id", "0.0.0.2", "0.0.0.2", false, "0.0.0.3")]
        [TestCase("some-id", "0.0.0.2", "0.0.0.1", false, "0.0.0.3")]
        [TestCase("some-id", "0.0.0.9", "0.0.0.1", false, "0.0.0.10")]
        public void FindNewNextVersion_Inputs_OutputVersion(
            string gameId, 
            string currentVersion, 
            string nextVersion, 
            bool forceMinimalNextVersion,
            string expectedNewNextVersion)
        {
            var synchronizer = new BuildVersionsSynchronizer();

            var newNextVersion = synchronizer.FindNewNextVersion(
                gameId,
                new BuildVersion(currentVersion),
                new BuildVersion(nextVersion),
                forceMinimalNextVersion);

            Assert.AreEqual(expectedNewNextVersion, newNextVersion.ToString());
        }
    }
}
