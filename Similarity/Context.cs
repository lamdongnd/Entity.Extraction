using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Similarity
{
    public class Context
    {
        private int width;

        public Context(int width)
        {
            this.width = width;
        }

        public string[] Generate(IEnumerable<Token> tokens, int index, IEnumerable<string> previousOutcomes)
        {
            var features = new List<string>();

            for (var i = this.width - 1; i > -1; i--)
            {
                var currentIndex = index - i;
                if (currentIndex > -1)
                {
                    var token = tokens.ElementAt(currentIndex);
                    features.AddRange(token.Features.Select(S => i + "-" + S).ToArray());
                    features.Add(i + "-" + token.GetText());
                    features.Add(i + "-" + previousOutcomes.ElementAt(currentIndex));
                }
            }

            return features.ToArray();
        }
    }
}
