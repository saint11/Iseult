using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull;
using Monocle;
using Microsoft.Xna.Framework;

namespace Iseult.UI
{
    public class Logos:Scene
    {
        private Image fade;

        public Logos()
            : base()
        {
            SetLayer(0, new Layer(Microsoft.Xna.Framework.Graphics.BlendState.NonPremultiplied,
                Microsoft.Xna.Framework.Graphics.SamplerState.AnisotropicClamp));
            Screen screen = Engine.Instance.Screen;
            screen.ClearColor = Color.Black;


            Entity miniE = new Entity();
            Image bg = new Image(new Texture(screen.Width, screen.Height, Color.White));

            int randomMascot = (int)Calc.Random.NextFloat(8) + 1;
            Image mascot = new Image(new Texture(IseultGame.Path + "Content/Gfx/MiniBoss/mascot0" + randomMascot.ToString() + ".png", true));
            mascot.CenterOrigin();
            mascot.Position = screen.Size / 2;
            mascot.Y -= 80;

            Image logo = new Image(new Texture(IseultGame.Path + "Content/Gfx/MiniBoss/logo.png", true));
            logo.CenterOrigin();
            logo.Position = screen.Size / 2;
            logo.Y += 150;


            fade = new Image(new Texture(screen.Width, screen.Height, Color.Black));

            miniE.Add(bg);
            miniE.Add(mascot);
            miniE.Add(logo);
            miniE.Add(fade);
            Add(miniE);
            Tween.Alpha(fade, 0, 80, Ease.CubeIn).OnComplete = (t) =>
            {
                OldSkull.Utils.Sounds.Play("VINHETA");
                Tween tween = new Tween(Tween.TweenMode.Oneshot, null, 300, true);
                miniE.Add(tween);
                tween.OnComplete = (u) =>
                {
                    FadeOut();
                };
            };
            
        }

        private void FadeOut()
        {
            Tween.Alpha(fade, 1, 60, Ease.CubeOut).OnComplete = (r) => { OnComplete(); };
        }

        public override void Update()
        {
            base.Update();
            KeyboardInput.Update();
            if (KeyboardInput.pressedInput("accept")) FadeOut();
        }

        private void OnComplete()
        {
            Engine.Instance.Scene = new MainMenu();
        }
    }
}
