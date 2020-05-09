using System;
using System.Text;

namespace Extensions.NumberExtensions
{
    public static class AdultExtensions
    {

        public static string ToAdultStringFromDisplayAdult(this int adultLevel, int userMax = 39)
        {
            return adultLevel.ToRealAdult(userMax).ToAdultString();
        }

        public static string ToAdultString(this int adultLevel)
        {
            var sb = new StringBuilder();

            if (adultLevel >= 101)
            {
                sb.Append("Unknown");
            }
            else if (adultLevel >= 90)
            {
                sb.Append("PornHC+");
            }
            else if (adultLevel >= 80)
            {
                sb.Append("PornHC");
            }
            else if (adultLevel >= 70)
            {
                sb.Append("Porn+");
            }
            else if (adultLevel >= 60)
            {
                sb.Append("Porn");
            }
            else if (adultLevel >= 50)
            {
                sb.Append("Erotic+");
            }
            else if (adultLevel >= 40)
            {
                sb.Append("Erotic");
            }
            else if (adultLevel >= 30)
            {
                sb.Append("Sexy+");
            }
            else if (adultLevel >= 20)
            {
                sb.Append("Sexy");
            }
            else if (adultLevel >= 10)
            {
                sb.Append("Girl");
            }
            else if (adultLevel >= 0)
            {
                sb.Append("None");
            }
            return sb.ToString();
        }

        public static int ToDisplayAdult(this int adultLevel, int userMax)
        {
            var userRange = (userMax <= 100) ? 100 : 101;
            var result = (int)Math.Round((1.0 * adultLevel / userMax) * userRange);
            return result;
        }

        public static int ToRealAdult(this int displayAdult, int userMax)
        {
            var userRange = (userMax <= 100) ? 100 : 101;


            if (displayAdult < 0) displayAdult = 0;
            if (displayAdult > userRange) displayAdult = userRange;


            var result = (int)Math.Round((1.0 * displayAdult / userRange) * userMax);
            return result;
        }


    }
}
