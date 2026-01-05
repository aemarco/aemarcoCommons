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





    [GeneratedRegex(@"\d{8}", RegexOptions.Compiled)]
    private static partial Regex ClearNumberDatePattern();
    private static readonly string[] ClearNumberFormats =
    [
        // Number-only formats LAST
        "yyyyMMdd", "ddMMyyyy", "MMddyyyy",
    ];


    [GeneratedRegex(@"(\d{4}([\-_.|\/\s]\d{1,2}){2}|\d{1,2}([\-_.|\/\s]\d{1,2}){1}([\-_.|\/\s]\d{4}))", RegexOptions.Compiled)]
    private static partial Regex ClearSeparatedDatePattern();
    private static readonly string[] ClearSeparatedFormats =
    [
        // Jahr vorne (YYYY sep M/D)
        "yyyy-M-d", "yyyy.M.d", "yyyy/M/d", "yyyy M d", "yyyy_M_d",
        "yyyy-MM-d", "yyyy.MM.d", "yyyy/MM/d", "yyyy MM d", "yyyy_MM_d",
        "yyyy-M-dd", "yyyy.M.dd", "yyyy/M/dd", "yyyy M dd", "yyyy_M_dd",
        "yyyy-MM-dd", "yyyy.MM.dd", "yyyy/MM/dd", "yyyy MM dd", "yyyy_MM_dd",

        // Jahr hinten (D/M sep YYYY)
        "d-M-yyyy", "d_M_yyyy", "d.M.yyyy", "d|M|yyyy", "d/M/yyyy", "d M yyyy",
        "d-MM-yyyy", "d_MM_yyyy", "d.MM.yyyy", "d|MM|yyyy", "d/MM/yyyy", "d MM yyyy",
        "dd-M-yyyy", "dd_M_yyyy", "dd.M.yyyy", "dd|M|yyyy", "dd/M/yyyy", "dd M yyyy",
        "dd-MM-yyyy", "dd_MM_yyyy", "dd.MM.yyyy", "dd|MM|yyyy", "dd/MM/yyyy", "dd MM yyyy",

        // optional: US-Variante Monat-Tag-Jahr
        "M-d-yyyy", "M_d_yyyy", "M.d.yyyy", "M|d|yyyy", "M/d/yyyy", "M d yyyy",
        "M-dd-yyyy", "M_dd_yyyy", "M.dd.yyyy", "M|dd|yyyy", "M/dd/yyyy", "M dd yyyy",
        "MM-d-yyyy", "MM_d_yyyy", "MM.d.yyyy", "MM|d|yyyy", "MM/d/yyyy", "MM d yyyy",
        "MM-dd-yyyy", "MM_dd_yyyy", "MM.dd.yyyy", "MM|dd|yyyy", "MM/dd/yyyy", "MM dd yyyy"
    ];

    [GeneratedRegex(@"\d{6}", RegexOptions.Compiled)]
    private static partial Regex AmbiguousNumberDatePattern();
    private static readonly string[] AmbiguousNumberFormats =
    [
        "yyMMdd",
        "ddMMyy",
        "MMddyy"
    ];

    [GeneratedRegex(@"\d{2}([\-_.|\/\s]\d{1,2}){1,2}", RegexOptions.Compiled)]
    private static partial Regex AmbiguousSeparatedDatePattern();
    private static readonly string[] AmbiguousSeparatedFormats =
    [

        // Asiatische/abgekürzte Variante: Jahr-Monat-Tag (yy)
        "yy-M-d", "yy.M.d", "yy/M/d", "yy M d", "yy_M_d",
        "yy-MM-d", "yy.MM.d", "yy/MM/d", "yy MM d", "yy_MM_d",
        "yy-M-dd", "yy.M.dd", "yy/M/dd", "yy M dd", "yy_M_dd",
        "yy-MM-dd", "yy.MM.dd", "yy/MM/dd", "yy MM dd", "yy_MM_dd",

        // US-Variante: Monat-Tag-Jahr (yy)
        "M-d-yy", "M.d.yy", "M/d/yy", "M d yy", "M_M_yy",
        "M-dd-yy", "M.dd.yy", "M/dd/yy", "M DD yy", "M_DD_yy",
        "MM-d-yy", "MM.d.yy", "MM/d/yy", "MM d yy", "MM_MM_yy",
        "MM-dd-yy", "MM.dd.yy", "MM/dd/yy", "MM DD yy", "MM_DD_yy",

        // Europäische Variante: Tag-Monat-Jahr (yy)
        "d-M-yy", "d.M.yy", "d/M/yy", "d M yy", "d_M_yy",
        "d-MM-yy", "d.MM.yy", "d/MM/yy", "d MM yy", "d_MM_yy",
        "dd-M-yy", "dd.M.yy", "dd/M/yy", "dd M yy", "dd_M_yy",
        "dd-MM-yy", "dd.MM.yy", "dd/MM/yy", "dd MM yy", "dd_MM_yy"
    ];

    [GeneratedRegex(@"\d{4}", RegexOptions.Compiled)]
    private static partial Regex YearOnlyPattern();

    public static DateTimeOffset? ToDateTimeOffset(
        this string input,
        int? minYear = null,
        int? maxYear = null)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var min = minYear ?? 1980;
        var max = maxYear ?? DateTimeOffset.Now.Year;
        var ci = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        ci.Calendar.TwoDigitYearMax = DateTimeOffset.Now.Year;


        var matches = ClearNumberDatePattern().Matches(input);
        foreach (var m in matches.OrderByDescending(x => x.Value.Length))
        {
            foreach (var format in ClearNumberFormats)
            {
                // format-Match
                if (DateTimeOffset.TryParseExact(
                        m.Value,
                        format,
                        ci,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                        out var dt))
                {
                    if (dt.Year >= min && dt.Year <= max)
                        return dt;
                }
            }
        }
        matches = ClearSeparatedDatePattern().Matches(input);
        foreach (var m in matches.OrderByDescending(x => x.Value.Length))
        {
            foreach (var format in ClearSeparatedFormats)
            {
                // format-Match
                if (DateTimeOffset.TryParseExact(
                        m.Value,
                        format,
                        ci,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                        out var dt))
                {
                    if (dt.Year >= min && dt.Year <= max)
                        return dt;
                }
            }
        }
        matches = AmbiguousNumberDatePattern().Matches(input);
        foreach (var m in matches.OrderByDescending(x => x.Value.Length))
        {
            foreach (var format in AmbiguousNumberFormats)
            {
                // format-Match
                if (DateTimeOffset.TryParseExact(
                        m.Value,
                        format,
                        ci,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                        out var dt))
                {
                    if (dt.Year >= min && dt.Year <= max)
                        return dt;
                }
            }
        }
        matches = AmbiguousSeparatedDatePattern().Matches(input);
        foreach (var m in matches.OrderByDescending(x => x.Value.Length))
        {
            foreach (var format in AmbiguousSeparatedFormats)
            {
                // format-Match
                if (DateTimeOffset.TryParseExact(
                        m.Value,
                        format,
                        ci,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                        out var dt))
                {
                    if (dt.Year >= min && dt.Year <= max)
                        return dt;
                }
            }
        }
        matches = YearOnlyPattern().Matches(input);
        foreach (var m in matches.OrderByDescending(x => x.Value.Length))
        {
            // only year
            if (int.TryParse(m.Value, out var year) &&
                year >= min && year <= max)
            {
                return new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
            }
        }
        return null;
    }




}