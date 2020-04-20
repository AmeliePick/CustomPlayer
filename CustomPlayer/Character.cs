using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;

namespace CustomPlayer
{
    public class Component
    {
        public PedVariationData ComponentId { get; }

        public int DrawableId { get; }

        public int TextureId { get; }

        public Component(PedVariationData componentId, int drawableId, int textureId)
        {
            this.ComponentId    = componentId;
            this.DrawableId     = drawableId;
            this.TextureId      = textureId;
        }
    }


    public class Character
    {
        public string Name { get; }

        public string ModelHash { get; }

        public List<Component> PedComponents { get; }


        public Character(string CharacterName, int ModelHash, List<Component> PedComponents)
        {
            Name = CharacterName;
            this.ModelHash = ModelHash.ToString();
            this.PedComponents = new List<Component>(PedComponents);
        }


        public void Save()
        {
            XElement MakeElement(XDocument doc)
            {
                XElement person = new XElement("person", new XAttribute("name", this.Name),
                                  new XElement("hash", this.ModelHash));

                XElement personComponent = new XElement("Components");

                foreach (var component in this.PedComponents)
                {
                    XElement Components = new XElement(component.ComponentId.ToString(), component.DrawableId, component.TextureId);
                    personComponent.Add(Components);
                }

                person.Add(personComponent);


                return person;
            }


            if (!Directory.Exists("scripts/CustomPlayer"))
                Directory.CreateDirectory("scripts/CustomPlayer");

            if (!File.Exists("scripts/CustomPlayer/characters.xml"))
            {
                XDocument xdoc = new XDocument();

                // Make root
                XElement Characters = new XElement("Characters");
                xdoc.Add(Characters);


                Characters.Add(MakeElement(xdoc));

                xdoc.Save("scripts/CustomPlayer/characters.xml");
            }
            else
            {
                XDocument xdoc = XDocument.Load("scripts/CustomPlayer/characters.xml");
                XElement root = xdoc.Element("Characters");

                root.Add(MakeElement(xdoc));


                xdoc.Save("scripts/CustomPlayer/characters.xml");

            }
        }
    }
}
