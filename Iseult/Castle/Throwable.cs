using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OldSkull.GameLevel;
using OldSkull;
using Monocle;

namespace Iseult
{
    class Throwable:PlatformerObject
    {
        private int side;
        private string ItemName;

        private bool Fallen = false;
        private bool Stuck = false;

        private Sprite<string> Image;
        private float Rotation=0;

        public Throwable(Vector2 Position, int side, string ItemName)
            :base(Position,new Vector2(20,10))
        {
            this.Position = Position;
            this.side = side;
            this.ItemName = ItemName;
            Image = OldSkullGame.SpriteData.GetSpriteString("throwable");
            Image.Play(ItemName); Image.Effects =
                 Image.Effects = side == -1 ? Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally :
                 Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
            Add(Image);
            Speed = new Vector2(20 * side, -3);
            MaxSpeed = new Vector2(20, 10);
            AirDamping = new Vector2(1, 0.95f);
            GroundDamping = new Vector2(0.5f, 0.5f);
            Gravity = new Vector2(0, 0.1f);
        }
        protected override void onCollideV(Solid solid)
        {
            //base.onCollideV(solid);
            if (Speed.Y > 0)
            {
                onGround = true;
            }

            Speed.Y *= -0.8f;
            Rotation = 0;

            if (Math.Abs(Speed.Y) < 0.1)
            {
                Speed.Y = 0;
            }
        }
        protected override void onCollideH(Solid solid)
        {
            base.onCollideH(solid);
            HitSomething();
        }


        private void HitSomething()
        {
            if (!Stuck && !Fallen)
            {
                if (Calc.Chance(Calc.Random, 0.9f))
                {
                    Fallen = true;
                    Rotation = 0.3f - Calc.Random.NextFloat(0.15f);
                    Speed.X = -side*2;
                    Speed.Y = -6;
                }
                else
                {
                    Stuck = true;
                    Image.Origin.X = side == 1 ? Image.Width - 10 : 10;
                    Tween tween = new Tween(Tween.TweenMode.Oneshot, Ease.CubeOut, 15, true);
                    float FinalRotation = Calc.Random.NextFloat() * 0.2f - 0.1f;
                    float StartX = Image.X;
                    float FinalX = Image.X + 12 * side;

                    tween.OnUpdate = (t) =>
                    {
                        Image.Rotation = Calc.LerpSnap(0, FinalRotation, t.Eased);
                        Image.X = Calc.LerpSnap(StartX, FinalX, t.Eased);
                    };
                    
                    tween.OnComplete = (t) =>Tween.Alpha(Image, 0, 20, Ease.CubeOut).OnComplete = (j) => { RemoveSelf(); };
                    Add(tween);

                    Speed = new Vector2(0);
                    Gravity = new Vector2(0);
                }
            }
        }

        public override void Update()
        {
            base.Update();

            Image.Rotation += Rotation;
            if (onGround) Speed *= GroundDamping;

            if (Fallen || Stuck)
            {
                if (Level.CollideCheck(Collider.Bounds, GameTags.Player))
                {
                    RemoveSelf();
                    IseultGame.Stats.AddStats("knife", 1);
                }
            }
            else
            {
                Enemy en = (Enemy)Level.CollideFirst(Collider.Bounds, GameTags.Enemy);
                if (en != null)
                {
                    en.Hp--;
                    HitSomething();
                }
            }
        }
    }
}
