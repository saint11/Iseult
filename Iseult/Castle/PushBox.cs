using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using Monocle;

namespace Iseult.Castle
{
    public class PushBox: PlatformerObject
    {
        private bool Locked;
        public PushBox(Vector2 Position, Vector2 Size)
            : base(Position, Size)
        {
            image = new Image(IseultGame.Atlas["environment/smallBox"]);
            image.CenterOrigin();
            Add(image);

            Position.X = (int)(Position.X / 32) * 32;
            Tag(GameTags.Solid);
        }

        public override void Added()
        {
            base.Added();
            Level.Solids.Add(this);
        }

        public void Push(float Speed)
        {
            if (!Locked) this.Speed.X = Speed;
        }

        internal void Unlock()
        {
            Locked = false;
        }

        internal void LockOnPlace()
        {
            Locked = true;
            Speed = Vector2.Zero;
        }
    }
}
