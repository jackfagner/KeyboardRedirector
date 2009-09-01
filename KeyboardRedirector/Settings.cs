#region Copyright (C) 2009 Nate

/* 
 *	Copyright (C) 2009 Nate
 *	http://nate.dynalias.net
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace KeyboardRedirector
{
    public class Settings
    {
        #region Static
        private static XMLFileStore<Settings> _xmlStore;
        public static void EnsureXmlStoreExists()
        {
            if (_xmlStore == null)
            {
                string filename = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"\settings.xml";
                _xmlStore = new XMLFileStore<Settings>(filename);
            }
        }
        public static void Save()
        {
            EnsureXmlStoreExists();
            _xmlStore.Save();
        }
        public static Settings Current
        {
            get
            {
                EnsureXmlStoreExists();
                return _xmlStore.Data;
            }
        }
        #endregion

        public bool MinimizeOnStart = false;
        public SettingsKeyboardList Keyboards = new SettingsKeyboardList();
        public SettingsApplicationList Applications = new SettingsApplicationList();

    }


    public class SettingsApplicationList : List<SettingsApplication>
    {
        public SettingsApplicationList()
        {
        }

        public new void Add(SettingsApplication item)
        {
            if (FindByName(item.Name) != null)
                throw new ArgumentException("An element with the same application name already exists.");
            base.Add(item);
        }

        public SettingsApplication FindByName(string name)
        {
            foreach (SettingsApplication app in this)
            {
                if (string.Equals(app.Name, name, StringComparison.CurrentCultureIgnoreCase))
                    return app;
            }
            return null;
        }
    }
    public class SettingsApplication
    {
        public string Name = "New Application";

        public string WindowTitle = "";
        public string ClassName = "";
        public string Executable = "";

        public override string ToString()
        {
            return Name;
        }
    }



    public class SettingsKeyboardList : List<SettingsKeyboard>
    {
        public SettingsKeyboard FindByDeviceName(string deviceName)
        {
            foreach (SettingsKeyboard kb in this)
            {
                if (kb.DeviceName == deviceName)
                    return kb;
            }
            return null;
        }
    }
    public class SettingsKeyboard
    {
        public string Name = "";
        public string DeviceName = "";

        public bool CaptureAllKeys = false;

        public SettingsKeyboardKeyList Keys = new SettingsKeyboardKeyList();

        public override bool Equals(object obj)
        {
            SettingsKeyboard objTyped = obj as SettingsKeyboard;
            return ((objTyped != null) && (DeviceName == objTyped.DeviceName));
        }
        public override int GetHashCode()
        {
            return DeviceName.GetHashCode();
        }
        public override string ToString()
        {
            return Name;
        }
    }



    public class SettingsKeyboardKeyList : List<SettingsKeyboardKey>
    {
        public SettingsKeyboardKey FindKey(Keys KeyCode)
        {
            foreach (SettingsKeyboardKey key in this)
            {
                if (key.Keys == KeyCode)
                    return key;
            }
            return null;
        }
    }
    public class SettingsKeyboardKey
    {
        private uint _keyCode = 0;

        public uint KeyCode
        {
            get { return _keyCode; }
            set { _keyCode = value; }
        }

        [XmlIgnore()]
        public Keys Keys
        {
            get { return (Keys)_keyCode; }
            set { _keyCode = (uint)value; }
        }

        public bool Capture = false;
        public string Name = "";

        public SettingsKeyboardKeyFocusedApplicationList FocusedApplications = new SettingsKeyboardKeyFocusedApplicationList();

        public SettingsKeyboardKey()
        {
        }
        public SettingsKeyboardKey(Keys KeyCode)
        {
            this.Keys = KeyCode;
            Name = KeyCode.ToString();
        }

        public override bool Equals(object obj)
        {
            SettingsKeyboardKey objTyped = obj as SettingsKeyboardKey;
            return ((objTyped != null) && (Keys == objTyped.Keys));
        }
        public override int GetHashCode()
        {
            return Keys.GetHashCode();
        }
        public override string ToString()
        {
            if (Name.Length == 0)
                return "0x" + KeyCode.ToString("x6");
            else
                return "0x" + KeyCode.ToString("x6") + " - " + Name;
        }
    }



    public class SettingsKeyboardKeyFocusedApplicationList : List<SettingsKeyboardKeyFocusedApplication>
    {
        public SettingsKeyboardKeyFocusedApplication FindByName(string name)
        {
            foreach (SettingsKeyboardKeyFocusedApplication app in this)
            {
                if (string.Equals(app.ApplicationName, name, StringComparison.CurrentCultureIgnoreCase))
                    return app;
            }
            return null;
        }
        public SettingsKeyboardKeyFocusedApplication FindByExecutable(string executable)
        {
            foreach (SettingsKeyboardKeyFocusedApplication app in this)
            {
                if (string.Equals(app.Application.Executable, executable, StringComparison.CurrentCultureIgnoreCase))
                    return app;
            }
            return null;
        }
    }
    public class SettingsKeyboardKeyFocusedApplication
    {
        public string ApplicationName = "";

        public SettingsKeyboardKeyActionList Actions = new SettingsKeyboardKeyActionList();

        [XmlIgnore()]
        public SettingsApplication Application
        {
            get { return Settings.Current.Applications.FindByName(ApplicationName); }
            set { ApplicationName = value.Name; }
        }
    }



    public class SettingsKeyboardKeyActionList : List<SettingsKeyboardKeyAction>
    {
    }
    public class SettingsKeyboardKeyAction
    {
        //private Dictionary<SettingsKeyboardKeyActionType, SettingsKeyboardKeyTypedAction> _actionTypes = new Dictionary<SettingsKeyboardKeyActionType, SettingsKeyboardKeyTypedAction>();

        public SettingsKeyboardKeyActionType ActionType = SettingsKeyboardKeyActionType.LaunchApplication;
        public SettingsKeyboardKeyTypedActionLaunchApplication LaunchApplication = new SettingsKeyboardKeyTypedActionLaunchApplication();
        public SettingsKeyboardKeyTypedActionKeyboard Keyboard = new SettingsKeyboardKeyTypedActionKeyboard();

        public SettingsKeyboardKeyAction()
        {
            //_actionTypes.Add(SettingsKeyboardKeyActionType.LaunchApplication, LaunchApplication);
            //_actionTypes.Add(SettingsKeyboardKeyActionType.Keyboard, Keyboard);
        }

        public SettingsKeyboardKeyTypedAction CurrentActionType
        {
            get { return GetActionType(ActionType); }
        }

        public SettingsKeyboardKeyTypedAction GetActionType(SettingsKeyboardKeyActionType actionType)
        {
            if (actionType == SettingsKeyboardKeyActionType.LaunchApplication)
                return LaunchApplication;
            if (ActionType == SettingsKeyboardKeyActionType.Keyboard)
                return Keyboard;
            return null;
        }

    }
    public enum SettingsKeyboardKeyActionType
    {
        LaunchApplication,
        Keyboard
    }
    public class SettingsKeyboardKeyTypedAction
    {
        public virtual string GetName()
        {
            return "";
        }
        public virtual string GetDetails()
        {
            return "";
        }
        public override string ToString()
        {
            return GetName() + " - " + GetDetails();
        }
    }
    public class SettingsKeyboardKeyTypedActionLaunchApplication : SettingsKeyboardKeyTypedAction
    {
        public string Command = "";
        public bool WaitForInputIdle = false;
        public bool WaitForExit = false;

        public override string GetName()
        {
            return "Launch Application";
        }
        public override string GetDetails()
        {
            return Command;
        }
    }
    public class SettingsKeyboardKeyTypedActionKeyboard : SettingsKeyboardKeyTypedAction
    {
        public bool Control = false;
        public bool Shift = false;
        public bool Alt = false;
        public ushort VirtualKeyCode = 0;
        public int RepeatCount = 1;

        [XmlIgnore()]
        public Keys VirtualKey
        {
            get { return (Keys)VirtualKeyCode; }
            set { VirtualKeyCode = (ushort)value; }
        }

        public override string GetName()
        {
            return "Keyboard";
        }
        public override string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            if (Control)
                sb.Append("Control + ");
            if (Shift)
                sb.Append("Shift + ");
            if (Alt)
                sb.Append("Alt + ");
            sb.Append(NiceKeyName.GetName(VirtualKey));
            if (RepeatCount > 1)
                sb.Append("  x" + RepeatCount.ToString());
            return sb.ToString();
        }

    }

}