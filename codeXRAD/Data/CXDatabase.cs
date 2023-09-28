/*  ------------------------------------------------------------------------
 *  
 *  File:       XDatabase.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD database connection component.
 *
 *  ------------------------------------------------------------------------
 */

using MySql.Data.MySqlClient;
using Npgsql;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD database connection component.</summary>
    [ToolboxItem(true)]
    public partial class CXDatabase : Component
    {

        /* */

        #region Declarations

        /*  --------------------------------------------------------------------
         *  Declarations
         *  --------------------------------------------------------------------
         */

        /// <summary>Database alias.</summary>
        private string alias;

        /// <summary>Database default executable.</summary>
        private static string defaultExecutable = "";

        #endregion

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Database instance constructor.</summary>
        public CXDatabase()
        {
            CX.Initialize();
            InitializeComponent();
            InitializeInstance();
        }

        /// <summary>Database instance constructor with container.</summary>
        public CXDatabase(IContainer _Container)
        {
            CX.Initialize();
            _Container.Add(this);
            InitializeComponent();
            InitializeInstance();
        }

        /// <summary>Initialize instance.</summary>
        private void InitializeInstance()
        {
            Clear();
            VersionChecked = false;
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Specifies whether or not database is open.</summary>
        [Browsable(true)]
        [Description("Specifies whether or not database is open.")]
        public bool Active
        {
            get
            {
                if (Type == CXDatabaseType.Sql)
                {
                    if (SqlDB != null) return SqlDB.State == ConnectionState.Open;
                    else return false;
                }
                else if (Type == CXDatabaseType.MySql)
                {
                    if (MySqlDB != null) return MySqlDB.State == ConnectionState.Open;
                    else return false;
                }
                else if (Type == CXDatabaseType.Access)
                {
                    if (OleDB != null) return OleDB.State == ConnectionState.Open;
                    else return false;
                }
                else if (Type == CXDatabaseType.DBase4)
                {
                    if (OleDB != null) return OleDB.State == ConnectionState.Open;
                    else return false;
                }
                else if (Type == CXDatabaseType.PostgreSQL)
                {
                    if (PostgreDB != null) return PostgreDB.State == ConnectionState.Open;
                    else return false;
                }
                else return false;
            }
            set
            {
                if (value) Open();
                else Close();
            }
        }

        /// <summary>Specifies database alias name.</summary>
        [Browsable(true)]
        [Description("Specifies database alias name.")]
        public string Alias
        {
            get { return alias; }
            set { alias = value.Trim().ToUpper(); }
        }

        /// <summary>Specifies database command timeout in seconds.</summary>
        [Browsable(true)]
        [Description("Specifies database command timeout in seconds.")]
        public int CommandTimeout { get; set; }

        /// <summary>Specifies database connection timeout in seconds.</summary>
        [Browsable(true)]
        [Description("Specifies database connection timeout in seconds.")]
        public int ConnectionTimeout { get; set; }

        /// <summary>Specifies default database name.</summary>
        [Browsable(true)]
        [Description("Specifies default database name.")]
        public string Database { get; set; }

        /// <summary>Specifies database host name or address.</summary>
        [Browsable(true)]
        [Description("Specifies database host name or address.")]
        public string Host { get; set; }

        /// <summary>Indicated used connection form MySql database type.</summary>
        [Browsable(false)]
        public MySqlConnection MySqlDB { get; set; }

        /// <summary>Indicated used connection form OleDB database type.</summary>
        [Browsable(false)]
        public OleDbConnection OleDB { get; set; }

        /// <summary>Specifies database password.</summary>
        [Browsable(true)]
        [Description("Specifies database password.")]
        public string Password { get; set; }

        /// <summary>Specifies database path.</summary>
        [Browsable(true)]
        [Description("Specifies database path.")]
        public string Path { get; set; }

        /// <summary>Indicated used connection form PostgreSQL database type.</summary>
        [Browsable(false)]
        public NpgsqlConnection PostgreDB { get; set; }

        /// <summary>Indicated used connection form Sql database type.</summary>
        [Browsable(false)]
        public SqlConnection SqlDB { get; set; }

        /// <summary>Specifies database templates file id without extension.</summary>
        [Browsable(true)]
        [Description("Specifies database templates file id without extension.")]
        public string Template { get; set; }

        /// <summary>Specifies database type.</summary>
        [Browsable(true)]
        [Description("Specifies database type.")]
        public CXDatabaseType Type { get; set; }

        /// <summary>Specifies database user name.</summary>
        [Browsable(true)]
        [Description("Specifies database user name.")]
        public string User { get; set; }

        #endregion

        /* */

        #region Static properties

        /*  --------------------------------------------------------------------
         *  Static properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Specifies database client mode.</summary>
        public static bool ClientMode { get; set; } = false;

        /// <summary>Opened alias databases collection.</summary>
        public static List<CXDatabase> Databases { get; set; } = new List<CXDatabase>();

        /// <summary>Specifies default access database password.</summary>
        public static string DefaultAccessPassword { get; set; } = "";

        /// <summary>Specifies database command default timeout in seconds.</summary>
        [Browsable(true)]
        [Description("Specifies database command default timeout in seconds.")]
        public static int DefaultCommandTimeout { get; set; }

        /// <summary>Specifies database connection default timeout in seconds.</summary>
        [Browsable(true)]
        [Description("Specifies database connection default timeout in seconds.")]
        public static int DefaultConnectionTimeout { get; set; }

        /// <summary>Get or set INI default executable name without extension.</summary>
        [Browsable(false)]
        public static string DefaultExecutable
        {
            get
            {
                if (defaultExecutable.Trim().Length < 1) defaultExecutable = CX.ExecutableName;
                return defaultExecutable;
            }
            set { defaultExecutable = value; }
        }

        /// <summary>Specifies database default fetch delay in milliseconds.</summary>
        [Browsable(true)]
        [Description("Specifies database default fetch delay in milliseconds.")]
        public static int DefaultFetchDelay { get; set; }
        /// <summary>Last connection database string.</summary>
        public static string LastConnectionString { get; set; } = "";

        /// <summary>Last failed connection database date-time.</summary>
        private static DateTime LastConnectionFailed { get; set; } = DateTime.MinValue;

        /// <summary>Get or set MySQL database object prefix.</summary>
        public static char MySqlPrefix { get; set; } = '`';

        /// <summary>Get or set MySQL database object suffix.</summary>
        public static char MySqlSuffix { get; set; } = '`';

        /// <summary>Get or set Microsoft SQL database object prefix.</summary>
        public static char SqlPrefix { get; set; } = '[';

        /// <summary>Get or set Microsoft SQL database object suffix.</summary>
        public static char SqlSuffix { get; set; } = ']';

        /// <summary>Get or set version checked flag.</summary>
        public static bool VersionChecked { get; set; } = false;

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Clear instance variables.</summary>
        public void Clear()
        {
            alias = "";
            CommandTimeout = 0;
            ConnectionTimeout = 30;
            Database = "";
            Host = "";
            MySqlDB = null;
            OleDB = null;
            PostgreDB = null;
            Password = "";
            Path = "";
            SqlDB = null;
            Template = "";
            Type = CXDatabaseType.Access;
            User = "";
        }

        /// <summary>Close database connection. Returns true if succeed.</summary>
        public bool Close()
        {
            //
            // close ole connection
            //
            try
            {
                if (OleDB != null)
                {
                    if (OleDB.State != ConnectionState.Closed) OleDB.Close();
                    OleDB.Dispose();
                    OleDB = null;
                }
            }
            catch (Exception ex)
            {
                CX.Error(ex);
            }
            //
            // close sql connection
            //
            try
            {
                if (SqlDB != null)
                {
                    if (SqlDB.State != ConnectionState.Closed) SqlDB.Close();
                    SqlDB.Dispose();
                    SqlDB = null;
                }
            }
            catch (Exception ex)
            {
                CX.Error(ex);
            }
            //
            // close mysql connection
            //
            try
            {
                if (MySqlDB != null)
                {
                    if (MySqlDB.State != ConnectionState.Closed) MySqlDB.Close();
                    MySqlDB.Dispose();
                    MySqlDB = null;
                }
            }
            catch (Exception ex)
            {
                CX.Error(ex);
            }
            //
            // close postgresql connection
            //
            try
            {
                if (PostgreDB != null)
                {
                    if (PostgreDB.State != ConnectionState.Closed) PostgreDB.Close();
                    PostgreDB.Dispose();
                    PostgreDB = null;
                }
            }
            catch (Exception ex)
            {
                CX.Error(ex);
            }
            //
            // check if closed
            //
            return !this.Active;
        }

        /// <summary>Return connection string for current settings.</summary>
        public string ConnectionString()
        {
            string r = "";
            if (Type == CXDatabaseType.Access)
            {
                r = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + CX.Combine(Path, Database, "mdb") + ";";
                if (Password.Trim().Length > 0) r += "Jet OLEDB:Database Password=" + Password + ";";
            }
            else if (Type == CXDatabaseType.Sql)
            {
                r = "Data Source=" + Host + "; Initial Catalog=" + Database + "; User Id=" + User + "; Password=" + Password
                    + ";Connection Timeout=" + ConnectionTimeout.ToString() + ";Encrypt=True;TrustServerCertificate=True;";
            }
            else if (Type == CXDatabaseType.MySql)
            {
                r = "Persist Security Info=False; Database=" + Database + "; Data Source=" + Host + "; Connect Timeout="
                    + ConnectionTimeout.ToString() + "; User Id=" + User + "; Password=" + Password + ";";
            }
            else if (Type == CXDatabaseType.DBase4)
            {
                r = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + CX.Combine(Path, Database, "dbf") + ";Extended Properties=dBASE IV;User ID=Admin;Password=;";
            }
            else if (Type == CXDatabaseType.PostgreSQL)
            {
                r = "Server=" + Host + ";Port=5432;Database=" + Database + ";User Id=" + User + ";Password=" + Password + ";"
            }
            return r;
        }

        /// <summary>Test if database connection is active otherwise try to open it. Returns true if succeed.</summary>
        public bool Keep()
        {
            bool r = this.Active;
            if (!r) r = Open();
            return r;
        }

        /// <summary>Load database parameters by alias from INI file. If filename is empty will be assumed default 
        /// application INI file. If alias is empty will be assumed actual database alias. Returns true if succeed.</summary>
        public bool Load(string _FileName, string _Alias)
        {
            string sz;
            CXIni ini;
            if (Close())
            {
                _Alias = _Alias.Trim().ToUpper();
                if (_Alias.Length < 1) _Alias = alias;
                if (_Alias.Length > 0)
                {
                    ini = new CXIni(_FileName);
                    ini.WriteDefault = true;
                    sz = "DATABASE_ALIAS_" + _Alias;
                    CommandTimeout = ini.ReadInteger(sz, "COMMAND_TIMEOUT", DefaultCommandTimeout);
                    ConnectionTimeout = ini.ReadInteger(sz, "CONNECTION_TIMEOUT", DefaultConnectionTimeout);
                    Database = ini.ReadString(sz, "DATABASE", Template);
                    Host = ini.ReadString(sz, "HOST", "localhost");
                    Password = ini.ReadHexMask(sz, "PASS", "");
                    Path = ini.ReadString(sz, "PATH", CX.DataPath);
                    User = ini.ReadString(sz, "USER", "");
                    Type = TypeFromString(ini.ReadString(sz, "TYPE", TypeToString(CXDatabaseType.Access)));
                    return ini.Save();
                }
                else return false;
            }
            else return false;
        }

        /// <summary>Close and reopen database connection with actual parameters. Returns true if succeed.</summary>
        public bool Open()
        {
            bool r = false;
            string f, s, t;
            Close();
            if (Type != CXDatabaseType.None)
            {
                if (alias.Trim().Length > 0) r = Load("", alias);
                else r = true;
                if (r)
                {
                    r = false;
                    s = ConnectionString();
                    if ((s == LastConnectionString) && (DateTime.Now < LastConnectionFailed))
                    {
                        CX.Raise("Failed database connection retried before minimum waiting time.", false);
                    }
                    else
                    {
                        LastConnectionString = s;
                        LastConnectionFailed = DateTime.MinValue;
                        try
                        {
                            if (Type == CXDatabaseType.Sql)
                            {
                                SqlDB = new SqlConnection(s);
                                SqlDB.Open();
                            }
                            else if (Type == CXDatabaseType.MySql)
                            {
                                MySqlDB = new MySqlConnection(s);
                                MySqlDB.Open();
                            }
                            else if (Type == CXDatabaseType.DBase4)
                            {
                                f = CX.Combine(Path, Database, "dbf");
                                if (CX.FileExists(f))
                                {
                                    OleDB = new OleDbConnection(s);
                                    OleDB.Open();
                                }
                            }
                            else if (Type == CXDatabaseType.PostgreSQL)
                            {
                                PostgreDB = new NpgsqlConnection(s);
                                PostgreDB.Open();
                            }
                            else
                            {
                                f = CX.Combine(Path, Database, "mdb");
                                if (!ClientMode && CX.FolderExists(CX.FilePath(f)))
                                {
                                    if (!CX.FileExists(f))
                                    {
                                        t = TemplatePath();
                                        if (t.Length > 0)
                                        {
                                            if (t.ToLower().Trim() != f.ToLower().Trim())
                                            {
                                                if (CX.FolderExists(CX.FilePath(f)))
                                                {
                                                    CX.FileCopy(t, f, CX.FileRetries);
                                                }
                                                else CX.Raise("Data path cannot be found.", true);
                                            }
                                        }
                                    }
                                }
                                if (CX.FileExists(f))
                                {
                                    OleDB = new OleDbConnection(s);
                                    OleDB.Open();
                                }
                                else CX.Raise("Data file not found.", true);
                            }
                        }
                        catch (Exception ex)
                        {
                            LastConnectionFailed = DateTime.Now.AddSeconds(5);
                            CX.Error(ex);
                        }
                        r = this.Active;
                        if (!r) this.Close();
                    }
                }
                else CX.Raise("Error reading database alias parameters.", false);
            }
            else CX.Raise("Attempt to open a database whose type was not specified.", false);
            return r;
        }

        /// <summary>Open database connection with parameters stored for alias.
        /// Returns true if succeed.</summary>
        public bool Open(string _Alias)
        {
            if (Load("", _Alias)) return Open();
            else return false;
        }

        /// <summary>Open database connection with database type, host name, database name, user name and password parameters.
        /// Returns true if succeed. </summary>
        public bool Open(CXDatabaseType _DatabaseType,
            string _DatabaseHost, string _DatabaseName, string _DatabaseTemplate,
            string _DatabasePath, string _UserName, string _Password)
        {
            bool r;
            Type = _DatabaseType;
            Host = _DatabaseHost;
            Database = _DatabaseName;
            Template = _DatabaseTemplate;
            Path = _DatabasePath;
            User = _UserName;
            Password = _Password;
            r = Open();
            return r;
        }

        /// <summary>Open DBase IV DBF database connection with file fileName.
        /// Returns true if succeed.</summary>
        public bool OpenDbf(string _FileName)
        {
            if (_FileName.Trim().Length > 0)
            {
                return Open(CXDatabaseType.DBase4, "localhost",
                    CX.FileNameWithoutExt(_FileName), "",
                    CX.FilePath(_FileName), "", "");
            }
            else return false;
        }

        /// <summary>Open Microsoft Access® MDB database connection with file fileName.
        /// Returns true if succeed.</summary>
        public bool OpenMdb(string _FileName)
        {
            if (_FileName.Trim().Length > 0)
            {
                return Open(CXDatabaseType.Access, "localhost", 
                    CX.FileNameWithoutExt(_FileName), "",
                    CX.FilePath(_FileName), "", 
                    DefaultAccessPassword);
            }
            else return false;
        }

        /// <summary>Save database parameters by alias to INI file. If filename is empty will be assumed default 
        /// application INI file. If alias is empty will be assumed actual database alias. Returns true if succeed.</summary>
        public bool Save(string _FileName, string _Alias)
        {
            string sz;
            CXIni ini;
            _Alias = _Alias.Trim().ToUpper();
            if (_Alias.Length < 1) _Alias = alias;
            if (_Alias.Length > 0)
            {
                ini = new CXIni(_FileName);
                sz = "DATABASE_ALIAS_" + _Alias;
                ini.WriteInteger(sz, "COMMAND_TIMEOUT", CommandTimeout);
                ini.WriteInteger(sz, "CONNECTION_TIMEOUT", ConnectionTimeout);
                ini.WriteString(sz, "TYPE", TypeToString(Type));
                ini.WriteString(sz, "DATABASE", Database);
                ini.WriteString(sz, "HOST", Host);
                ini.WriteHexMask(sz, "PASS", Password);
                ini.WriteString(sz, "PATH", Path);
                ini.WriteString(sz, "USER", User);
                alias = _Alias;
                return ini.Save();
            }
            else return false;
        }

        /// <summary>Returns return MDB file path represent local database template.</summary>
        public string TemplatePath()
        {
            if (Template.Trim().Length > 0) return CX.Combine(CX.OnLibraryPath(""), Template, "mdb");
            else return "";
        }

        /// <summary>Returns MDB file path represent local database temporary template.</summary>
        public string TemporaryPath()
        {
            return CX.Combine(CX.TempPath, Database, "mdb");
        }

        #endregion

        /* */

        #region Static methods

        /*  --------------------------------------------------------------------
         *  Static methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Add alias database to collection loading or creating parameters from application 
        /// default INI files. Parameters will be copied from existing alias specified with "create as alias" 
        /// parameter. Return alias databases collection index or -1 if fail.</summary>
        public static int Add(string _Alias, string _DatabaseTemplate)
        {
            int r = -1;
            CXDatabase db;
            _Alias = _Alias.Trim().ToUpper();
            _DatabaseTemplate = _DatabaseTemplate.Trim();
            if (_DatabaseTemplate.Length < 1) _DatabaseTemplate = DefaultExecutable;
            if (_Alias.Length > 0)
            {
                db = new CXDatabase();
                db.Alias = _Alias;
                db.Template = _DatabaseTemplate;
                if (db.Load("", _Alias))
                {
                    r = Databases.Count;
                    Databases.Add(db);
                }
                else db.Dispose();
            }
            return r;
        }

        /// <summary>Close all database connection. Returns true if succeed.</summary>
        public static bool CloseAll()
        {
            int i = 0;
            bool r = true;
            while (i < Databases.Count)
            {
                if (Databases[i] != null)
                {
                    if (!Databases[i].Close()) r = false;
                }
                i++;
            }
            CX.MemoryRelease(true);
            return r;
        }

        /// <summary>Compact MDB database file specified in file name parameter. Password must be setted 
        /// to security password to access database file or "" if not necessary. Return true if succeed.</summary>
        public static bool CompactMDB(string _FileName, string _Password)
        {
            bool r = false;
            object jro;
            object[] par;
            string tmp = CX.Combine(CX.FilePath(_FileName), CX.FileNameWithoutExt(_FileName) + "_tmp", "mdb");
            string bkp = CX.Combine(CX.FilePath(_FileName), CX.FileNameWithoutExt(_FileName), "bak");
            string src = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = " + _FileName;
            string tgt = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = " + tmp + "; Jet OLEDB:Engine Type=5";
            if (!CX.Empty(_Password))
            {
                tgt += "; Jet OLEDB:Database Password = " + _Password;
                src += "; Jet OLEDB:Database Password = " + _Password;
            }
            try
            {
                CX.FileDelete(tmp);
                jro = Activator.CreateInstance(System.Type.GetTypeFromProgID("JRO.JetEngine"));
                par = new object[] { src, tgt };
                jro.GetType().InvokeMember("CompactDatabase", System.Reflection.BindingFlags.InvokeMethod, null, jro, par);
                CX.FileDelete(bkp);
                if (CX.FileMove(_FileName, bkp, CX.FileRetries)) r = CX.FileMove(tmp, _FileName, CX.FileRetries);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(jro);
                jro = null;
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                r = false;
            }
            return r;
        }

        /// <summary>Return type of database by alias.</summary>
        public static CXDatabaseType GetType(string _Alias)
        {
            CXDatabase r = Keep(_Alias);
            if (r != null) return r.Type;
            else return CXDatabaseType.None;
        }

        /// <summary>Test if exists an alias database in collection. If exists try to open it. Returns database instance if succeed.</summary>
        public static CXDatabase Keep(string _Alias)
        {
            int i = 0;
            CXDatabase r = null;
            _Alias = _Alias.Trim().ToUpper();
            if (_Alias.Length > 0)
            {
                while ((r == null) && (i < Databases.Count))
                {
                    if (Databases[i].alias == _Alias) r = Databases[i];
                    i++;
                }
                if (r != null) r.Keep();
            }
            return r;
        }

        /// <summary>Try to lock database working area. Return true if succeed.</summary>
        public static bool Lock()
        {
            if (!Locked())
            {
                return CX.SaveString(CX.Combine(CX.DataPath, DefaultExecutable, "lck"),
                    DefaultExecutable + ';' + CX.Str(DateTime.Now) + ';' + CX.Machine() + ";" + CX.User());
            }
            else return false;
        }

        /// <summary>Return true if database working area is locked.</summary>
        public static bool Locked()
        {
            string f = CX.Combine(CX.DataPath, DefaultExecutable, "lck");
            if (CX.FileExists(f)) return CX.FileDate(f) > DateTime.Now.AddHours(-4);
            else return false;
        }

        /// <summary>Close, release and remove alias database to collection. Return true if succeed.</summary>
        public static bool Remove(string _Alias)
        {
            int i = 0, r = -1;
            _Alias = _Alias.Trim().ToUpper();
            while ((r < 0) && (i < Databases.Count))
            {
                if (Databases[i].alias == _Alias) r = i;
                i++;
            }
            if (r > -1)
            {
                Databases[r].Close();
                Databases[r].Dispose();
                Databases[r] = null;
                Databases.RemoveAt(r);
                return true;
            }
            else return false;
        }

        /// <summary>Returns database type corresponding to passed string.</summary>
        public static CXDatabaseType TypeFromString(string _DatabaseType)
        {
            _DatabaseType = _DatabaseType.Trim().ToUpper();
            if (_DatabaseType == "ACCESS") return CXDatabaseType.Access;
            else if (_DatabaseType == "SQLSVR") return CXDatabaseType.Sql;
            else if (_DatabaseType == "MYSQL") return CXDatabaseType.MySql;
            else if (_DatabaseType == "DBASE4") return CXDatabaseType.DBase4;
            else if (_DatabaseType == "POSTGRESQL") return CXDatabaseType.PostgreSQL;
            else return CXDatabaseType.None;
        }

        /// <summary>Returns string corresponding to database type passed.</summary>
        public static string TypeToString(CXDatabaseType _DatabaseType)
        {
            if (_DatabaseType == CXDatabaseType.Access) return "ACCESS";
            else if (_DatabaseType == CXDatabaseType.Sql) return "SQLSVR";
            else if (_DatabaseType == CXDatabaseType.MySql) return "MYSQL";
            else if (_DatabaseType == CXDatabaseType.DBase4) return "DBASE4";
            else if (_DatabaseType == CXDatabaseType.PostgreSQL) return "POSTGRESQL";
            else return "";
        }

        /// <summary>Try to unlock database working area. Return true if succeed.</summary>
        public static bool Unlock()
        {
            CX.FileDelete(CX.Combine(CX.DataPath, DefaultExecutable, "lck"));
            return !Locked();
        }

        #endregion

        /* */

    }

    /* */

}
