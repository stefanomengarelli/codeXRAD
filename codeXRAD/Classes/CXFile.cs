/*  ------------------------------------------------------------------------
 *  
 *  File:       CXFile.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD file class.
 *
 *  ------------------------------------------------------------------------
 */

using System.Text;

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD file class.</summary>
    public class CXFile
    {

        /* */

        #region Declarations

        /*  --------------------------------------------------------------------
         *  Declarations
         *  --------------------------------------------------------------------
         */

        /// <summary>Random access file record text trim char array.</summary>
        private char[] trimRecordChars = new char[] { '\0' };

        #endregion

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Class constructor.</summary>
        public CXFile()
        {
            this.Buffer = null;
            this.TextEncoding = CX.TextEncoding;
            this.Mode = CXFileMode.None;
            this.NewLine = "\r\n";
            this.Offset = -1;
            this.Path = "";
            this.Reader = null;
            this.RecordLength = 0;
            this.Stream = null;
            this.Text = "";
            this.Writer = null;
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get if file is opened.</summary>
        public bool Active
        {
            get { return (Reader != null) || (Stream != null) || (Writer != null); }
        }

        /// <summary>Get or set bytes buffer.</summary>
        public byte[] Buffer { get; set; }

        /// <summary>Get or set text encoding (defauls is ASCII).</summary>
        public Encoding TextEncoding { get; set; }

        /// <summary>Get EOF state.</summary>
        public bool EOF
        {
            get
            {
                if (Reader != null) return Reader.EndOfStream;
                else return false;
            }
        }

        /// <summary>Get file open mode.</summary>
        public CXFileMode Mode { get; private set; }

        /// <summary>Get or set new line chars sequence.</summary>
        public string NewLine { get; set; }

        /// <summary>Get record offset position or -1 if closed.</summary>
        public long Offset { get; private set; }

        /// <summary>Get file full path.</summary>
        public string Path { get; private set; }

        /// <summary>Get stream reader.</summary>
        public StreamReader Reader { get; private set; }

        /// <summary>Get or set record length (for random access file).</summary>
        public int RecordLength { get; set; }

        /// <summary>Get random access stream.</summary>
        public FileStream Stream { get; private set; }

        /// <summary>Get readed file text.</summary>
        public string Text { get; private set; }

        /// <summary>Get stream writer.</summary>
        public StreamWriter Writer { get; private set; }

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Close file.</summary>
        public bool Close()
        {
            bool r = true;
            if (this.Reader != null)
            {
                try
                {
                    this.Reader.Close();
                    this.Reader.Dispose();
                    this.Reader = null;
                }
                catch (Exception ex)
                {
                    CX.Error(ex);
                    r = false;
                }
            }
            if (this.Stream != null)
            {
                try
                {
                    this.Stream.Flush();
                    this.Stream.Close();
                    this.Stream.Dispose();
                    this.Stream = null;
                }
                catch (Exception ex)
                {
                    CX.Error(ex);
                    r = false;
                }
            }
            if (this.Writer != null)
            {
                try
                {
                    this.Writer.Close();
                    this.Writer.Dispose();
                    this.Writer = null;
                }
                catch (Exception ex)
                {
                    CX.Error(ex);
                    r = false;
                }
            }
            if (r) Mode = CXFileMode.None;
            return r;
        }

        /// <summary>Write all direct access file buffered data and clear buffers.</summary>
        public bool Flush()
        {
            try
            {
                if (this.Stream != null)
                {
                    this.Stream.Flush();
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                return false;
            }
        }

        /// <summary>Get content of record at position (random access file) and store it in buffer 
        /// and text property if encoding property specified. Return true if succeed.</summary>
        public bool Get(long _RecordPosition)
        {
            bool r = false, b;
            int i;
            Text = "";
            try
            {
                if ((Stream != null) && (RecordLength > 0))
                {
                    if (this.Buffer == null) b = true;
                    else b = this.Buffer.Length != RecordLength;
                    if (b)
                    {
                        this.Buffer = new byte[RecordLength];
                        for (i = 0; i < RecordLength; i++) this.Buffer[i] = 0;
                    }
                    if (Position(_RecordPosition, RecordLength))
                    {
                        if (Stream.Read(this.Buffer, 0, RecordLength) == RecordLength)
                        {
                            if (this.TextEncoding != null) Text = this.TextEncoding.GetString(this.Buffer).TrimEnd(trimRecordChars);
                            else Text = Encoding.ASCII.GetString(this.Buffer).TrimEnd(trimRecordChars);
                            r = true;
                        }
                    }
                    Offset = Stream.Position;
                }
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                r = false;
            }
            return r;
        }

        /// <summary>Open file with mode, retrying default times. Return true if succeed.</summary>
        public bool Open(string _FileName, CXFileMode _Mode)
        {
            return Open(_FileName, _Mode, TextEncoding, CX.FileRetries);
        }

        /// <summary>Open file with random access and record length specified. Return true if succeed.</summary>
        public bool Open(string _FileName, int _RecordLength)
        {
            if (_RecordLength > 0)
            {
                RecordLength = _RecordLength;
                return Open(_FileName, CXFileMode.RandomAccess, TextEncoding, CX.FileRetries);
            }
            else return false;
        }

        /// <summary>Open file with mode with retries. Return true if succeed.</summary>
        public bool Open(string _FileName, CXFileMode _Mode, Encoding _Encoding, int _Retry)
        {
            bool r = Open(_FileName, _Mode, _Encoding);
            if (!r) CX.MemoryRelease(true);
            while (!r && (_Retry > 0))
            {
                if (Open(_FileName, _Mode, _Encoding)) r = true;
                else CX.Wait(CX.FileRetriesDelay, true);
                _Retry--;
            }
            return r;
        }

        /// <summary>Open file with mode and encoding. Return true if succeed.</summary>
        public bool Open(string _FileName, CXFileMode _Mode, Encoding _Encoding)
        {
            try
            {
                Path = _FileName;
                Mode = _Mode;
                Offset = 0;
                Text = "";
                if (_Mode == CXFileMode.Read)
                {
                    Reader = new StreamReader(_FileName, _Encoding);
                    RecordLength = 0;
                    return true;
                }
                else if (_Mode == CXFileMode.Write)
                {
                    Writer = new StreamWriter(_FileName, false, _Encoding);
                    Writer.NewLine = NewLine;
                    RecordLength = 0;
                    return true;
                }
                else if (_Mode == CXFileMode.Append)
                {
                    Writer = new StreamWriter(_FileName, true, _Encoding);
                    Writer.NewLine = NewLine;
                    RecordLength = 0;
                    return true;
                }
                else if (_Mode == CXFileMode.RandomAccess)
                {
                    if ((RecordLength > 0) && (RecordLength < 65536))
                    {
                        Stream = new FileStream(_FileName, FileMode.OpenOrCreate);
                        return true;
                    }
                    else
                    {
                        RecordLength = 0;
                        return false;
                    }
                }
                else return false;
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                return false;
            }
        }

        /// <summary>Set random access file stream position at begin of record with length in bytes specified. Return true if succeed.</summary>
        public bool Position(long _RecordPosition, int _RecordLength)
        {
            bool r = false;
            long p;
            try
            {
                if ((Stream != null) && (_RecordPosition > -1) && (_RecordLength > 0))
                {
                    p = _RecordPosition * _RecordLength;
                    if (p < Stream.Length)
                    {
                        if (Stream.Seek(0, SeekOrigin.End) > -1)
                        {
                            while (Stream.Length < p) Stream.WriteByte(0);
                        }
                    }
                    r = Stream.Seek(p, SeekOrigin.Begin) == p;
                }
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                r = false;
            }
            return r;
        }

        /// <summary>Put current buffer content in to record at position of random access file. Return true if succeed.</summary>
        public bool Put(long _RecordPosition)
        {
            bool r = false;
            int i;
            byte[] buf;
            Text = "";
            try
            {
                if ((Stream != null) && (RecordLength > 0))
                {
                    if (Position(_RecordPosition, RecordLength))
                    {
                        buf = new byte[RecordLength];
                        if (this.Buffer == null)
                        {
                            this.Buffer = new byte[RecordLength];
                            for (i = 0; i < RecordLength; i++) this.Buffer[i] = 0;
                        }
                        else if (this.Buffer.Length != RecordLength)
                        {
                            buf = this.Buffer;
                            this.Buffer = new byte[RecordLength];
                            for (i = 0; i < RecordLength; i++)
                            {
                                if (i < buf.Length) this.Buffer[i] = buf[i];
                                else this.Buffer[i] = 0;
                            }
                        }
                        Stream.Write(this.Buffer, 0, RecordLength);
                        if (this.TextEncoding == null) this.Text = Encoding.ASCII.GetString(this.Buffer).TrimEnd(trimRecordChars);
                        else this.Text = this.TextEncoding.GetString(this.Buffer).TrimEnd(trimRecordChars);
                        r = true;
                    }
                    Offset = Stream.Position;
                }
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                r = false;
            }
            return r;
        }

        /// <summary>Put passed text in to record at position of random access file. Return true if succeed.</summary>
        public bool Put(long _RecordPosition, string _Text)
        {
            if (this.TextEncoding == null) this.Buffer = Encoding.ASCII.GetBytes(_Text);
            else this.Buffer = this.TextEncoding.GetBytes(_Text);
            return Put(_RecordPosition);
        }
        
        /// <summary>Read next char of file. Return text length or -1 if fail.</summary>
        public int Read()
        {
            int r = -1;
            try
            {
                if (Reader != null)
                {
                    r = Reader.Read();
                    Text = "" + (char)r;
                    Offset = Reader.BaseStream.Position;
                }
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                r = -1;
            }
            return r;
        }

        /// <summary>Read all file and store it in text property. Return text length or -1 if fail.</summary>
        public int ReadAll()
        {
            try
            {
                if (Reader != null)
                {
                    Text = Reader.ReadToEnd();
                    Offset = Reader.BaseStream.Position;
                    return Text.Length;
                }
                else return -1;
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                return -1;
            }
        }

        /// <summary>Read a line of file and store it in text property. Return text length or -1 if fail.</summary>
        public int ReadLine()
        {
            try
            {
                if (Reader != null)
                {
                    Text = Reader.ReadLine();
                    Offset = Reader.BaseStream.Position;
                    return Text.Length;
                }
                else return -1;
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                return -1;
            }
        }

        /// <summary>Write string to file.</summary>
        public bool Write(string _String)
        {
            try
            {
                if (Writer != null)
                {
                    Writer.Write(_String);
                    Text = _String;
                    Offset += _String.Length;
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                return false;
            }
        }

        /// <summary>Write string terminated with newline sequence on file.</summary>
        public bool WriteLine(string _String)
        {
            try
            {
                if (Writer != null)
                {
                    Writer.WriteLine(_String);
                    Text = _String;
                    Offset += _String.Length;
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                return false;
            }
        }

        #endregion

        /* */

    }

    /* */

}
