using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldSkull;
using System.Xml;

namespace Iseult
{
    public class Enemy:PlatformerObject
    {
        public string uid { get; private set; }
        public PlatformLevelEntity Target;
        public int Hp;

        public Enemy(Vector2 Position, string uid)
            : base(Position, new Vector2(48, 90))
        {
            this.uid = uid;
            Hp = 3;

            image = IseultGame.SpriteData.GetSpriteString("monk");
            Add(image);
            MaxSpeed = new Vector2(4, 10);
            Tag(GameTags.Enemy);
        }

        public Enemy(EnemyTracker tracker)
            : base(tracker.LastSeen, new Vector2(48, 90))
        {
            uid = tracker.Uid;
            Hp = tracker.Hp;

            image = IseultGame.SpriteData.GetSpriteString("monk");
            Add(image);
            MaxSpeed = new Vector2(4, 10);
            Tag(GameTags.Enemy);
        }

        public override void Added()
        {
            base.Added();
            Target = ((GameLevel)Level).Player;
        }

        public override void Update()
        {
            base.Update();

            int TargetSide = Math.Sign(Target.X - X);
            MoveH(0.6f*TargetSide);

            if (Hp == 0) OnDeath();
        }

        private void OnDeath()
        {
            EnemyTracker.KillEnemy(this);
            RemoveSelf();
        }

        private void MoveH(float HSpeed)
        {
            image.Effects = Math.Sign(HSpeed) == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            ((Sprite<string>)image).Play("walk");

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
