using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using Monocle;

namespace Iseult
{
    public class GameLevel:PlatformerLevel
    {
        public GameLevel(PlatformerLevelLoader Loader, Side EntrySide)
            :base(Loader.size)
        {
            Engine.Instance.Screen.ClearColor = Color.RoyalBlue;
            loadLevel(Loader);
        }

        public override void LoadEntity(System.Xml.XmlElement e)
        {
            if (e.Name == "Player")
            {
                Iseult player = new Iseult(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")));
                Add(player);
                CameraTarget = player;
            }
            else if (e.Name == "DoorWay")
            {
                Add(new DoorWay(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")),e.Attr("GoTo"),e.AttrBool("Back")));
            }
        }
    }
}
