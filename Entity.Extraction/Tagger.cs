using SharpEntropy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Extraction
{
    public class Tagger
    {
        private Tokenizer tokenizer;
        private Context context;
        private GisModel model;
        private Annotator annotator;

        public Tagger(Tokenizer tokenizer, Context context, Annotator annotator, GisModel model)
        {
            this.tokenizer = tokenizer;
            this.context = context;
            this.annotator = annotator;
            this.model = model;
        }

        public Token[] Tag(string text, out double[] probabilities)
        {
            var outcomeCount = model.GetOutcomeNames().Length;
            var outcomeNames = model.GetOutcomeNames();
            var outcomeIndexes = new Dictionary<string, int>();
            foreach (var outcomeName in outcomeNames)
            {
                outcomeIndexes.Add(outcomeName, this.model.GetOutcomeIndex(outcomeName));
            }
            var tokens = this.annotator.Annotate(this.tokenizer.Tokenize(text));

            var possibleSequences = new List<Sequence>();
            possibleSequences.Add(new Sequence(tokens.Length));

            for (var i = 0; i < tokens.Length; i++)
            {
                var nextSequences = new List<Sequence>(outcomeCount * 10);
                for (var j = 0; j < outcomeNames.Length; j++)
                {
                    var possibleOutcome = outcomeNames[j];
                    for (var k = 0; k < possibleSequences.Count; k++)
                    {
                        var sequence = possibleSequences[k];
                        var nextSequence = new Sequence(sequence.Outcomes, sequence.Scores, sequence.Score, tokens.Length);
                        var possibleOutcomes = nextSequence.Outcomes;
                        possibleOutcomes.Add(possibleOutcome);
                        var outcomes = model.Evaluate(context.Generate(tokens, i, possibleOutcomes));
                        possibleOutcomes.RemoveAt(possibleOutcomes.Count - 1);
                        var outcomeIndex = outcomeIndexes[possibleOutcome]; 
                        nextSequence.AddOutcome(possibleOutcome, outcomes[outcomeIndex]);
                        nextSequences.Add(nextSequence);
                    }
                }
                possibleSequences = nextSequences.OrderByDescending(S => S.Score).Take(10).ToList();
            }
            var bestSequence = possibleSequences.OrderByDescending(S => S.Score).First();
            var bestOutcomes = bestSequence.Outcomes;
            probabilities = bestSequence.Scores.ToArray();
            for (var i = 0; i < tokens.Count(); i++)
            {
                tokens[i].Type = bestOutcomes[i].Split('.')[1];
            }
            return tokens;
        }
    }
}
