using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;


namespace Iseult
{
    public class Iseult : PlayerObject
    {
        private DoorWay SelectedDoor;

        public Iseult(Vector2 Position)
            : base(Position, new Vector2(128, 128),"iseult")
        {
            MaxSpeed.X = 5;
            MaxSpeed.Y = 10;
            Acceleration = 1f;
            JumpForce = 4;
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
                SelectedDoor.Enter();
            }
        }
        
        protected override void OnChangeSides(int newSide)
        {
            side = newSide;
        }

        protected override void PlayAnim(string animation, bool restart = false)
        {
            base.PlayAnim(animation+(side==-1?"Left":""), restart);
        }
    }
}
