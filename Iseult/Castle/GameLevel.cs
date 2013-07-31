using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using OldSkull.GameLevel;
using OldSkull.Graphics;
using Microsoft.Xna.Framework;
using Monocle;
using Microsoft.Xna.Framework.Graphics;
using Iseult.Castle;


namespace Iseult
{
    public class GameLevel:PlatformerLevel
    {
        private int DoorUid;
        private Side EntrySide;

        public IseultPlayer Player { get; private set; }
        public Mordecai Mordecai { get; private set; }
        private TiledImage Sky;

        private List<DoorWay> Doors;
        private Inventory Inventory;

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

            Layer MistLayer;
            SetLayer(2, MistLayer = new Layer(BlendState.NonPremultiplied, SamplerState.PointClamp, 0));
            
            Sky = new TiledImage(IseultGame.Atlas["environment/sky"],
                (int)(Camera.Viewport.Width + skyGameLayer.CameraMultiplier * (Width - Camera.Viewport.Width)),
                (int)(Camera.Viewport.Height + skyGameLayer.CameraMultiplier * (Height - Camera.Viewport.Height)));
            Entity BgE = new Entity(SKY_GAME_LAYER);
            BgE.Add(Sky);
            Add(BgE);

            Add(new Mist(2));
            Add(new Hud());

            CheckForMessages(Loader);
        }

        private void CheckForMessages(PlatformerLevelLoader Loader)
        {
            if (Loader.Message != "" && !IseultGame.Stats.HasTrigger("Message" + Name)) CallMessage(Loader.Message, Loader.MessageTittle);
        }

        public void CallMessage(string Message, string MessageTittle)
        {
            CurrentState = GameState.Paused;
            Add(new Message(Message, MessageTittle));
            IseultGame.Stats.SetTrigger("Message" + Name);
        }

        public void CallMessage(string MessageName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(IseultGame.Path + @"Content\Misc\Messages.xml");
            XmlElement MessageXml = ((XmlElement)xmlDoc["messages"])[MessageName];
            CallMessage(MessageXml.ChildText("Text"), MessageXml.ChildText("Title"));
        }


        public override void Begin()
        {
            base.Begin();
            if (EntrySide == Side.Secret && Mordecai != null) Mordecai.ToggleFollow(Player);
            Player.CheckMusic();
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
            else if (e.Name == "SmallBox")
            {
                Add(new PushBox(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), new Vector2(32)));
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
            else if (e.Name == "FadeArea")
            {
                string uid = Name + e.Attr("id");
                if (!IseultGame.Stats.HasTrigger(uid))
                {
                    Add(new FadeArea(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), new Vector2(e.AttrFloat("width"), e.AttrFloat("height")), uid,e.AttrInt("id")));
                }
            }
            else if (e.Name == "PressurePlate")
            {
                Add(new PressurePlate(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), e.AttrInt("triggerID")));
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
                Door.BeingAttacked.Add(enemy);
            }

            if (Mordecai.CurrentOn == Name && EntrySide != Side.Secret && e.AttrFloat("uid") == Mordecai.DoorUid)
            {
                Add(Mordecai = new Mordecai(new Vector2(Door.X,Door.Y+96)));
                if (Mordecai.LastTarget is IseultPlayer) Mordecai.ToggleFollow(Player);
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
                if (Mordecai.Joining())
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
            Sky.Color = Color.Lerp(new Color(0.1f, 0.1f, 0.2f), Color.OrangeRed, IseultPlayer.AliveTime * .0001f);
        }

        public void CheckForMonstersAtTheDoors()
        {
            foreach (DoorWay Door in Doors)
            {
                Door.CheckForAttackers();
            }
        }

        protected override void OnPause()
        {

            if (CurrentState == GameState.Game)
            {
                Add(Inventory = new Inventory());
                CurrentState = GameState.Paused;
            }
            else
            {
                Inventory.FadeAway(() => { CurrentState = GameState.Game; Inventory.RemoveSelf(); });
            }
        }

        public override void LoadTileset(XmlElement e)
        {
            int layerIndex = -3;
            if (e.Name == "OverTiles") layerIndex = GAMEPLAY_LAYER;
                
            Tileset newTile = new Tileset(layerIndex, e.InnerText, IseultGame.Atlas["tilesets/" + e.Attr("tileset")], new Vector2(32, 32));
            newTile.Depth = tilesetCount;
            if (e.Name == "OverTiles") newTile.Depth = -500;
            tilesetCount++;
            Add(newTile);
        }


        internal Mechanical getMechanical(int id)
        {
            foreach (Entity e in Tags[(int)GameTags.Mechanical])
            {
                if (e is Mechanical)
                {
                    if (((Mechanical)e).GetID() == id) return (Mechanical)e;
                }
            }
            return null;
        }
    }
}
