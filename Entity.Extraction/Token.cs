using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Extraction
{
    public class Token : IComparable<Token>
    {
        private int mStart;
        private int mEnd;
        public string Type { get; set; }
        private string text;
        public HashSet<string> Features { get; private set; }

        /// <summary>
        /// Return the start of a span.
        /// </summary>
        /// <returns> 
        /// the start of a span.
        /// </returns>
        virtual public int Start
        {
            get
            {
                return mStart;
            }

        }
        /// <summary>
        /// Return the end of a span.
        /// </summary>
        /// <returns> 
        /// the end of a span.
        /// </returns>
        virtual public int End
        {
            get
            {
                return mEnd;
            }

        }


        /// <summary>Constructs a new Span object.
        /// </summary>
        /// <param name="startOfSpan">
        /// start of span.
        /// </param>
        /// <param name="endOfSpan">
        /// end of span.
        /// </param>
        public Token(int startOfSpan, int endOfSpan, ref string text)
        {
            mStart = startOfSpan;
            mEnd = endOfSpan;
            this.text = text;
            this.Features = new HashSet<string>();
        }

        public virtual int Length()
        {
            return (mEnd - mStart);
        }

        /// <summary>
        /// Returns true is the specified span is contained by this span.  
        /// Identical spans are considered to contain each other. 
        /// </summary>
        /// <param name="span">
        /// The span to compare with this span.
        /// </param>
        /// <returns>
        /// true if the specified span is contained by this span; false otherwise. 
        /// </returns>
        public virtual bool Contains(Token span)
        {
            return (mStart <= span.Start && span.End <= mEnd);
        }

        /// <summary>
        /// Returns true if the specified span intersects with this span.
        /// </summary>
        /// <param name="span">
        /// The span to compare with this span. 
        /// </param>
        /// <returns>
        /// true is the spans overlap; false otherwise. 
        /// </returns>
        public bool Intersects(Token span)
        {
            int spanStart = span.Start;
            //either span's start is in this or this's start is in span
            return (this.Contains(span) || span.Contains(this) ||
                (mStart <= spanStart && spanStart < mEnd ||
                spanStart <= mStart && mStart < span.End));
        }

        /// <summary>
        /// Returns true if the specified span crosses this span.
        /// </summary>
        /// <param name="span">
        /// The span to compare with this span.
        /// </param>
        /// <returns>
        /// true if the specified span overlaps this span and contains a non-overlapping section; false otherwise.
        /// </returns>
        public bool Crosses(Token span)
        {
            int spanStart = span.Start;
            //either span's Start is in this or this's Start is in span
            return (!this.Contains(span) && !span.Contains(this) &&
                (mStart <= spanStart && spanStart < mEnd ||
                spanStart <= mStart && mStart < span.End));
        }

        public virtual int CompareTo(Token o)
        {
            Token compareSpan = (Token)o;
            if (Start < compareSpan.Start)
            {
                return -1;
            }
            else if (Start == compareSpan.Start)
            {
                if (End > compareSpan.End)
                {
                    return -1;
                }
                else if (End < compareSpan.End)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 1;
            }
        }

        public override int GetHashCode()
        {
            return ((Start << 16) | (0x0000FFFF | this.End));
        }

        public override bool Equals(object o)
        {
            if (!(o is Token))
            {
                return false;
            }
            Token currentSpan = (Token)o;
            return (Start == currentSpan.Start && End == currentSpan.End);
        }

        public override string ToString()
        {
            return this.Type + ": " + this.GetText();
        }

        public string GetText()
        {
            return this.text.Substring(this.Start, this.End - this.Start);
        }
    }
}
