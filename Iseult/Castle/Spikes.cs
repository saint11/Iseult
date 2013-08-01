using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using Monocle;
using Microsoft.Xna.Framework;

namespace Iseult.Castle
{
    public class Spikes:PlatformLevelEntity
    {
        private int Timer=0;
        private Vector2 Original;
        private int id;

        public Spikes(Vector2 Position, int id)
            : base(0)
        {
            this.id = id;
            this.Position = Original = Position;
            this.Position.Y += 32;
            Image Image = new Image(IseultGame.Atlas["environment/spikes"]);
            Add(Image);

        }

        public override void Update()
        {
            base.Update();

            Timer--;
            if (Timer == 0)
            {
                Tween.Position(this, Original, 30, Ease.BackIn).OnComplete = (t) => {
                        Tween.Position(this, Original + new Vector2(0,32), 50,Ease.BackInOut);
                    };
            }
        }

    }
}
