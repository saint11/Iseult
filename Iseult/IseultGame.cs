using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using OldSkull;
using OldSkull.Utils;
using Monocle;
using OldSkull.GameLevel;

namespace Iseult
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class IseultGame : OldSkullGame
    {
        static public Atlas Atlas1;
        static public SpriteFont FontText { get; private set; }

        public IseultGame()
            : base(1366,768,60)
        { }


        static void Main(string[] args)
        {
            using (IseultGame demo = new IseultGame())
            {
                demo.Run();
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Atlas = new Atlas(Path + @"Content/Atlas/atlas0.xml", true);
            Atlas1 = new Atlas(Path + @"Content/Atlas/atlas1.xml", true);

            SpriteData = new SpriteData(Path + @"Content/Atlas/SpriteData.xml", new Atlas[] { Atlas, Atlas1 });
            Font = Content.Load<SpriteFont>(@"Misc/medieval");
            FontText = Content.Load<SpriteFont>(@"Misc/celtic");
        }

        protected override void Initialize()
        {
            base.Initialize();
            Screen.Scale = 1;

            Scene = new UI.Logos();
            EnemyTracker.Init();
            Sounds.Init();
            Sounds.Load(new string[]{
                "VINHETA"
            });

            Commands.RegisterCommand("goto", (args) =>
            {
                if (File.Exists(OldSkullGame.Path + @"Content\Level\" + args[0] + ".oel"))
                {
                    IseultPlayer.AliveTime = 0;
                    PlatformerLevelLoader loader = PlatformerLevelLoader.load(args[0]);
                    GameLevel level = new GameLevel(loader, PlatformerLevel.Side.Debug);

                    Engine.Instance.Scene = level;
                }
                else
                {
                    Commands.Log("Cannot find the file " + args[0] + ".oel");
                }
            });
        }

    }
}
