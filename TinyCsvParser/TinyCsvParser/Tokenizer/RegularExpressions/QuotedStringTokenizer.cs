using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TinyCsvParser.Tokenizer.RegularExpressions
{
    public class QuotedStringTokenizer : RegularExpressionTokenizer
    {
        private readonly Regex regexp;

        public QuotedStringTokenizer(char columnDelimiter)
            : base()
        {
            var regularExpressionString = string.Format("((?<=\")[^\"]*(?=\"({0}|$)+)|(?<={0}|^)[^{0}\"]*(?={0}|$))", columnDelimiter);

            this.regexp = new Regex(regularExpressionString, RegexOptions.Compiled);
        }

        public override Regex Regexp
        {
            get { return regexp; }
        }

        public override string ToString()
        {
            return string.Format("QuotedStringTokenizer({0})", base.ToString());
        }
    }
}