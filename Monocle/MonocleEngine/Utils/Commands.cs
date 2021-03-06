﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Monocle
{
    public class Commands
    {
        private const int UNDERSCORE_TIME = 30;
        private const int REPEAT_DELAY = 30;
        private const int REPEAT_EVERY = 2;
        public delegate void CommandCallback(string[] args);

        static private Commands Instance;

        private KeyboardState oldState;
        private KeyboardState currentState;

        private string currentText = "";        
        private List<string> drawCommands;

        private bool underscore;
        private int underscoreCounter;

        private List<string> commandHistory;
        private int seekIndex = -1;

        private int tabIndex = -1;
        private string tabSearch;

        private int repeatCounter = 0;
        private Keys? repeatKey = null;

        private Dictionary<string, CommandCallback> commands;
        private List<string> sorted;

        public bool Open;
        public float Opacity = .8f;

        public Commands()
        {
            Instance = this;

            commandHistory = new List<string>();
            drawCommands = new List<string>();
            commands = new Dictionary<string, CommandCallback>();
            sorted = new List<string>();

            RegisterCommand("clear", (string[] args) => { Clear(); });
            RegisterCommand("exit", (string[] args) => { Engine.Instance.Exit(); });
            RegisterCommand("count", (string[] args) => { if (Engine.Instance.Scene != null) Log("Entity Count: " + Engine.Instance.Scene.EntityCount); });
            RegisterCommand("tags", (string[] args) =>
            {
                if (Engine.Instance.Scene != null)
                {
                    string[] str = Engine.Instance.Scene.DumpTagInfo().Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in str)
                        Log(s);
                }
            });
            RegisterCommand("help", (string[] args) =>
            {
                Log("Commands: " + string.Join(", ", sorted.ToArray()));
            });
            RegisterCommand("opacity", (string[] args) =>
            {
                if (args.Length == 0)
                    Log("Provide a float (0-1.0) to set console opacity (eg: 'opacity 0.8').");
                else
                    Opacity = ParseFloat(args, 0, .8f);
            });
            RegisterCommand("memory", (string[] args) =>
            {
                Log((GC.GetTotalMemory(true) / 1048576f).ToString("F") + " MB");
            });
            RegisterCommand("fullscreen", (string[] args) =>
            {
                if (Engine.Instance.Screen.IsFullscreen)
                    Engine.Instance.Screen.DisableFullscreen();
                else
                    Engine.Instance.Screen.EnableFullscreen(Screen.FullscreenMode.LargestIntegerScale);
            });
        }

        private void EnterCommand()
        {
            string[] data = currentText.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (commandHistory.Count == 0 || commandHistory[0] != currentText)
                commandHistory.Insert(0, currentText);
            drawCommands.Insert(0, ">" + currentText);
            currentText = "";
            seekIndex = -1;

            string[] args = new string[data.Length - 1];
            for (int i = 1; i < data.Length; i++)
                args[i - 1] = data[i];
            ExecuteCommand(data[0].ToLower(), args);
        }

        public void RegisterCommand(string command, CommandCallback action)
        {
            command = command.ToLower();

            if (action == null)
            {
                if (commands.ContainsKey(command))
                {
                    commands.Remove(command);
                    sorted.Remove(command);
                }
            }
            else
            {
                commands[command] = action;
                if (!sorted.Contains(command))
                {
                    int i;
                    for (i = 0; i < sorted.Count; i++)
                        if (sorted[i].CompareTo(command) > 0)
                            break;
                    sorted.Insert(i, command);
                }
            }
        }

        public void ExecuteCommand(string command, params string[] args)
        {
            if (commands.ContainsKey(command))
                commands[command](args);
            else
                Log("Command '" + command + "' not found! Type 'help' to see all commands.");
        }

        public void Clear()
        {
            drawCommands.Clear();
        }

        public void Log(string str)
        {
            //Split the string if you overlow horizontally
            int maxWidth = Engine.Instance.Window.ClientBounds.Width - 40;
            while (Draw.DefaultFont.MeasureString(str).X > maxWidth)
            {
                int split = -1;
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == ' ')
                    {
                        if (Draw.DefaultFont.MeasureString(str.Substring(0, i)).X <= maxWidth)
                            split = i;
                        else
                            break;
                    }
                }

                if (split == -1)
                    break;

                drawCommands.Insert(0, str.Substring(0, split));
                str = str.Substring(split + 1);
            }

            drawCommands.Insert(0, str);

            //Don't overflow top of window
            int maxCommands = (Engine.Instance.Window.ClientBounds.Height - 100) / 30;
            while (drawCommands.Count > maxCommands)
                drawCommands.RemoveAt(drawCommands.Count - 1);
        }

        static public void Trace(Object obj)
        {
            Instance.Log(obj.ToString());
        }

        internal void UpdateClosed()
        {
            if (Input.Pressed(Microsoft.Xna.Framework.Input.Keys.OemTilde))
            {
                Open = true;
                currentState = Input.CurrentKeyboardState;
            }
        }

        internal void UpdateOpen()
        {
            oldState = currentState;
            currentState = Keyboard.GetState();

            underscoreCounter++;
            if (underscoreCounter == UNDERSCORE_TIME)
            {
                underscoreCounter = 0;
                underscore = !underscore;
            }

            if (repeatKey.HasValue)
            {
                if (currentState[repeatKey.Value] == KeyState.Down)
                {
                    if (repeatCounter < REPEAT_DELAY)
                        repeatCounter++;
                    else
                    {
                        if (repeatCounter == REPEAT_DELAY)
                        {
                            repeatCounter++;
                            HandleKey(repeatKey.Value);
                        }
                        else
                        {
                            repeatCounter++;
                            if (repeatCounter == REPEAT_DELAY + REPEAT_EVERY)
                                repeatCounter = REPEAT_DELAY;
                        }
                    }
                }
                else
                    repeatKey = null;
            }

            foreach (Keys key in currentState.GetPressedKeys())
            {
                if (oldState[key] == KeyState.Up)
                {
                    HandleKey(key);
                    break;
                }
            }
        }

        private void HandleKey(Keys key)
        {
            if (key != Keys.Tab && key != Keys.LeftShift && key != Keys.RightShift && key != Keys.RightAlt && key != Keys.LeftAlt && key != Keys.RightControl && key != Keys.LeftControl)
                tabIndex = -1;

            if (key != Keys.OemTilde && key != Keys.Enter && repeatKey != key)
            {
                repeatKey = key;
                repeatCounter = 0;
            }

            switch (key)
            {
                default:
                    if (key.ToString().Length == 1)
                    {
                        if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                            currentText += key.ToString();
                        else
                            currentText += key.ToString().ToLower();
                    }
                    break;

                case (Keys.D1):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '!';
                    else
                        currentText += '1';
                    break;
                case (Keys.D2):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '@';
                    else
                        currentText += '2';
                    break;
                case (Keys.D3):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '#';
                    else
                        currentText += '3';
                    break;
                case (Keys.D4):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '$';
                    else
                        currentText += '4';
                    break;
                case (Keys.D5):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '%';
                    else
                        currentText += '5';
                    break;
                case (Keys.D6):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '^';
                    else
                        currentText += '6';
                    break;
                case (Keys.D7):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '&';
                    else
                        currentText += '7';
                    break;
                case (Keys.D8):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '*';
                    else
                        currentText += '8';
                    break;
                case (Keys.D9):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '(';
                    else
                        currentText += '9';
                    break;
                case (Keys.D0):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += ')';
                    else
                        currentText += '0';
                    break;
                case (Keys.OemComma):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '<';
                    else
                        currentText += ',';
                    break;
                case (Keys.OemPeriod):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '>';
                    else
                        currentText += '.';
                    break;
                case (Keys.OemQuestion):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '?';
                    else
                        currentText += '/';
                    break;
                case (Keys.OemSemicolon):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += ':';
                    else
                        currentText += ';';
                    break;
                case (Keys.OemQuotes):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '"';
                    else
                        currentText += '\'';
                    break;
                case (Keys.OemBackslash):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '|';
                    else
                        currentText += '\\';
                    break;
                case (Keys.OemOpenBrackets):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '{';
                    else
                        currentText += '[';
                    break;
                case (Keys.OemCloseBrackets):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '}';
                    else
                        currentText += ']';
                    break;
                case (Keys.OemMinus):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '_';
                    else
                        currentText += '-';
                    break;
                case (Keys.OemPlus):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                        currentText += '+';
                    else
                        currentText += '=';
                    break;

                case (Keys.Space):
                    currentText += " ";
                    break;
                case (Keys.Back):
                    if (currentText.Length > 0)
                        currentText = currentText.Substring(0, currentText.Length - 1);
                    break;
                case (Keys.Delete):
                    currentText = "";
                    break;

                case (Keys.Up):
                    if (seekIndex < commandHistory.Count - 1)
                    {
                        seekIndex++;
                        currentText = string.Join(" ", commandHistory[seekIndex]);
                    }
                    break;
                case (Keys.Down):
                    if (seekIndex > -1)
                    {
                        seekIndex--;
                        if (seekIndex == -1)
                            currentText = "";
                        else
                            currentText = string.Join(" ", commandHistory[seekIndex]);
                    }
                    break;

                case (Keys.Tab):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (tabIndex == -1)
                        {
                            tabSearch = currentText;
                            FindLastTab();
                        }
                        else
                        {
                            tabIndex--;
                            if (tabIndex < 0 || (tabSearch != "" && sorted[tabIndex].IndexOf(tabSearch) != 0))
                                FindLastTab();
                        }
                    }
                    else
                    {
                        if (tabIndex == -1)
                        {
                            tabSearch = currentText;
                            FindFirstTab();
                        }
                        else
                        {
                            tabIndex++;
                            if (tabIndex >= sorted.Count || (tabSearch != "" && sorted[tabIndex].IndexOf(tabSearch) != 0))
                                FindFirstTab();
                        }
                    }
                    if (tabIndex != -1)
                        currentText = sorted[tabIndex];
                    break;

                case (Keys.Enter):
                    if (currentText.Length > 0)
                        EnterCommand();
                    break;

                case (Keys.OemTilde):
                    Open = false;
                    break;
            }
        }

        private void FindFirstTab()
        {
            for (int i = 0; i < sorted.Count; i++)
            {
                if (tabSearch == "" || sorted[i].IndexOf(tabSearch) == 0)
                {
                    tabIndex = i;
                    break;
                }
            }
        }

        private void FindLastTab()
        {
            for (int i = 0; i < sorted.Count; i++)
                if (tabSearch == "" || sorted[i].IndexOf(tabSearch) == 0)
                    tabIndex = i;
        }

        internal void Render()
        {
            int screenWidth = (int)(Engine.Instance.Screen.Width * Engine.Instance.Screen.Scale);
            int screenHeight = (int)(Engine.Instance.Screen.Height * Engine.Instance.Screen.Scale);

            Draw.SpriteBatch.Begin();

            Draw.Rect(10, screenHeight - 50, screenWidth - 20, 40, Color.Black * Opacity);
            if (underscore)
                Draw.SpriteBatch.DrawString(Draw.DefaultFont, ">" + currentText + "_", new Vector2(20, screenHeight - 42), Color.White);
            else
                Draw.SpriteBatch.DrawString(Draw.DefaultFont, ">" + currentText, new Vector2(20, screenHeight - 42), Color.White);

            if (drawCommands.Count > 0)
            {
                int height = 10 + (30 * drawCommands.Count);
                Draw.Rect(10, screenHeight - height - 60, screenWidth - 20, height, Color.Black * Opacity);
                for (int i = 0; i < drawCommands.Count; i++)
                    Draw.SpriteBatch.DrawString(Draw.DefaultFont, drawCommands[i], new Vector2(20, screenHeight - 92 - (30 * i)), drawCommands[i].IndexOf(">") == 0 ? Color.Yellow : Color.White);
            }

            Draw.SpriteBatch.End();
        }

        static public bool ParseBool(string[] args, int argumentNum, bool defaultValue = false)
        {
            try
            {
                return !(args[argumentNum] == "0" || args[argumentNum].ToLower() == "false" || args[argumentNum].ToLower() == "f");
            }
            catch
            {
                return defaultValue;
            }
        }

        static public int ParseInt(string[] args, int argumentNum, int defaultValue = 0)
        {
            try
            {
                return Convert.ToInt32(args[argumentNum]);
            }
            catch
            {
                return defaultValue;
            }
        }

        static public float ParseFloat(string[] args, int argumentNum, float defaultValue = 0)
        {
            try
            {
                return Convert.ToSingle(args[argumentNum]);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}

