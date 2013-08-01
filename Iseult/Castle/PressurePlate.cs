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
        private int[] id;
        private Image Image;

        public PressurePlate(Vector2 Position, string ids)
            :base(0)
        {
            id = Calc.ReadCSV(ids);
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

            List<Entity>EList = Level.CollideAll(Collider.Bounds, GameTags.Heavy);

            bool GotStepped = false;

            foreach (Entity e in EList)
            {
                if (e != null && e is PlatformerObject)
                {
                    if (((PlatformerObject)e).onGround)
                    {
                        if (!WasStepped)
                        {
                            WasStepped = true;
                            OnHit();
                        }
                        GotStepped = true;
                        OnStepped();
                    }
                }
            }

            if (!GotStepped)
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

            foreach (int i in id)
            {
                Mechanical Target = ((GameLevel)Level).getMechanical(i);
                if (Target != null) Target.OnLetGo();
            }
            
        }

        private void OnHit()
        {
            Tween.Scale(Image, new Vector2(1, 0.1f), 20, Ease.BackOut);

            foreach (int i in id)
            {
                Mechanical Target = ((GameLevel)Level).getMechanical(i);
                if (Target != null) Target.OnHit();
            }
        }

        private void OnStepped()
        {
            foreach (int i in id)
            {
                Mechanical Target = ((GameLevel)Level).getMechanical(i);
                if (Target != null) Target.OnStepped();
            }
        }
    }
}
