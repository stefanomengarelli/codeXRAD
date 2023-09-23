/*  ------------------------------------------------------------------------
 *  
 *  File:       IO.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD static basic class: I/O.
 *
 *  ------------------------------------------------------------------------
 */

using System.Text;

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD static basic class: I/O.</summary>
    static public partial class CX
    {

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Initialize static I/O class environment.</summary>
        static public void InitializeIO()
        {
            FileRetries = 3;
            FileRetriesDelay = 1.0d;
            MaxLoadFileSize = 16384000;
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get or set file operations retries time.</summary>
        public static int FileRetries { get; set; }

        /// <summary>Get or set file operations retries delay.</summary>
        public static double FileRetriesDelay { get; set; }

        /// <summary>Get or set max load file size in bytes (default 16MB).</summary>
        public static int MaxLoadFileSize { get; set; }

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Append text string content to text file specified by file name.
        /// Returns true if succeed.</summary>
        static public bool AppendString(string _FileName, string _Text)
        {
            return AppendString(_FileName, _Text, FileEncoding(_FileName), FileRetries);
        }

        /// <summary>Append text string content to text file specified by file name.
        /// Returns true if succeed.</summary>
        static public bool AppendString(string _FileName, string _Text, Encoding _Encoding)
        {
            StreamWriter sw;
            try
            {
                if (_Encoding == null) _Encoding = FileEncoding(_FileName);
                sw = new StreamWriter(_FileName, true, _Encoding);
                sw.Write(_Text);
                sw.Close();
                sw.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }

        /// <summary>Append text string content to text file specified by file name.
        /// Returns true if succeed.</summary>
        static public bool AppendString(string _FileName, string _Text, Encoding _Encoding, int _FileRetries)
        {
            bool r = AppendString(_FileName, _Text, _Encoding);
            if (!r) MemoryRelease(true);
            while (!r && (_FileRetries > 1))
            {
                Wait(FileRetriesDelay, true);
                r = AppendString(_FileName, _Text, _Encoding);
                _FileRetries--;
            }
            return r;
        }

        /// <summary>Returns the full file path changing extension in new extension.</summary>
        static public string FileChangeExtension(string _FilePath, string _NewExtension)
        {
            return Combine(FilePath(_FilePath), FileNameWithoutExt(_FilePath), _NewExtension);
        }

        /// <summary>Copy file which path is specified in source file to path 
        /// target file eventually overwriting existing file with same name. 
        /// Returns true if succeed.</summary>
        static public bool FileCopy(string _SourceFile, string _TargetFile)
        {
            try
            {
                File.Copy(_SourceFile, _TargetFile, true);
                return true;
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }

        /// <summary>Copy file retrying if fail which path is specified in source file 
        /// to path target file eventually overwriting existing file with same name. 
        /// If not succeed, retry for specified times. Returns true if succeed.</summary>
        static public bool FileCopy(string _SourceFile, string _TargetFile, int _FileRetries)
        {
            bool r = FileExists(_SourceFile);
            if (r)
            {
                r = FileCopy(_SourceFile, _TargetFile);
                if (!r) MemoryRelease(true);
                while (!r && (_FileRetries > 1))
                {
                    Wait(FileRetriesDelay, true);
                    r = FileCopy(_SourceFile, _TargetFile);
                    _FileRetries--;
                }
            }
            return r;
        }

        /// <summary>Returns the datetime of file creation data.</summary>
        static public DateTime FileCreated(string _FileName)
        {
            FileInfo fi;
            try
            {
                fi = new FileInfo(_FileName);
                return fi.CreationTime;
            }
            catch (Exception ex)
            {
                Error(ex);
                return DateTime.MinValue;
            }
        }

        /// <summary>Returns the datetime of last changes about file located in file name path.</summary>
        static public DateTime FileDate(string _FileName)
        {
            FileInfo fi;
            try
            {
                fi = new FileInfo(_FileName);
                return fi.LastWriteTime;
            }
            catch (Exception ex)
            {
                Error(ex);
                return DateTime.MinValue;
            }
        }

        /// <summary>Set the datetime of last changes about file located in file name path. 
        /// Return true if succeed.</summary>
        static public bool FileDate(string _FileName, DateTime _DateTime)
        {
            try
            {
                File.SetLastWriteTime(_FileName, _DateTime);
                return true;
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }

        /// <summary>Delete file specified retrying for settings planned times. Return true if succeed.</summary>
        public static bool FileDelete(string _FileName)
        {
            int i = FileRetries;
            bool r = FileKill(_FileName);
            if (!r) MemoryRelease(true);
            while (!r && (i > 1))
            {
                Wait(FileRetriesDelay, true);
                r = FileKill(_FileName);
                i--;
            }
            return r;
        }

        /// <summary>Returns the extension of file path file without starting comma.</summary>
        static public string FileExtension(string _FilePath)
        {
            string r;
            try
            {
                r = Path.GetExtension(_FilePath);
                if (r != "")
                {
                    if (r[0] == '.')
                    {
                        if (r.Length > 1) r = r.Substring(1);
                        else r = "";
                    }
                }
                return r;
            }
            catch (Exception ex)
            {
                Error(ex);
                return "";
            }
        }

        /// <summary>Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.</summary>
        static public Encoding FileEncoding(string _FileName)
        {
            byte[] bom = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
            FileStream fileStream;
            try
            {
                if (File.Exists(_FileName))
                {
                    fileStream = new FileStream(_FileName, FileMode.Open, FileAccess.Read);
                    fileStream.Read(bom, 0, 4);
                    fileStream.Close();
                    fileStream.Dispose();
                    if ((bom[0] == 0x2b) && (bom[1] == 0x2f) && (bom[2] == 0x76)) return Encoding.UTF7;
                    else if ((bom[0] == 0xef) && (bom[1] == 0xbb) && (bom[2] == 0xbf)) return Encoding.UTF8;
                    else if ((bom[0] == 0xff) && (bom[1] == 0xfe)) return Encoding.Unicode; //UTF-16LE
                    else if ((bom[0] == 0xfe) && (bom[1] == 0xff)) return Encoding.BigEndianUnicode; //UTF-16BE
                    else if ((bom[0] == 0) && (bom[1] == 0) && (bom[2] == 0xfe) && (bom[3] == 0xff)) return Encoding.UTF32;
                    else return Encoding.Default;
                }
                else return TextEncoding;
            }
            catch (Exception ex)
            {
                Error(ex);
                return TextEncoding;
            }
        }

        /// <summary>Determines a text stream encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.</summary>
        static public Encoding FileEncoding(Stream _TextStream)
        {
            byte[] bom = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
            try
            {
                if (_TextStream != null)
                {
                    _TextStream.Read(bom, 0, 4);
                    _TextStream.Seek(0, SeekOrigin.Begin);
                    if ((bom[0] == 0x2b) && (bom[1] == 0x2f) && (bom[2] == 0x76)) return Encoding.UTF7;
                    else if ((bom[0] == 0xef) && (bom[1] == 0xbb) && (bom[2] == 0xbf)) return Encoding.UTF8;
                    else if ((bom[0] == 0xff) && (bom[1] == 0xfe)) return Encoding.Unicode; //UTF-16LE
                    else if ((bom[0] == 0xfe) && (bom[1] == 0xff)) return Encoding.BigEndianUnicode; //UTF-16BE
                    else if ((bom[0] == 0) && (bom[1] == 0) && (bom[2] == 0xfe) && (bom[3] == 0xff)) return Encoding.UTF32;
                    else return Encoding.ASCII;
                }
                else return TextEncoding;
            }
            catch (Exception ex)
            {
                Error(ex);
                return TextEncoding;
            }
        }

        /// <summary>Return true if file specified exists.</summary>
        public static bool FileExists(string _FileName)
        {
            try
            {
                if (_FileName.Trim().Length < 1) return false;
                else if (File.Exists(_FileName)) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Returns hash MD5 code about the file specified.</summary>
        static public string FileHashMD5(string _FileName)
        {
            int i;
            byte[] b;
            string r = "";
            FileStream fs;
            try
            {
                fs = File.OpenRead(_FileName);
                b = System.Security.Cryptography.MD5.Create().ComputeHash(fs);
                for (i = 0; i < b.Length; i++) r += Convert.ToString(b[i], 16);
                fs.Close();
                fs.Dispose();
                return r;
            }
            catch (Exception ex)
            {
                Error(ex);
                return "";
            }
        }

        /// <summary>Create history backup of file specified, mantaining a maximum of files.</summary>
        static public bool FileHistory(string _FileName, int _MaximumFiles)
        {
            if (FileExists(_FileName))
            {
                FileHistoryWipe(_FileName, _MaximumFiles);
                return FileCopy(_FileName, FileTimeStamp(_FileName, DateTime.Now));
            }
            else return false;
        }

        /// <summary>Wipe history backup of file specified mantaining a maximum-1 of files.</summary>
        static public void FileHistoryWipe(string _FileName, int _MaximumFiles)
        {
            string p;
            List<string> l = FileTimeStampList(_FileName);
            if (l.Count > 0)
            {
                l.Sort();
                _MaximumFiles--;
                p = FilePath(_FileName);
                while (_MaximumFiles < l.Count)
                {
                    FilesDelete(Combine(p, l[0], ""));
                    l.RemoveAt(0);
                }
            }
        }

        /// <summary>Delete file specified if exists (no retries). Return true if succeed.</summary>
        public static bool FileKill(string _FileName)
        {
            try
            {
                if (File.Exists(_FileName)) File.Delete(_FileName);
                return true;
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }

        /// <summary>Returns FileInfo array of all file matches file path or null if function fails.</summary>
        static public FileInfo[] FileList(string _Path)
        {
            string p;
            DirectoryInfo di;
            FileInfo[] fi;
            try
            {
                p = FilePath(_Path);
                if (FolderExists(p))
                {
                    di = new DirectoryInfo(p);
                    p = FileName(_Path);
                    fi = di.GetFiles(p);
                    return fi;
                }
                else return null;
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        /// <summary>Returns string list of all file names (or full path if specified) 
        /// matching file path or null if function fails.</summary>
        static public List<string> FileList(string _Path, bool _StoreFullPath)
        {
            List<string> sl = new List<string>();
            FileInfo[] fi = FileList(_Path);
            if (fi != null)
            {
                int i, h = fi.Length;
                if (_StoreFullPath)
                {
                    for (i = 0; i < h; i++) sl.Add(fi[i].FullName);
                }
                else for (i = 0; i < h; i++) sl.Add(fi[i].Name);
            }
            return sl;
        }

        /// <summary>Returns array of bytes with file content (see SM.MaxLoadFileSize).</summary>
        static public byte[] FileLoad(string _FileName)
        {
            byte[] r = null;
            FileStream fs;
            BinaryReader br;
            if (FileExists(_FileName))
            {
                if (FileSize(_FileName) > MaxLoadFileSize)
                {
                    Raise("File exceed maximum load size (" + MaxLoadFileSize.ToString() + ")", false);
                }
                else
                {
                    try
                    {
                        fs = new FileStream(_FileName, FileMode.Open);
                        br = new BinaryReader(fs);
                        r = br.ReadBytes(MaxLoadFileSize);
                        br.Close();
                        fs.Close();
                        fs.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Error(ex);
                        r = null;
                    }
                }
            }
            return r;
        }

        /// <summary>Move the file located in to old file path in to new file path (no retries). 
        /// Returns true if succeed.</summary>
        static public bool FileMove(string _OldFile, string _NewFile)
        {
            try
            {
                if (FileExists(_OldFile))
                {
                    FileDelete(_NewFile);
                    File.Move(_OldFile, _NewFile);
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }

        /// <summary>Try to move the file located in to old file path in to new file path 
        /// retrying for passed times. Returns true if succeed.</summary>
        static public bool FileMove(string _OldFile, string _NewFile, int _FileRetries)
        {
            bool r = false;
            if (FileExists(_OldFile))
            {
                r = FileMove(_OldFile, _NewFile);
                if (!r) MemoryRelease(true);
                while (!r && (_FileRetries > 1))
                {
                    Wait(FileRetriesDelay, true);
                    r = FileMove(_OldFile, _NewFile);
                    _FileRetries--;
                }
            }
            return r;
        }

        /// <summary>Returns the name of file specified in file path, with extension.</summary>
        static public string FileName(string _Path)
        {
            try
            {
                if (_Path.Trim().Length > 0) return System.IO.Path.GetFileName(_Path);
                else return "";
            }
            catch (Exception ex)
            {
                Error(ex);
                return "";
            }
        }

        /// <summary>Returns the name of file specified in file path, without extension.</summary>
        static public string FileNameWithoutExt(string _Path)
        {
            try
            {
                if (_Path.Trim().Length > 0) return System.IO.Path.GetFileNameWithoutExtension(_Path);
                else return "";
            }
            catch (Exception ex)
            {
                Error(ex);
                return "";
            }
        }

        /// <summary>Returns directory path of file specified in full file path.</summary>
        static public string FilePath(string _Path)
        {
            try
            {
                if (_Path.Trim().Length > 0) return System.IO.Path.GetDirectoryName(_Path);
                else return "";
            }
            catch (Exception ex)
            {
                Error(ex);
                return "";
            }
        }

        /// <summary>Delete files that matches the file path.
        /// Returns true if succeed.</summary>
        static public bool FilesDelete(string _Path)
        {
            bool b = false;
            FileInfo[] fi;
            try
            {
                if (_Path.Trim().Length > 0)
                {
                    fi = FileList(_Path);
                    if (fi != null)
                    {
                        b = true;
                        for (int i = 0; i < fi.Length; i++)
                        {
                            if (!FileDelete(fi[i].FullName)) b = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex);
                b = false;
            }
            return b;
        }

        /// <summary>Returns the size of file specified or -1 if does not exists.</summary>
        static public long FileSize(string _FileName)
        {
            FileInfo fi;
            try
            {
                if (_FileName.Trim().Length > 0)
                {
                    fi = new FileInfo(_FileName);
                    return fi.Length;
                }
                else return -1;
            }
            catch (Exception ex)
            {
                Error(ex);
                return -1;
            }
        }

        /// <summary>Delete all temporary files witch name start by ~</summary>
        static public bool FileTempWipe()
        {
            return FilesDelete(Combine(TempPath, "~*", "*"));
        }

        /// <summary>Return a string containing a timestamp with format 
        /// "yyyy-MM-dd_hhmmss_fff" suitable for file naming usage.</summary>
        static public string FileTimeStamp(DateTime _DateTime)
        {
            return _DateTime.ToString("yyyy-MM-dd_HHmmss_fff");
        }

        /// <summary>Return a string containing file name full path with timestamp of datetime passed.</summary>
        static public string FileTimeStamp(string _FileName, DateTime _DateTime)
        {
            return Combine(
                FilePath(_FileName),
                FileNameWithoutExt(_FileName) + " (" + FileTimeStamp(_DateTime) + ")",
                FileExtension(_FileName)
                );
        }

        /// <summary>Return a string containing file name full path with timestamp of last modified datetime.</summary>
        static public string FileTimeStamp(string _FileName)
        {
            return FileTimeStamp(_FileName, FileDate(_FileName));
        }

        /// <summary>Return a list of history files (with timestamp) with same file name.</summary>
        static public List<string> FileTimeStampList(string _FileName)
        {
            int i = 0;
            string s;
            List<string> l = FileList(Combine(FilePath(_FileName), FileNameWithoutExt(_FileName) + " (*)", FileExtension(_FileName)), false);
            while (i < l.Count)
            {
                s = FileNameWithoutExt(l[i]);
                if (s.Length > 24)
                {
                    s = s.Substring(s.Length - 23, 23);
                    if ((s[0] == '(') && (s[5] == '-') && (s[8] == '-')
                        && (s[11] == '_') && (s[18] == '_') && (s[22] == ')')) i++;
                    else l.RemoveAt(i);
                }
                else l.RemoveAt(i);
            }
            if (l.Count > 1) l.Sort();
            return l;
        }

        /// <summary>Return first existing file name of specified in array or default if none exists.</summary>
        static public string FirstExists(string[] _FileNames, string _DefaultFileName)
        {
            int i = 0;
            string r = "";
            if (_FileNames != null)
            {
                while ((r == "") && (i < _FileNames.Length))
                {
                    if (FileExists(_FileNames[i])) r = _FileNames[i];
                    i++;
                }
            } 
            if (r == "") return _DefaultFileName;
            else return r;
        }

        /// <summary>Returns true if folder path exists.</summary>
        public static bool FolderExists(string _Path)
        {
            try
            {
                return System.IO.Directory.Exists(_Path);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Returns DirectoryInfo array of all folders 
        /// matches path or null if function fails.</summary>
        static public DirectoryInfo[] FolderList(string _Path)
        {
            DirectoryInfo di;
            try
            {
                di = new DirectoryInfo(FilePath(_Path));
                return di.GetDirectories(FileName(_Path));
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        /// <summary>Create folders included in file path if does not exists.
        /// Returns true if succeed.</summary>
        static public bool ForceFolders(string _Path)
        {
            try
            {
                System.IO.Directory.CreateDirectory(_Path);
                return true;
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }

        /// <summary>Return path and try to create if not exists. Return always passed path.</summary>
        static public string ForcePath(string _Path)
        {
            if (!FolderExists(_Path)) ForceFolders(_Path);
            return _Path;
        }

        /// <summary>Returns a string containing all chars of text file with encoding.</summary>
        public static string LoadString(string _FileName, Encoding _TextEncoding)
        {
            try
            {
                return File.ReadAllText(_FileName, _TextEncoding);
            }
            catch (Exception ex)
            {
                Error(ex);
                return "";
            }
        }

        /// <summary>Returns a string containing all chars of text file retrying for specified times.</summary>
        public static string LoadString(string _FileName, Encoding _TextEncoding, int _FileRetries)
        {
            string r = "";
            Error();
            if (FileExists(_FileName))
            {
                r = LoadString(_FileName, _TextEncoding);
                if (IsError) MemoryRelease(true);
                while (IsError && (_FileRetries > 1))
                {
                    Wait(FileRetriesDelay, true);
                    r = LoadString(_FileName, _TextEncoding);
                    _FileRetries--;
                }
            }
            return r;
        }

        /// <summary>Load sl string list with lines of text file fileName.
        /// Returns true if succeed.</summary>
        static public bool LoadString(string _FileName, List<string> _StringList, bool _Append, Encoding _Encoding)
        {
            StreamReader sr;
            if (_StringList != null)
            {
                if (!_Append) _StringList.Clear();
                if (System.IO.File.Exists(_FileName))
                {
                    try
                    {
                        sr = new StreamReader(_FileName, _Encoding);
                        while (!sr.EndOfStream) _StringList.Add(sr.ReadLine());
                        sr.Close();
                        sr.Dispose();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Error(ex);
                        return false;
                    }
                }
                else return false;
            }
            else return false;
        }

        /// <summary>Returns a string containing all chars of text file retrying for IO planned times.</summary>
        public static string LoadString(string _FileName)
        {
            return LoadString(_FileName, TextEncoding, FileRetries);
        }

        /// <summary>Move directory indicate by dir path and all subdirs to a new path (no retries). 
        /// Returns true if succeed.</summary>
        static public bool MoveFolder(string _FolderPath, string _NewPath)
        {
            try
            {
                System.IO.Directory.Move(_FolderPath, _NewPath);
                return true;
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }

        /// <summary>Move directory indicate by dir path and all subdirs to a new path 
        /// retrying for passed time. Returns true if succeed.</summary>
        static public bool MoveFolder(string _FolderPath, string _NewPath, int _FileRetries)
        {
            bool r = false;
            if (FolderExists(_FolderPath))
            {
                r = MoveFolder(_FolderPath, _NewPath);
                if (!r) MemoryRelease(true);
                while (!r && (_FileRetries > 1))
                {
                    Wait(FileRetriesDelay, true);
                    r = MoveFolder(_FolderPath, _NewPath);
                    _FileRetries--;
                }
            }
            return r;
        }

        /// <summary>Remove directory indicate by dir path and all subdirs (no retries). 
        /// Returns true if succeed.</summary>
        static public bool RemoveFolder(string _Path)
        {
            try
            {
                System.IO.Directory.Delete(_Path, true);
                return true;
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }

        /// <summary>Remove directory indicate by dir path and all subdirs
        /// retrying for passed time. Returns true if succeed.</summary>
        static public bool RemoveFolder(string _Path, int _FileRetries)
        {
            bool r = false;
            if (FolderExists(_Path))
            {
                r = RemoveFolder(_Path);
                if (!r) MemoryRelease(true);
                while (!r && (_FileRetries > 1))
                {
                    Wait(FileRetriesDelay, true);
                    r = RemoveFolder(_Path);
                    _FileRetries--;
                }
            }
            return r;
        }

        /// <summary>Save text string content in the text file specified with encoding.
        /// Returns true if succeed.</summary>
        public static bool SaveString(string _FileName, string _Text, Encoding _TextEncoding)
        {
            try
            {
                File.WriteAllText(_FileName, _Text, _TextEncoding);
                return true;
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }

        /// <summary>Save text string content in the text file specified retrying for specified times.
        /// Returns true if succeed.</summary>
        public static bool SaveString(string _FileName, string _Text, Encoding _TextEncoding, int _FileRetries)
        {
            bool r;
            r = SaveString(_FileName, _Text, _TextEncoding);
            if (!r) MemoryRelease(true);
            while (!r && (_FileRetries > 1))
            {
                Wait(FileRetriesDelay, true);
                r = SaveString(_FileName, _Text, _TextEncoding);
                _FileRetries--;
            }
            return r;
        }

        /// <summary>Save text string content in the text file specified retrying for IO planned times.
        /// Returns true if succeed.</summary>
        public static bool SaveString(string _FileName, string _Text)
        {
            return SaveString(_FileName, _Text, TextEncoding, FileRetries);
        }

        /// <summary>Save string list in the text file specified by file name
        /// with specified text encoding. Returns true if succeed.</summary>
        static public bool SaveString(string _FileName, List<string> _StringList, Encoding _Encoding)
        {
            int i = 0;
            StreamWriter sw;
            if (_StringList != null)
            {
                try
                {
                    sw = new StreamWriter(_FileName, false, _Encoding);
                    sw.NewLine = CR;
                    while (i < _StringList.Count)
                    {
                        sw.WriteLine(_StringList[i]);
                        i++;
                    }
                    sw.Close();
                    sw.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    Error(ex);
                    return false;
                }
            }
            else return false;
        }

        /// <summary>Save string list in the text file specified by file name
        /// with specified text encoding. Returns true if succeed.</summary>
        static public bool SaveString(string _FileName, List<string> _StringList, Encoding _Encoding, int _FileRetries)
        {
            bool r;
            r = SaveString(_FileName, _StringList, _Encoding);
            if (!r) MemoryRelease(true);
            while (!r && (_FileRetries > 1))
            {
                Wait(FileRetriesDelay, true);
                r = SaveString(_FileName, _StringList, _Encoding);
                _FileRetries--;
            }
            return r;
        }

        /// <summary>Save string list content in the text file specified retrying for IO planned times.
        /// Returns true if succeed.</summary>
        public static bool SaveString(string _FileName, List<string> _StringList)
        {
            return SaveString(_FileName, _StringList, TextEncoding, FileRetries);
        }

        #endregion

        /* */

    }

    /* */

}
