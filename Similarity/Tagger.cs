using SharpEntropy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Similarity
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
            var possibleSequences = new List<Sequence>();
            possibleSequences.Add(new Sequence());
            var outcomeCount = model.GetOutcomeNames().Length;
            var tokens = this.annotator.Annotate(this.tokenizer.Tokenize(text));
            for (var i = 0; i < tokens.Length; i++)
            {
                var nextSequences = new List<Sequence>();
                foreach (var possibleOutcome in model.GetOutcomeNames())
                {
                    foreach (var sequence in possibleSequences)
                    {
                        var nextSequence = new Sequence(sequence.Outcomes(), sequence.Probabilities());
                        var possibleOutcomes = nextSequence.Outcomes().ToList();
                        possibleOutcomes.Add(possibleOutcome);
                        var outcomes = model.Evaluate(context.Generate(tokens, i, possibleOutcomes));
                        var outcomeIndex = model.GetOutcomeIndex(possibleOutcome);
                        nextSequence.AddOutcome(possibleOutcome, outcomes[outcomeIndex]);
                        nextSequences.Add(nextSequence);
                    }
                }
                possibleSequences = nextSequences.OrderByDescending(S => S.Score).Take(outcomeCount * 2).ToList();
            }
            var bestSequence = possibleSequences.OrderByDescending(S => S.Score).First();
            var bestOutcomes = bestSequence.Outcomes().ToArray();
            probabilities = bestSequence.Probabilities().ToArray();
            for (var i = 0; i < tokens.Count(); i++)
            {
                tokens[i].Type = bestOutcomes[i].Split('.')[1];
            }
            return tokens;
        }
    }
}
