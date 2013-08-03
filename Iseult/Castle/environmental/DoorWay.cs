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

        public List<EnemyTracker> BeingAttacked;
        private int hp = 10;

        private int uid;
        private int interacting = 0;
        private int MAX_HP = 60;

        public bool Barricated { get; private set; }

        public bool IsBeingAttacked { get { return hp < MAX_HP; } }

        public DoorWay(Vector2 Position, bool Back, int uid, string Destiny="")
            : base(GameLevel.GAMEPLAY_LAYER)
            //: base(Back? GameLevel.GAMEPLAY_LAYER:GameLevel.FRONT_GAMEPLAY_LAYER)
        {
            Back = true;
            this.Position = Position;
            this.Back = Back;
            this.Destiny = Destiny;
            this.uid = uid;

            Barricated = false;

            Tag(GameTags.Door);

            interacting = 0;
            BeingAttacked = new List<EnemyTracker>();

            if (IseultGame.Stats.HasStats("Door" + uid))
                hp = (int)IseultGame.Stats.GetStats("Door" + uid);
            else
                hp = MAX_HP;
        }
        public override void Added()
        {
            base.Added();
            LoadImage();
            //Image.Color.A = (byte)((0.8f) * 255);

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
            if (hp <= 0) Image.Play("broken");
            else if (hp < MAX_HP * 0.25f) Image.Play("breaking3");
            else if (hp < MAX_HP * 0.5f) Image.Play("breaking2");
            else if (hp < MAX_HP *0.75f) Image.Play("breaking2");
            else if (IsBeingAttacked) Image.Play("attacked");
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
            bool hpChanged = false;
            foreach (EnemyTracker tracker in BeingAttacked)
            {
                if (Calc.Chance(Calc.Random, 0.03f))
                {
                    if (!Barricated)
                    {
                        hp--;
                        hpChanged = true;
                        UpdateVisuals();
                        if (hp <= 0 && Calc.Chance(Calc.Random, 0.05f))
                        {
                            Enemy enemy = new Enemy(tracker);
                            enemy.SetPosition(new Vector2(Position.X + 32, Position.Y + 82));
                            Level.Add(enemy);
                            EnemyTracker.UpdateTracker(enemy, Level.Name);
                            enemyOut = true;
                        }
                    }
                }
            }

            if (enemyOut) CheckForAttackers();
            if (hpChanged) IseultGame.Stats.SetStats("Door" + uid.ToString(), hp);
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
            if (!Barricated && !IsBeingAttacked)
            {
                ((GameLevel)Scene).GoToLevel(Destiny, uid, PlatformerLevel.Side.Door);
            }
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
            hp += MAX_HP/2;
            if (hp > MAX_HP * 2) hp = MAX_HP * 2;
            Barricated = true;

            Image bar = new Image(IseultGame.Atlas["environment/woodBarricate"]);
            bar.Y = Image.Height / 2 + 10;
            bar.X = Image.Width / 2;
            bar.CenterOrigin();
            bar.Rotation = Calc.Random.Range(-0.2f, 0.2f);
            Add(bar);
        }
    }
}
