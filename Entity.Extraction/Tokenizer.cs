using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Extraction
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
                var c = input[characterIndex];
                if (c == '\r' && input[characterIndex + 1] == '\n')
                {
                    if (characterIndex - start > 0 && start > -1)
                    {
                        tokens.Add(new Token(start, characterIndex, ref input));
                    }
                    tokens.Add(new Token(characterIndex, characterIndex + 2, ref input));
                    start = characterIndex + 2;
                    characterIndex += 1;
                    continue;
                }
                else if (System.Char.IsWhiteSpace(c))
                //if (char.IsWhiteSpace(c))
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
            if (charType != CharacterEnum.Whitespace && start < inputLength)
            {
                tokens.Add(new Token(start, inputLength, ref input));
            }
            return tokens.ToArray();
        }
    }

}
