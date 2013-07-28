using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using OldSkull;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace Iseult
{
    public class Mordecai : PlatformerObject
    {
        IseultPlayer Player { get { return ((GameLevel)Level).Player; } }
        private Sprite<string> Image { get { return (Sprite<string>)image; } }

        private bool following=false;


        public static string CurrentOn;
        public static int DoorUid=0;
        public static Mordecai Instance;
        public static Vector2 LastSeen;

        public Mordecai(Vector2 Position)
            :base(Position, new Vector2(32,64))
        {
            this.Position = Position;
            image = IseultGame.SpriteData.GetSpriteString("mordecai");
            Add(image);

            Tag(GameTags.Npc);

            Instance = this;
        }

        public override void Added()
        {
            base.Added();
            CurrentOn = Level.Name.ToUpper();
        }

        public override void Step()
        {
            base.Step();

            int side = Math.Sign(Player.X - X);
            if (following) FollowPlayer(side);
            Image.Effects = side == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }

        public void ToggleFollow()
        {
            following = !following;
        }

        private void FollowPlayer(int side)
        {
            float HMovement = side * 1.8f;

            Rectangle Check = Collider.Bounds;
            Check.X += (int)HMovement;
            if (Level.CollideCheck(Check, GameTags.Solid))
            {
                Check.Y -= 32;
                Check.X += 32 * side - (int)HMovement;
                if (!Level.CollideCheck(Check, GameTags.Solid))
                {
                    Image.Play("climb");
                    Image.OnAnimationComplete = (t) =>
                    {
                        Y -= 32;
                        X += 32 * side;
                        Image.Play("idle");
                        Image.OnAnimationComplete = null;
                    };
                }
            }
            else
            {
                X += (int)HMovement;
            }
        }
    }
}