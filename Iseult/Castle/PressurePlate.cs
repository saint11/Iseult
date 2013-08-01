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
        private Image Image;

        public PressurePlate(Vector2 Position, int id)
            :base(0)
        {
            this.id = id;
            this.Position = Position;
            Image = new Image(IseultGame.Atlas["environment/pressurePlate"]);
            Image.Origin.Y = Image.Height;
            Image.Y = Image.Height;
            Add(Image);
            Collider = new Hitbox(32, 16);
        }

        public override void Update()
        {
            base.Update();

            PlatformerObject e = (PlatformerObject)Level.CollideFirst(Collider.Bounds, GameTags.Heavy);
            if (e!=null && e.onGround)
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
            Tween.Scale(Image, new Vector2(1, 1f), 20, Ease.BackOut);

            Mechanical Target = ((GameLevel)Level).getMechanical(id);
            if (Target!=null) Target.OnLetGo();
        }

        private void OnHit()
        {

            Tween.Scale(Image, new Vector2(1, 0.1f), 20, Ease.BackOut);
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
