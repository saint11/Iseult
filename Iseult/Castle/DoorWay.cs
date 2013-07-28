using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using Monocle;
using OldSkull;

namespace Iseult
{
    public class DoorWay:PlatformLevelEntity
    {
        public string Destiny { get; private set; }
        private bool Back;
        
        protected Sprite<string> Image;

        protected List<EnemyTracker> BeingAttacked;
        private int hp = 10;

        private int uid;
        private int interacting = 0;


        public bool IsBeingAttacked { get { return hp < 10; } }

        public DoorWay(Vector2 Position, bool Back, int uid, string Destiny="")
            : base(Back? GameLevel.GAMEPLAY_LAYER:GameLevel.FRONT_GAMEPLAY_LAYER)
        {
            this.Position = Position;
            this.Back = Back;
            this.Destiny = Destiny;
            this.uid = uid;

            Tag(GameTags.Door);

            interacting = 0;
            BeingAttacked = new List<EnemyTracker>();

            if (IseultGame.Stats.HasTrigger("Door" + uid)) hp = 0;
            else
            {
                if (IseultGame.Stats.HasStats("Door" + uid))
                    hp = (int)IseultGame.Stats.GetStats("Door" + uid);
                else
                    hp = 10;
            }

        }
        public override void Added()
        {
            base.Added();
            LoadImage();
            Image.Color.A = (byte)((0.8f) * 255);

            Collider = new Hitbox(Image.Width, Image.Height);
        }
        protected virtual void LoadImage()
        {
            Image = IseultGame.SpriteData.GetSpriteString("door");
            Add(Image);

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (IsBeingAttacked) Image.Play("attacked");
            else Image.Play("closed");
        }

        public override void Update()
        {
            base.Update();

            if (interacting == 0)
            {
                if (!Back) Tween.Alpha(Image, 0.8f, 25, null);
                interacting--;
            }
            else
            {
                interacting--;
            }

            bool enemyOut = false;
            foreach (EnemyTracker tracker in BeingAttacked)
            {
                if (Calc.Chance(Calc.Random, 0.05f))
                {
                    hp--;
                    UpdateVisuals();
                    if (hp<=0)
                    {
                        Enemy enemy = new Enemy(tracker);
                        enemy.SetPosition(new Vector2(Position.X + 32, Position.Y + 32));
                        Level.Add(enemy);
                        EnemyTracker.UpdateTracker(enemy,Level.Name);
                        enemyOut = true;
                    }
                }
            }
            if (enemyOut) CheckForAttackers();
        }

        internal void OnTouched(IseultPlayer Player)
        {
            if (interacting<=0)
            {
                if (!Back) Tween.Alpha(Image, 0.3f, 25, null);
                interacting = 5;
            }
        }

        public virtual void Enter(PlatformLevelEntity Entity)
        {
            ((GameLevel)Scene).GoToLevel(Destiny,uid);
        }

        internal void CheckForAttackers()
        {
            BeingAttacked = new List<EnemyTracker>();
            foreach (EnemyTracker enemy in EnemyTracker.GetEnemiesEngaded(Destiny))
            {
                BeingAttacked.Add(enemy);
            }
        }

        internal void Barricade()
        {
            hp++;
        }
    }
}
