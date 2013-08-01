using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull;
using OldSkull.GameLevel;
using Monocle;
using Microsoft.Xna.Framework;

namespace Iseult
{
    public class Message: PlatformLevelEntity
    {
        private Image Image;
        private bool Fading=false;

        public Message(string Message, string Title)
            : base(GameLevel.PAUSE_LAYER, PlatformerLevel.GameState.Paused)
        {
            Image = new Image(IseultGame.Atlas["hud/messageBg"]);
            Image.CenterOrigin();
            Add(Image);


            AddTextLine(-90, Title, Color.Black, 1f).Scale = Vector2.One;
            int i = 0;
            foreach (string line in WrapText(IseultGame.Font,Message,Image.Width-60))
            {
                if (line == "" || line==null) break;

                AddTextLine(10 + i * 30 + 2, line, Color.DarkOliveGreen,1f);

                i++;
            }



            Y = - Image.Height/2;
            Tween.Position(this, new Vector2(Engine.Instance.Screen.Width / 2, Engine.Instance.Screen.Height / 2),
                30, Ease.BackOut);
        }

        private Text AddTextLine(int YPosition, string line, Color color, float alpha)
        {
            Text TextMessage = new Text(IseultGame.Font, line, new Vector2(10));

            TextMessage.Y = YPosition;
            TextMessage.Color = color;
            TextMessage.Scale = new Vector2(0.8f);
            TextMessage.Color.A = 0;
            Add(TextMessage);
            Tween.Alpha(TextMessage, alpha, 50, Ease.CubeIn);

            return TextMessage;
        }

        public override void Step()
        {
            base.Step();
            if (KeyboardInput.pressedInput("accept")) FadeAway(() => { Level.CurrentState = PlatformerLevel.GameState.Game; });
        }

        internal void FadeAway(Action Action)
        {
            if (!Fading)
            {
                Fading = true;
                Tween.Position(this, new Vector2(Engine.Instance.Screen.Width, Engine.Instance.Screen.Height + Image.Height / 2),
                   30, Ease.BackIn).OnComplete = (t) => { Action(); };
            }
        }

        public string[] WrapText(Microsoft.Xna.Framework.Graphics.SpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');

            string[] sb = new string[6];
            int currentLine = 0;

            float lineWidth = 0f;

            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb[currentLine] += word + " ";
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    currentLine++;
                    sb[currentLine] += word + " ";
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb;
        }

    }
}
