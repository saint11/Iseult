using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
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
                Add(new DoorWay(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), e.AttrBool("Back"), e.AttrInt("uid"), e.Attr("GoTo")));
                if (e.AttrInt("uid") == DoorUid && EntrySide==Side.Door)
                {
                    AddPlayer(e.AttrFloat("x") + Iseult.WIDTH/2, e.AttrFloat("y") + Iseult.HEIGHT/2);
                }
            }
            else if (e.Name == "Stairs")
            {
                XmlElement Node = (XmlElement)e.ChildNodes[0];
                Stairs.Direction Dir = (Node.AttrFloat("y") > e.AttrFloat("y")) ? Stairs.Direction.down : Stairs.Direction.up;
                Stairs.Direction Dir2 = Dir == Stairs.Direction.down ? Stairs.Direction.up : Stairs.Direction.down;

                Stairs First = new Stairs(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), e.AttrBool("Back"), Dir);
                Stairs Second = new Stairs(new Vector2(Node.AttrFloat("x"), Node.AttrFloat("y")), e.AttrBool("Back"), Dir2);
                Add(First);
                Add(Second);
                Stairs.Link(First, Second);
            }
        }

        private void AddPlayer(float x, float y)
        {
            Iseult player = new Iseult(new Vector2(x,y));
            Add(player);
            CameraTarget = player;
            Camera.X = player.Position.X - Camera.Viewport.Bounds.Width / 2;
            Camera.Y = player.Position.Y - Camera.Viewport.Bounds.Height / 2;
            UpdateCamera();
        }
    }
}
