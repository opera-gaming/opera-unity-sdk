using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Opera
{
    public class OperaGxValidNamesCheckerTests
    {
        [TestCase(@"Simle Name", true)]
        [TestCase(@"Name with cyrillic: Это кириллица", true)]
        [TestCase(@"Name with unicode: Den här namn är unicåde", true)]
        [TestCase(@"Name with unicode: éèêëçę", true)]
        [TestCase(@"Doom 1234567890", true)]
        [TestCase(@"!? :.,()'&-", true)]
        [TestCase(@"[Prohibited parentheses]", false)]
        [TestCase(@"{Prohibited parentheses}", false)]
        [TestCase(@"Prohibited_underscore", false)]
        [TestCase(@"\", false)]
        [TestCase(@"/", false)]
        [TestCase(@">", false)]
        [TestCase(@"<", false)]
        [TestCase(@"", false)]
        [TestCase(@" ", false)]
        [TestCase(@"  ", false)]
        public void IsValid_Name_Output(string name, bool expectedOutput)
        {
            var checker = new OperaGxValidNamesChecker();

            var output = checker.IsValid(name);

            Assert.AreEqual(expectedOutput, output);
        }
    }
}
