using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull;
using Monocle;

namespace Iseult.UI
{
    public class Credits : Scene
    {
        public Credits()
            : base()
        {
            SetLayer(0, new Layer());

            Image BackGround = new Image(new Texture(OldSkullGame.Path + "Content/Gfx/backgrounds/credits.png", true));
            Entity e = new Entity();
            e.Add(BackGround);
            Add(e);
        }

        public override void Update()
        {
            base.Update();

            KeyboardInput.Update();
            if (KeyboardInput.pressedAny())
            {
                Engine.Instance.Scene = new MainMenu();
            }
        }
    }
}
