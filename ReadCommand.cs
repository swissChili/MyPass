using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MyPass
{
	public class CommandReader
	{
		List<string> history;

		public CommandReader()
		{
			history = new List<string>();
		}

		public List<Command> Read()
		{
			string rawCommands = ReadLine.Read("-> ").Trim();
			ReadLine.AddHistory(new string[] { rawCommands });

			// Split on unquoted pipes
			var pipedCommands = from c in Regex.Split(rawCommands,
				@"\|(?=([^""]*""[^""]*"")*[^""]*$)")
								select c.Trim();

			var cmds = new List<Command>();

			try
			{
				foreach (string command in pipedCommands)
				{
					var p = CommandParser.Parse(command);

					try
					{
						if (command == "")
						{
							cmds.Add(new NoCommand());
						}
						else if (command == "quit" || command == "exit")
						{
							cmds.Add(new QuitCommand());
						}
						else if (command == "clear" || command == "cls")
						{
							cmds.Add(new ClearCommand());
						}
						else if (command.StartsWith("help"))
						{
							if (command == "help")
								cmds.Add(new HelpCommand(""));
							else
							{
								cmds.Add(new HelpCommand(p[1]));
							}
						}
						else if (command.StartsWith("add"))
						{
							cmds.Add(new AddCommand(p));
						}
						else if (command.StartsWith("find"))
						{
							cmds.Add(new FindCommand(p));
						}
						else if (command.StartsWith("rm"))
						{
							cmds.Add(new RemoveCommand(p));
						}
						else if (new string[]{
								"username", "password",
								"for", "notes", "tags"
							}.Contains(command))
						{
							cmds.Add(new GetCommand(command));
						}
						else if (command.StartsWith("set-password"))
						{
							cmds.Add(new SetPasswordCommand());
						}
						else if (command == "ls" || command == "list")
						{
							cmds.Add(new ListCommand());
						}
						else if (command.StartsWith("nth"))
						{
							cmds.Add(new NthCommand(Int32.Parse(p[1])));
						}
						else if (command.StartsWith("open"))
						{

							cmds.Add(new OpenCommand(p[1]));
						}
						else if (command.StartsWith("save"))
						{
							cmds.Add(new SaveCommand(p.Count > 1 ? p[1] : null));
						}
						else
						{
							cmds.Add(new UnknownCommand(command));
						}
					}
					catch (ArgumentOutOfRangeException)
					{
						Error.Write($"'{command}' expects more arguments");

						break;
					}
					catch (FormatException)
					{
						Error.Write($"'{command}' argument is malformed");
					}
				}
			}
			catch (ParseError e)
			{
				e.PrintParse();
			}
			return cmds;
		}
	}
}
