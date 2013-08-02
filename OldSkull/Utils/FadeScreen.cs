using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.Utils
{
    public class FadeScreen:Entity
    {
        private Action<FadeScreen> onComplete;



        static public FadeScreen FadeOut(Scene Scene, int Duration, int layer, int Depth)
        {
            FadeScreen Instance = new FadeScreen(Duration, true, layer, Depth, 0, 1);
            Scene.Add(Instance);
            return Instance;
        }
        static public FadeScreen FadeIn(Scene Scene, int Duration, int layer, int Depth)
        {
            FadeScreen Instance = new FadeScreen(Duration, true, layer, Depth, 1, 0);
            Scene.Add(Instance);
            return Instance;
        }

        private FadeScreen(int Duration, bool Start, int layer,int Depth,float startA, float endA)
            : base(layer)
        {
            Image black = new Image(new Texture(Engine.Instance.Screen.Width, Engine.Instance.Screen.Height, Color.Black));
            Add(black);

            black.Color.A = (byte)(black.Color.A*startA);
            Tween.Alpha(black, endA, Duration, null).OnComplete = CompleteFade;

            this.Depth = Depth;
        }

        private void CompleteFade(Tween t)
        {
            if (onComplete!=null) onComplete(this);
            RemoveSelf();
        }

        public FadeScreen OnComplete(Action<FadeScreen> value) { onComplete = value; return this; }
    }
}
