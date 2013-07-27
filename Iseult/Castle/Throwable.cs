using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OldSkull.GameLevel;
using OldSkull;
using Monocle;

namespace Iseult.Castle
{
    class Throwable:PlatformLevelEntity
    {
        private int side;
        private string ItemName;
        private bool Stuck=false;
        private Sprite<string> Image;

        public Throwable(Vector2 Position, int side, string ItemName)
            :base(GameLevel.GAMEPLAY_LAYER)
        {
            this.Position = Position;
            this.side = side;
            this.ItemName = ItemName;
            Image = OldSkullGame.SpriteData.GetSpriteString("throwable");
            Image.Play(ItemName); Image.Effects =
                 Image.Effects = side == -1 ? Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally :
                 Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
            Add(Image);

            Collider = new Hitbox(10, 10);
        }

        public override void Update()
        {
            base.Update();
            if (!Stuck)
            {
                if (Level.CollideCheck(Collider.Bounds, GameTags.Solid))
                {
                    Stuck = true;
                    Image.Origin.X = Image.Width - 10;
                    Tween tween = new Tween(Tween.TweenMode.Oneshot, Ease.BackInOut, 30,true);
                    float FinalRotation = Calc.Random.NextFloat() * 0.6f - 0.3f;
                    tween.OnUpdate = (t) => {
                        Image.Rotation = Calc.LerpSnap(0.2f, FinalRotation, t.Eased);
                    };
                    Add(tween);
                }
                else
                {
                    X += 12 * side;
                }
            }
        }
    }
}
