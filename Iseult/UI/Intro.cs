using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;
using OldSkull;
using OldSkull.Utils;

namespace Iseult.UI
{
    public class Intro:Scene
    {
        private Scene nextScene;
        private string[] TextLines;
        private int currentLine;
        private bool canSkip;

        private Text txt;

        private bool InputBlock = true;

        public Intro(Scene nextScene)
            : base()
        {
            this.nextScene = nextScene;

            SetLayer(0, new Layer(Microsoft.Xna.Framework.Graphics.BlendState.NonPremultiplied,
                Microsoft.Xna.Framework.Graphics.SamplerState.LinearClamp));
            SetLayer(1, new Layer(Microsoft.Xna.Framework.Graphics.BlendState.Additive,
                Microsoft.Xna.Framework.Graphics.SamplerState.LinearClamp));

            Engine.Instance.Screen.ClearColor = Color.Black;
            Image BackGround = new Image(new Texture(IseultGame.Path + "Content/Gfx/backgrounds/introBg.png", true));
            Entity e = new Entity();
            e.Add(BackGround);
            Add(e);
            Tween.Position(e, new Vector2(0, -92), 800, Ease.CubeInOut, Tween.TweenMode.YoyoLooping);

            Image ForeGround = new Image(new Texture(IseultGame.Path + "Content/Gfx/backgrounds/intro.png", true));
            Entity f = new Entity(1);
            f.Add(ForeGround);
            Add(f);
            Tween.Position(f, new Vector2(0, -92), 500, Ease.CubeInOut, Tween.TweenMode.YoyoLooping);
            Tween.Scale(ForeGround, new Vector2(1.25f, 1.25f), 900, Ease.CubeInOut, Tween.TweenMode.YoyoLooping);
            FadeScreen.FadeIn(this,30, 0,-100).OnComplete(ShowText);

            TextLines = new string[]
            {"1348, the year when the world ends.\n A dark cloud coming from the East.",
                "I've been dreaming about this,\n but I never should have warned them.",
                "Now the people from Fallibris are\n accusing me of witchery and want me dead.",
                "I'm locked here, waiting for my end to come,\n one way or another."};
            currentLine = 0;
        }

        private void ShowText(FadeScreen f=null)
        {
            if (currentLine < TextLines.Count())
            {
                Image Holder = new Image(IseultGame.Atlas["menu/messageBase"]);
                Holder.Color.A = 0;
                Holder.CenterOrigin();
                Holder.Position = new Vector2(Engine.Instance.Screen.Width / 2, Engine.Instance.Screen.Height / 2);

                AddComponent(Holder).Depth = -1;
                Tween.Alpha(Holder, 1, 60, null).OnComplete = (a) => {
                    canSkip = true;
                };

                if (txt!=null && txt.Entity != null)
                {
                    //Tween.Alpha(txt, 0, 15, null).OnComplete = (t) => { txt.Entity.RemoveSelf(); };
                    txt.Entity.RemoveSelf();
                }

                txt = new Text(IseultGame.FontText, TextLines[currentLine], new Vector2(Engine.Instance.Screen.Width / 2, Engine.Instance.Screen.Height / 2),Text.HorizontalAlign.Center);
                txt.Color = Color.WhiteSmoke;
                txt.Color.A = 0;
                txt.Scale = new Vector2(0.6f);
                AddComponent(txt).Depth = -2;
                Tween.Alpha(txt, 1, 50, null);

                currentLine++;
                canSkip = false;
            }
            else
            {
                FadeScreen.FadeOut(this, 30, 0, -100).OnComplete((a) =>
                {
                    Engine.Instance.Scene = nextScene;
                });
            }
        }

        private Entity AddComponent(Component c)
        {
            Entity e = new Entity();
            e.Add(c);
            Add(e);
            return e;
        }

        public override void Update()
        {
            base.Update();

            if (KeyboardInput.pressedInput("accept") && canSkip && !InputBlock)
            {
                ShowText();
            }
            if (!KeyboardInput.checkInput("accept")) InputBlock = false;

            KeyboardInput.Update();
        }
    }
}
