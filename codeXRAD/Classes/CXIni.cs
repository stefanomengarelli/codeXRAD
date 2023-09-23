/*  ------------------------------------------------------------------------
 *  
 *  File:       CXIni.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD INI configuration file management class.
 *
 *  ------------------------------------------------------------------------
 */

using System.Text;

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD INI configuration file management class.</summary>
    public class CXIni
    {

        /* */

        #region Declarations

        /*  --------------------------------------------------------------------
         *  Declarations
         *  --------------------------------------------------------------------
         */

        /// <summary>Last section found index.</summary>
        private int lastSectionFound = -1;

        #endregion

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Class constructor.</summary>
        public CXIni()
        {
            Clear();
        }

        /// <summary>Class constructor.</summary>
        public CXIni(string _FileName)
        {
            Load(_FileName);
        }

        /// <summary>Class constructor.</summary>
        public CXIni(string _FileName, string _Password)
        {
            Load(_FileName, _Password);
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get or set INI file changed flag.</summary>
        public bool Changed { get; set; } = false;

        /// <summary>Get or set INI text enconding.</summary>
        public Encoding TextEncoding { get; set; } = CX.TextEncoding;

        /// <summary>Get configuration INI file lines collection.</summary>
        public List<string> Lines { get; private set; } = new List<string>();

        /// <summary>Get or set INI file encoding password.</summary>
        public string Password { get; set; } = "";

        /// <summary>Get or set INI file full path.</summary>
        public string Path { get; set; } = "";

        /// <summary>Get or set write default flag.</summary>
        public bool WriteDefault { get; set; } = false;

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Clear item.</summary>
        public void Clear()
        {
            Changed = false;
            Lines.Clear();
            Path = "";
        }

        /// <summary>Return index of section line with key. Return -1 if not found.</summary>
        public int Find(string _Section, string _Key)
        {
            int r = -1, i = Lines.Count, j, lastKey = -1;
            string l;
            _Section = _Section.Trim().ToLower();
            _Key = _Key.Trim().ToLower();
            lastSectionFound = -1;
            if ((_Section.Length > 0) && (_Key.Length > 0))
            {
                while ((r < 0) && (i > 0))
                {
                    i--;
                    l = Lines[i].Trim();
                    if (l.Length > 2)
                    {
                        if ((l[0] == '[') && (l[l.Length - 1] == ']'))
                        {
                            if (_Section == l.Substring(1, l.Length - 2).Trim().ToLower())
                            {
                                lastSectionFound = i;
                                if (lastKey > -1) r = lastKey;
                            }
                            else lastKey = -1;
                        }
                        else
                        {
                            j = l.IndexOf('=');
                            if (j > 0)
                            {
                                if (_Key == l.Substring(0, j).Trim().ToLower()) lastKey = i;
                            }
                        }
                    }
                }
            }
            return r;
        }

        /// <summary>Get INI file values and content from string.</summary>
        public void FromString(string _String)
        {
            if (!CX.Empty(Password)) _String = CX.FromHexMask(_String, Password);
            else if (CX.IsHexMask(_String)) _String = "";
            CX.StrList(_String, Lines, false);
            Changed = true;
        }

        /// <summary>Load INI file values and content from file. A password for decoding 
        /// encrypted INI files can be specified. Return true if succeed.</summary>
        public bool Load(string _FileName, string _Password = null)
        {
            _FileName = _FileName.Trim();
            if (CX.Empty(_FileName)) _FileName = CX.Combine(CX.ApplicationPath, CX.ExecutableName, "ini");
            if (!CX.Empty(_Password)) Password = _Password;
            this.Clear();
            CX.Error();
            if (CX.FileExists(_FileName)) FromString(CX.LoadString(_FileName, TextEncoding, CX.FileRetries));
            Path = _FileName;
            Changed = false;
            return !CX.IsError;
        }

        /// <summary>Return string value of key at section of file INI specified. Return default value if not found.</summary>
        public static string QuickRead(string _FileName, string _Section, string _Key, string _Default)
        {
            string r;
            CXIni ini = new CXIni(_FileName);
            ini.WriteDefault = true;
            r = ini.ReadString(_Section, _Key, _Default);
            ini.Save();
            return r;
        }

        /// <summary>Write and save string value of key at section of file INI specified. Return true if succeed.</summary>
        public static bool QuickWrite(string _FileName, string _Section, string _Key, string _Value)
        {
            CXIni ini = new CXIni(_FileName);
            ini.WriteString(_Section, _Key, _Value);
            return ini.Save();
        }

        /// <summary>Read string value of key at section. Return default value if not found.</summary>
        public string ReadString(string _Section, string _Key, string _Default)
        {
            int i = Find(_Section, _Key);
            if (i < 0)
            {
                if (WriteDefault) WriteString(_Section, _Key, _Default);
                return _Default;
            }
            else return CX.Unquote2(CX.After(Lines[i], "=").Trim());
        }

        /// <summary>Read boolean value of key at section. Return default value if not found.</summary>
        public bool ReadBool(string _Section, string _Key, bool _Default)
        {
            return CX.Bool(ReadString(_Section, _Key, CX.Bool(_Default)));
        }

        /// <summary>Read date-time value of key at section. Return default value if not found.</summary>
        public DateTime ReadDateTime(string _Section, string _Key, DateTime _Default)
        {
            return CX.Date(ReadString(_Section, _Key, 
                CX.Str(_Default, CXDateFormat.iso8601, true)), 
                CXDateFormat.iso8601, true);
        }

        /// <summary>Read double value of key at section. Return default value if not found.</summary>
        public double ReadDouble(string _Section, string _Key, double _Default)
        {
            return CX.ToDouble(ReadString(_Section, _Key,
                CX.Str(_Default).Replace(CX.DecimalSeparator, '.')
                ).Replace('.', CX.DecimalSeparator));
        }

        /// <summary>Read hex masked value of key at section. Return default value if not found.</summary>
        public string ReadHexMask(string _Section, string _Key, string _Default)
        {
            string r = ReadString(_Section, _Key, _Default), p = Password;
            if (CX.Empty(p)) p = CX.InternalPassword;
            if (CX.IsHexMask(r)) return CX.FromHexMask(r, p);
            else 
            {
                WriteHexMask(_Section, _Key, r);
                return r;
            }
        }

        /// <summary>Read integer value of key at section. Return default value if not found.</summary>
        public int ReadInteger(string _Section, string _Key, int _Default)
        {
            return CX.ToInt(ReadString(_Section, _Key, _Default.ToString()));
        }

        /// <summary>Save INI file values and content to file.</summary>
        public bool Save(string _FileName)
        {
            if (CX.SaveString(_FileName, this.ToString(), TextEncoding, CX.FileRetries))
            {
                Changed = false;
                return true;
            }
            else return false;
        }

        /// <summary>Save INI file values and content to file.</summary>
        public bool Save()
        {
            if (Changed && (Path.Trim().Length > 0)) return Save(Path);
            else return !Changed;
        }

        /// <summary>Return INI file values and content as string.</summary>
        public override string ToString()
        {
            if (Password.Trim().Length > 0) return CX.ToHexMask(CX.Str(Lines, false), Password.Trim());
            else return CX.Str(Lines, false);
        }

        /// <summary>Write string value of key at section. Return line index if success otherwise -1.</summary>
        public int WriteString(string _Section, string _Key, string _Value)
        {
            int r = Find(_Section, _Key), i;
            string ln;
            if (r < 0)
            {
                if (lastSectionFound < 0)
                {
                    if (Lines.Count > 0)
                    {
                        if (Lines[Lines.Count - 1].Trim().Length > 0) Lines.Add("");
                    }
                    Lines.Add("[" + _Section + "]");
                    r = Lines.Count;
                    Lines.Add(_Key + " = " + _Value);
                }
                else
                {
                    r = -1;
                    i = lastSectionFound + 1;
                    while ((r < 0) && (i < Lines.Count))
                    {
                        ln = Lines[i].Trim();
                        if (ln.Length > 1)
                        {
                            if ((ln[0] == '[') && (ln[ln.Length - 1] == ']')) r = i;
                        }
                        i++;
                    }
                    if (r > -1)
                    {
                        if (Lines[r - 1].Trim().Length > 0)
                        {
                            Lines.Insert(r, "");
                            Lines.Insert(r, _Key + " = " + _Value);
                        }
                        else Lines.Insert(r - 1, _Key + " = " + _Value);
                    }
                    else Lines.Add(_Key + " = " + _Value);
                }
            }
            else Lines[r] = Lines[r].Substring(0, Lines[r].IndexOf('=') + 1) + ' ' + _Value;
            Changed = true;
            return r;
        }

        /// <summary>Write double value of key at section. Return line index if success otherwise -1.</summary>
        public int WriteBool(string _Section, string _Key, bool _Value)
        {
            return WriteString(_Section, _Key, CX.Bool(_Value));
        }

        /// <summary>Write double value of key at section. Return line index if success otherwise -1.</summary>
        public int WriteDateTime(string _Section, string _Key, DateTime _Value)
        {
            return WriteString(_Section, _Key, CX.Str(_Value, CXDateFormat.iso8601, true));
        }

        /// <summary>Write double value of key at section. Return line index if success otherwise -1.</summary>
        public int WriteDouble(string _Section, string _Key, double _Value)
        {
            return WriteString(_Section, _Key, CX.Str(_Value).Replace(CX.ThousandSeparator + "", "").Replace(CX.DecimalSeparator, '.'));
        }

        /// <summary>Write hex masked value of key at section. Return line index if success otherwise -1.</summary>
        public int WriteHexMask(string _Section, string _Key, string _Value)
        {
            string p = Password;
            if (CX.Empty(p)) p = CX.InternalPassword;
            if (CX.IsHexMask(_Value)) return WriteString(_Section, _Key, _Value);
            else return WriteString(_Section, _Key, CX.ToHexMask(_Value, p));
        }

        /// <summary>Write integer value of key at section. Return line index if success otherwise -1.</summary>
        public int WriteInteger(string _Section, string _Key, int _Value)
        {
            return WriteString(_Section, _Key, _Value.ToString());
        }

        #endregion

        /* */

    }

    /* */

}
