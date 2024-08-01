using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class Terminal : Packet
        {
            public IMyShipController Controlled;

            public Terminal(IMyShipController controlled)
            {
                Controlled = controlled;
            }

            /// <summary>
            /// Checks for updates and then pushes changes in data IF user has not edited the terminal data.
            /// </summary>
            /// <returns>True if the terminal data has been edited by the user, False otherwise.</returns>
            /// 
            public bool Update()
            {
                if (data.ToString() == Controlled.CustomData)
                {
                    return false;
                }
                string flag = TryParse(Controlled.CustomData);
                if (flag == "Changed")
                {
                    return true;
                }
                else if (flag == "Failed")
                {
                    return false;
                }
                else if (flag == "UnChanged")
                {
                    Controlled.CustomData = data.ToString();
                }
                return false;
            }



        }

        public class Packet
        {
            // Definitions

            public MyIni data = new MyIni();
            private MyIni defaults = new MyIni();

            // Initialization

            public Packet()
            {
                setDefaults();
                Reset();
            }
            // Accessors

            public string CMDMode
            {
                get
                {
                    return (data.Get("Setup", "CMDMode").ToString());
                }
                set
                {
                    data.Set("Setup", "CMDMode", value);
                }
            }

            public bool CustomUI
            {
                get
                {
                    if (data.Get("Setup", "CustomUI").ToString().ToLower() == "true")
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                }
                set
                {
                    if (value)
                    {
                        data.Set("Setup", "CustomUI", "True");
                    }
                    else
                    {
                        data.Set("Setup", "CustomUI", "False");
                    }
                }
            }

            public string SessionID
            {
                get
                {
                    return (data.Get("Setup", "SessionID").ToString());
                }
                set
                {
                    data.Set("Setup", "SessionID", value);
                }
            }

            public bool FirstRun
            {
                get
                {
                    if (data.Get("States", "FirstRun").ToString().ToLower() == "true")
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                }
                set
                {
                    if (value)
                    {
                        data.Set("States", "FirstRun", "True");
                    }
                    else
                    {
                        data.Set("States", "FirstRun", "False");
                    }
                }
            }

            public string Text
            {
                get
                {
                    return (data.Get("States", "Text").ToString());
                }
                set
                {
                    data.Set("States", "Text", value);
                }
            }

            public string User
            {
                get
                {
                    return (data.Get("States", "User").ToString());
                }
                set
                {
                    data.Set("States", "User", value);
                }
            }

            public float Alpha
            {
                get
                {
                    return ((float)data.Get("UI", "Alpha").ToDecimal());
                }
                set
                {
                    data.Set("UI", "Alpha", value.ToString());
                }
            }

            public bool CanPlaySoundOnMouseOver
            {
                get
                {
                    if (data.Get("UI", "CanPlaySoundOnMouseOver").ToString().ToLower() == "true")
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                }
                set
                {
                    if (value)
                    {
                        data.Set("UI", "CanPlaySoundOnMouseOver", "True");
                    }
                    else
                    {
                        data.Set("UI", "CanPlaySoundOnMouseOver", "False");
                    }
                }
            }

            public float TextScale
            {
                get
                {
                    return ((float)data.Get("UI", "TextScale").ToDecimal());
                }
                set
                {
                    data.Set("UI", "TextScale", value.ToString());
                }
            }

            public string VisualStyle
            {
                get
                {
                    return (data.Get("UI", "VisualStyle").ToString());
                }
                set
                {
                    data.Set("UI", "VisualStyle", value);
                }
            }

            public int ErrorCount
            {
                get
                {
                    return (int)data.Get("Debug", "ErrorCount").ToInt32();
                }
                set
                {
                    data.Set("Debug", "ErrorCount", value);
                }
            }

            public string lastError
            {
                get
                {
                    return (data.Get("Debug", "Last Error").ToString());
                }
                set
                {
                    data.Set("Debug", "Last Error", value);
                }
            }

            // public Methods

            protected void Clear()
            {
                data.Clear();
            }

            protected void Reset()
            {
                // INI Initialization and default values
                // SETUP
                data.Set("Setup", "CMDMode", "CMDTerminal");
                data.Set("Setup", "CustomUI", "False");// Bool
                data.Set("Setup", "SessionID", "NA");
                // STATES
                data.Set("States", "FirstRun", "True"); // Bool
                data.Set("States", "Text", "PLACEHOLDERTEXT");
                data.Set("States", "User", "PLACEHOLDERUSER");
                // UI Configuration
                data.Set("UI", "Alpha", "1");// Float
                data.Set("UI", "CanPlaySoundOnMouseOver", "False"); // Bool
                data.Set("UI", "TextScale", "0.8f"); // Float
                data.Set("UI", "VisualStyle", "Debug");
                // Debug
                data.Set("Debug", "ErrorCount", "0"); // Int
                data.Set("Debug", "Last Error", "None");
            }

            protected string TryParse(String xmlstring)
            {

                Packet temp = new Packet();
                if (temp.data.TryParse(xmlstring))
                {
                    parser(temp);
                    if (temp.data == data)
                    {
                        return "UnChanged";
                    }
                    else
                    {
                        return "Changed";
                    }
                }
                else
                {
                    return "Failed";
                }

            }

            protected void flagError(string error)
            {
                lastError = error;
                ErrorCount++;
            }


            // private Methods

            private void parser(Packet temp)
            {
                List<string> sections = new List<string>();
                temp.data.GetSections(sections);
                List<MyIniKey> keys = new List<MyIniKey>();
                List<MyIniKey> tempkeys = new List<MyIniKey>();
                List<string> keynames = new List<string>();

                foreach (var section in sections)
                {
                    data.GetKeys(section, tempkeys);
                    keys.AddList(tempkeys);
                }
                foreach (var key in keys)
                {
                    string value = temp.data.Get(key.Section, key.Name).ToString();
                    if (data.Get(key.Section, key.Name).ToString() != value)
                    {
                        data.Set(key.Section, key.Name, value);
                    }
                }

            }

            private void setDefaults()
            {
                // INI Initialization and default values
                // SETUP
                data.Set("Setup", "CMDMode", "CMDTerminal");
                data.Set("Setup", "CustomUI", "False");// Bool
                data.Set("Setup", "SessionID", "NA");
                // STATES
                data.Set("States", "FirstRun", "True"); // Bool
                data.Set("States", "Text", "PLACEHOLDERTEXT");
                data.Set("States", "User", "PLACEHOLDERUSER");
                // UI Configuration
                data.Set("UI", "Alpha", "1");// Float
                data.Set("UI", "CanPlaySoundOnMouseOver", "False"); // Bool
                data.Set("UI", "TextScale", "0.8f"); // Float
                data.Set("UI", "VisualStyle", "Debug");
                // Debug
                data.Set("Debug", "ErrorCount", "0"); // Int
                data.Set("Debug", "Last Error", "None");
            }
        }
    }
}
