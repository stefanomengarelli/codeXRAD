/*  ------------------------------------------------------------------------
 *  
 *  File:       Path.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD static basic class: path.
 *
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD static basic class: path.</summary>
    static public partial class CX
    {

        /* */

        #region Declarations

        /*  --------------------------------------------------------------------
         *  Declarations
         *  --------------------------------------------------------------------
         */

        #endregion

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Initialize path static class environment.</summary>
        static public void InitializePath()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                ExecutableName = assembly.GetName().Name;
                ExecutablePath = FilePath(assembly.Location);
            }
            else
            {
                ExecutableName = "";
                ExecutablePath = "";
            }
            CommonPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            if (Empty(OEM)) ApplicationPath = ForcePath(Combine(CommonPath, ExecutableName));
            else ApplicationPath = ForcePath(Combine(Combine(CommonPath, OEM, ""), ExecutableName));
            DataPath = Combine(ApplicationPath, "Data");
            TempPath = ForcePath(Combine(ApplicationPath, "Temp"));
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get or set application path.</summary>
        public static string ApplicationPath { get; set; }

        /// <summary>Get or set common path.</summary>
        public static string CommonPath { get; set; }

        /// <summary>Get or set data path.</summary>
        public static string DataPath { get; set; }

        /// <summary>Get or set executable name without extension.</summary>
        public static string ExecutableName { get; set; }

        /// <summary>Get or set executable path.</summary>
        public static string ExecutablePath { get; set; }

        /// <summary>Get or set temporary path.</summary>
        public static string TempPath { get; set; } = "";


        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Returns a string containing full path of file name, in file path directory and with file extension.</summary>
        static public string Combine(string _FilePath, string _FileName, string _FileExtension = "")
        {
            string p = FixPath(_FilePath), f = FileName(_FileName).Trim(), e = _FileExtension.Trim();
            if (p != "")
            {
                if (p[p.Length - 1] != TrailingChar) p += TrailingChar;
            }
            if (e != "")
            {
                if (e[0] != '.') e = '.' + e;
                f = Mid(f, 0, Pos('.', f + '.'));
            }
            return p + f + e;
        }

        /// <summary>Returns a string containing full path of file name, in file path and subfolder directory (created if not exists) and with file extension.</summary>
        static public string Combine(string _FilePath, string _SubFolder, string _FileName, string _FileExtension)
        {
            return Combine(ForcePath(Combine(_FilePath, _SubFolder, "")), _FileName, _FileExtension);
        }

        /// <summary>Return fixed path without ending trailing char.</summary>
        static public string FixPath(string _Path, char _TrailingChar)
        {
            _Path = _Path.Trim();
            int l = _Path.Length;
            if (l > 1)
            {
                if ((_Path[l - 1] == _TrailingChar) && (_Path[l - 2] != ':') && (_Path[l - 2] != _TrailingChar)) return Mid(_Path, 0, l - 1);
                else return _Path;
            }
            else if (_Path == "" + _TrailingChar) return _Path;
            else return "";
        }

        /// <summary>Return fixed path without ending default trailing char.</summary>
        static public string FixPath(string _Path)
        {
            return FixPath(_Path, TrailingChar);
        }

        /// <summary>Returns a string with path 1 and path 2 merged considering trailing char. 
        /// Paths will be normalized to trailing char replacing all \ and / chars with it.</summary>
        static public string Merge(string _Path1, string _Path2, char _TrailingChar)
        {
            bool b1, b2;
            if (_TrailingChar != '\\')
            {
                _Path1.Replace('\\', _TrailingChar);
                _Path2.Replace('\\', _TrailingChar);
            }
            if (_TrailingChar != '/')
            {
                _Path1.Replace('/', _TrailingChar);
                _Path2.Replace('/', _TrailingChar);
            }
            _Path1 = FixPath(_Path1.Trim(), _TrailingChar);
            _Path2 = FixPath(_Path2.Trim(), _TrailingChar);
            if (_Path1.Length < 1) return _Path2;
            else if (_Path2.Length < 1) return _Path1;
            else
            {
                if (_Path1.Length > 0) b1 = _Path1[_Path1.Length - 1] == _TrailingChar; else b1 = false;
                if (_Path2.Length > 0) b2 = _Path2[0] == _TrailingChar; else b2 = false;
                if (b1 && b2)
                {
                    if (_Path2.Length > 1) return _Path1 + _Path2.Substring(1);
                    else return _Path1;
                }
                else if (!b1 && !b2) return _Path1 + _TrailingChar + _Path2;
                else return _Path1 + _Path2;
            }
        }

        /// <summary>Returns a string with path 1 and path 2 merged.</summary>
        static public string Merge(string _Path1, string _Path2)
        {
            return Merge(_Path1, _Path2, TrailingChar);
        }

        /// <summary>Return full path of file name, on application folder.</summary>
        static public string OnApplicationPath(string _FileName)
        {
            return Combine(ApplicationPath, _FileName);
        }

        /// <summary>Return full path of file name, on application subfolder.</summary>
        static public string OnApplicationPath(string _SubFolder, string _FileName)
        {
            return Combine(Combine(ApplicationPath, _SubFolder), _FileName);
        }

        /// <summary>Return full path of file name, on library folder.</summary>
        static public string OnLibraryPath(string _FileName)
        {
            return Combine(Combine(ExecutablePath, "Library"), _FileName);
        }

        /// <summary>Return full path of file name, on library subfolder.</summary>
        static public string OnLibraryPath(string _SubFolder, string _FileName)
        {
            return Combine(Combine(Combine(ExecutablePath, "Library"), _SubFolder), _FileName);
        }

        /// <summary>Return full path of file name, on data folder.</summary>
        static public string OnDataPath(string _FileName)
        {
            return Combine(ForcePath(DataPath), _FileName);
        }

        /// <summary>Return full path of file name, on data subfolder.</summary>
        static public string OnDataPath(string _SubFolder, string _FileName)
        {
            return Combine(Combine(ForcePath(DataPath), _SubFolder), _FileName);
        }

        /// <summary>Remove from file path, initial base path if found.</summary>
        static public string RemoveBase(string _FilePath, string _BasePath)
        {
            _FilePath = _FilePath.Trim();
            _BasePath = FixPath(_BasePath.Trim());
            if (_FilePath.StartsWith(_BasePath))
            {
                if (_FilePath.Length == _BasePath.Length) return "";
                else return _FilePath.Substring(_BasePath.Length);
            }
            else return _FilePath;
        }

        /// <summary>Returns a random temporary file full path with extension.</summary>
        static public string TempFile(string _Extension)
        {
            return Combine(TempPath, "~" + RndName(7), _Extension);
        }

        /// <summary>Returns a temporary ini file with full path with extension.</summary>
        static public string TempIniFile(string _FileNameWithoutExtension)
        {
            return Combine(TempPath, _FileNameWithoutExtension, "ini");
        }

        /// <summary>Delete on temp path temporary files matches ~*.*</summary>
        static public bool WipeTemp()
        {
            return FilesDelete(Combine(TempPath, "~*.*", ""));
        }

        #endregion

        /* */

    }

    /* */

}
