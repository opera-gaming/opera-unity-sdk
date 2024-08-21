using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Opera
{
    public class GroupForNewGameFinderTests
    {
        // An array of groups with empty names and IDs looking like id0, id1, id2 etc.
        private static readonly GroupDataApi[] groups = Enumerable.Range(0, 5).Select(index => new GroupDataApi { studioId = $"id{index}" }).ToArray();

        private static readonly object[] returnedGroupTestCases =
        {
            // Currently selected group is found on cloud -> return the same group
            new object[] { groups[3], groups, groups[3] },

            // Currently selected group is not found on cloud -> return the first group
            new object[] { groups[4], groups.Take(2).ToArray(), groups[0] },

            // Currently selected group is null -> return the first group
            new object[] { null, groups, groups[0] },
        };

        [TestCaseSource(nameof(returnedGroupTestCases))]
        public void FindGroup_TestCaseData_ReturnMostAppropriateGroup(GroupDataApi currentGroup, GroupDataApi[] allGroups, GroupDataApi expectedGroup)
        {
            var finder = new GroupForNewGameFinder();

            var actualGroup = finder.FindGroup(currentGroup, allGroups).group;

            Assert.AreSame(expectedGroup, actualGroup);
        }

        private static readonly object[] eventTestCases =
{
            // Currently selected group is found on cloud
            new object[] { groups[3], groups, true },

            // Currently selected group is not found on cloud
            new object[] { groups[4], groups.Take(2).ToArray(), false },

            // Currently selected group is null
            new object[] { null, groups, true },
        };

        [TestCaseSource(nameof(eventTestCases))]
        public void FindGroup_TestCaseData_ReturnSuccessFlag(GroupDataApi currentGroup, GroupDataApi[] allGroups, bool expectedSuccess)
        {
            var finder = new GroupForNewGameFinder();

            var actualSuccess = finder.FindGroup(currentGroup, allGroups).success;
            
            Assert.AreEqual(expectedSuccess, actualSuccess);
        }
    }
}
