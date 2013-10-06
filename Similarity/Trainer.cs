using SharpEntropy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Similarity
{
    public class Trainer : ITrainingEventReader
    {
        private Tokenizer tokenizer;
        private Annotator annotator;
        private List<TrainingEvent> samples = new List<TrainingEvent>();
        private int currentIndex = 0;
        private Context context;

        public Trainer(Tokenizer tokenizer, Annotator annotator, Context context)
        {
            this.tokenizer = tokenizer;
            this.annotator = annotator;
            this.context = context;
        }

        public void AddSample(string input)
        {
            var tokens = this.annotator.Annotate(this.tokenizer.Tokenize(input)).ToList();
            var currentIndex = 0;
            while (currentIndex < tokens.Count)
            {
                // an underscore means the next token is the label
                if (tokens[currentIndex].GetText() == "_")
                {
                    var label = tokens[currentIndex + 1].GetText();
                    tokens[currentIndex - 1].Type = label;
                    tokens[currentIndex - 1].Features.Add(label);
                    // remove the underscore and label from the list
                    tokens.RemoveAt(currentIndex);
                    tokens.RemoveAt(currentIndex);
                    currentIndex = currentIndex - 2;
                }
                currentIndex++;
            }
            var lastType = "";
            var previousOutcomes = new List<string>();
            for (var i = 0; i < tokens.Count; i++)
            {
                var trainingOutcome = "";
                // boundary tokens have a type of other
                if (string.IsNullOrEmpty(tokens[i].Type))
                {
                    trainingOutcome = "other";
                    lastType = "other";
                }
                // indicates the start of a sequence
                else if (lastType != tokens[i].Type)
                {
                    trainingOutcome = "start";
                    lastType = tokens[i].Type;
                }
                // indicates a continuation of a sequence
                else if (lastType == tokens[i].Type)
                {
                    trainingOutcome = "continue";
                    lastType = tokens[i].Type;
                }

                previousOutcomes.Add(trainingOutcome + "." + lastType);
                this.samples.Add(new TrainingEvent(trainingOutcome + "." + lastType, this.context.Generate(tokens, i, previousOutcomes)));
            }
        }

        public TrainingEvent ReadNextEvent()
        {        
            var sample = this.samples[this.currentIndex];
            this.currentIndex++;
            return sample;
        }

        public bool HasNext()
        {
            return this.currentIndex < this.samples.Count;
        }
    }
}
