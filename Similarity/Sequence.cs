using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Similarity
{
    public class Sequence
    {
        private List<string> outcomes = new List<string>();
        private List<double> scores = new List<double>();
        public double Score { get; private set; }

        public Sequence()
        {

        }

        public Sequence(IEnumerable<string> outcomes, IEnumerable<double> scores)
        {
            this.outcomes.AddRange(outcomes);
            this.scores.AddRange(scores);
            this.Score = scores.Sum(S => Math.Log(S));
        }

        public void AddOutcome(string outcome, double score)
        {
            this.outcomes.Add(outcome);
            this.scores.Add(score);
            this.Score += Math.Log(score);
        }

        public IEnumerable<double> Probabilities()
        {
            return this.scores;
        }

        public IEnumerable<string> Outcomes()
        {
            return this.outcomes;
        }
    }
}
