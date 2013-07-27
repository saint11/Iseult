﻿using System;
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
        public IseultPlayer Player { get; private set; }

        public GameLevel(PlatformerLevelLoader Loader, Side EntrySide, int DoorUid=0)
            :base(Loader.size)
        {
            this.DoorUid = DoorUid;
            this.EntrySide = EntrySide;
            Engine.Instance.Screen.ClearColor = Color.RoyalBlue;
            loadLevel(Loader);

            Gravity = new Vector2(0, 0.18f);
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
                    AddPlayer(e.AttrFloat("x") + IseultPlayer.WIDTH/2, e.AttrFloat("y") + IseultPlayer.HEIGHT/2);
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
            else if (e.Name == "Item")
            {
                Add(new Collectible(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), "knife"));
            }
            else if (e.Name == "Enemy")
            {
                Add(new Enemy(new Vector2(e.AttrFloat("x"), e.AttrFloat("y"))));
            }
        }

        private void AddPlayer(float x, float y)
        {
            Player = new IseultPlayer(new Vector2(x, y));
            Add(Player);
            CameraTarget = Player;
            Camera.X = Player.Position.X - Camera.Viewport.Bounds.Width / 2;
            Camera.Y = Player.Position.Y - Camera.Viewport.Bounds.Height / 2;
            UpdateCamera();
        }
    }
}
