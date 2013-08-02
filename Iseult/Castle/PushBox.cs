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

            Position.X +=32;
            CollideOnTag = new GameTags[] { GameTags.Solid, GameTags.BlockSolid };
            Tag(GameTags.Solid, GameTags.Heavy);
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

        public override void Step()
        {
            base.Step();

            Mordecai Npc = (Mordecai)Level.CollideFirst(Collider.Bounds, GameTags.Npc);
            if (Npc !=null)
            {
                Speed.X = - Npc.Speed.X;
            }
        }


    }
}
