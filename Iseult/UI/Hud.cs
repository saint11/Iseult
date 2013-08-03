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
        private Sprite<string> MordecaiStats;

        public Hud()
            : base(GameLevel.HUD_LAYER)
        {
            Image bg;
            Add(bg = new Image(IseultGame.Atlas["hud/hudBg"]));
            bg.Position = new Vector2(1000, 10);

            EquipedItem = IseultGame.SpriteData.GetSpriteString("item");
            EquipedItem.Position = new Vector2(1020,20);
            Add(EquipedItem);

            MordecaiStats = IseultGame.SpriteData.GetSpriteString("mordecaiStats");
            MordecaiStats.Position = new Vector2(10);
            Add(MordecaiStats);
        }

        public override void Step()
        {
            base.Step();
            if (Mordecai.Instance != null)
            {
                string target="idle";
                if (Mordecai.Instance.Target==null) target = "idle";
                else {
                    if (Mordecai.Instance.Target is IseultPlayer) target = "follow";
                    else target = "followOther";
                }
                MordecaiStats.Play(target);

            }

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
