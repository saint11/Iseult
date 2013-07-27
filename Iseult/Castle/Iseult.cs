using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using OldSkull;

namespace Iseult
{
    public class Iseult : PlayerObject
    {
        private DoorWay SelectedDoor;
        public static int HEIGHT = 128;
        public static int WIDTH = 128;
        private Sprite<string> imageLeft;
        public Iseult(Vector2 Position)
            : base(Position, new Vector2(48, 90),"iseult")
        {
            MaxSpeed.X = 6.3f;
            MaxSpeed.Y = 10;
            Acceleration = 1f;
            JumpForce = 5f;
            imageLeft = OldSkullGame.SpriteData.GetSpriteString("iseultLeft");
            Add(imageLeft);
            imageLeft.Y = image.Y = (Collider.Height - image.Height)/2;
            SetPosition(Position);
        }

        protected override void UpdateColisions()
        {
            SelectedDoor = (DoorWay)Level.CollideFirst(Collider.Bounds, GameTags.Door);
            if (SelectedDoor != null)
            {
                SelectedDoor.OnTouched(this);
            }
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
                Level.Add(new Throwable(Position, side,"knife"));
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
