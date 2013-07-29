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

        public PlatformLevelEntity Target { get; private set; }
        private List<PlatformLevelEntity> NextTarget;

        public static string CurrentOn;
        public static int DoorUid=0;
        public static Mordecai Instance;
        public static Vector2 LastSeen;
        public static PlatformLevelEntity LastTarget;

        private int Interest;
        private int MAX_INTEREST = 60;
        private float Acceleration = 1.95f;

        public Mordecai(Vector2 Position)
            :base(Position, new Vector2(32,64))
        {
            this.Position = Position;
            image = IseultGame.SpriteData.GetSpriteString("mordecai");
            Add(image);

            Tag(GameTags.Npc);

            MaxSpeed.X = 4.2f;

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
            SearchForIseult();
            FollowTarget();

        }

        private void SearchForIseult()
        {
            if (Target is DoorWay){
                if (Vector2.DistanceSquared(Position, Player.Position) < (Math.Pow(400,2)) &&
                    Math.Abs(Position.Y-Player.Y)<96)
                {
                    ToggleFollow(Player);
                }
            }
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

                    //if (NextTarget.Count > 0)
                    //{
                    //    Target = NextTarget[NextTarget.Count-1];
                    //    NextTarget.RemoveAt(NextTarget.Count-1);
                    //}
                    //else 
                    ToggleFollow(Player);
                }
            }
        }

        public void ToggleFollow(PlatformLevelEntity NextTarget)
        {
            if (Target == NextTarget) Target = null;
            else
            {
                this.NextTarget.Add(NextTarget);
                Target = NextTarget;
            }
            Interest = MAX_INTEREST;
            LastTarget = Target;
        }

        private void FollowTarget()
        {
            if (Target == null)
            {
                int side = Math.Sign(Player.X - X);
                Image.Effects = side == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Image.Play("idle");
            }
            else
            {
                if (Image.CurrentAnimID!="climb") Image.Play("walk");
                int side = Math.Sign(Target.X - X);
                Image.Effects = side == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                float HMovement = side * Acceleration;

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
                    else Image.Play("idle");
                }
                else
                {
                    Check.X += 32*side;
                    Check.Y += 64 + 1;
                    if (Level.CollideCheck(Check, GameTags.Solid)) X += (int)HMovement;
                    else Image.Play("idle");
                }

                if (Math.Abs(Target.Y - Y) > 96)
                {
                    Interest--;
                    if (Interest <= 0) Target = null;
                }
                else Interest = MAX_INTEREST;
            }
        }

        public bool Joining()
        {
            return (Vector2.DistanceSquared(Position, Player.Position) < Math.Pow(400,2));
        }

        internal bool isFollowing(PlatformLevelEntity entity)
        {
            return Target == entity;
        }
    }
}