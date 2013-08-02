using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OldSkull.GameLevel
{
    public class PlatformerLevel : Scene
    {
        //Layers
        protected Layer skyGameLayer;
        protected Layer bgGameLayer;
        protected Layer gameLayer;
        protected Layer hudLayer;
        protected Layer pauseLayer;

        //Layer Constants
        public static readonly int SKY_GAME_LAYER = -5;
        public static readonly int BG_GAME_LAYER = -3;
        public static readonly int GAMEPLAY_LAYER = 0;
        public static readonly int FRONT_GAMEPLAY_LAYER = 1;
        public static readonly int HUD_LAYER = 3;
        public static readonly int PAUSE_LAYER = 4;
        public static readonly int REPLAY_LAYER = 10;

        //LevelLoader Variables
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2 Gravity = new Vector2(0f,0.1f);
        public string Name { get; private set; }
        protected int tilesetCount = 0;

        //Lists
        public List<Entity> Solids {get;private set;}

        //Camera
        public Entity CameraTarget;
        protected string ConnectionRight;
        protected string ConnectionLeft;


        public enum Side { Left, Right, Secret, Door };

        public enum GameState { Game, Paused, Talk, Transition, ExitGame };
        public GameState CurrentState = GameState.Game;
        private HudElement HudBuilder;

        public PlatformerLevel(Vector2 size)
        {
            this.Width = (int)size.X;
            this.Height = (int)size.Y;

            SetLayer(SKY_GAME_LAYER, skyGameLayer = new Layer(BlendState.Opaque,SamplerState.PointClamp,0.2f));
            SetLayer(BG_GAME_LAYER, bgGameLayer = new Layer(BlendState.NonPremultiplied, SamplerState.PointClamp));
            SetLayer(GAMEPLAY_LAYER, gameLayer = new Layer(BlendState.NonPremultiplied, SamplerState.AnisotropicClamp));
            SetLayer(FRONT_GAMEPLAY_LAYER, gameLayer = new Layer(BlendState.NonPremultiplied, SamplerState.PointClamp));
            SetLayer(HUD_LAYER, hudLayer = new Layer(BlendState.NonPremultiplied, SamplerState.PointClamp, 0));
            SetLayer(PAUSE_LAYER, pauseLayer = new Layer(BlendState.NonPremultiplied, SamplerState.AnisotropicClamp , 0));

            Solids = new List<Entity>();

            BuildHud();

            int compensateX=0;
            int compensateY=0;
            if (Width < Camera.Viewport.Width)
            {
                compensateX = (int)(Camera.Viewport.Width - Width)/2 + 4;
                AddComponent(new Image(new Monocle.Texture(compensateX, Math.Max(Height, Camera.Viewport.Height), Color.Black)), FRONT_GAMEPLAY_LAYER).X = -compensateX;
                AddComponent(new Image(new Monocle.Texture(compensateX, Math.Max(Height, Camera.Viewport.Height), Color.Black)), FRONT_GAMEPLAY_LAYER).X = Width;
            }
            if (Height < Camera.Viewport.Height)
            {
                compensateY = (int)(Camera.Viewport.Height - Height) / 2 + 4;
                Entity e = AddComponent(new Image(new Monocle.Texture(Math.Max(Width, Camera.Viewport.Width) + compensateX, compensateY, Color.Black)), FRONT_GAMEPLAY_LAYER);
                e.Y = -compensateY;
                e.X = -compensateX;
                AddComponent(new Image(new Monocle.Texture(Math.Max(Width, Camera.Viewport.Width), compensateY, Color.Black)), FRONT_GAMEPLAY_LAYER).Y = Height;
            }
        }

        protected virtual void BuildHud()
        {
            HudBuilder = new HudElement();
            Add(HudBuilder);
        }

        public virtual void loadLevel(PlatformerLevelLoader ll)
        {
            Name = ll.Name;
            foreach (Solid solid in ll.solids)
            {
                Add(solid);
                Solids.Add(solid);
            }

            foreach (XmlElement e in ll.entities)
            {
                LoadEntity(e);
            }

            foreach (XmlElement e in ll.tilesets)
            {
                LoadTileset(e);
            }

            ConnectionRight = ll.Right;
            ConnectionLeft = ll.Left;
            Add(new SolidGrid(ll.solidGrid));
        }

        public Entity AddComponent(Component c, int layer=0)
        {
            Entity e = new Entity(layer);
            e.Add(c);
            Add(e);
            return e;
        }

        public virtual void LoadTileset(XmlElement e)
        {
            Graphics.Tileset newTile = new Graphics.Tileset(-3, e.InnerText, OldSkullGame.Atlas["tilesets/" + e.Attr("tileset")], new Vector2(32, 32));
            newTile.Depth = tilesetCount;
            tilesetCount++;
            Add(newTile);
        }   

        public virtual void LoadEntity(XmlElement e)
        {
            if (e.Name == "Player")
            {
                PlayerObject player = new PlayerObject(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), new Vector2(32, 32));
                Add(player);
                CameraTarget = player;
            }
        }

        public override void Update()
        {
            base.Update();

            UpdateCamera();
            KeyboardInput.Update();

            if (KeyboardInput.pressedInput("pause"))
            {
                OnPause();
            }
        }

        protected virtual void OnPause()
        {
            CurrentState = CurrentState == GameState.Paused ? GameState.Game : GameState.Paused;
        }
        protected virtual void UpdateCamera()
        {
            if (CameraTarget != null)
            {
                if (Width > Camera.Viewport.Width) Camera.X = Calc.LerpSnap(Camera.X, CameraTarget.X - Camera.Viewport.Width / 2, 0.1f);
                else Camera.X = (Width - Camera.Viewport.Width) / 2;

                if (Height > Camera.Viewport.Height) Camera.Y = Calc.LerpSnap(Camera.Y, CameraTarget.Y - Camera.Viewport.Height / 2, 0.1f);
                else Camera.Y = (Height - Camera.Viewport.Height) / 2;

            }
            KeepCameraOnBounds();
        }

        private void KeepCameraOnBounds()
        {
            if (Width > Camera.Viewport.Width)
            {
                if (Camera.X < 0) Camera.X = 0;
                if (Camera.X + Camera.Viewport.Width > Width) Camera.X = Width - Camera.Viewport.Width;
            }
            if (Height > Camera.Viewport.Height)
            {
                if (Camera.Y < 0) Camera.Y = 0;
                if (Camera.Y + Camera.Viewport.Height > Height) Camera.Y = Height - Camera.Viewport.Height;
            }
        }

        public virtual void OutOfBounds(Side side)
        {
            
        }

        //Loads an ogmo file into a platformer level. Specially useful if overrided with extra content.
        public static Scene autoLoad(string levelName)
        {
            PlatformerLevelLoader loader = PlatformerLevelLoader.load(levelName);
            PlatformerLevel level = new PlatformerLevel(loader.size);
            level.loadLevel(loader);
            return level;
        }
    }
}
