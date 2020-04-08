using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Contracts.Api;
using Contracts.Api.ResponseObjects;
using Extensions.netExtensions;
using Newtonsoft.Json;


namespace Extensions.contentExtensions
{
    public static class JwtExtensions
    {
        public static JwtTokenModel ToJwtTokenModel(this string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            try
            {
                //get payload base64
                var payload = Regex.Match(token, Contracts.Constants.JwtRegex).Groups[2].Value;
                //no valid token ?
                if (string.IsNullOrWhiteSpace(payload)) return null;
                //add filling signs
                while (payload.Length % 4 != 0) payload += "=";
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
                var model = JsonConvert.DeserializeObject<JwtTokenModel>(decoded);
                return model;
            }
            catch
            {
                return null;
            }
        }

        public static bool StillValid(this JwtTokenModel model)
        {
            return model?.ValidUntil.IsInFuture(TimeSpan.FromMinutes(1)) ?? false;
        }

        public static bool ShouldBeRenewed(this JwtTokenModel model, int percentTillRenewal = 70)
        {
            if (!model.StillValid()) return false;

            var durationSeconds = (model.ValidUntil - model.IssuedAt).TotalSeconds;
            var renewalAt = model.IssuedAt.AddSeconds(durationSeconds / 100 * percentTillRenewal);

            return !renewalAt.IsInFuture();
        }

        public static bool ShouldBeRenewed(this JwtTokenModel model, TimeSpan minimumDuration)
        {
            if (!model.StillValid()) return false;
            return !model.ValidUntil.IsInFuture(minimumDuration);
        }



    }
}
