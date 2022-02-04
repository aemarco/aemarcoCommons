using aemarcoCommons.Toolbox.AppConfiguration;
using aemarcoCommons.Toolbox.AppConfiguration.Transformations;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ToolboxTests.AppConfiguration
{
    public class SettingsBaseTests
    {

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SettingsBase.ConfigurationRoot = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["TestConfig:Key1"] = "Value1",
                        ["TestConfig:Key2:1"] = "Array0",
                        ["TestConfig:Key2:2"] = "Array1",
                        ["TestConfig:Key3"] = bool.TrueString,
                        ["TestConfig:Protected"] = "/Qdo8NegyvGzmxx/A7FrDGf3zLD1hj0/zZzZtSAGs7wCkJViLedzWVguKIvegiShAg==",
                        ["TestConfig:Combined"] = "{{{Key1}}}_Combined",
                        ["TestConfig:TestSubConfig:Sub1"] = "SubValue1",
                        ["TestConfig:TestSubConfig:Sub2:1"] = "SubArray0",
                        ["TestConfig:TestSubConfig:Sub2:2"] = "SubArray1",
                        ["TestConfig:TestSubConfig:Sub3"] = bool.TrueString,
                        ["Root1"] = "RootValue1",
                        ["Root2:1"] = "RootArray0",
                        ["Root2:2"] = "RootArray1",
                        ["Root3"] = bool.TrueString,
                        ["Reload"] = "ReloadSaved",
                    })
                .Build();
            SettingsBase.ConfigurationOptions = new ConfigurationOptions
            {
                StringTransformations =
                {
                    new PlaceholderTransformation(),
                    new ProtectedTransformer("Password")
                    
                },
                WatchSavedFiles = true
            };
        }
        

        [Test]
        public void Init_WorksWithString()
        {
            var config = new TestConfig();
            config.Key1.Should().Be("Value1");
        }
        [Test]
        public void Init_WorksWithBool()
        {
            var config = new TestConfig();
            config.Key3.Should().BeTrue();
        }
        [Test]
        public void Init_WorksWithArray()
        {
            var config = new TestConfig();
            config.Key2.Length.Should().Be(2);
            config.Key2[0].Should().Be("Array0");
            config.Key2[1].Should().Be("Array1");
        }

        [Test]
        public void Init_WorksWithSubSettingString()
        {
            var subConfig = new TestConfig().TestSubConfig;
            subConfig.Sub1.Should().Be("SubValue1");
        }
        [Test]
        public void Init_WorksWithSubSettingBool()
        {
            var subConfig = new TestConfig().TestSubConfig;
            subConfig.Sub3.Should().BeTrue();
        }
        [Test]
        public void Init_WorksWithSubSettingArray()
        {
            var subConfig = new TestConfig().TestSubConfig;
            subConfig.Sub2.Length.Should().Be(2);
            subConfig.Sub2[0].Should().Be("SubArray0");
            subConfig.Sub2[1].Should().Be("SubArray1");
        }

        [Test]
        public void Init_WorksWithRootString()
        {
            var rootConfig = new RootConfig();
            rootConfig.Root1.Should().Be("RootValue1");
        }
        [Test]
        public void Init_WorksWithRootBool()
        {
            var rootConfig = new RootConfig();
            rootConfig.Root3.Should().BeTrue();
        }
        [Test]
        public void Init_WorksWithRootArray()
        {
            var rootConfig = new RootConfig();
            rootConfig.Root2.Length.Should().Be(2);
            rootConfig.Root2[0].Should().Be("RootArray0");
            rootConfig.Root2[1].Should().Be("RootArray1");
        }

        [Test]
        public void Init_DoesProtectedTransformations()
        {
            var config = new TestConfig();
            config.Protected.Should().Be("Decrypted");
        }

        [Test]
        public void Init_DoesPlaceHolderTransformations()
        {
            var config = new TestConfig();
            config.Combined.Should().Be("Value1_Combined");
        }

        [Test]
        public void Save_SaveWorks()
        {
            var subConfig = new TestConfig().TestSubConfig;

            var path = subConfig.Save();
            var saved = File.ReadAllText(path);
            File.Delete(path);
            var expeted = @"{
  ""TestConfig"": {
    ""TestSubConfig"": {
      ""Sub1"": ""SubValue1"",
      ""Sub2"": [
        ""SubArray0"",
        ""SubArray1""
      ],
      ""Sub3"": true
    }
  }
}";
            //saved content all right
            saved.Should().Be(expeted);

            //saved at the right path
            path.Should().Be(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "savedSettings.TestConfig_TestSubConfig.json"));
        }


        [Test]
        public void Save_SaveWorksWithPath()
        {
            var subConfig = new TestConfig().TestSubConfig;
            

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testFile.txt");
            subConfig.Save(path);

            //saved at the right path
            new FileInfo(path).Exists.Should().BeTrue();
            
            File.Delete(path);
        }
        
    }
    

    public class TestConfig : SettingsBase
    {
        public string Key1 { get; set; }
        public string[] Key2 { get; set; }
        public bool Key3 { get; set; }
        public TestSubConfig TestSubConfig { get; set; }
        
        [Protected]
        public string Protected { get; set; }

        public string Combined { get; set; }
    }
    [SettingPath("TestConfig:TestSubConfig")]
    public class TestSubConfig : SettingsBase
    {
        public string Sub1 { get; set; }
        public string[] Sub2 { get; set; }
        public bool Sub3 { get; set; }
    }
    [SettingPath("")]
    public class RootConfig : SettingsBase
    {
        public string Root1 { get; set; }
        public string[] Root2 { get; set; }
        public bool Root3 { get; set; }
        public string Reload { get; set; }
    }
}
