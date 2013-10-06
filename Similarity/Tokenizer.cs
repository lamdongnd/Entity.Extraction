using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Similarity
{
    public class Tokenizer
    {
        internal enum CharacterEnum
        {
            Whitespace,
            Alphabetic,
            Numeric,
            Other
        }

        public Token[] Tokenize(string input)
        {
            CharacterEnum charType = CharacterEnum.Whitespace;
            CharacterEnum state = charType;

            List<Token> tokens = new List<Token>();
            int inputLength = input.Length;
            int start = -1;
            char previousChar = (char)(0);
            for (int characterIndex = 0; characterIndex < inputLength; characterIndex++)
            {
                char c = input[characterIndex];
                if (System.Char.IsWhiteSpace(c))
                {
                    charType = CharacterEnum.Whitespace;
                }
                else if (System.Char.IsLetter(c))
                {
                    charType = CharacterEnum.Alphabetic;
                }
                else if (System.Char.IsDigit(c))
                {
                    charType = CharacterEnum.Numeric;
                }
                else
                {
                    charType = CharacterEnum.Other;
                }
                if (state == CharacterEnum.Whitespace)
                {
                    if (charType != CharacterEnum.Whitespace)
                    {
                        start = characterIndex;
                    }
                }
                else
                {
                    if (charType != state || (charType == CharacterEnum.Other && c != previousChar))
                    {
                        tokens.Add(new Token(start, characterIndex, ref input));
                        start = characterIndex;
                    }
                }
                state = charType;
                previousChar = c;
            }
            if (charType != CharacterEnum.Whitespace)
            {
                tokens.Add(new Token(start, inputLength, ref input));
            }
            return tokens.ToArray();
        }
    }

}
