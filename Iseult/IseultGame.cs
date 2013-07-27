using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using OldSkull;
using Monocle;

namespace Iseult
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class IseultGame : OldSkullGame
    {
        static public Atlas Atlas1;

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
        }

        protected override void Initialize()
        {
            base.Initialize();
            Screen.Scale = 1;
            Stats.SetStats("hp", 10);
            Scene = new MainMenu();
            EnemyTracker.Init();
        }

    }
}
