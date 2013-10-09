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
            var context = new Context(2);
            var trainer = new Trainer(tokenizer, annotator, context);

            // Add some training text. Words that will be labeled are added using _<label name>.
            foreach (var sample in File.ReadAllLines("training.txt"))
            {
                trainer.AddSample("\r\n" + sample + "\r\n");
            }

            // train the model
            var gisTrainer = new GisTrainer();
            gisTrainer.TrainModel(1000, new TwoPassDataIndexer(trainer, 0));
            var model = new GisModel(gisTrainer);
            
            // initialize the tagger
            var tagger = new Tagger(tokenizer, context, annotator, model);

            var recipe = @"
1 cup packed brown sugar
 1/3 cup butter, melted
 2 tablespoons light corn syrup
 1/3 cup chopped pecans
 12 (3/4 inch thick) slices French bread
 1 teaspoon grated orange zest
 1 cup fresh orange juice
 1/2 cup 2% milk
 3 tablespoons white sugar
 1 teaspoon ground cinnamon
 1 teaspoon vanilla extract
 3 egg whites
 2 eggs
 1 tablespoon confectioners' sugar for dusting

1/3 cup olive oil
 3 cloves garlic, minced
 1/4 teaspoon crushed red pepper flakes, or to taste
 1 teaspoon dried oregano
 3 anchovy fillets, chopped, or more to taste
 2 (15 ounce) cans diced tomatoes, drained.
 1 (8 ounce) package spaghetti
 1/2 cup chopped pitted kalamata olives
 1/4 cup capers, chopped

2 cups shredded sharp Cheddar cheese
 2 cups shredded Colby cheese
 2 (4 ounce) jars diced pimento peppers, drained
 1/2 (16 ounce) jar creamy salad dressing (e.g. Miracle Whip)
 salt and pepper to taste

1/4 cup butter, melted
 3 tablespoons Dijon mustard
 1 1/2 tablespoons honey
 1/4 cup dry bread crumbs
 1/4 cup finely chopped pecans
 4 teaspoons chopped fresh parsley
 4 (4 ounce) fillets salmon
 salt and pepper to taste
 1 lemon, for garnish

Kosher salt
12 ounces linguine
2 tablespoons extra-virgin olive oil
4 cloves garlic, thinly sliced
1/4 to 1/2 teaspoon red pepper flakes
2 tablespoons capers, drained
1/2 cup roughly chopped kalamata olives
1 28-ounce can San Marzano plum tomatoes
4 basil leaves, torn, plus more for garnish
1 5-ounce can albacore tuna, packed in olive oil
Freshly ground pepper
";

            var output = new StreamWriter("results.txt", false);
            var probabilities = new double[0];
            var tokens = tagger.Tag(recipe, out probabilities);
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                output.WriteLine("{0,20}{1,20}{2,20}", probabilities[i].ToString("N2"), token.Type, token.GetText());
                output.WriteLine(new string('-', 60));
            }
            output.Flush();

            //foreach (var file in Directory.GetFiles("examples", "*.txt"))
            //{
            //    var text = File.ReadAllText(file);

            //    var probabilities = new double[0];
            //    var tokens = tagger.Tag(text, out probabilities);

            //    for (var i = 0; i < tokens.Length; i++)
            //    {
            //        var token = tokens[i];
            //        Console.WriteLine("{0,20}{1,20}{2,20}", token.Type, probabilities[i].ToString("N2"), token.GetText());
            //        Console.WriteLine(new string('-', 60));
            //    }
            //}
        }
    }
}
