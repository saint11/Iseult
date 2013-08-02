using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using Monocle;

namespace Iseult.Castle
{
    public class Altar : PlatformLevelEntity
    {
        public int ID { get; private set; }
        public bool Prayed { get; private set; }
        private Image image;

        public Altar(Vector2 Position, int ID)
            : base(0)
        {
            this.Position = Position;
            
            this.ID = ID;
            Prayed = false;

            image = new Image(IseultGame.Atlas["environment/altar"]);
            image.Origin.X = image.Width / 2;
            Position.X -= image.Width / 2;
            Add(image);
            Collider = new Hitbox(image.Width, image.Height, -image.Height/2);

            Depth = 100;
        }

        public override void Update()
        {
            base.Update();
            if (CollideCheck(new GameTags[] { GameTags.Npc }) && !Prayed)
            {
                Prayed = true;

                UserData.LastestRoomName = Level.FileName;
                UserData.LastestCheckpointID = ID;

                Image Fx = new Image(IseultGame.Atlas["environment/altarGlow"]);
                Fx.Color.A = 100;
                Vector2 Position = this.Position - new Vector2(12+image.Width/2, 20);
                Level.AddEffect(Position, Fx);
                Tween.Alpha(Fx,0.2f,120,Ease.CubeInOut,Tween.TweenMode.YoyoLooping);
            }

        }
    }
}
