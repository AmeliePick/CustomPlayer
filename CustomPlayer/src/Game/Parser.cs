using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;



namespace CustomPlayer
{
    public class Parser
    {
        public static void WriteLog()
        {
            FileStream aFile = new FileStream("scripts/CustomPlayer/Log.txt", FileMode.Append);
            StreamWriter sw = new StreamWriter(aFile);
            sw.WriteLine("File error. Try to delete scripts/CustomPlayer/characters.xml or deal with it yourself =)");
        }


        public static XDocument OpenFile(string path)
        {
            if (File.Exists("scripts/CustomPlayer/characters.xml"))
            {
                if (File.ReadAllText("scripts/CustomPlayer/characters.xml") != "")
                {
                    return XDocument.Load("scripts/CustomPlayer/characters.xml");
                }
                else
                    File.Delete("scripts/CustomPlayer/characters.xml");
            }
            return null;
        }


        public static void ParseCharactersNames(List<dynamic> ListOfNames)
        {
            XDocument xdoc = Parser.OpenFile("scripts/CustomPlayer/characters.xml");

            if(xdoc != null)
            {
                foreach (var element in xdoc.Element("Characters").Elements("person"))
                {
                    if (element != null)
                    {
                        ListOfNames.Add(element.Attribute("name").Value);
                    }
                }
            }
        }



    }
}
