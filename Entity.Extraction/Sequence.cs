using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Extraction
{
    public class Sequence : IComparable<Sequence>
    {
        public List<string> Outcomes { get; private set; }
        public List<double> Scores { get; private set; }
        public double Score { get; private set; }

        public Sequence(int capacity)
        {
            this.Outcomes = new List<string>(capacity);
            this.Scores = new List<double>(capacity);
        }

        public Sequence(List<string> outcomes, List<double> scores, double score, int capacity) : this(capacity)
        {
            this.Outcomes.AddRange(outcomes);
            this.Scores.AddRange(scores);
            this.Score = score;
        }

        public void AddOutcome(string outcome, double score)
        {
            this.Outcomes.Add(outcome);
            this.Scores.Add(score);
            this.Score += Math.Log(score);
        }

        int IComparable<Sequence>.CompareTo(Sequence other)
        {
            return this.Score.CompareTo(other.Score) * -1;
        }
    }
}
