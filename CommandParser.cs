using System;
using System.Collections.Generic;
using System.Globalization;

namespace MyPass
{
	public class ParseError : Exception
	{
		string Parsed;
		string Error;
		int Where;
		int Len;

		public ParseError(string parsed, string error, int where, int len)
		{
			Parsed = parsed;
			Error = error;
			Where = where;
			Len = len;
		}

		public void PrintParse()
		{
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"Error parsing:");
			Console.ForegroundColor = oldColor;
			Console.WriteLine($"  | {Parsed}");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(
				new String(' ', Where + 4) + "^" + new String('~', Len));
			Console.WriteLine(Error);
			Console.ForegroundColor = oldColor;
		}
	}

	public class CommandParser
	{
		public static List<string> Parse(string what)
		{
			List<string> res = new List<string>();
			string buffer = "";

			for (int j = 0; j < what.Length; j++)
			{
				buffer = "";
				// skip ws
				if (Char.IsWhiteSpace(what[j]))
				{
					while (Char.IsWhiteSpace(what[j]))
					{
						j++;
					}
				}
				if (j < what.Length && what[j] == '"')
				{
					int escapeStart = j;
					j++;
					while (j < what.Length && what[j] != '"')
					{
						buffer += what[j++];
					}
					if (j >= what.Length || what[j] != '"')
					{
						throw new ParseError(what, "Expected a closing quote",
							escapeStart, j - escapeStart);
					}
					res.Add(buffer);
				}
				else
				{
					while (j < what.Length && !Char.IsWhiteSpace(what[j]))
					{
						buffer += what[j++];
					}
					res.Add(buffer);
				}
			}

			return res;
		}
	}
}