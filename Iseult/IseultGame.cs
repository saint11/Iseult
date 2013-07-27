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

namespace Iseult
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class IseultGame : OldSkullGame
    {
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

        protected override void Initialize()
        {
            base.Initialize();
            Screen.Scale = 1;
            //Screen.EnableFullscreen(Monocle.Screen.FullscreenMode.KeepScale);
            Scene = new MainMenu();
        }
    }
}
