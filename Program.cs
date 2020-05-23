using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace MyPass
{
	class MyPassCli
	{
		private List<Login> Logins;
		private string Password;
		private string FileName;

		public MyPassCli()
		{
			Logins = new List<Login>();
		}

		private XDocument ToXml()
		{
			return Login.LoginsToXDocument(Logins);
		}

		public void Run()
		{
			var reader = new CommandReader();
			while (true)
			{
				List<Command> cmds = reader.Read();
				List<Login> input = Logins;
				foreach (Command cmd in cmds)
				{
					if (cmd is QuitCommand)
					{
						quitHandler(null, null);
						return;
					}
					else if (cmd is IOCommand io)
					{
						if (Password == null)
						{
							Error.Write("Password not set, try " +
								"'set-password'");
							continue;
						}
						if (io.File != null)
							FileName = io.File;
						else
							io.File = FileName;

						// Needed for encryption and decryption
						io.Password = Password;

						if (FileName == null)
						{
							Error.Write("No file name set, try " +
								"'save <file>' or 'open <file>'");
						}

						input = cmd.Execute(input);
						Logins = input;
					}
					else if (cmd is SetPasswordCommand pass)
					{
						pass.Execute(input);
						Password = pass.Password;
					}
					else
					{
						input = cmd.Execute(input);
					}
				}
			}
		}

		static void Main(string[] args)
		{
			Console.CancelKeyPress += new ConsoleCancelEventHandler(quitHandler);

			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Welcome to MyPass");
			Console.WriteLine("Type help [command] for help.");
			Console.ForegroundColor = oldColor;

			new MyPassCli().Run();
		}

		private static void quitHandler(object sender, ConsoleCancelEventArgs args)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Bye");
			Environment.Exit(0);
		}
	}
}
