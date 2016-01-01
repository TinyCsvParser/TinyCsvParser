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
        private Regex regexp;

        public override Regex Regexp
        {
            get { return regexp; }
        }

        public QuotedStringTokenizer(char columnDelimiter)
            : base()
        {
            BuildCompiledRegexp(columnDelimiter);
        }

        private void BuildCompiledRegexp(char columnDelimiter)
        {
            regexp = new Regex(GetPreparedRegexp(columnDelimiter), RegexOptions.Compiled);
        }

        private string GetPreparedRegexp(char columnDelimiter)
        {
            return string.Format("((?<=\")[^\"]*(?=\"({0}|$)+)|(?<={0}|^)[^{0}\"]*(?={0}|$))", columnDelimiter);
        }

        public override string ToString()
        {
            return string.Format("QuotedStringTokenizer({0})", base.ToString());
        }
    }
}