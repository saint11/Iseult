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
        IseultPlayer Player { get { return ((GameLevel)Level).Player; } }
        private Sprite<string> Image { get { return (Sprite<string>)image; } }
            
        public PlatformLevelEntity Target { get; private set; }
        public PlatformLevelEntity NextTarget;

        public string uid { get; private set; }
        public int Hp;

        private int Interest;
        private int MAX_INTEREST = 60;
        private float Acceleration = 1.95f;

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
            MaxSpeed = new Vector2(5, 10);
            
            Tag(GameTags.Enemy);
        }

        public override void Added()
        {
            base.Added();
            ToggleFollow(Player);
        }

        public override void Step()
        {
            base.Step();
            CheckForDoors();
            SearchForIseult();
            FollowTarget();


            if (NextTarget != null)
            {
                ToggleFollow(NextTarget);
                NextTarget = null;
            }
            if (Hp == 0) OnDeath();
        }

        private void OnDeath()
        {
            EnemyTracker.KillEnemy(this);
            RemoveSelf();
        }

        private void SearchForIseult()
        {
            if (Target is DoorWay)
            {
                if (Vector2.DistanceSquared(Position, Player.Position) < (Math.Pow(400, 2)) &&
                    Math.Abs(Position.Y - Player.Y) < 96)
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
                    Y += 16;

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
            else Target = NextTarget;

            Interest = MAX_INTEREST;

            if (NextTarget is IseultPlayer)
            {
                List<Enemy> list = ((IseultPlayer)NextTarget).TailedBy;
                if (!list.Contains(this)) list.Add(this);
            }
            else
            {
                List<Enemy> list = Player.TailedBy;
                list.Remove(this);
            }
        }

        private void FollowTarget()
        {
            if (Target == null)
            {
                int side = Math.Sign(Player.X - X);
                Image.Effects = side == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Image.Play("idle");

                if (Vector2.DistanceSquared(Player.Position, Position) < Math.Pow(250, 2))
                {
                    ToggleFollow(Player);
                }
            }
            else
            {
                if (Image.CurrentAnimID != "climb") Image.Play("walk");
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
                        if (!Scene.CollideCheck(Check, GameTags.Solid))
                        {
                            Speed.Y = -4.0f;
                        }
                        else
                        {
                            Check.Y -= 32;
                            if (!Scene.CollideCheck(Check, GameTags.Solid))
                                Speed.Y = -6.0f;
                        }
                    }
                }
                else
                {
                    X += (int)HMovement;
                }

                if (Math.Abs(Target.Y - Y) > 96)
                {
                    Interest--;
                    if (Interest <= 0) Target = null;
                }
                else Interest = MAX_INTEREST;
            }
        }

        /*private void MoveH(float HSpeed)
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
        }*/
    }
}
