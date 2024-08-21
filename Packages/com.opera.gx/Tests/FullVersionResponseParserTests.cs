using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TestTools;

namespace Opera
{
    public class FullVersionResponseParserTests
    {
        // Valid JSONs without errors:
        [TestCase(@"{""data"":{""isFullVersionPurchased"":false},""errors"":[]}", false)]
        [TestCase(@"{""data"":{""isFullVersionPurchased"":true},""errors"":[]}", true)]
        
        // Valid JSONs with errors:
        [TestCase(@"{""data"":null,""errors"":[{""code"":""game_full_version_not_allowed""}]}", false)]
        [TestCase(@"{""data"":"""",""errors"":[{""code"":""game_full_version_not_allowed""}]}", false)]

        // Invalid JSONs:
        [TestCase(@"{""data"":"""",""errors"":""something""}", false)]
        [TestCase(@"{""another field"": ""something""}", false)]
        public void Parse_Input_IsFullVersionPurchased(string input, bool expectedIsFullVersionPurchased)
        {
            var parser = CreateFullVersionResponseParser();

            var (data, _, _) = parser.Parse(input);

            var actualIsFullVersionPurchased = data?.isFullVersionPurchased ?? false;
            Assert.AreEqual(expectedIsFullVersionPurchased, actualIsFullVersionPurchased);
        }

        [Test]
        public void Parse_NotAJson_NullData()
        {
            var parser = CreateFullVersionResponseParser();
            var input = @"Not a JSON";

            var (data, _, _) = parser.Parse(input);

            Assert.IsNull(data);
        }

        // Valid JSONs without errors:
        [TestCase(@"{""data"":{""isFullVersionPurchased"":false},""errors"":[]}", true)]
        [TestCase(@"{""data"":{""isFullVersionPurchased"":true},""errors"":[]}", true)]

        // Valid JSONs with errors:
        [TestCase(@"{""data"":null,""errors"":[{""code"":""game_full_version_not_allowed""}]}", false)]
        [TestCase(@"{""data"":"""",""errors"":[{""code"":""game_full_version_not_allowed""}]}", false)]

        // Invalid JSONs:
        [TestCase(@"{""data"":"""",""errors"":""something""}", false)]
        [TestCase(@"{""another field"": ""something""}", false)]
        [TestCase(@"Not a JSON", false)]
        public void Parse_Input_IsOK(string input, bool expectedIsOk)
        {
            var parser = CreateFullVersionResponseParser();

            var (_, actualIsOk, _) = parser.Parse(input);

            Assert.AreEqual(expectedIsOk, actualIsOk);
        }

        private static readonly object[] Parse_Input_Errors_Testcases = 
        {
            // Valid JSONs:
            new object[] { @"{""data"":{""isFullVersionPurchased"":false},""errors"":[]}", new string[0] },
            new object[] { @"{""data"":{""isFullVersionPurchased"":false},""errors"":[{""code"":""error1""}, {""code"":""error2""}]}", new[] { "error1", "error2" } },
            new object[] { @"{""data"":null,""errors"":[{""code"":""error1""}, {""code"":""error2""}]}", new[] { "error1", "error2" } },
            new object[] { @"{""data"":"""",""errors"":[{""code"":""error1""}, {""code"":""error2""}]}", new[] { "error1", "error2" } },

            // Invalid JSONs:
            new object[] { @"{""data"":null,""errors"":""something""}", new string[0] },
            new object[] { @"{""another field"": ""something""}", new string[0] },
            new object[] { @"Not a JSON", new string[0] },

        };
        [TestCaseSource(nameof(Parse_Input_Errors_Testcases))]
        public void Parse_Input_Errors(string input, string[] expectedErrors)
        {
            var parser = CreateFullVersionResponseParser();

            var (_, _, actualErrors) = parser.Parse(input);

            Assert.IsTrue(expectedErrors.SequenceEqual(actualErrors));
        }

        private GxResponseParser<FullVersionResponseApi, FullVersionData> CreateFullVersionResponseParser() => new GxResponseParser<FullVersionResponseApi, FullVersionData>(logError: _ => { });
    }
}
