// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Model
{
    public struct TokenizedRow
    {
        /// <summary>
        /// The original Row Data.
        /// </summary>
        public struct RowData
        {
            public long LineNo { get; set; }

            public string Data { get; set; }
        }

        /// <summary>
        /// An error happened during parsing.
        /// </summary>
        public struct TokenizeError
        {
            public long LineNo { get; set; }

            public string Reason { get; set; }
        }

        /// <summary>
        /// Row Data the Tokens are generated from.
        /// </summary>
        public RowData[] Rows { get; set; }

        /// <summary>
        /// Tokens.
        /// </summary>
        public string[] Tokens { get; set; }

        /// <summary>
        /// An Optional Error.
        /// </summary>
        public TokenizeError? Error { get; set; }

        /// <summary>
        /// A Getter to signal if the Row is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !Error.HasValue;
            }
        }
    }

}
