using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using OldSkull;
using Iseult.Castle;

namespace Iseult
{
    public class IseultPlayer : PlayerObject
    {
        public static int HEIGHT = 128;
        public static int WIDTH = 128;
        private Sprite<string> imageLeft;
        private Sprite<string> OverHeadDisplay;
        public static int AliveTime = 0;

        private PlatformLevelEntity SelectedNpc;
        private DoorWay SelectedDoor;
        private Collectible SelectedItem;
        public List<Enemy> TailedBy;

        public static Collectible Carrying { get; private set; }

        public IseultPlayer(Vector2 Position)
            : base(Position, new Vector2(48, 90),"iseult")
        {
            MaxSpeed.X = 6.3f;
            MaxSpeed.Y = 10;
            Acceleration = 1f;
            JumpForce = 5.4f;
            
            imageLeft = OldSkullGame.SpriteData.GetSpriteString("iseultLeft");
            OverHeadDisplay = OldSkullGame.SpriteData.GetSpriteString("OverHeadDisplay");
            OverHeadDisplay.Y -= 100;
            Add(imageLeft);
            Add(OverHeadDisplay);
            
            imageLeft.Y = image.Y = (Collider.Height - image.Height)/2;
            SetPosition(Position);

            TailedBy = new List<Enemy>();

            Depth = -10;
        }

        protected override void UpdateColisions()
        {
            OverHeadDisplay.Visible = false;


            SelectedNpc = (PlatformLevelEntity)Level.CollideFirst(Collider.Bounds, GameTags.Npc);
            if (SelectedNpc != null)
            {
                if (SelectedNpc is Mordecai)
                {
                    Mordecai M = ((Mordecai)SelectedNpc);
                    //  if (M.Target!=null && !M.isFollowing(this)) M.ToggleFollow(this);

                    OverHeadDisplay.Visible = true;
                    OverHeadDisplay.Play(M.isFollowing(this) ? "unfollow" : "follow");
                }
            }

            SelectedDoor = (DoorWay)Level.CollideFirst(Collider.Bounds, GameTags.Door);
            if (SelectedDoor != null)
            {
                SelectedDoor.OnTouched(this);
                OverHeadDisplay.Visible = true;
                if (SelectedDoor.IsBeingAttacked && IseultGame.Stats.GetStats("materials") > 0)
                    OverHeadDisplay.Play("fixIt");
                else
                {
                    if (Mordecai.Instance.Joining())
                        OverHeadDisplay.Play("enterTogether");
                    else
                        OverHeadDisplay.Play("up");
                }
            }

            SelectedItem = (Collectible)Level.CollideFirst(Collider.Bounds, GameTags.Item);
            if (SelectedItem != null)
            {
                if (Carrying != null)
                {
                    if (Carrying.ItemName == SelectedItem.ItemName)
                    {
                        SelectedItem.onPickUp();
                    }
                }
                else
                {
                    SelectedItem.OnTouched(this);
                    OverHeadDisplay.Visible = true;
                    OverHeadDisplay.Play("down");
                }
            }

            Enemy SelectedEnemy = (Enemy)Level.CollideFirst(Collider.Bounds, GameTags.Enemy);
            if (SelectedEnemy != null)
            {
                IseultGame.Stats.AddStats("hp", -1);
                if (IseultGame.Stats.GetStats("hp") <= 0)
                {
                    Engine.Instance.Scene = new GameOver();
                }
            }

        }

        protected override void OnCrouching()
        {
            if (SelectedItem != null && !Crouching)
            {
                if (Carrying != null && Carrying.ItemName != SelectedItem.ItemName) Carrying.onDrop(Position, Scene);
                SelectedItem.onPickUp();
                Carrying = SelectedItem;
            }
            base.OnCrouching();
        }

        protected override void OnPressedUp()
        {
            if (SelectedDoor != null)
            {
                SelectedDoor.Enter(this);
                if (SelectedDoor is Stairs) OnEnterStairs((Stairs)SelectedDoor);
            }
            
        }

        private void OnEnterStairs(Stairs SelectedDoor)
        {
            Mordecai Mordecai = Mordecai.Instance;
            if (Mordecai.isFollowing(this))
                Mordecai.ToggleFollow(SelectedDoor);

            foreach (Enemy enemy in TailedBy)
            {
                enemy.NextTarget = SelectedDoor;
            }
        }
        public override void Step()
        {
            base.Step();

            if (KeyboardInput.pressedInput("use"))
            {
                if (SelectedNpc != null)
                {
                    if (SelectedNpc is Mordecai) ((Mordecai)SelectedNpc).ToggleFollow(this);
                }
                else if (SelectedDoor != null && SelectedDoor.IsBeingAttacked && IseultGame.Stats.GetStats("materials")>0)
                {
                    SelectedDoor.Barricade();
                    IseultGame.Stats.AddStats("materials", -1);
                }
                else
                {
                    if (IseultGame.Stats.GetStats("knife") > 0)
                    {
                        IseultGame.Stats.AddStats("knife", -1);
                        Level.Add(new Throwable(Position, side, "knife"));
                    }
                }
            }

            if (IseultGame.Stats.GetStats("hp") <= 0)
            {
                RemoveSelf();
            }
            else
            {
                AliveTime++;
            }

        }

        protected override void OnChangeSides(int newSide)
        {
            side = newSide;
        }

        protected override void PlayAnim(string animation, bool restart = false)
        {
            if (side == 1)
            {
                image.Visible = true;
                ((Sprite<string>)image).Play(animation, restart);
                imageLeft.Visible = false;
            }
            else
            {
                imageLeft.Visible = true;
                imageLeft.Play(animation, restart);
                image.Visible = false;
            }
        }
        public override void SetPosition(Vector2 Position)
        {
            base.SetPosition(Position);
            Y += (image.Height - Collider.Height) / 2;
        }


    }
}
