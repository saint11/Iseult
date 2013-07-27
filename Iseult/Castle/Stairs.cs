using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OldSkull.GameLevel;

namespace Iseult
{
    public class Stairs : DoorWay
    {
        public enum Direction { up, down };

        private Stairs Sister;
        private Direction direction;

        public static void Link(Stairs First, Stairs Second)
        {
            First.Sister = Second;
            Second.Sister = First;
        }

        public Stairs(Vector2 Position, bool Back, Direction direction)
            :base(Position, Back, 0)
        {
            this.direction = direction;
        }

        protected override void LoadImage()
        {
            Image = IseultGame.SpriteData.GetSpriteString("stairs");
            Image.Play(direction == Direction.up ? "up" : "down");
            Add(Image);
        }

        public override void  Enter(PlatformLevelEntity Entity)
        {
            Entity.SetPosition(Sister.Position + new Vector2(46,64));
        }
    }
}
