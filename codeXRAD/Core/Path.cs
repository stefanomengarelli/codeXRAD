/*  ------------------------------------------------------------------------
 *  
 *  File:       XPath.cs
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
            //
            // System paths
            //
            CommonApplicationData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
            Desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            Documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            ProgramFiles = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles);
            Windows = System.Environment.SystemDirectory;
            UserApplicationData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            Executable = FilePath(System.Windows.Forms.Application.ExecutablePath);
            //
            // System related paths
            //
            OEM = ForcePath(Combine(CommonApplicationData, XApplication.Company, ""));
            PrintMerge = ForcePath(Combine(CommonApplicationData, "PrintMerge", ""));
            Ultramarine = ForcePath(Combine(CommonApplicationData, "Microrun Ultramarine System", ""));
            //
            // Executable related paths
            //
            Library = Combine(Executable, "Library", "");
            //
            // Library related paths
            //
            LibraryDatabase = Combine(Library, "Database", "");
            LibraryDocuments = Combine(Library, "Documents", "");
            LibraryFonts = Combine(Library, "Fonts", "");
            LibraryHelp = Combine(Library, "Help", "");
            LibraryInstall = Combine(Library, "Install", "");
            LibraryResources = Combine(Library, "Resources", "");
            LibrarySystem = Combine(Library, "System", "");
            //
            // Set application work paths
            //
            SetApplicationWorkPaths(OEM, XApplication.Product);
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get or set application work path.</summary>
        public static string Application { get; set; }

        /// <summary>Get or set application backup path.</summary>
        public static string Backup { get; set; }

        /// <summary>Get or set OEM common data path.</summary>
        public static string Common { get; set; }

        /// <summary>Get or set system common application data path.</summary>
        public static string CommonApplicationData { get; set; }

        /// <summary>Get or set application configuration path.</summary>
        public static string Config { get; set; }

        /// <summary>Get or set application data path.</summary>
        public static string Data { get; set; }

        /// <summary>Get or set system desktop path.</summary>
        public static string Desktop { get; set; }

        /// <summary>Get or set system user documents path.</summary>
        public static string Documents { get; set; }

        /// <summary>Get or set application download path.</summary>
        public static string Download { get; set; }

        /// <summary>Get or set system executable path without file name.</summary>
        public static string Executable { get; set; }

        /// <summary>Get or set executable default library path.</summary>
        public static string Library { get; set; }

        /// <summary>Get or set executable default library path database subfolder.</summary>
        public static string LibraryDatabase { get; set; }

        /// <summary>Get or set executable default library path documens subfolder.</summary>
        public static string LibraryDocuments { get; set; }

        /// <summary>Get or set executable default library path fonts subfolder.</summary>
        public static string LibraryFonts { get; set; }

        /// <summary>Get or set executable default library path help subfolder.</summary>
        public static string LibraryHelp { get; set; }

        /// <summary>Get or set executable default library path install subfolder.</summary>
        public static string LibraryInstall { get; set; }

        /// <summary>Get or set executable default library path resources subfolder.</summary>
        public static string LibraryResources { get; set; }

        /// <summary>Get or set executable default library path system subfolder.</summary>
        public static string LibrarySystem { get; set; }

        /// <summary>Get or set application log path.</summary>
        public static string Log { get; set; }

        /// <summary>Get or set OEM common application path.</summary>
        public static string OEM { get; set; }

        /// <summary>Get or set system printmerge path.</summary>
        public static string PrintMerge { get; set; }

        /// <summary>Get or set system program files path.</summary>
        public static string ProgramFiles { get; set; }

        /// <summary>Get or set application temp path.</summary>
        public static string Temp { get; set; }

        /// <summary>Get or set microrun ultramarine system reserved common path.</summary>
        public static string Ultramarine { get; set; }

        /// <summary>Get or set application ugprade path.</summary>
        public static string Upgrade { get; set; }

        /// <summary>Get or set system user related application data path.</summary>
        public static string UserApplicationData { get; set; }

        /// <summary>Get or set system Windows(r) path.</summary>
        public static string Windows { get; set; }

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Returns a string containing full path of file name, in file path directory and with file extension.</summary>
        static public string Combine(string _FilePath, string _FileName, string _FileExtension)
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

        /// <summary>Set application work paths.</summary>
        static public void SetApplicationWorkPaths(string _OEM, string _Product)
        {
            //
            // OEM related paths
            //
            Application = ForcePath(Combine(_OEM, _Product, ""));
            Common = ForcePath(Combine(_OEM, "Common files", ""));
            //
            // Application related paths
            //
            Backup = ForcePath(Combine(Application, "Backup", ""));
            Config = ForcePath(Combine(Application, "Config", ""));
            if (Debug) Debug = ForcePath(Combine(Application, "Debug", ""));
            Download = ForcePath(Combine(Application, "Download", ""));
            Log = ForcePath(Combine(Application, "Log", ""));
            Temp = ForcePath(Combine(Application, "Temp", ""));
            Upgrade = ForcePath(Combine(Application, "Upgrade", ""));
            //
            // Default ini path
            //
            XIni.DefaultExecutable = _Product;
            //
            // Application data path
            //
            XDatabase.DefaultExecutable = _Product;
            Data = XIni.QuickRead("", "SETUP", "DATA_PATH", ForcePath(Combine(Application, "Data", "")));
        }

        /// <summary>Returns a random temporary file full path with extension.</summary>
        static public string TempFile(string _Extension)
        {
            return Combine(Temp, "~" + RndName(7), _Extension);
        }

        /// <summary>Returns a temporary ini file with full path with extension.</summary>
        static public string TempIniFile(string _FileNameWithoutExtension)
        {
            return Combine(Temp, _FileNameWithoutExtension, "ini");
        }

        /// <summary>Delete on temp path temporary files matches ~*.*</summary>
        static public bool WipeTemp()
        {
            return FilesDelete(Combine(Temp, "~*.*", ""));
        }

        /// <summary>Delete all files in upgrade path.</summary>
        public static bool WipeUpgrade()
        {
            return FilesDelete(Combine(Upgrade, "*.*", ""));
        }

        #endregion

        /* */

    }

    /* */

}
