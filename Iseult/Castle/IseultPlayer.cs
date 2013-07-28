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
        private DoorWay SelectedDoor;
        public static int HEIGHT = 128;
        public static int WIDTH = 128;
        private Sprite<string> imageLeft;
        private Sprite<string> OverHeadDisplay;

        private Collectible SelectedItem;

        public IseultPlayer(Vector2 Position)
            : base(Position, new Vector2(48, 90),"iseult")
        {
            MaxSpeed.X = 6.3f;
            MaxSpeed.Y = 10;
            Acceleration = 1f;
            JumpForce = 5f;
            
            imageLeft = OldSkullGame.SpriteData.GetSpriteString("iseultLeft");
            OverHeadDisplay = OldSkullGame.SpriteData.GetSpriteString("OverHeadDisplay");
            OverHeadDisplay.Y -= 100;
            Add(imageLeft);
            Add(OverHeadDisplay);
            
            imageLeft.Y = image.Y = (Collider.Height - image.Height)/2;
            SetPosition(Position);
        }

        protected override void UpdateColisions()
        {
            OverHeadDisplay.Visible = false;

            SelectedDoor = (DoorWay)Level.CollideFirst(Collider.Bounds, GameTags.Door);
            if (SelectedDoor != null)
            {
                SelectedDoor.OnTouched(this);
                OverHeadDisplay.Visible = true;
                if (SelectedDoor.IsBeingAttacked && IseultGame.Stats.GetStats("materials") > 0)
                    OverHeadDisplay.Play("fixIt");
                else
                    OverHeadDisplay.Play("up");
            }

            SelectedItem = (Collectible)Level.CollideFirst(Collider.Bounds, GameTags.Item);
            if (SelectedItem != null)
            {
                SelectedItem.OnTouched(this);
                OverHeadDisplay.Visible = true;
                OverHeadDisplay.Play("down");
            }

            Enemy SelectedEnemy = (Enemy)Level.CollideFirst(Collider.Bounds, GameTags.Enemy);
            if (SelectedEnemy != null)
            {
                Engine.Instance.Scene = new GameOver();
            }
        }

        protected override void OnCrouching()
        {
            base.OnCrouching();
            if (SelectedItem != null) SelectedItem.onPickUp();
        }

        protected override void OnPressedUp()
        {
            if (SelectedDoor != null)
            {
                SelectedDoor.Enter(this);
            }
        }

        public override void Update()
        {
            base.Update();

            if (KeyboardInput.pressedInput("use"))
            {
                if (SelectedDoor != null && SelectedDoor.IsBeingAttacked && IseultGame.Stats.GetStats("materials")>0)
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
