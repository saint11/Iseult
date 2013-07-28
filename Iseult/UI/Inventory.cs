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

            string ItemName = IseultPlayer.Carrying == null ? "empty" : IseultPlayer.Carrying.ItemName;

            Sprite<string> ItemIcon = IseultGame.SpriteData.GetSpriteString("bigItems");
            

            ItemIcon.Play(ItemName);
            Add(ItemIcon);

            if (IseultPlayer.Carrying != null)
            {
                Text TextItemName = new Text(IseultGame.Font, ItemName + ": " + IseultGame.Stats.GetStats(ItemName).ToString(), new Vector2(10));
                TextItemName.Y = 80;
                Add(TextItemName);
            }

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
