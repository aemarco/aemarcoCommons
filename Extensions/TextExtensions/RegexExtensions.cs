using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace aemarcoCommons.Extensions.TextExtensions;

public static partial class RegexExtensions
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
                   new MailAddress(email) is not null;
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


#nullable enable

    private static readonly string[] DefaultFormats =
    [
        "yyyyMMdd", "MMddyyyy", "ddMMyyyy", "MMddyy", "ddMMyy", "yyMMdd",
        "yyyy-MM-dd", "dd-MM-yy", "MM-dd-yy", "yy-MM-dd",
        "yyyy_MM_dd", "dd_MM_yy", "MM_dd_yy", "yy_MM_dd",
        "yyyy.MM.dd", "dd.MM.yy", "MM.dd.yy", "yy.MM.dd",
        "yyyy/MM/dd", "MM/dd/yy", "dd/MM/yy", "yy/MM/dd",
        "yyyy|MM|dd", "MM|dd|yy", "dd|MM|yy", "yy|MM|dd",
    ];


    [GeneratedRegex(@"(\d{8})|\d{2,4}([\-_.|\/]\d{1,2}){0,2}", RegexOptions.Compiled)]
    private static partial Regex DatePattern();

    public static DateTimeOffset? ToDateTimeOffset(
        this string input,
        int? minYear = null,
        int? maxYear = null,
        IEnumerable<string>? formats = null,
        IEnumerable<string>? ignoreList = null)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var min = minYear ?? 1980;
        var max = maxYear ?? DateTimeOffset.Now.Year;

        var formatsInternal = formats?.ToArray() ?? DefaultFormats;
        var ignore = ignoreList != null
            ? new HashSet<string>(ignoreList)
            : [];

        var matches = DatePattern().Matches(input);
        var ci = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        ci.Calendar.TwoDigitYearMax = DateTimeOffset.Now.Year;
        foreach (var m in matches.OrderByDescending(x => x.Value.Length))
        {
            var value = m.Value;

            if (ignore.Contains(value))
                continue;

            // format-Match
            if (DateTimeOffset.TryParseExact(
                    value,
                    formatsInternal,
                    ci,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var dt))
            {
                if (dt.Year >= min && dt.Year <= max)
                    return dt;
                continue;
            }

            // only year
            if (int.TryParse(value, out var year) &&
                year >= min && year <= max)
            {
                return new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
            }
        }

        return null;
    }
}