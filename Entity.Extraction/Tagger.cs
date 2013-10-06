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
            // cache a few things needed to save time
            var outcomeCount = model.GetOutcomeNames().Length;
            var outcomeNames = model.GetOutcomeNames();
            var outcomeIndexes = new Dictionary<string, int>();
            foreach (var outcomeName in outcomeNames)
            {
                outcomeIndexes.Add(outcomeName, this.model.GetOutcomeIndex(outcomeName));
            }

            // generate a sequence of features
            var tokens = this.annotator.Annotate(this.tokenizer.Tokenize(text));

            // holds the most likely sequences
            var possibleSequences = new List<Sequence>();

            // start with an empty sequence
            possibleSequences.Add(new Sequence(tokens.Length));

            // iterate through each token while trying to find the best combination of labels
            for (var i = 0; i < tokens.Length; i++)
            {
                // working set for the n best sequences
                var nextSequences = new List<Sequence>(outcomeCount * 10);

                // iterate through each possible outcome and evaluate based on the most likely sequences so far
                for (var j = 0; j < outcomeNames.Length; j++)
                {
                    var possibleOutcome = outcomeNames[j];

                    for (var k = 0; k < possibleSequences.Count; k++)
                    {
                        var sequence = possibleSequences[k];

                        // take a previous sequence, add this outcome, and then evaluate
                        var nextSequence = new Sequence(sequence.Outcomes, sequence.Scores, sequence.Score, tokens.Length);
                        var possibleOutcomes = nextSequence.Outcomes;
                        possibleOutcomes.Add(possibleOutcome);
                        
                        // ngram based feature generation
                        var outcomes = model.Evaluate(context.Generate(tokens, i, possibleOutcomes));

                        // remove the previously added outcome because it will be added below with the score
                        possibleOutcomes.RemoveAt(possibleOutcomes.Count - 1);
                        var outcomeIndex = outcomeIndexes[possibleOutcome]; 

                        // add the possible outcome and keep track of the score
                        nextSequence.AddOutcome(possibleOutcome, outcomes[outcomeIndex]);
                        nextSequences.Add(nextSequence);
                    }
                }

                // advance the n most likely sequences to be processed using the next token
                possibleSequences = nextSequences.OrderByDescending(S => S.Score).Take(10).ToList();
            }

            // find the highest scoring sequence and assign the labels
            var bestSequence = possibleSequences.OrderByDescending(S => S.Score).First();
            var bestOutcomes = bestSequence.Outcomes;
            probabilities = bestSequence.Scores.ToArray();
            for (var i = 0; i < tokens.Count(); i++)
            {
                // outcomes from the sharpentropy model are stored as <start|continue>.<label>
                tokens[i].Type = bestOutcomes[i].Split('.')[1];
            }
            return tokens;
        }
    }
}
