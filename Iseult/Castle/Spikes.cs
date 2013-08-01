﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using Monocle;
using Microsoft.Xna.Framework;

namespace Iseult.Castle
{
    public class Spikes:PlatformLevelEntity,Mechanical
    {
        private int Timer=10;
        private Vector2 Original;
        private int id;
        private bool Working;

        public Spikes(Vector2 Position, bool invert, int id)
            : base(0)
        {
            Working = !invert;
            this.id = id;
            this.Position = Original = Position;
            this.Position.Y += 32;
            Image Image = new Image(IseultGame.Atlas["environment/spikes"]);
            Add(Image);

            Collider = new Hitbox(32, 32);
            Tag(new GameTags[] { GameTags.Enemy, GameTags.Mechanical });

            Depth = -100;
        }

        public override void DebugRender()
        {
            if (Collider != null)
                Collider.Render(Collidable ? Color.Red : Color.Black);
        }

        public override void Update()
        {
            base.Update();

            if (Working)
            {
                Timer--;
                if (Timer == 0)
                {
                    Tween goUp = new Tween(Tween.TweenMode.Oneshot,Ease.BackInOut,10,true);
                    Vector2 StartPosition = Position;

                    goUp.OnUpdate = (t) => { 
                        Position = Vector2.Lerp(StartPosition, Original, t.Eased);
                        if (t.Percent > 0.4f)
                            Collidable = true;
                    };

                    goUp.OnComplete = (t) =>
                    {    
                        Tween.Position(this, Original + new Vector2(0, 32), 50, Ease.BackInOut);
                        Collidable=false;
                        Timer = 50;   
                    };
                    Add(goUp);
                }
            }
            else Collidable = false;
        }


        public void OnLetGo()
        {
            Working = !Working;
        }

        public void OnHit()
        {
            Working = !Working;
        }

        public void OnStepped()
        {
            
        }

        public int GetID()
        {
            return id;
        }
    }
}
