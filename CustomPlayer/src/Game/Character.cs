using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;

namespace CustomPlayer
{
    public class GamePlayer
    {
        public string Name { set; get; }

        public string Voice { set;  get; }

        public GamePlayer(string Name, string Voice)
        {
            this.Name  = Name;
            this.Voice = Voice;
        }
    }

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

        public string Voice { get; }

        public List<Component> PedComponents { get; }


        public Character(string CharacterName, int ModelHash, string Voice, List<Component> PedComponents)
        {
            this.Name          = CharacterName;
            this.ModelHash     = ModelHash;
            this.Voice         = Voice;
            this.PedComponents = new List<Component>(PedComponents);
        }



        public bool Save()
        {
            XElement MakePerson(XDocument doc)
            {
                XElement person = new XElement("person", new XAttribute("name", this.Name),
                                  new XElement("hash", this.ModelHash),
                                  new XElement("Voice", this.Voice));

                XElement personComponents = new XElement("Components");

                foreach (var component in this.PedComponents)
                {
                    XElement components = new XElement(component.ComponentId.ToString());
                    XElement drawableID = new XElement("Drawable", component.DrawableId.ToString());
                    XElement textureID  = new XElement("Texture", component.TextureId.ToString());

                    components.Add(drawableID);
                    components.Add(textureID);
                    personComponents.Add(components);
                }

                person.Add(personComponents);


                return person;
            }



            if (!File.Exists("scripts/CustomPlayer/characters.xml"))
            {
                XDocument xdoc = new XDocument();

                // Make root
                XElement Characters = new XElement("Characters");
                xdoc.Add(Characters);


                Characters.Add(MakePerson(xdoc));

                xdoc.Save("scripts/CustomPlayer/characters.xml");

                return true;
            }
            else
            {
                XDocument xdoc = Parser.OpenFile("scripts/CustomPlayer/characters.xml");
                
                if(xdoc == null)
                    return false;

                XElement root = xdoc.Element("Characters");
                root.Add(MakePerson(xdoc));

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
                string Voice = Person.Element("Voice").Value;
                List<Component> ListOfComponents = new List<Component>(12);


                XElement pedComponent = Person.Element("Components");

                PedVariationData pedVariation = PedVariationData.PED_VARIATION_HEAD;
                while((int)pedVariation < 12)
                {
                    XElement drawable = pedComponent.Element(pedVariation.ToString()).Element("Drawable");
                    XElement texture  = pedComponent.Element(pedVariation.ToString()).Element("Texture");

                    int DrawableId = int.Parse(drawable.Value);
                    int TextureId  = int.Parse(texture.Value);


                    Component component = new Component(pedVariation, DrawableId, TextureId);
                    ListOfComponents.Add(component);

                    ++pedVariation;
                }




                Character character = new Character(PersonName, Hash, Voice, ListOfComponents);

                return character;
            }


            return null;
        }
    }
}
