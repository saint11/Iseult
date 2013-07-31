using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using Iseult.Castle;

namespace Iseult
{
    public class PressurePlate:PlatformLevelEntity
    {
        private bool WasStepped=false;
        private int id;

        public PressurePlate(Vector2 Position, int id)
            :base(0)
        {
            this.id = id;
            this.Position = Position;
            Image Image = new Image(IseultGame.Atlas["environment/pressurePlate"]);
            Add(Image);
            Collider = new Hitbox(32, 16);
        }

        public override void Update()
        {
            base.Update();
            if (Level.CollideCheck(Collider.Bounds, GameTags.Heavy))
            {
                if (!WasStepped)
                {
                    WasStepped = true;
                    OnHit();
                }
                OnStepped();
            }
            else
            {
                if (WasStepped)
                {
                    WasStepped = false;
                    OnLetGo();
                }
            }
        }

        private void OnLetGo()
        {
            Mechanical Target = ((GameLevel)Level).getMechanical(id);
            if (Target!=null) Target.OnLetGo();
        }

        private void OnHit()
        {
            Mechanical Target = ((GameLevel)Level).getMechanical(id);
            if (Target != null) Target.OnHit();
        }

        private void OnStepped()
        {
            Mechanical Target = ((GameLevel)Level).getMechanical(id);
            if (Target != null) Target.OnStepped();
        }
    }
}
