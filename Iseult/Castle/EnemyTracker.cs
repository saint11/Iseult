using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using Monocle;

namespace Iseult
{
    public class EnemyTracker
    {
        public static List<EnemyTracker> Enemies;

        public string CurrentOn;
        public int DoorUid;
        public Vector2 LastSeen;
        public string Uid { get; private set; }
        public int TimeForDoor = 0;
        private bool Engaded=false;
        public int Hp;

        public static void Init()
        {
            Enemies = new List<EnemyTracker>();
        }

        public EnemyTracker(string Name, XmlElement e)
        {
            Uid = Name + e.Attr("id");
            Hp = 3;
            LastSeen = new Vector2(e.AttrFloat("x"), e.AttrFloat("y"));
            CurrentOn = Name;
        }

        public static void UpdateTracker(Enemy enemy, string Level, IseultPlayer Player = null)
        {
            EnemyTracker tracker = findEnemy(enemy.uid);
            tracker.LastSeen = enemy.Position;
            tracker.CurrentOn = Level;
            if (Player!=null)
                tracker.TimeForDoor = (int)(Vector2.Distance(enemy.Position,Player.Position)*0.5f);
            tracker.Engaded = false;
            tracker.Hp = enemy.Hp;
        }

        public static bool HasEnemy(string Name, XmlElement e)
        {
            string Uid = Name + e.Attr("id");
            foreach (EnemyTracker track in Enemies)
            {
                if (track.Uid == Uid) return true;
            }
            return false;
        }

        public static void RegisterEnemy(string Name, XmlElement e)
        {
            Enemies.Add(new EnemyTracker(Name, e));
        }


        public static EnemyTracker findEnemy(string Name, XmlElement e)
        {
            string Uid = Name + e.Attr("id");
            foreach (EnemyTracker track in Enemies)
            {
                if (track.Uid == Uid) return track;
            }
            return null;
        }

        public static EnemyTracker findEnemy(string Uid)
        {
            foreach (EnemyTracker track in Enemies)
            {
                if (track.Uid == Uid) return track;
            }
            return null;
        }

        internal static List<EnemyTracker> HasEnemyHere(string Name)
        {
            List<EnemyTracker> list = new List<EnemyTracker>();
            foreach (EnemyTracker track in Enemies) if (track.CurrentOn == Name) list.Add(track);
            return list;
        }

        public static void UpdateOffScreen(Action OnEngaged, string CurrentLevel)
        {
            foreach (EnemyTracker track in Enemies)
            {
                if (track.CurrentOn != CurrentLevel)
                {
                    track.TimeForDoor--;
                    if (track.TimeForDoor == 0)
                    {
                        track.Engaded = true;
                        OnEngaged();
                    }
                }
            }
        }

        internal static List<EnemyTracker> GetEnemiesEngaded(string from)
        {
            List<EnemyTracker> list = new List<EnemyTracker>();
            foreach (EnemyTracker track in Enemies) if (track.Engaded && track.CurrentOn == from.ToUpper()) list.Add(track);
            return list;
        }

        internal static void KillEnemy(Enemy enemy)
        {
            EnemyTracker tracker = findEnemy(enemy.uid);
            tracker.Hp = 0;
        }
    }
}
