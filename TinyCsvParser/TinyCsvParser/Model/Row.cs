// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Model
{
    public class Row
    {
        public readonly int Index;

        public readonly string Data;

        public Row(int index, string data)
        {
            Index = index;
            Data = data;
        }
    }
}
