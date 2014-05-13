using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Artificial.XNATutorial.Bomberman
{
    public class Player
    {
        public string Name = "NEW";
        public CharacterType CharacterType = CharacterType.Alien;
        public ControlType ControllerType = ControlType.None;

        [XmlIgnore]
        public Controller Controller;
        [XmlIgnore]
        public Character Character;

        public void GenerateCharacter()
        {
            Character = new Character(this);
        }
    }
}
