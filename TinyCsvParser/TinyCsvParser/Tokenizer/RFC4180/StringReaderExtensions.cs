using System.IO;
using System.Text;

namespace TinyCsvParser.Tokenizer.RFC4180
{
  public static class StringReaderExtensions
  {
    public static string ReadTo(this StringReader reader, char readTo)
    {
      var buffer = new StringBuilder();
      while (reader.Peek() != -1 && reader.Peek() != readTo)
      {
        buffer.Append((char) reader.Read());
      }

      return buffer.ToString();
    }
  }
}