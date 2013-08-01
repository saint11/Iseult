using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using Monocle;

namespace Iseult.Castle
{
    public class MovingBlock:Solid,Mechanical
    {
        private int id;
        private Vector2 Original;
        private Vector2 Alternate;
        private Vector2 counter;
        private float OriginalDistance;

        private bool Powered = false;

        public MovingBlock(Vector2 Position, Vector2 Position2, Vector2 Size,int ID)
            : base((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y)
        {
            this.id = ID;


            this.Position = Original = Position;
            this.Alternate = Position2;

            Tag(new GameTags[] {GameTags.Solid, GameTags.Mechanical});

            Image image = new Image(IseultGame.Atlas["environment/smallBox"]);
            image.Scale = Size/(new Vector2(image.Width,image.Height));
            Add(image);
            Collider = new Hitbox(Size.X, Size.Y);
            counter = Vector2.Zero;

            OriginalDistance = Vector2.Distance(Position, Alternate);
        }

        public void Move(Vector2 Direction)
        {
            counter += Direction;
            int moveH = (int)Math.Round(counter.X);
            int moveV = (int)Math.Round(counter.Y);

            Vector2 ToPosition = Position;
            counter.X -= moveH;
            ToPosition.X += moveH;

            counter.Y -= moveV;
            ToPosition.Y += moveV;

            foreach (Entity e in Scene.Tags[(int)GameTags.Player])
            {
                if (e is PlatformerObject)
                {
                    PlatformerObject p = (PlatformerObject)e;
                    if (p.IsRiding(this))
                    {
                        p.MoveExactH(moveH);
                        p.MoveExactV(moveV);
                    }
                }
            }

            foreach (Entity e in CollideAll(new GameTags[]{GameTags.Player,GameTags.Npc},ToPosition))
            {
                if (e is PlatformerObject)
                {
                    PlatformerObject p = (PlatformerObject)e;
                    p.MoveExactH(moveH,p.OnCrush);
                    p.MoveExactV(moveV, p.OnCrush);
                }
            }

            Position = ToPosition;
        }

        public override void Update()
        {
            base.Update();

            if (Powered) MoveTowards(Alternate, 0.5f + 0.1f * OriginalDistance);
            else MoveTowards(Original, 0.5f + 0.01f * OriginalDistance);
        }

        void Mechanical.OnLetGo()
        {
            Powered = !Powered;
            //Tween.Position(this, Original, 50, Ease.BackOut);
        }

        void Mechanical.OnHit()
        {
            Powered = !Powered;
            //Tween.Position(this, Alternate, 50, Ease.BackOut);
        }

        void Mechanical.OnStepped()
        {
            
        }

        private void MoveTowards(Vector2 MoveTarget, float Speed)
        {
            Move((Calc.Approach(Position + counter, MoveTarget, Speed)) - (Position + counter));
        }

        int Mechanical.GetID()
        {
            return id;
        }
    }
}
