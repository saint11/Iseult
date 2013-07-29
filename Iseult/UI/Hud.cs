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
    public class Hud: PlatformLevelEntity
    {
        private Sprite<string> EquipedItem;

        public Hud()
            : base(GameLevel.HUD_LAYER)
        {
            Add(new Image(IseultGame.Atlas["hud/hudBg"]));
            Position = new Vector2(10);

            EquipedItem = IseultGame.SpriteData.GetSpriteString("item");
            EquipedItem.Position = new Vector2(16);
            Add(EquipedItem);
        }

        public override void Step()
        {
            base.Step();
            if (IseultPlayer.Carrying != null)
            {
                EquipedItem.Play(IseultPlayer.Carrying.ItemName);
                EquipedItem.Visible = true;
            }
            else
                EquipedItem.Visible = false;
        }
    }
}
