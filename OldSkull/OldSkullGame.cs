#region Using Statements
using Monocle;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using OldSkull.Utils;
#endregion

namespace OldSkull
{

    public class OldSkullGame : Engine
    {
        static public Atlas Atlas;
        static public SpriteData SpriteData;
        static public SpriteFont Font;
        static public GameStats Stats;

        public int PlayTime = 0;

        public const string Path = @"Assets\";

        static void Main(string[] args)
        {
            using (OldSkullGame demo = new OldSkullGame())
            {
                demo.Run();
            }
        }

        public OldSkullGame(int width=340, int height=220, float fps=60f, string title = "OldSkull Game")
            : base(width, height, fps, title)
        {
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            //Atlas = new Atlas(Path + @"Content/Atlas/atlas.xml", true);
            //SpriteData = new SpriteData(Path + @"Content/Atlas/SpriteData.xml", new Atlas[]{Atlas,});
            Content.RootDirectory = Path + "Content";
            //Font = Content.Load<SpriteFont>(@"Misc/pixel");
            Stats = GameStats.Init();
        }

        protected override void Initialize()
        {
            base.Initialize();

            Sounds.Load(new string[]{"JUMP"});

            Screen.Scale = 2f;

            KeyboardInput.InitDefaultInput();
            KeyboardInput.Add("jump", new Keys[]{ Keys.Z,Keys.Space});
            KeyboardInput.Add("use", new Keys[] { Keys.X, Keys.Enter });
            KeyboardInput.Add("special", new Keys[] { Keys.C, Keys.Tab });
            KeyboardInput.Add("accept", new Keys[] { Keys.Z, Keys.X, Keys.Space, Keys.Enter });
            KeyboardInput.Add("pause", new Keys[] {Keys.P, Keys.Escape});
            KeyboardInput.Add("escape", Keys.Escape);
        }
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            PlayTime++;
            if (PlayTime == int.MaxValue) PlayTime = 0;
        }
        public static int GetTotalTime() { return ((OldSkullGame)Instance).PlayTime; }

    }
}
