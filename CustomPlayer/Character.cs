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

        public int ModelHash { get; }

        public List<Component> PedComponents { get; }


        public Character(string CharacterName, int ModelHash, List<Component> PedComponents)
        {
            Name = CharacterName;
            this.ModelHash = ModelHash;
            this.PedComponents = new List<Component>(PedComponents);
        }



        public bool Save()
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



            if (!File.Exists("scripts/CustomPlayer/characters.xml"))
            {
                XDocument xdoc = new XDocument();

                // Make root
                XElement Characters = new XElement("Characters");
                xdoc.Add(Characters);


                Characters.Add(MakeElement(xdoc));

                xdoc.Save("scripts/CustomPlayer/characters.xml");

                return true;
            }
            else
            {
                XDocument xdoc = Parser.OpenFile("scripts/CustomPlayer/characters.xml");
                
                if(xdoc == null)
                    return false;

                XElement root = xdoc.Element("Characters");
                root.Add(MakeElement(xdoc));

                xdoc.Save("scripts/CustomPlayer/characters.xml");

                return true;
            }
        }


        public static Character Load(string CharacterName)
        {
            XDocument xdoc = Parser.OpenFile("scripts/CustomPlayer/characters.xml");

            XElement Person = null;

            foreach (XElement personElement in xdoc.Element("Characters").Elements("person"))
            {

                if (personElement.Attribute("name").Value == CharacterName)
                {
                    Person = personElement;
                    break;
                }
            }


            if(Person != null)
            {
                string PersonName = Person.Attribute("name").Value;
                int Hash = int.Parse(Person.Element("hash").Value);
                List<Component> ListOfComponents = new List<Component>();


                XElement pedComponent = Person.Element("Components");

                PedVariationData pedVariation = PedVariationData.PED_VARIATION_HEAD;
                while((int)pedVariation < 11)
                {
                    string componentsInfo = pedComponent.Element(pedVariation.ToString()).Value;

                    int DrawableId = componentsInfo[0] - '0';
                    int TextureId  = componentsInfo[1] - '0';
                    

                    Component component = new Component(pedVariation, DrawableId, TextureId);
                    ListOfComponents.Add(component);

                    ++pedVariation;
                }




                Character character = new Character(PersonName, Hash, ListOfComponents);

                return character;
            }


            return null;
        }
    }
}
