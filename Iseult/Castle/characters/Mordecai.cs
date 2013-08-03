﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Iseult.Castle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using OldSkull;
using OldSkull.GameLevel;

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
        private int Wandering;
        private int WanderingSide;
        private int Side;
        private bool Blocked=false;
        private int MAX_INTEREST = 60;
        private float Acceleration = .5f;
        private bool Alive;
        private int MinDistance;

        public Mordecai(Vector2 Position)
            :base(Position, new Vector2(32,64))
        {
            Tag(new GameTags[] { GameTags.Player, GameTags.Heavy });
            this.Position = Position;
            image = IseultGame.SpriteData.GetSpriteString("mordecai");
            Add(image);

            Tag(GameTags.Npc);

            MaxSpeed.X = 2.2f;
            GroundDamping = new Vector2(0.9f, 1);

            Instance = this;
            NextTarget=new List<PlatformLevelEntity>();
        }

        public override void Added()
        {
            base.Added();
            CurrentOn = Level.Name.ToUpper();
            Alive = true;
        }

        public override void Step()
        {
            base.Step();

            if (Alive)
            {
                SearchForPriorities();
                if (Wandering > 0)
                {
                    Speed.X = WanderingSide * Acceleration * 1.5f;
                    Image.Play("walk");
                    Wandering--;
                    Image.Effects = WanderingSide == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                }
                else if (Image.CurrentAnimID != "climb")
                {
                    CheckForDoors();
                    SearchForIseult();
                    FollowTarget();
                }

                checkForHazards();
            }
        }

        private void SearchForPriorities()
        {
            if (((GameLevel)Level).Altars.Count > 0 && !(Target is Altar))
            {
                foreach (Altar a in ((GameLevel)Level).Altars)
                {
                    if (!a.Prayed && Vector2.DistanceSquared(Position, a.Position) < Math.Pow(320, 2))
                    {
                        ToggleFollow(a,90);
                    }
                }
            }
        }

        private void checkForHazards()
        {
            Entity e = Level.CollideFirst(Collider.Bounds, GameTags.Enemy);
            if (e != null)
            {
                OnDeath();
            }
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

        public void ToggleFollow(PlatformLevelEntity NextTarget, int MinDistance=80)
        {
            this.MinDistance = MinDistance;
            if (Target == NextTarget) Target = null;
            else
            {
                this.NextTarget.Add(NextTarget);
                Target = NextTarget;
            }
            Interest = MAX_INTEREST;
            LastTarget = Target;
            Blocked = false;
        }

        private void FollowTarget()
        {
            image.X = 0;
            if (Target == null || Vector2.DistanceSquared(Target.Position, Position) < Math.Pow(MinDistance, 2) || Blocked)
            {
                int side = Math.Sign(Player.X - X);
                Image.Effects = side == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Image.Play("idle");
                if (Blocked) ToggleFollow(Target);
                if (!(Target is IseultPlayer)) Target = null;
            }
            else
            {
                if (Image.CurrentAnimID != "climb")
                {
                    Image.Play("walk");
                    Side = Math.Sign(Target.X - X);
                    Image.Effects = Side == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    Blocked = false;
                }
                float HMovement = Side * Math.Max(1,Acceleration);

                Rectangle Check = Collider.Bounds;
                Check.X += (int)HMovement;


                if (Level.CollideCheck(Check, GameTags.Solid))
                {
                    Entity block = Level.CollideFirst(Check, GameTags.Solid);
                    Check.Y -= 32;
                    Check.X += 32 * Side - (int)HMovement;

                    if (!Level.CollideCheck(Check, GameTags.Solid) && onGround)
                    {
                        image.X += 13 * Side;
                        Image.Play("climb");
                        Speed = Vector2.Zero;

                        if (block is Castle.PushBox) ((Castle.PushBox)block).LockOnPlace();
                        Image.OnAnimationComplete = (t) =>
                        {
                            Y -= 32;
                            X += 32 * Side;
                            Blocked = false;
                            Image.Play("idle");
                            Image.OnAnimationComplete = null;
                            if (block is Castle.PushBox) ((Castle.PushBox)block).Unlock();
                        };
                    }
                    else
                    {
                        Image.Play("idle");
                        Blocked = true;
                        Wandering = 80;
                        WanderingSide = -Side;
                    }
                }
                else
                {
                    if (onGround) Speed.X += Acceleration * Side;
                }

                if (Math.Abs(Target.Y - Y) > 256) LooseInterest();
                else Interest = MAX_INTEREST;
            }
        }

        private void LooseInterest()
        {
            Interest--;
            if (Interest <= 0) Target = null;
        }

        public bool Joining()
        {
            return (Vector2.DistanceSquared(Position, Player.Position) < Math.Pow(200,2));
        }

        internal bool isFollowing(PlatformLevelEntity entity)
        {
            return Target == entity;
        }

        public override void OnCrush(Solid by)
        {
            base.OnCrush(by);
            OnDeath();
        }

        private void OnDeath()
        {
            if (Alive)
            {
                PlayAnim("death").OnAnimationComplete = (a) =>
                {
                    Player.OnGrieve();
                };
                Alive = false;
            }
        }

        internal void OnGrieve()
        {

            Side = Math.Sign(Player.X - X);
            Image.Effects = Side == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            PlayAnim("grieve");
            Alive = false;
        }
    }
}