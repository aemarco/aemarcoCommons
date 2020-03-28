using System;

namespace Contracts.Api.RequestObjects
{
    public class DownloadableCategoryReqObject
    {
        public string UserId { get; set; }
        public Version Version { get; set; }
    }
}
