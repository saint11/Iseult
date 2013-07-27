using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Monocle;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using OldSkull;

namespace Iseult
{
    public class Enemy:PlatformerObject
    {
        public Enemy(Vector2 Position)
            : base(Position, new Vector2(48, 90))
        {
            image = IseultGame.SpriteData.GetSpriteString("fanatic");
            Add(image);
            MaxSpeed = new Vector2(4, 10);
        }

        public override void Update()
        {
            base.Update();
            int PlayerSide = Math.Sign( ((GameLevel)Level).Player.X - X);
            MoveH(0.6f*PlayerSide);
        }

        private void MoveH(float HSpeed)
        {
            Rectangle check = Collider.Bounds;
            check.X += Math.Sign(HSpeed) * 8;
            if (onGround && Scene.CollideCheck(check, GameTags.Solid))
            {
                check.Y -= 32;
                if (!Scene.CollideCheck(check, GameTags.Solid))
                {
                    Speed.Y = -4.0f;
                }
                else
                {
                    check.Y -= 32;
                    if (!Scene.CollideCheck(check, GameTags.Solid))
                        Speed.Y = -6.0f;
                }
            }
            Speed.X = HSpeed;
            if (onGround) PlayAnim("walk");
            else
            {
                if (Speed.Y > 0) PlayAnim("jumpUp");
                else PlayAnim("jumpDown");
            }
        }
    }
}
