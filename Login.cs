using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace MyPass
{
	public class Login
	{
		public string For; // what is this for?
		public List<string> Tags;
		public string Username;
		public string Password;
		public string Notes;

        public Login()
        {
            For = "";
            Tags = new List<string>();
            Username = "";
            Password = "";
            Notes = "";
        }

		public XElement ToXml()
		{
			return new XElement("Login",
				new XElement("For", For),
				new XElement("Tags",
					from t in Tags
					select new XElement("Tag", t)),
				new XElement("Username", Username),
				new XElement("Password", Password),
				new XElement("Notes", Notes));
		}

		public void PrintHidden()
		{
			var oldColor = Console.ForegroundColor;

			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write($"{For}  '{Username}'  ");
			Console.ForegroundColor = oldColor;

			var firstTags = String.Join(", ",
				Tags.GetRange(0, Math.Min(3, Tags.Count)));

			Console.WriteLine($"**********  [ {firstTags} ]");
		}

		public Login(XElement xml)
		{
			For = xml.Descendants("For").First().Value;
            Tags = new List<string>(
				from t in xml.Descendants("Tag")
				select (t.FirstNode as XText).Value);
			Username = xml.Descendants("Username").First().Value;
			Password = xml.Descendants("Password").First().Value;
			Notes = xml.Descendants("Notes").First().Value;
		}

        public static IEnumerable<Login> FromXDocument(XDocument doc)
        {
            return from login in doc.Root.Descendants("Login")
                     select new Login(login);
        }

        public static XDocument LoginsToXDocument(IEnumerable<Login> logins)
        {
            var d = new XDocument(
				new XElement("Logins",
					from l in logins
					select l.ToXml()));
			d.Declaration = new XDeclaration("1.0", "utf-8", "true");

			return d;
        }
	}
}
