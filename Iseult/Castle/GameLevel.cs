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

        public IseultPlayer Player { get; private set; }
        public Mordecai Mordecai { get; private set; }

        private List<DoorWay> Doors;

        public GameLevel(PlatformerLevelLoader Loader, Side EntrySide, int DoorUid=0)
            :base(Loader.size)
        {
            this.DoorUid = DoorUid;
            this.EntrySide = EntrySide;
            Engine.Instance.Screen.ClearColor = Color.RoyalBlue;

            Doors = new List<DoorWay>();
            loadLevel(Loader);

            LoadPersistent();


            Gravity = new Vector2(0, 0.18f);
        }

        private void LoadPersistent()
        {
            foreach (EnemyTracker enemy in EnemyTracker.HasEnemyHere(Name))
	        {
                Add(new Enemy(enemy));
	        }

            if (Mordecai == null && Mordecai.CurrentOn == Name)
            {
                Add(Mordecai = new Mordecai(Mordecai.LastSeen));
            }
        }

        public override void LoadEntity(System.Xml.XmlElement e)
        {
            if (e.Name == "Player" && EntrySide==Side.Secret)
            {
                AddPlayer(e.AttrFloat("x"), e.AttrFloat("y"));
            }
            else if (e.Name == "DoorWay")
            {
                AddDoorWay(e);
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
                Add(new Collectible(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), e.Attr("Type")));
            }
            else if (e.Name == "Enemy")
            {
                if (!EnemyTracker.HasEnemy(Name, e))
                {
                    Add(new Enemy(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), Name+e.Attr("id")));
                    EnemyTracker.RegisterEnemy(Name, e);
                }
            }
            else if (e.Name == "Mordecai" && EntrySide == Side.Secret)
            {
                Add(Mordecai = new Mordecai(new Vector2(e.AttrFloat("x"), e.AttrFloat("y"))));
            }
        }

        private void AddDoorWay(System.Xml.XmlElement e)
        {
            DoorWay Door = new DoorWay(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), e.AttrBool("Back"), e.AttrInt("uid"), e.Attr("GoTo"));
            Doors.Add(Door);
            Add(Door);
            if (e.AttrInt("uid") == DoorUid && EntrySide == Side.Door)
            {
                AddPlayer(e.AttrFloat("x") + IseultPlayer.WIDTH / 2, e.AttrFloat("y") + IseultPlayer.HEIGHT / 2);
            }

            foreach (EnemyTracker enemy in EnemyTracker.GetEnemiesEngaded(e.Attr("GoTo")))
            {
                Enemy en = new Enemy(enemy);
                en.SetPosition(new Vector2(Door.X,Door.Y+64));
                Add(en);
            }

            if (Mordecai.CurrentOn == Name && EntrySide != Side.Secret && e.AttrFloat("uid") == Mordecai.DoorUid)
            {
                Add(Mordecai = new Mordecai(new Vector2(Door.X,Door.Y+96)));
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

        internal void GoToLevel(string Destiny, int uid)
        {
            foreach (var e in Tags[(int)GameTags.Enemy]) EnemyTracker.UpdateTracker((Enemy)e, Name, Player);

            if (Mordecai != null)
            {
                if (Vector2.Distance(Mordecai.Position, Player.Position) < 100)
                {
                    Mordecai.CurrentOn = Destiny.ToUpper();
                    Mordecai.DoorUid = uid;
                }
                else
                {
                    Mordecai.LastSeen = Mordecai.Position;
                    Mordecai.DoorUid = 0;
                }
            }

            PlatformerLevelLoader loader = PlatformerLevelLoader.load(Destiny);
            GameLevel level = new GameLevel(loader, PlatformerLevel.Side.Door, uid);

            Engine.Instance.Scene = level;
        }

        public override void Update()
        {
            base.Update();
            EnemyTracker.UpdateOffScreen(CheckForMonstersAtTheDoors,Name);
        }

        public void CheckForMonstersAtTheDoors()
        {
            foreach (DoorWay Door in Doors)
            {
                Door.CheckForAttackers();
            }
        }
    }
}
