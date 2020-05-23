using System;

namespace MyPass
{
	public static class Error
	{
		public static void Write(string message)
		{
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"Error: {message}");
			Console.ForegroundColor = oldColor;
		}

		public static void Warning(string message)
		{
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"Warning: {message}");
			Console.ForegroundColor = oldColor;
		}
	}
}