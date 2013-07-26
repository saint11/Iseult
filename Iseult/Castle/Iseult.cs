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
        public Iseult(Vector2 Position)
            : base(Position, new Vector2(128, 128),"iseult")
        {
            MaxSpeed.X = 5;
            MaxSpeed.Y = 10;
            Acceleration = 1f;
            JumpForce = 4;
        }
    }
}
