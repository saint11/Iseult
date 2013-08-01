using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace OldSkull
{
    using Input = Monocle.Input;

    public abstract class KeyboardInput
    {
        public static float xAxis { get; private set; }
        public static float yAxis { get; private set; }

        public struct KeyAction
        {
            public String name;
            public List<Keys> key;

            public Boolean check;
            public Boolean pressed;
        }

        private static List<KeyAction> keyList = new List<KeyAction>();
        public static bool Active=true;
        private static bool Any = false;

        public static Boolean checkInput(String name)
        {
            foreach (KeyAction k in keyList)
            {
                if (k.name == name)
                {
                    return k.check;
                }
            }
            return false;
        }
        public static Boolean pressedInput(String name)
        {
            foreach (KeyAction k in keyList)
            {
                if (k.name == name)
                {
                    return k.pressed;
                }
            }
            return false;
        }
        public static void Add(String name, Keys key)
        {
            //TODO: Check for existing key.
            KeyAction newKey = new KeyAction();
            newKey.name = name;
            newKey.key = new List<Keys>();
            newKey.key.Add(key);
            keyList.Add(newKey);
        }

        public static void Add(String name, Keys[] key)
        {
            //TODO: Check for existing key.
            KeyAction newKey = new KeyAction();
            newKey.name = name;
            newKey.key = new List<Keys>();
            foreach (Keys k in key)
            {
                newKey.key.Add(k);
            }
            
            keyList.Add(newKey);
        }


        public static void AddAxis(Keys up = Keys.Up, Keys down = Keys.Down, Keys left = Keys.Left, Keys right = Keys.Right)
        {
            KeyAction newKey;

            newKey = new KeyAction();
            newKey.name = "up";
            newKey.key = new List<Keys>();
            newKey.key.Add(up);
            keyList.Add(newKey);
            
            newKey = new KeyAction();
            newKey.name = "down";
            newKey.key = new List<Keys>();
            newKey.key.Add(down);
            keyList.Add(newKey);

            newKey = new KeyAction();
            newKey.name = "left";
            newKey.key = new List<Keys>();
            newKey.key.Add(left);
            keyList.Add(newKey);

            newKey = new KeyAction();
            newKey.name = "right";
            newKey.key = new List<Keys>();
            newKey.key.Add(right);
            keyList.Add(newKey);

        }


        public static void Update()
        {
            if (Active)
            {
                Any = false;
                for (int i = 0; i < keyList.Count; i++) // Loop through List with for
                {

                    KeyAction k = keyList[i];
                    k.check = false;
                    k.pressed = false;
                    foreach (Keys cKey in k.key)
                    {
                        k.check = k.check || Input.Check(cKey);
                        k.pressed = k.pressed || Input.Pressed(cKey);
                    }
                    
                    if (k.pressed) Any = true;
                    keyList[i] = k;
                }
                GetAxis();
            }
            else
            {
                for (int i = 0; i < keyList.Count; i++) // Loop through List with for
                {
                    KeyAction k = keyList[i];
                    k.check = false;
                    k.pressed = false;

                    keyList[i] = k;
                }
                xAxis = 0;
                yAxis = 0;
            }
        }

        private static void GetAxis()
        {
            yAxis = 0;
            if (checkInput("up"))
                yAxis -= 1;
            if (checkInput("down"))
                yAxis += 1;

            xAxis = 0;
            if (checkInput("right"))
                xAxis += 1;
            if (checkInput("left"))
                xAxis -= 1;
        }

        public static void InitDefaultInput()
        {
            KeyboardInput.Add("accept", Microsoft.Xna.Framework.Input.Keys.Z);
            KeyboardInput.AddAxis();
        }

        public static bool pressedAny()
        {
            return Any;
        }
    }
}
