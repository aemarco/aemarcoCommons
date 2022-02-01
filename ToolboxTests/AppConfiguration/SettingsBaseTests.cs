using aemarcoCommons.Toolbox.AppConfiguration;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ToolboxTests.AppConfiguration
{
    public class SettingsBaseTests
    {
        [Test]
        public void Init_Works()
        {
            SettingsBase.ConfigurationRoot = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["TestConfig:Key1"] = "Value1",
                        ["TestConfig:Key2:1"] = "Array0",
                        ["TestConfig:Key2:2"] = "Array1",
                        ["TestConfig:Key3"] = bool.TrueString,
                        ["TestConfig:TestSubConfig:Sub1:1"] = "SubArray0",
                        ["TestConfig:TestSubConfig:Sub1:2"] = "SubArray1"
                    })
                .Build();
            SettingsBase.ConfigurationOptions = new ConfigurationOptions
            {
                WatchSavedFiles = false
            };

            var config = new TestConfig();
            //populate properties
            config.Key1.Should().Be("Value1");
            config.Key3.Should().BeTrue();

            //populate collections
            config.Key2.Length.Should().Be(2);
            config.Key2[0].Should().Be("Array0");
            config.Key2[1].Should().Be("Array1");

            //populate sub configs
            
            //TODO: uncomment, when array merging is not a thing anymore in SettingsBase
            //config.TestSubConfig.Sub1.Length.Should().Be(2);
            config.TestSubConfig.Sub1[0].Should().Be("SubArray0");
            config.TestSubConfig.Sub1[1].Should().Be("SubArray1");
        }
    }


    public class TestConfig : SettingsBase
    {
        public string Key1 { get; set; }
        public string[] Key2 { get; set; }
        public bool Key3 { get; set; }
        public TestSubConfig TestSubConfig { get; set; }
    }

    [SettingPath("TestConfig:TestSubConfig")]
    public class TestSubConfig : SettingsBase
    {
        public string[] Sub1 { get; set; }
    }

}
