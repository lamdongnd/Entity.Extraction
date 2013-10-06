using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Similarity
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
    }
}
