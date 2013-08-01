using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using OldSkull;
using Iseult.Castle;
using Iseult.UI;

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
        private bool Wait;

        public static Collectible Carrying { get; private set; }

        public IseultPlayer(Vector2 Position)
            : base(Position, new Vector2(24, 90),"iseult")
        {
            Tag(new GameTags[] { GameTags.Player, GameTags.Heavy });
            MaxSpeed.X = 5.50f;
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
                    OverHeadDisplay.Visible = true;
                    Mordecai M = ((Mordecai)SelectedNpc);
                    if (Carrying != null)
                    {
                        if (Carrying.ItemName == "bandages" ||
                            Carrying.ItemName == "domecq")
                            OverHeadDisplay.Play("give");
                        else OverHeadDisplay.Play(M.isFollowing(this) ? "unfollow" : "follow");
                    }
                    else
                    {
                        OverHeadDisplay.Play(M.isFollowing(this) ? "unfollow" : "follow");
                    }
                }
            }

            SelectedDoor = (DoorWay)Level.CollideFirst(Collider.Bounds, GameTags.Door);
            if (SelectedDoor != null)
            {
                SelectedDoor.OnTouched(this);
                OverHeadDisplay.Visible = true;
                if (SelectedDoor.IsBeingAttacked && IseultGame.Stats.GetStats("wood") > 0)
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

            Entity e = Level.CollideFirst(Collider.Bounds, GameTags.Enemy);
            if (e is Enemy)
            {
                Enemy SelectedEnemy = (Enemy)e;
                if (SelectedEnemy != null)
                {
                    IseultGame.Stats.AddStats("hp", -1);
                    if (IseultGame.Stats.GetStats("hp") <= 0)
                    {
                        Engine.Instance.Scene = new GameOver();
                    }
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

            if (KeyboardInput.pressedInput("use")) OnUse();

            if (IseultGame.Stats.GetStats("hp") <= 0)
            {
                RemoveSelf();
            }
            else
            {
                AliveTime++;
            }

        }

        private void OnUse()
        {
            if (SelectedNpc != null) 
                InteractWithNpc();
            else if (SelectedDoor != null && SelectedDoor.IsBeingAttacked && IseultGame.Stats.GetStats("wood") > 0)
            {
                SelectedDoor.Barricade();
                IseultGame.Stats.AddStats("wood", -1);
            }
            else
            {
                if (IseultGame.Stats.GetStats("knife") > 0 && onGround)
                {
                    IseultGame.Stats.AddStats("knife", -1);
                    PlayAnim("idleThrowing", true).OnAnimationComplete = (a) =>
                    {
                        Wait = false;
                        Speed = Vector2.Zero;
                        imageRight.OnAnimationComplete = imageLeft.OnAnimationComplete = null;
                    };
                    Wait = true;
                    
                    Level.Add(new Throwable(Position, side, "knife"));
                }
            }
        }

        protected override void UpdateControls()
        {
            if (!Wait) base.UpdateControls();
        }

        private void InteractWithNpc()
        {
            if (SelectedNpc is Mordecai)
            {
                if (Carrying != null)
                {
                    switch (Carrying.ItemName)
                    {
                        case "bandages":
                        case "domecq":
                            GiveItem(Carrying.ItemName, (Mordecai)SelectedNpc); break;
                        default:
                            ((Mordecai)SelectedNpc).ToggleFollow(this);
                            break;
                    }
                }
                else ((Mordecai)SelectedNpc).ToggleFollow(this);
            }
        }

        private void GiveItem(string item, Mordecai Npc)
        {
            ((GameLevel)Level).CallMessage(item);
            IseultGame.Stats.AddStats(item, -1);
            if (IseultGame.Stats.GetStats(item)<=0) Carrying = null;
        }

        protected override void OnChangeSides(int newSide)
        {
            side = newSide;
        }
        protected override Sprite<string>  PlayAnim(string animation, bool restart = false)
        {
            if (side == 1)
            {
                imageRight.Visible = true;
                imageRight.Play(animation, restart);
                imageLeft.Visible = false;

                return imageRight;
            }
            else
            {
                imageLeft.Visible = true;
                imageLeft.Play(animation, restart);
                image.Visible = false;

                return imageLeft;
            }
        }
        public override void SetPosition(Vector2 Position)
        {
            base.SetPosition(Position);
            Y += (image.Height - Collider.Height) / 2;
        }

        protected override void onCollideH(Entity solid)
        {
            if (solid is PushBox)
            {
                ((PushBox)solid).Push(Speed.X*0.7f);
            }
            base.onCollideH(solid);
        }

        internal void CheckMusic()
        {
            if (TailedBy.Count>0) OldSkull.Utils.Sounds.PlayMusic("music1");
            else OldSkull.Utils.Sounds.PlayMusic("music2");
        }

        internal static void Reset()
        {
            AliveTime = 0;
            Carrying = null;
        }

        public Sprite<string> imageRight { get { return (Sprite<string>)image; } set { image = value; } }
    }
}
