using System;
using System.Text;
using System.Text.RegularExpressions;
using Contracts.Api;
using Extensions.TimeExtensions;
using Newtonsoft.Json;

namespace Extensions.JwtExtensions
{
    public static class Extensions
    {
        /// <summary>
        /// Sanitizes a string to exactly the token
        /// </summary>
        /// <param name="token">string to sanitize</param>
        /// <returns>entire token</returns>
        public static string SanitizeToken(this string token)
        {
            return Regex.Match(token, Contracts.Constants.JwtRegex).Value;
        }

        /// <summary>
        /// Converts a bearer token to a JwtTokenModel
        /// </summary>
        /// <param name="token">token to convert</param>
        /// <returns>JtwTokenModel from payload, or null on invalid token</returns>
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

        /// <summary>
        /// Checks if at least valid for 1 minute according
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool StillValid(this JwtTokenModel model)
        {
            return model?.ValidUntil.IsFuture() ?? false;
        }

        /// <summary>
        /// Check if given token should be renewed, based on percent of duration till renewal suggestion
        /// </summary>
        /// <param name="model">token to check</param>
        /// <param name="percentTillRenewal">percent of duration till renewal suggested</param>
        /// <returns>true only if still valid and should be renewed</returns>
        public static bool ShouldBeRenewed(this JwtTokenModel model, int percentTillRenewal = 70)
        {
            if (model == null) return false;
            if (!model.StillValid()) return false;

            var durationSeconds = (model.ValidUntil - model.IssuedAt).TotalSeconds;
            var renewalAt = model.IssuedAt.AddSeconds(durationSeconds / 100 * percentTillRenewal);

            return renewalAt.IsPast();
        }

        /// <summary>
        /// Check if given token should be renewed, based on duration which the token should be still valid
        /// </summary>
        /// <param name="model">token to check</param>
        /// <param name="minimumDuration">duration, for which the token should be still valid</param>
        /// <returns>true only if still valid and should be renewed</returns>
        public static bool ShouldBeRenewed(this JwtTokenModel model, TimeSpan minimumDuration)
        {
            if (model == null) return false;
            if (!model.StillValid()) return false;
            return !model.ValidUntil.StillIsFuture(minimumDuration);
        }
    }
}
