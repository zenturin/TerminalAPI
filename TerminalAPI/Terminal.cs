using Sandbox.Game.EntityComponents;
using Sandbox.Graphics.GUI;
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
        /// <summary>
        /// Represents a interface for the terminal plugin
        /// data can be sent between the player terminal and the PB by supplying the flight seat they are currently occupying
        /// 
        /// 
        /// </summary>
        public class Terminal : Packet
        {
            /// <summary>
            /// The ship controller being controlled.
            /// </summary>
            public IMyShipController Controlled;

            /// <summary>
            /// Initializes a new instance of the <see cref="Terminal"/> class.
            /// </summary>
            /// <param name="controlled">The ship controller to be controlled.</param>
            public Terminal(IMyShipController controlled)
            {
                Controlled = controlled;
            }

            /// <summary>
            /// Checks for updates and then pushes changes in data IF user has not edited the terminal data.
            /// </summary>
            /// <returns>True if the terminal data has been edited by the user, False otherwise.</returns>
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

        /// <summary>
        /// Represents a packet of data with various properties and methods for manipulation.
        /// </summary>
        public class Packet
        {
            /// <summary>
            /// The data held by the terminal.
            /// </summary>
            public MyIni data = new MyIni();

            /// <summary>
            /// Initializes a new instance of the <see cref="Packet"/> class.
            /// </summary>
            public Packet()
            {
                Reset();
            }

            /// <summary>
            /// Gets or sets the command mode.
            /// Options:
            /// CMDTerminal : On screen text box that is readable
            /// CMDInput : a virtual hidden text box for use with lcd displays
            /// Custom : a custom UI controlled by customdata
            /// </summary>
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



            /// <summary>
            /// Gets the current SessionID
            /// This changes every time the player opens the terminal.
            /// </summary>
            public string SessionID
            {
                get
                {
                    return (data.Get("Setup", "SessionID").ToString());
                }
            }



            /// <summary>
            /// Gets or sets the text held in the player TextBox.
            /// </summary>
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

            /// <summary>
            /// Gets the user ID
            /// each user ID is unique to each player
            /// </summary>
            public string User
            {
                get
                {
                    return (data.Get("States", "User").ToString());
                }
            }

            /// <summary>
            /// Gets or sets the carriage index.
            /// </summary>
            public int CarriageIndex
            {
                get
                {
                    return (int)data.Get("States", "CarriageIndex").ToInt32();
                }
                set
                {
                    data.Set("States", "CarriageIndex", value);
                }
            }

            /// <summary>
            /// Gets a list containing all the keys currently pressed
            /// </summary>
            public List<string> SpecialKeys
            {
                get
                {
                    List<MyIniKey> IniKeys = new List<MyIniKey>();
                    List<string> Keys = new List<string>();
                    data.GetKeys("SpecialKeys", IniKeys);
                    foreach (var key in IniKeys)
                    {
                        Keys.Add(key.Name);
                    }
                    return Keys;
                }
            }

            /// <summary>
            /// Gets or sets the alpha value.
            /// </summary>
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

            /// <summary>
            /// Gets or sets a value indicating whether sound can be played on mouse over.
            /// </summary>
            public bool CanPlaySoundOnMouseOver
            {
                get
                {
                    return data.Get("UI", "CanPlaySoundOnMouseOver").ToString().ToLower() == "true";
                }
                set
                {
                    data.Set("UI", "CanPlaySoundOnMouseOver", value ? "True" : "False");
                }
            }

            /// <summary>
            /// Gets or sets the text scale.
            /// </summary>
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

            /// <summary>
            /// Gets or sets the visual style.
            /// Options:
            /// Debug
            /// Default
            /// Custom
            /// NoHighlight
            /// </summary>
            public String VisualStyle
            {
                get
                {
                    var value = data.Get("UI", "VisualStyle").ToString();
                    switch (value)
                    {
                        case "Debug":
                            return value;
                        case "Default":
                            return value;
                        case "Custom":
                            return value;
                        case "NoHighlight":
                            return value;
                        default:
                            return "Default";
                    }
                }
                set
                {
                    switch (value)
                    {
                        case "Debug":
                            data.Set("UI", "VisualStyle", value);
                            break;
                        case "Default":
                            data.Set("UI", "VisualStyle", value);
                            break;
                        case "Custom":
                            data.Set("UI", "VisualStyle", value);
                            break;
                        case "NoHighlight":
                            data.Set("UI", "VisualStyle", value);
                            break;
                    }
                }
            }

            /// <summary>
            /// Gets or sets the X position of the TextBox.
            /// </summary>
            public float PositionX
            {
                get
                {
                    return ((float)data.Get("UI", "PositionX").ToDecimal());
                }
                set
                {
                    data.Set("UI", "PositionX", value.ToString());
                }
            }

            /// <summary>
            /// Gets or sets the Y position of the TextBox.
            /// </summary>
            public float PositionY
            {
                get
                {
                    return ((float)data.Get("UI", "PositionY").ToDecimal());
                }
                set
                {
                    data.Set("UI", "PositionY", value.ToString());
                }
            }

            /// <summary>
            /// Gets or sets the size of the TextBox.
            /// </summary>
            public Vector2 Size
            {
                get
                {
                    Vector2 x = new Vector2();
                    string y = data.Get("UI", "Size").ToString();
                    try
                    {
                        var z = y.Split(',');
                        List<float> results = new List<float>();
                        foreach (var item in z)
                        {
                            string temp = item;
                            int index = 0;
                            if ((index = item.IndexOf("(")) != -1)
                            {
                                temp = temp.Remove(index, 1);
                            }
                            if ((index = item.IndexOf(")")) != -1)
                            {
                                temp = temp.Remove(index, 1);
                            }
                            temp = temp.Trim();
                            results.Add(float.Parse(temp));
                        }
                        x.X = results[0];
                        x.Y = results[1];
                        return x;
                    }
                    catch (Exception)
                    {
                        return new Vector2();
                    }
                }
                set
                {
                    string str = "( " + value.X + " , " + value.Y + " )";
                    data.Set("UI", "Size", str);
                }
            }

            /// <summary>
            /// Gets or sets the color mask applied to the TextBox.
            /// </summary>
            public Vector4D ColorMask
            {
                get
                {
                    Vector4D x = new Vector4D();
                    string y = data.Get("UI", "ColorMask").ToString();
                    try
                    {
                        var z = y.Split(',');
                        List<float> results = new List<float>();
                        foreach (var item in z)
                        {
                            string temp = item;
                            int index = 0;
                            if ((index = item.IndexOf("(")) != -1)
                            {
                                temp = temp.Remove(index, 1);
                            }
                            if ((index = item.IndexOf(")")) != -1)
                            {
                                temp = temp.Remove(index, 1);
                            }
                            temp.Trim();
                            results.Add(float.Parse(temp));
                        }
                        x.X = results[0];
                        x.Y = results[1];
                        x.Z = results[2];
                        x.W = results[3];
                        return x;
                    }
                    catch (Exception)
                    {
                        return new Vector4D();
                    }
                }
                set
                {
                    string str = "( " + value.X + " , " + value.Y + " , " + value.Z + " , " + value.W + " )";
                    data.Set("UI", "ColorMask", str);
                }
            }

            /// <summary>
            /// Gets or sets the border color of the TextBox.
            /// BROKEN
            /// </summary>
            public Vector4D BorderColor
            {
                get
                {
                    Vector4D x = new Vector4D();
                    string y = data.Get("UI", "BorderColor").ToString();
                    try
                    {
                        var z = y.Split(',');
                        List<float> results = new List<float>();
                        foreach (var item in z)
                        {
                            string temp = item;
                            int index = 0;
                            if ((index = item.IndexOf("(")) != -1)
                            {
                                temp = temp.Remove(index, 1);
                            }
                            if ((index = item.IndexOf(")")) != -1)
                            {
                                temp = temp.Remove(index, 1);
                            }
                            temp.Trim();
                            results.Add(float.Parse(temp));
                        }
                        x.X = results[0];
                        x.Y = results[1];
                        x.Z = results[2];
                        x.W = results[3];
                        return x;
                    }
                    catch (Exception)
                    {
                        return new Vector4D();
                    }
                }
                set
                {
                    string str = "( " + value.X + " , " + value.Y + " , " + value.Z + " , " + value.W + " )";
                    data.Set("UI", "BorderColor", str);
                }
            }

            /// <summary>
            /// Gets a list of all of the Generic Keys like WASD.
            /// </summary>
            public List<string> GeneralKeys
            {
                get
                {
                    List<MyIniKey> IniKeys = new List<MyIniKey>();
                    List<string> Keys = new List<string>();
                    data.GetKeys("GeneralKeys", IniKeys);
                    foreach (var key in IniKeys)
                    {
                        Keys.Add(key.Name);
                    }
                    return Keys;
                }
            }

            /// <summary>
            /// Gets the error count.
            /// </summary>
            public int ErrorCount
            {
                get
                {
                    return (int)data.Get("Debug", "ErrorCount").ToInt32();
                }
            }

            /// <summary>
            /// Gets the last error message.
            /// </summary>
            public string lastError
            {
                get
                {
                    return (data.Get("Debug", "Last Error").ToString());
                }
            }

            /// <summary>
            /// Clears the data.
            /// </summary>
            protected void Clear()
            {
                data.Clear();
            }

            /// <summary>
            /// Resets the data to default values.
            /// </summary>
            public void Reset()
            {
                // INI Initialization and default values
                // SETUP
                data.Set("Setup", "CMDMode", "CMDTerminal");
                data.Set("Setup", "SessionID", "NA");
                // STATES
                data.Set("States", "FirstRun", "True"); // Bool
                data.Set("States", "Text", "");
                data.Set("States", "User", "PLACEHOLDERUSER");
                data.Set("States", "CarriageIndex", "0");
                // Special Keys
                data.AddSection("SpecialKeys");
                // UI Configuration
                data.Set("UI", "Alpha", "1");// Float
                data.Set("UI", "CanPlaySoundOnMouseOver", "False"); // Bool
                data.Set("UI", "TextScale", "0.8"); // Float
                data.Set("UI", "VisualStyle", "Debug");
                data.Set("UI", "PositionX", "0.15"); // Float
                data.Set("UI", "PositionY", "0"); // Float
                data.Set("UI", "Size", "( 0.15 , 0.15 )"); // Vector2
                data.Set("UI", "ColorMask", "( 1 , 1 , 1 , 1 )"); // Vector4
                data.Set("UI", "BorderColor", "( 0 , 0 , 0 , 1 )"); // Vector4
                                                                    // General Keys
                data.AddSection("GeneralKeys");

                // Debug
                data.Set("Debug", "ErrorCount", "0"); // Int
                data.Set("Debug", "Last Error", "None");
            }

            /// <summary>
            /// Tries to parse the given XML string.
            /// </summary>
            /// <param name="xmlstring">The XML string to parse.</param>
            /// <returns>A string indicating the result of the parse operation.</returns>
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

            /// <summary>
            /// Parses the given packet and updates the data.
            /// </summary>
            /// <param name="temp">The packet to parse.</param>
            private void parser(Packet temp)
            {
                List<string> sections = new List<string>();
                temp.data.GetSections(sections);
                List<MyIniKey> keys = new List<MyIniKey>();
                List<MyIniKey> tempkeys = new List<MyIniKey>();

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
        }
    }
}
