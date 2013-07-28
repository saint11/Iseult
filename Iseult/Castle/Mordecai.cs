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

        private PlatformLevelEntity Target;
        private List<PlatformLevelEntity> NextTarget;

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
            NextTarget=new List<PlatformLevelEntity>();
        }

        public override void Added()
        {
            base.Added();
            CurrentOn = Level.Name.ToUpper();
        }

        public override void Step()
        {
            base.Step();

            CheckForDoors();
            FollowTarget();

        }

        private void CheckForDoors()
        {
            if (Target is DoorWay)
            {
                DoorWay SelectedDoor = (DoorWay)Level.CollideFirst(Collider.Bounds, GameTags.Door);
                if (SelectedDoor == Target)
                {
                    SelectedDoor.Enter(this);
                    Y += 32;

                    if (NextTarget.Count > 0)
                    {
                        Target = NextTarget[0];
                        NextTarget.RemoveAt(0);
                    }
                    else Target = Player;
                }
            }
        }

        public void ToggleFollow(PlatformLevelEntity NextTarget)
        {
            if (Target == NextTarget) Target = null;
            else Target = NextTarget;
        }

        private void FollowTarget()
        {
            if (Target == null)
            {
                int side = Math.Sign(Player.X - X);
                Image.Effects = side == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            }
            else
            {
                int side = Math.Sign(Target.X - X);
                Image.Effects = side == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
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

                if (Math.Abs(Target.Y - Y) > 96) Target = null;
            }
        }

        internal bool isFollowing(PlatformLevelEntity entity)
        {
            return Target == entity;
        }
    }
}