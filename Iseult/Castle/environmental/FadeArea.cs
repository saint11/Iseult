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
    public class FadeArea:PlatformLevelEntity,Mechanical
    {
        Image image;
        private bool Fading;
        private string uid;
        public int ID { get; private set; }

        public FadeArea(Vector2 Position, Vector2 Size, string uid, int ID)
            : base(GameLevel.FRONT_GAMEPLAY_LAYER)
        {
            this.Position = Position;
            this.uid = uid;
            this.ID = ID;

            Tag(GameTags.Mechanical);

            Collider = new Hitbox(Size.X,Size.Y);

            image = new Image(new Texture((int)Size.X, (int)Size.Y, Color.Black));
            image.Color.A = 245;
            Add(image);
        }

        public override void Update()
        {
            base.Update();
            
            if (Level.CollideCheck(Collider.Bounds,GameTags.Player))
            {
                Reveal();
            }
        }

        private void Reveal()
        {
            if (!Fading)
            {
                Fading = true;
                Tween.Alpha(image, 0, 40, Ease.CubeIn).OnComplete = (t) =>
                {
                    IseultGame.Stats.SetTrigger(uid);
                    RemoveSelf();
                };
            }
        }



        public void OnLetGo()
        {
            
        }

        public void OnHit()
        {
            Reveal();
        }

        public void OnStepped()
        {
        }


        public int GetID()
        {
            return ID;
        }
    }
}
