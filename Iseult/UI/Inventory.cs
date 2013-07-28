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
    public class Inventory : PlatformLevelEntity
    {
        private Image Image;
        private bool Fading=false;
        public Inventory()
            : base(GameLevel.PAUSE_LAYER, PlatformerLevel.GameState.Paused)
        {
            Image = new Image(IseultGame.Atlas["menu/pauseBg"]);
            Image.CenterOrigin();
            Add(Image);

            Text Knives = new Text(IseultGame.Font, "Knives: "+IseultGame.Stats.GetStats("knife").ToString(), new Vector2(10), Text.HorizontalAlign.Left);
            Add(Knives);

            Y = Engine.Instance.Screen.Height + Image.Height/2;
            Tween.Position(this, new Vector2(Engine.Instance.Screen.Width / 2, Engine.Instance.Screen.Height / 2),
                30, Ease.BackOut);
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
    }
}
