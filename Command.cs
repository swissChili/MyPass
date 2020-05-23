using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MyPass
{
	public abstract class Command
	{
		public virtual List<Login> Execute(List<Login> input)
		{
			return input;
		}
	}

	public class QuitCommand : Command
	{
	}

	public class NoCommand : Command
	{
	}

	public abstract class IOCommand : Command
	{
		public string File { get; set; }
		public string Password { get; set; }

		public IOCommand(string what)
		{
			File = what;
		}
	}

	public class SaveCommand : IOCommand
	{
		public SaveCommand(string what)
			: base(what)
		{ }

		public override List<Login> Execute(List<Login> input)
		{
			System.IO.File.WriteAllBytes(File, Encryption.Encrypt(
				Login.LoginsToXDocument(input).ToString(), Password));

			return input;
		}
	}

	public class UnknownCommand : Command
	{
		private string Cmd;

		public UnknownCommand(string cmd)
		{
			Cmd = cmd;
		}
		public override List<Login> Execute(List<Login> input)
		{
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"Error: '{Cmd}' is not a command. Try 'help'");
			Console.ForegroundColor = oldColor;

			return input;
		}
	}

	public class HelpCommand : Command
	{
		string What;

		public HelpCommand(string what)
		{
			What = what;
		}

		public override List<Login> Execute(List<Login> input)
		{
			var oldColor = Console.ForegroundColor;

			Console.ForegroundColor = ConsoleColor.Cyan;
			switch (What)
			{
				case "":
					Console.WriteLine("Type 'ls' to view logins, 'open <file>' to open");
					break;
				default:
					Console.WriteLine($"Help '{What}' is not defined, sorry...");
					break;
			}
			Console.ForegroundColor = oldColor;
			return input;
		}
	}

	public class OpenCommand : IOCommand
	{
		public OpenCommand(string what)
			: base(what)
		{
		}

		public override List<Login> Execute(List<Login> input)
		{
			byte[] bytes = System.IO.File.ReadAllBytes(File);

			try
			{
				string clearText = Encryption.Decrypt(bytes, Password);
				input = new List<Login>(
					Login.FromXDocument(XDocument.Parse(clearText)));
			}
			catch (DecryptionError)
			{
				Error.Write(
					$"Could not decrypt {File}, is the password correct?");
			}
			return input;
		}
	}

	public class ClearCommand : Command
	{
		public override List<Login> Execute(List<Login> input)
		{
			Console.Clear();

			return input;
		}
	}

	public class SetPasswordCommand : Command
	{
		public string Password;

		public override List<Login> Execute(List<Login> input)
		{
			Console.WriteLine("Enter your password");
			Password = ReadLine.ReadPassword("(hidden)> ");
			return input;
		}
	}

	public class ListCommand : Command
	{
		public override List<Login> Execute(List<Login> input)
		{
			for (int i = 0; i < input.Count; i++)
			{
				Console.Write(i.ToString().PadLeft(4, ' ') + " ");
				input[i].PrintHidden();
			}
			return input;
		}
	}

	public class NthCommand : Command
	{
		private int N;

		public NthCommand(int n)
		{
			N = n;
		}

		public override List<Login> Execute(List<Login> input)
		{
			var l = new List<Login>();
			if (N < input.Count)
			{
				l.Add(input[N]);
			}

			return l;
		}
	}

	public class AddCommand : Command
	{
		private Login ToAdd;

		public AddCommand(List<string> args)
		{
			ToAdd = new Login();
			for (int i = 1; i < args.Count; i++)
			{
				switch (args[i])
				{
					case "--username":
					case "-u":
						ToAdd.Username = args[++i];
						break;
					case "--password":
					case "-p":
						ToAdd.Password = args[++i];
						break;
					case "--for":
					case "-f":
						ToAdd.For = args[++i];
						break;
					case "--notes":
					case "-n":
						ToAdd.Notes = args[++i];
						break;
					case "--tag":
					case "-t":
						ToAdd.Tags.Add(args[++i]);
						break;
					default:
						Error.Warning($"Unknown flag {args[i]}");
						break;
				}
			}
			if (ToAdd.Password == "")
			{
				Console.WriteLine("Password is required:");
				ToAdd.Password = ReadLine.ReadPassword("(hidden)> ");
			}
		}

		public override List<Login> Execute(List<Login> input)
		{
			input.Add(ToAdd);
			return input;
		}
	}

	public class GetCommand : Command
	{
		private string What;

		public GetCommand(string what)
		{
			What = what;
		}

		public override List<Login> Execute(List<Login> input)
		{
			foreach (var i in input)
			{
				if (What == "password")
					Console.WriteLine(i.Password);
				else if (What == "username")
					Console.WriteLine(i.Username);
				else if (What == "for")
					Console.WriteLine(i.For);
				else if (What == "tags")
					Console.WriteLine(String.Join(", ", i.Tags));
				else if (What == "ntoes")
					Console.WriteLine(i.Notes);
			}
			return input;
		}
	}

	public class FindCommand : Command
	{
		private string Username;
		private string For;
		private List<string> Tags;

		public FindCommand(List<string> args)
		{
			Tags = new List<string>();
			for (int i = 1; i < args.Count; i++)
			{
				switch (args[i])
				{
					case "--username":
					case "-u":
						Username = args[++i];
						break;
					case "--for":
					case "-f":
						For = args[++i];
						break;
					case "--tag":
					case "-t":
						Tags.Add(args[++i]);
						break;
					default:
						Error.Warning($"Unknown flag {args[i]}");
						break;
				}
			}
		}

        protected bool DoesMatch(Login i)
        {
            return (Username != null ? (i.Username == Username) : true)
					&& (For != null ? (i.For == For) : true)
					&& (Tags != null ? (i.Tags.Aggregate(true, (a, x) =>
						a && i.Tags.Contains(x))) : true);
        }

		protected List<Login> FindMatches(List<Login> input)
		{
			return new List<Login>(
				from i in input
				where DoesMatch(i)
				select i);
		}

		public override List<Login> Execute(List<Login> input)
		{
			return FindMatches(input);
		}
	}

	public class RemoveCommand : FindCommand
	{
		public RemoveCommand(List<string> args)
			: base(args)
		{ }

        public override List<Login> Execute(List<Login> input)
		{
            Console.WriteLine(
                $"Are you sure you want to remove these items?");
            if (ReadLine.Read("(y/N)> ") == "y")
            {
                input.RemoveAll(DoesMatch);
            }
            return input;
		}
	}
}
