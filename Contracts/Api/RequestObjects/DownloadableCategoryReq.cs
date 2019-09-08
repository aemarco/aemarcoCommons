using System;

namespace Contracts.Api.RequestObjects
{
    public class DownloadableCategoryReq
    {
        public string Cheat { get; set; }
        public Version Version { get; set; }
    }
}
