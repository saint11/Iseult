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
        private int DoorUid;
        private Side EntrySide;
        public GameLevel(PlatformerLevelLoader Loader, Side EntrySide, int DoorUid=0)
            :base(Loader.size)
        {
            this.DoorUid = DoorUid;
            this.EntrySide = EntrySide;
            Engine.Instance.Screen.ClearColor = Color.RoyalBlue;
            loadLevel(Loader);
        }

        public override void LoadEntity(System.Xml.XmlElement e)
        {
            if (e.Name == "Player" && EntrySide==Side.Secret)
            {
                AddPlayer(e.AttrFloat("x"), e.AttrFloat("y"));
            }
            else if (e.Name == "DoorWay")
            {
                Add(new DoorWay(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")),e.Attr("GoTo"),e.AttrBool("Back"),e.AttrInt("uid")));
                if (e.AttrInt("uid") == DoorUid && EntrySide==Side.Door)
                {
                    AddPlayer(e.AttrFloat("x") + Iseult.WIDTH/2, e.AttrFloat("y") + Iseult.HEIGHT/2);
                }
            }
        }

        private void AddPlayer(float x, float y)
        {
            Iseult player = new Iseult(new Vector2(x,y));
            Add(player);
            CameraTarget = player;
        }
    }
}
