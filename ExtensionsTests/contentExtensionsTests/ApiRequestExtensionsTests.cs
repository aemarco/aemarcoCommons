using Contracts.Api.RequestObjects;
using Extensions.contentExtensions;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;

namespace ExtensionsTests.contentExtensionsTests
{
    public class ApiRequestExtensionsTests
    {

        static object[] CheatCases
        {
            get
            {
                List<object> result = new List<object>();

                foreach (var cheat in CheatCode.Cheats)
                {
                    result.Add(new object[] { cheat.Key, cheat.IsSupervisor, false });
                    result.Add(new object[] { cheat.Key, cheat.IsSupervisor, true });
                }
                return result.ToArray();
            }
        }
        [TestCaseSource(nameof(CheatCases))]
        public void ToUserIsSupervisorInfo_Correct(string cheat, bool cheatSupervisor, bool userSupervisor)
        {
            var expected = userSupervisor || cheatSupervisor;
            var reuest = new WallpaperFilterRequest
            {
                Search = cheat
            };

            reuest.ToUserIsSupervisorInfo(userSupervisor).Should().Be(expected);
        }

    }
}
