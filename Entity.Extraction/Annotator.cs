using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Entity.Extraction
{
    public class Annotator
    {
        private IEnumerable<Action<Token>> features;

        public Annotator(IEnumerable<Action<Token>> features)
        {
            this.features = features;
        }

        public Token[] Annotate(Token[] tokens)
        {
            foreach (var token in tokens)
            {
                foreach (var feature in this.features)
                {
                    feature(token);
                }
            }
            return tokens;
        }

        public static Annotator Default()
        {
            var lowercasePattern = new Regex("^[a-z]+$");
            var twoDigitsPattern = new Regex("^[0-9][0-9]$");
            var fourDigitsPattern = new Regex("^[0-9][0-9][0-9][0-9]$");
            var containsNumberPattern = new Regex("[0-9]");
            var containsLetterPattern = new Regex("[a-zA-Z]");
            var containsHyphensPattern = new Regex("-");
            var containsBackslashPattern = new Regex("/");
            var containsCommaPattern = new Regex(",");
            var containsPeriodPattern = new Regex("\\.");
            var allCapsPattern = new Regex("^[A-Z]+$");
            var capPeriodPattern = new Regex("^[A-Z]\\.$");
            var initialCapPattern = new Regex("^[A-Z]");

            var features = new List<Action<Token>>();

            features.Add(S =>
            {
                if (lowercasePattern.IsMatch(S.GetText()))
                {
                    S.Features.Add("lower");
                }
            });

            features.Add(S =>
            {
                if (twoDigitsPattern.IsMatch(S.GetText()))
                {
                    S.Features.Add("2-digit");
                }
            });

            features.Add(S =>
            {
                if (fourDigitsPattern.IsMatch(S.GetText()))
                {
                    S.Features.Add("4-digit");
                }
            });

            features.Add(S =>
            {
                if (containsNumberPattern.IsMatch(S.GetText()))
                {
                    S.Features.Add("contains-number");
                }
            });

            features.Add(S =>
            {
                if (containsLetterPattern.IsMatch(S.GetText()))
                {
                    S.Features.Add("contains-letter");
                }
            });

            features.Add(S =>
            {
                if (containsHyphensPattern.IsMatch(S.GetText()))
                {
                    S.Features.Add("contains-hyphen");
                }
            });

            features.Add(S =>
            {
                if (containsBackslashPattern.IsMatch(S.GetText()))
                {
                    S.Features.Add("contains-backslash");
                }
            });

            features.Add(S =>
            {
                if (containsCommaPattern.IsMatch(S.GetText()))
                {
                    S.Features.Add("contains-comma");
                }
            });

            features.Add(S =>
            {
                if (containsPeriodPattern.IsMatch(S.GetText()))
                {
                    S.Features.Add("contains-period");
                }
            });

            features.Add(S =>
            {
                if (allCapsPattern.IsMatch(S.GetText()))
                {
                    S.Features.Add("all-caps");
                }
            });

            features.Add(S =>
            {
                if (capPeriodPattern.IsMatch(S.GetText()))
                {
                    S.Features.Add("cap-period");
                }
            });

            features.Add(S =>
            {
                if (initialCapPattern.IsMatch(S.GetText()))
                {
                    S.Features.Add("first-cap");
                }
            });

            // prefixes
            features.Add(S =>
            {
                var s = S.GetText();
                if (s.Length > 4)
                {
                    for (var i = 1; i < Math.Min(4, s.Length); i++)
                    {
                        S.Features.Add("p-" + i + "-" + s.Substring(0, i));
                    }
                }
            });

            // suffixes
            features.Add(S =>
            {
                var s = S.GetText();
                if (s.Length > 4)
                {
                    for (var i = 1; i < Math.Min(4, s.Length); i++)
                    {
                        S.Features.Add("s-" + i + "-" + s.Substring(s.Length - i));
                    }
                }
            });

            return new Annotator(features);
        }
    }
}
