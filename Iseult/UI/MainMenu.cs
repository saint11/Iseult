using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull;
using OldSkull.Menu;
using Microsoft.Xna.Framework;
using Monocle;
using OldSkull.GameLevel;

namespace Iseult
{
    class MainMenu: OldSkull.Menu.MainMenu
    {
        private Entity title = new Entity(1);
        SelectorMenu menu;

        public override void Begin()
        {
            base.Begin();
            Engine.Instance.Screen.ClearColor = Color.Black;

            //Tittle Animation
            Image titleImage = new Image(OldSkullGame.Atlas["title"]);
            title.Add(titleImage);
            title.X = Engine.Instance.Screen.Width / 2 - titleImage.Width / 2;
            title.Y = -titleImage.Height;
            title.Depth = 10;
            Add(title);
            Tween.Position(title, new Vector2(title.X, 30), 30, Ease.BackOut, Tween.TweenMode.Oneshot);

            menu = new SelectorMenu(new string[] { "menu/startgame", "menu/credits", "menu/exit" }, new Action<MenuButton>[] { newGame, credits, exitGame }, null, SelectorMenuEffects.ColorSwap(Color.AntiqueWhite, Color.Red), true, 1);
            menu.X = Engine.Instance.Screen.Width / 2;
            Add(menu);
        }
        public void newGame(MenuButton Mb)
        {
            StartNewGame(true);
        }

        public static void StartNewGame(bool ResetAll)
        {
            IseultPlayer.AliveTime = 0;
            PlatformerLevelLoader loader = PlatformerLevelLoader.load("OuterWall");
            GameLevel level = new GameLevel(loader, PlatformerLevel.Side.Secret);
            
            Engine.Instance.Scene = level;
        }

        public void exitGame(MenuButton Mb)
        {
            Engine.Instance.Exit();
        }

        public void credits(MenuButton Mb)
        {

        }

        public override void Render()
        {
            menu.Y = title.Y + 300;
            base.Render();
        }
    }
}
