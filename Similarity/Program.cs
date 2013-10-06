using SharpEntropy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Entity.Extraction
{
    class Program
    {
        static void Main(string[] args)
        {
            var tokenizer = new Tokenizer();
            var annotator = Annotator.Default();
            var context = new Context(4);
            var trainer = new Trainer(tokenizer, annotator, context);

            trainer.AddSample("1_amount (8_amount ounce) can_uom Pillsbury® refrigerated crescent dinner_ingredient rolls_ingredient\r\n");
            trainer.AddSample("1_amount /_amount 4_amount cup_uom pizza_ingredient sauce_ingredient\r\n");
            trainer.AddSample("3_amount /_amount 4_amount cup_uom shredded mozzarella_ingredient cheese_ingredient\r\n");
            trainer.AddSample("1_amount /_amount 2_amount cup_uom sliced pepperoni_ingredient\r\n");
            trainer.AddSample("2_amount /_amount 3_amount cup_uom sliced pepperoni_ingredient\r\n");
            trainer.AddSample("7_amount /_amount 8_amount cup_uom sliced pepperoni_ingredient\r\n");
            trainer.AddSample("11_amount teaspoon_uom grated Parmesan_ingredient cheese_ingredient\r\n");
            trainer.AddSample("2_amount teaspoons_uom crushed garlic_ingredient\r\n");
            trainer.AddSample("1_amount /_amount 4_amount cup_uom olive_ingredient oil_ingredient\r\n");
            trainer.AddSample("4_amount skinless_preparation , boneless_preparation chicken_ingredient breast_ingredient halves\r\n");
            trainer.AddSample("2_amount eggs_ingredient\r\n");
            trainer.AddSample("3_amount eggs_ingredient\r\n");
            trainer.AddSample("1_amount /_amount 3_amount cup_uom butter_ingredient , melted\r\n");
            trainer.AddSample("1_amount cup_uom fresh orange_ingredient juice_ingredient\r\n");
            trainer.AddSample("1_amount /_amount 2_amount cup_uom 2_ingredient %_ingredient milk_ingredient\r\n");
            trainer.AddSample("This article has assumed that regular_programming expressions_programming are matched against an entire input string.");
            trainer.AddSample("Modern regular expression implementations must deal with large non-ASCII_programming character sets such as Unicode. ");

            var gisTrainer = new GisTrainer();
            gisTrainer.TrainModel(500, new TwoPassDataIndexer(trainer, 0));
            var model = new GisModel(gisTrainer);
            
            var tagger = new Tagger(tokenizer, context, annotator, model);

            foreach (var file in Directory.GetFiles("examples", "*.txt"))
            {
                var text = File.ReadAllText(file);
                var probabilities = new double[0];
                Console.WriteLine("trying " + Path.GetFileName(file));
                var tokens = tagger.Tag(text, out probabilities);

                Console.WriteLine(Path.GetFileName(file));
                var v = probabilities.Average();
            }
            Console.ReadLine();
        }
    }
}
