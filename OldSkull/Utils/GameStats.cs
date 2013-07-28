using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OldSkull.Utils
{
    public class GameStats
    {
        static public GameStats Instance { get; private set; }

        private List<string> Triggers;
        private Dictionary<string, float> Stats;

        public static GameStats Init()
        {
            GameStats instance = new GameStats();
            return instance;
        }

        public GameStats()
        {
            Instance = this;
            Triggers = new List<string>();
            Stats = new Dictionary<string, float>();
        }


        public void SetStats(string name,float value)
        {
            Stats[name] = value;
        }

        public float AddStats(string name, float value)
        {
            if (!Stats.Keys.Contains(name)) Stats[name] = 0;
            return Stats[name] += value;
        }

        public float GetStats(string name)
        {
            return !Stats.Keys.Contains(name) ? 0 : Stats[name];
        }

        public void SetTrigger(string trigger)
        {
            if (!HasTrigger(trigger))
            {
                if (Triggers == null) Triggers = new List<string>();
                Triggers.Add(trigger);
            }
        }

        public bool HasTrigger(string trigger)
        {
            if (Triggers == null) return false;
            foreach (string t in Triggers)
            {
                if (t == trigger) return true;
            }

            return false;
        }


        public bool HasStats(string name)
        {
            return Stats.Keys.Contains(name);
        }
    }
}
