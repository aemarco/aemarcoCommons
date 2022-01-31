using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace aemarcoCommons.Extensions.TextExtensions
{
    public static class RegexExtensions
    {
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)) &&
                       // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                       new MailAddress(email) is MailAddress _;
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        
        public static IEnumerable<long> GetNumbersFromText(this string text)
        {
            var matches = Regex.Matches(text, @"\d+");
            foreach (Match match in matches)
            {
                yield return long.Parse(match.Value);
            }
        }

        public static string ToFriendlyFilename(this string fileName)
        {
            return Regex.Replace(fileName, @"[^A-Za-z0-9_\.~]+", "-");
        }



    }
}
