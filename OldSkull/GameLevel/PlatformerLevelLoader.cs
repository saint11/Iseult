    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OldSkull.GameLevel
{
    public class PlatformerLevelLoader
    {
        public List<Solid> solids;
        public List<XmlElement> entities;
        public List<XmlElement> tilesets;
        public Vector2 size;
        public Grid solidGrid;
        public string Right;
        public string Left;
        public string Name;
        public string FileName;

        public string Message;
        public string MessageTittle;

        public static PlatformerLevelLoader load(string filename)
        {
            PlatformerLevelLoader current = new PlatformerLevelLoader();

            current.Name = filename.ToUpper();
            current.FileName = filename;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(OldSkullGame.Path + @"Content\Level\"+filename+".oel");
            XmlElement levelMap = xmlDoc["level"];

            current.Left = levelMap.Attr("leftExit");
            current.Right = levelMap.Attr("rightExit");
            current.Message = levelMap.Attr("Message","");
            current.MessageTittle = levelMap.Attr("MessageTitle","");

            current.size = new Vector2(int.Parse(levelMap.Attr("width")), int.Parse(levelMap.Attr("height")));
            current.solids = new List<Solid>();
            current.solidGrid = new Grid(32, 32, levelMap["Solid"].InnerText);

            current.entities = new List<XmlElement>();
            foreach (XmlElement e in levelMap["Objects"])
            {
                current.entities.Add(e);
            }

            current.tilesets = new List<XmlElement>();
            foreach (XmlElement e in levelMap)
            {
                if (e.HasAttr("tileset"))
                {
                    current.tilesets.Add(e);
                }
            }

            return current;
        }
    }
}
