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
            //var tokens = new List<Token>();
            //var lastCharType = CharacterEnum.Other;
            //var buffer = new StringBuilder();
            //for (var i = 0; i < input.Length; i++)
            //{
            //    var c = input[i];

            //    var charType = CharacterEnum.Other;
            //    if (c == ' ')
            //    {
            //        tokens.Add(new Token(i - buffer.Length, i, ref input));
            //        buffer.Clear();
            //        continue;
            //    }
            //    else if (System.Char.IsWhiteSpace(c))
            //    {
            //        charType = CharacterEnum.Whitespace;
            //    }
            //    else if (System.Char.IsLetter(c))
            //    {
            //        charType = CharacterEnum.Alphabetic;
            //    }
            //    else if (System.Char.IsDigit(c))
            //    {
            //        charType = CharacterEnum.Numeric;
            //    }
            //    else
            //    {
            //        charType = CharacterEnum.Other;
            //    }

            //    if (charType != lastCharType && i > 0)
            //    {
            //        if (buffer.Length > 0)
            //        {
            //            tokens.Add(new Token(i - buffer.Length, i, ref input));
            //        }
            //        buffer.Clear();
            //    }

            //    buffer.Append(c);

            //    lastCharType = charType;
            //}

            //if (buffer.Length > 0)
            //{
            //    tokens.Add(new Token(input.Length - buffer.Length, input.Length, ref input));
            //}

            //return tokens.ToArray();
            
            
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
