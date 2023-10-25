/*  ------------------------------------------------------------------------
 *  
 *  File:       CXDataset.cs
 *  Version:    1.0.0
 *  Date:       October 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD dataset component.
 *  
 *  ------------------------------------------------------------------------
 */

using MySql.Data.MySqlClient;
using Npgsql;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Timers;

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD dataset component.</summary>
    [ToolboxItem(true)]
    public partial class CXDataset : Component
    {

        /* */

        #region Declarations

        /*  --------------------------------------------------------------------
         *  Declarations
         *  --------------------------------------------------------------------
         */

        private string adaptedQuery;
        private string alias;
        private int changesBufferCount;
        private bool disposing = false;
        private bool readOnly;
        //
        private OleDbDataAdapter oleAdapter;
        private OleDbCommandBuilder oleBuilder;
        private OleDbCommand oleCommand;
        private OleDbDataReader oleReader;
        //
        private SqlDataAdapter sqlAdapter;
        private SqlCommandBuilder sqlBuilder;
        private SqlCommand sqlCommand;
        private SqlDataReader sqlReader;
        //
        private MySqlDataAdapter mySqlAdapter;
        private MySqlCommandBuilder mySqlBuilder;
        private MySqlCommand mySqlCommand;
        private MySqlDataReader mySqlReader;
        //
        private NpgsqlDataAdapter pgSqlAdapter;
        private NpgsqlCommandBuilder pgSqlBuilder;
        private NpgsqlCommand pgSqlCommand;
        private NpgsqlDataReader pgSqlReader;

        #endregion

        /* */

        #region Delegates and events

        /*  --------------------------------------------------------------------
         *  Delegates and events
         *  --------------------------------------------------------------------
         */

        /// <summary>Occurs after dataset completes a request to cancel modifications to the active record.</summary>
        public delegate void OnAfterCancel(object _Sender);
        /// <summary>Occurs after dataset completes a request to cancel modifications to the active record.</summary>
        public event OnAfterCancel AfterCancel;

        /// <summary>Occurs after dataset close query.</summary>
        public delegate void OnAfterClose(object _Sender);
        /// <summary>Occurs after dataset close query.</summary>
        public event OnAfterClose AfterClose;

        /// <summary>Occurs after record data fetch.</summary>
        public delegate void OnAfterFetch(object _Sender);
        /// <summary>Occurs after record data fetch.</summary>
        public event OnAfterFetch AfterFetch = null;

        /// <summary>Occurs after dataset deletes a record.</summary>
        public delegate void OnAfterDelete(object _Sender);
        /// <summary>Occurs after dataset deletes a record.</summary>
        public event OnAfterDelete AfterDelete;

        /// <summary>Occurs after dataset starts editing a record.</summary>
        public delegate void OnAfterEdit(object _Sender);
        /// <summary>Occurs after dataset starts editing a record.</summary>
        public event OnAfterEdit AfterEdit;

        /// <summary>Occurs after dataset inserts a new record.</summary>
        public delegate void OnAfterInsert(object _Sender);
        /// <summary>Occurs after dataset inserts a new record.</summary>
        public event OnAfterInsert AfterInsert;

        /// <summary>Occurs after dataset completes opening.</summary>
        public delegate void OnAfterOpen(object _Sender);
        /// <summary>Occurs after dataset completes opening.</summary>
        public event OnAfterOpen AfterOpen;

        /// <summary>Occurs after dataset posts modifications to the active record.</summary>
        public delegate void OnAfterPost(object _Sender);
        /// <summary>Occurs after dataset posts modifications to the active record.</summary>
        public event OnAfterPost AfterPost;

        /// <summary>Occurs before dataset executes a request to cancel changes to the active record.</summary>
        public delegate void OnBeforeCancel(object _Sender, ref bool _AbortOperation);
        /// <summary>Occurs before dataset executes a request to cancel changes to the active record.</summary>
        public event OnBeforeCancel BeforeCancel;

        /// <summary>Occurs before dataset close query.</summary>
        public delegate void OnBeforeClose(object _Sender, ref bool _AbortOperation);
        /// <summary>Occurs before dataset close query.</summary>
        public event OnBeforeClose BeforeClose;

        /// <summary>Occurs before dataset attempts to delete the active record.</summary>
        public delegate void OnBeforeDelete(object _Sender, ref bool _AbortOperation);
        /// <summary>Occurs before dataset attempts to delete the active record.</summary>
        public event OnBeforeDelete BeforeDelete;

        /// <summary>Occurs before dataset enters edit mode for the active record.</summary>
        public delegate void OnBeforeEdit(object _Sender, ref bool _AbortOperation);
        /// <summary>Occurs before dataset enters edit mode for the active record.</summary>
        public event OnBeforeEdit BeforeEdit;

        /// <summary>Occurs before dataset enters insert mode.</summary>
        public delegate void OnBeforeInsert(object _Sender, ref bool _AbortOperation);
        /// <summary>Occurs before dataset enters insert mode.</summary>
        public event OnBeforeInsert BeforeInsert;

        /// <summary>Occurs before dataset executes a request to open query.</summary>
        public delegate void OnBeforeOpen(object _Sender, ref bool _AbortOperation);
        /// <summary>Occurs before dataset executes a request to open query.</summary>
        public event OnBeforeOpen BeforeOpen;

        /// <summary>Occurs before dataset posts modifications to the active record.</summary>
        public delegate void OnBeforePost(object _Sender, ref bool _AbortOperation);
        /// <summary>Occurs before dataset posts modifications to the active record.</summary>
        public event OnBeforePost BeforePost;

        /// <summary>Occurs when dataset record bindings read needed.</summary>
        public delegate void OnBindingRead(object _Sender, string _ColumnName = null);
        /// <summary>Occurs when dataset record bindings read needed.</summary>
        public event OnBindingRead BindingRead;

        /// <summary>Occurs when dataset record bindings write needed.</summary>
        public delegate void OnBindingWrite(object _Sender);
        /// <summary>Occurs when dataset record bindings write needed.</summary>
        public event OnBindingWrite BindingWrite;

        /// <summary>Occurs when the state of dataset changes.</summary>
        public delegate void OnStateChange(object _Sender);
        /// <summary>Occurs when the state of dataset changes.</summary>
        public event OnStateChange StateChange;

        #endregion

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Dataset instance constructor.</summary>
        public CXDataset()
        {
            CX.Initialize();
            InitializeComponent();
            InitializeInstance();
        }

        /// <summary>Dataset instance constructor with db database connection.</summary>
        public CXDataset(CXDatabase _Database)
        {
            CX.Initialize();
            InitializeComponent();
            InitializeInstance();
            Database = _Database;
        }

        /// <summary>Dataset instance constructor with alias connection.</summary>
        public CXDataset(string _Alias)
        {
            CX.Initialize();
            InitializeComponent();
            InitializeInstance();
            alias = _Alias;
            Database = CXDatabase.Keep(alias);
        }

        /// <summary>Dataset instance constructor with ds dataset connection.</summary>
        public CXDataset(CXDataset _DataSet)
        {
            CX.Initialize();
            InitializeComponent();
            InitializeInstance();
            if (_DataSet != null)
            {
                if (_DataSet.Alias.Trim().Length > 0)
                {
                    alias = _DataSet.Alias;
                    Database = CXDatabase.Keep(alias);
                }
                else Database = _DataSet.Database;
            }
        }

        /// <summary>Dataset instance constructor with container</summary>
        public CXDataset(IContainer _Container)
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
            FetchTimer = new System.Timers.Timer()
            {
                Enabled = false,
            };
            FetchTimer.Elapsed += FetchTimerTick;
        }

        #endregion 

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Specifies whether or not dataset is open.</summary>
        [Browsable(true)]
        [Description("Specifies whether or not dataset is open.")]
        public bool Active
        {
            get { return State != CXDatasetState.Closed; }
            set
            {
                if (value) Open();
                else Close();
            }
        }

        /// <summary>Specifies database alias name to use with dataset.</summary>
        [Browsable(true)]
        [Description("Specifies database alias name to use with dataset.")]
        public string Alias
        {
            get { return alias; }
            set { alias = value.Trim().ToUpper(); }
        }

        /// <summary>Indicates whether the first record in the dataset is active.</summary>
        [Browsable(false)]
        public bool Bof { get; private set; }

        /// <summary>Indicates how many changes can buffered before writes them to the database.</summary>
        [Browsable(true)]
        [Description("Indicates how many changes can buffered before writes them to the database.")]
        public int ChangesBufferSize { get; set; }

        /// <summary>Get or set changes notify flag.</summary>
        [Browsable(true)]
        [Description("Get or set changes notify flag.")]
        public bool ChangesNotify { get; set; } = false;

        /// <summary>Specifies the database connection component to use.</summary>
        [Browsable(true)]
        [Description("Specifies the database connection component to use.")]
        public CXDatabase Database { get; set; }

        /// <summary>Indicates whether a dataset is positioned at the last record.</summary>
        [Browsable(false)]
        public bool Eof { get; private set; }

        /// <summary>Get or set form record data fetch delay interval in milliseconds (-1 disable, 0 = immediate).</summary>
        public int FetchDelay { get; set; }

        /// <summary>Get form record data fetch timer.</summary>
        public System.Timers.Timer FetchTimer { get; private set; } = null;

        /// <summary>Get or set if dataset has 12 char type parent unique id field (PID).</summary>
        [Browsable(false)]
        public bool HasParentUniqueId { get; set; }

        /// <summary>Get or set if dataset has last edit date system information field.</summary>
        [Browsable(false)]
        public bool HasSysDate { get; set; }

        /// <summary>Get or set if dataset has insert date system information field.</summary>
        [Browsable(false)]
        public bool HasSysInsert { get; set; }

        /// <summary>Get or set if dataset has last OS user system information field.</summary>
        [Browsable(false)]
        public bool HasSysOSUser { get; set; }

        /// <summary>Get or set if dataset has last user system information field.</summary>
        [Browsable(false)]
        public bool HasSysUser { get; set; }

        /// <summary>Get or set if dataset has 12 char type unique id field (ID)</summary>
        [Browsable(false)]
        public bool HasUniqueId { get; set; }

        /// <summary>Get internal dataset.</summary>
        public DataSet InternalDataSet { get; private set; }

        /// <summary>Contains the text of the SQL statement to execute for the dataset.</summary>
        [Browsable(true)]
        [Description("Contains the text of the SQL statement to execute for the dataset.")]
        public string Query { get; set; }

        /// <summary>Indicates the index of the current record in the dataset.</summary>
        [Browsable(false)]
        public int RecordIndex { get; private set; }

        /// <summary>Indicates the current active row of the dataset.</summary>
        [Browsable(false)]
        public DataRow Row { get; private set; }

        /// <summary>Indicates the current operating mode of the dataset.</summary>
        [Browsable(false)]
        public CXDatasetState State { get; private set; }

        /// <summary>Indicates the current table object open in dataset.</summary>
        [Browsable(false)]
        public DataTable Table { get; private set; }

        /// <summary>Indicates the current table name open in dataset.</summary>
        [Browsable(false)]
        public string TableName { get; private set; }

        /// <summary>Indicates if post method must update record system informations.</summary>
        [Browsable(false)]
        public bool UpdateRecordSysInformation { get; set; }

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Return last changes notify datetime.</summary>
        public DateTime Changed()
        {
            return Changed(TableName);
        }

        /// <summary>Return dataset database type.</summary>
        public CXDatabaseType DatabaseType()
        {
            if (Database == null) OpenDatabase();
            if (Database == null) return CXDatabaseType.None;
            else return Database.Type;
        }

        /// <summary>Return last changes notify datetime.</summary>
        public static DateTime Changed(string _TableName)
        {
            return CX.FileDate(ChangesFilePath(_TableName));
        }

        /// <summary>Update changes notify file.</summary>
        public bool Changes()
        {
            return Changes(TableName);
        }

        /// <summary>Update changes notify file.</summary>
        public static bool Changes(string _TableName)
        {
            const char sep = ';';
            string f = ChangesFilePath(_TableName);
            CX.FileDelete(f);
            return CX.SaveString(f, _TableName + sep + CX.Str(DateTime.Now, CXDateFormat.iso8601, true) + sep + CX.Machine() + sep + CX.User());
        }

        /// <summary>Return changes notify file path.</summary>
        public static string ChangesFilePath(string _TableName)
        {
            return CX.OnDataPath("Changes", CX.Combine("", _TableName, "chs"));
        }

        /// <summary>Set current dataset state to newState.</summary>
        public void ChangeState(CXDatasetState _State)
        {
            if (State != _State)
            {
                State = _State;
                if (BindingRead != null) BindingRead(this);
                if (StateChange != null) StateChange(this);
            }
        }

        /// <summary>Initialize and reset dataset variables.</summary>
        public void Clear()
        {
            adaptedQuery = "";
            alias = "";
            Bof = true;
            changesBufferCount = 0;
            ChangesBufferSize = 1;
            Database = null;
            FetchDelay = 0;
            Eof = true;
            HasParentUniqueId = false;
            HasSysDate = false;
            HasSysInsert = false;
            HasSysOSUser = false;
            HasSysUser = false;
            HasUniqueId = false;
            InternalDataSet = null;
            mySqlAdapter = null;
            mySqlBuilder = null;
            mySqlCommand = null;
            mySqlReader = null;
            oleAdapter = null;
            oleBuilder = null;
            oleCommand = null;
            oleReader = null;
            pgSqlAdapter = null;
            pgSqlBuilder = null;
            pgSqlCommand = null;
            pgSqlReader = null;
            Query = "";
            readOnly = true;
            RecordIndex = -1;
            Row = null;
            State = CXDatasetState.Closed;
            sqlAdapter = null;
            sqlBuilder = null;
            sqlCommand = null;
            sqlReader = null;
            Table = null;
            TableName = "";
            UpdateRecordSysInformation = true;
        }

        /// <summary>Close dataset. Return true if succeed.</summary>
        public bool Close()
        {
            bool r = false, cancel = false;
            if (BeforeClose != null) BeforeClose(this, ref cancel);
            if (!cancel)
            {
                try
                {
                    if (Database != null)
                    {
                        if (Database.Active)
                        {
                            if (changesBufferCount > 0) Commit();
                            //
                            if (oleReader != null)
                            {
                                oleReader.Close();
                                oleReader.Dispose();
                            }
                            if (sqlReader != null)
                            {
                                sqlReader.Close();
                                sqlReader.Dispose();
                            }
                            if (mySqlReader != null)
                            {
                                mySqlReader.Close();
                                mySqlReader.Dispose();
                            }
                            if (pgSqlReader != null)
                            {
                                pgSqlReader.Close();
                                pgSqlReader.Dispose();
                            }
                            //
                            if (oleCommand != null) oleCommand.Dispose();
                            if (sqlCommand != null) sqlCommand.Dispose();
                            if (mySqlCommand != null) mySqlCommand.Dispose();
                            if (pgSqlCommand != null) pgSqlCommand.Dispose();
                            //
                            if (oleBuilder != null) oleBuilder.Dispose();
                            if (sqlBuilder != null) sqlBuilder.Dispose();
                            if (mySqlBuilder != null) mySqlBuilder.Dispose();
                            if (pgSqlBuilder != null) pgSqlBuilder.Dispose();
                            //
                            if (InternalDataSet != null) InternalDataSet.Dispose();
                            //
                            if (oleAdapter != null) oleAdapter.Dispose();
                            if (sqlAdapter != null) sqlAdapter.Dispose();
                            if (mySqlAdapter != null) mySqlAdapter.Dispose();
                            if (pgSqlAdapter != null) pgSqlAdapter.Dispose();
                        }
                    }
                    r = true;
                }
                catch (Exception ex)
                {
                    CX.Error(ex);
                }
                //
                ChangeState(CXDatasetState.Closed);
                //
                oleReader = null;
                sqlReader = null;
                mySqlReader = null;
                pgSqlReader = null;
                //
                oleCommand = null;
                sqlCommand = null;
                mySqlCommand = null;
                pgSqlCommand = null;
                //
                oleBuilder = null;
                sqlBuilder = null;
                mySqlBuilder = null;
                pgSqlBuilder = null;
                //
                InternalDataSet = null;
                //
                oleAdapter = null;
                sqlAdapter = null;
                mySqlAdapter = null;
                pgSqlAdapter = null;
                //
                Table = null;
                Row = null;
                Eof = true;
                Bof = true;
                //
                if (AfterClose != null) AfterClose(this);
            }
            //
            return r;
        }

        /// <summary>Return true if fieldName is the name of one of any table open field.</summary>
        public bool IsField(string _FieldName)
        {
            if (_FieldName.Length < 1) return false;
            else if (Table != null) return Table.Columns.IndexOf(_FieldName) > -1;
            else if ((Database.Type == CXDatabaseType.Access) && (oleReader != null)) return oleReader.GetOrdinal(_FieldName) > -1;
            else if ((Database.Type == CXDatabaseType.Sql) && (sqlReader != null)) return sqlReader.GetOrdinal(_FieldName) > -1;
            else if ((Database.Type == CXDatabaseType.MySql) && (mySqlReader != null)) return mySqlReader.GetOrdinal(_FieldName) > -1;
            else if ((Database.Type == CXDatabaseType.PostgreSql) && (oleReader != null)) return pgSqlReader.GetOrdinal(_FieldName) > -1;
            else if ((Database.Type == CXDatabaseType.DBase4) && (oleReader != null)) return oleReader.GetOrdinal(_FieldName) > -1;
            else return false;
        }

        /// <summary>Return database already specified for this dataset or by alias.</summary>
        public CXDatabase Keep()
        {
            if (Database != null) Database.Keep();
            else Database = CXDatabase.Keep(alias);
            return Database;
        }

        /// <summary>Load the recordset. Return true if succeed.</summary>
        public bool Load()
        {
            Table = null;
            Row = null;
            TableName = "";
            try
            {
                if (OpenDatabase())
                {
                    InternalDataSet.Clear();
                    if (Database.Type == CXDatabaseType.Access) oleAdapter.Fill(InternalDataSet);
                    else if (Database.Type == CXDatabaseType.Sql) sqlAdapter.Fill(InternalDataSet);
                    else if (Database.Type == CXDatabaseType.MySql) mySqlAdapter.Fill(InternalDataSet);
                    else if (Database.Type == CXDatabaseType.PostgreSql) pgSqlAdapter.Fill(InternalDataSet);
                    else if (Database.Type == CXDatabaseType.DBase4) oleAdapter.Fill(InternalDataSet);
                    if (InternalDataSet.Tables.Count > 0) Table = InternalDataSet.Tables[0];
                    else Table = null;
                    if (Table==null)
                    {
                        HasUniqueId = false;
                        HasParentUniqueId = false;
                        HasSysDate = false;
                        HasSysInsert = false;
                        HasSysOSUser = false;
                        HasSysUser = false;
                    }
                    else
                    {
                        HasUniqueId = Table.Columns.IndexOf("ID") > -1;
                        if (HasUniqueId) HasUniqueId = Table.Columns["ID"].MaxLength == CX.UniqueIdLength;
                        HasParentUniqueId = Table.Columns.IndexOf("PID") > -1;
                        if (HasParentUniqueId) HasParentUniqueId = Table.Columns["PID"].MaxLength == CX.UniqueIdLength;
                        HasSysDate = Table.Columns.IndexOf("Sys_Date") > -1;
                        HasSysInsert = Table.Columns.IndexOf("Sys_Insert") > -1;
                        HasSysOSUser = Table.Columns.IndexOf("Sys_OSUser") > -1;
                        HasSysUser = Table.Columns.IndexOf("Sys_User") > -1;
                    }
                    TableName = CX.SqlTableName(adaptedQuery);
                    if (!readOnly && HasUniqueId)
                    {
                        if (Database.Type == CXDatabaseType.Access)
                        {
                            // Insert command
                            oleAdapter.InsertCommand.CommandText = InsertCommand(this);
                            if (Database.CommandTimeout > 0) oleAdapter.InsertCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(oleAdapter.InsertCommand);
                            // Update command
                            oleAdapter.UpdateCommand.CommandText = UpdateCommand(this);
                            if (Database.CommandTimeout > 0) oleAdapter.UpdateCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(oleAdapter.UpdateCommand);
                            // Delete command
                            oleAdapter.DeleteCommand.CommandText = DeleteCommand(this);
                            if (Database.CommandTimeout > 0) oleAdapter.DeleteCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(oleAdapter.DeleteCommand);
                        }
                        else if (Database.Type == CXDatabaseType.Sql)
                        {
                            // Insert command
                            sqlAdapter.InsertCommand.CommandText = InsertCommand(this);
                            if (Database.CommandTimeout > 0) sqlAdapter.InsertCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(sqlAdapter.InsertCommand);
                            // Update command
                            sqlAdapter.UpdateCommand.CommandText = UpdateCommand(this);
                            if (Database.CommandTimeout > 0) sqlAdapter.UpdateCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(sqlAdapter.UpdateCommand);
                            // Delete command
                            sqlAdapter.DeleteCommand.CommandText = DeleteCommand(this);
                            if (Database.CommandTimeout > 0) sqlAdapter.DeleteCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(sqlAdapter.DeleteCommand);
                        }
                        else if (Database.Type == CXDatabaseType.MySql)
                        {
                            // Insert command
                            mySqlAdapter.InsertCommand.CommandText = InsertCommand(this);
                            if (Database.CommandTimeout > 0) mySqlAdapter.InsertCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(mySqlAdapter.InsertCommand);
                            // Update command
                            mySqlAdapter.UpdateCommand.CommandText = UpdateCommand(this);
                            if (Database.CommandTimeout > 0) mySqlAdapter.UpdateCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(mySqlAdapter.UpdateCommand);
                            // Delete command
                            mySqlAdapter.DeleteCommand.CommandText = DeleteCommand(this);
                            if (Database.CommandTimeout > 0) mySqlAdapter.DeleteCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(mySqlAdapter.DeleteCommand);
                        }
                        else if (Database.Type == CXDatabaseType.PostgreSql)
                        {
                            // Insert command
                            pgSqlAdapter.InsertCommand.CommandText = InsertCommand(this);
                            if (Database.CommandTimeout > 0) pgSqlAdapter.InsertCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(pgSqlAdapter.InsertCommand);
                            // Update command
                            pgSqlAdapter.UpdateCommand.CommandText = UpdateCommand(this);
                            if (Database.CommandTimeout > 0) pgSqlAdapter.UpdateCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(pgSqlAdapter.UpdateCommand);
                            // Delete command
                            pgSqlAdapter.DeleteCommand.CommandText = DeleteCommand(this);
                            if (Database.CommandTimeout > 0) pgSqlAdapter.DeleteCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(pgSqlAdapter.DeleteCommand);
                        }
                        else if (Database.Type == CXDatabaseType.DBase4)
                        {
                            // Insert command
                            oleAdapter.InsertCommand.CommandText = InsertCommand(this);
                            if (Database.CommandTimeout > 0) oleAdapter.InsertCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(oleAdapter.InsertCommand);
                            // Update command
                            oleAdapter.UpdateCommand.CommandText = UpdateCommand(this);
                            if (Database.CommandTimeout > 0) oleAdapter.UpdateCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(oleAdapter.UpdateCommand);
                            // Delete command
                            oleAdapter.DeleteCommand.CommandText = DeleteCommand(this);
                            if (Database.CommandTimeout > 0) oleAdapter.DeleteCommand.CommandTimeout = Database.CommandTimeout;
                            ParametersByName(oleAdapter.DeleteCommand);
                        }
                    }
                    if (Table != null)
                    {
                        ChangeState(CXDatasetState.Browse);
                        First();
                    }
                    if (AfterOpen != null) AfterOpen(this);
                }
            }
            catch (Exception ex)
            {
                Close();
                CX.Error(ex);
            }
            return State == CXDatasetState.Browse;
        }

        /// <summary>Open dataset with query specified in Query property. Return true if succeed.</summary>
        public bool Open()
        {
            return Open(Query);
        }

        /// <summary>Open dataset with selection query specified in parameter. Return true if succeed.</summary>
        public bool Open(string _SqlSelectQuery)
        {
            return Open(_SqlSelectQuery, false);
        }

        /// <summary>Open dataset with query specified in sqlQuery parameter. 
        /// If readOnly is true dataset will be opened in read-only mode. 
        /// Return true if succeed.</summary>
        public bool Open(string _SqlSelectQuery, bool _ReadOnly)
        {
            bool r = false;
            Close();
            if (OpenDatabase())
            {
                bool cancel = false;
                if (BeforeOpen != null) BeforeOpen(this, ref cancel);
                if (!cancel)
                {
                    adaptedQuery = CX.SqlDelimiters(CX.SqlMacros(_SqlSelectQuery, Database.Type), Database.Type);
                    if (_ReadOnly) readOnly = true;
                    else readOnly = CX.Btw(adaptedQuery.ToLower(), " from ", " on ").IndexOf("join") > -1;
                    try
                    {
                        InternalDataSet = new DataSet();
                        try
                        {
                            if (Database.Type == CXDatabaseType.Access)
                            {
                                oleAdapter = new OleDbDataAdapter(adaptedQuery, Database.OleDB);
                                oleBuilder = new OleDbCommandBuilder(oleAdapter);
                                oleBuilder.QuotePrefix = "" + CXDatabase.SqlPrefix;
                                oleBuilder.QuoteSuffix = "" + CXDatabase.SqlSuffix;
                                oleAdapter.FillSchema(InternalDataSet, SchemaType.Source);
                                if (!readOnly)
                                {
                                    oleAdapter.InsertCommand = oleBuilder.GetInsertCommand();
                                    if (Database.CommandTimeout > 0) oleAdapter.InsertCommand.CommandTimeout = Database.CommandTimeout;
                                    oleAdapter.UpdateCommand = oleBuilder.GetUpdateCommand();
                                    if (Database.CommandTimeout > 0) oleAdapter.UpdateCommand.CommandTimeout = Database.CommandTimeout;
                                    oleAdapter.DeleteCommand = oleBuilder.GetDeleteCommand();
                                    if (Database.CommandTimeout > 0) oleAdapter.DeleteCommand.CommandTimeout = Database.CommandTimeout;
                                }
                            }
                            else if (Database.Type == CXDatabaseType.DBase4)
                            {
                                oleAdapter = new OleDbDataAdapter(adaptedQuery, Database.OleDB);
                                oleBuilder = new OleDbCommandBuilder(oleAdapter);
                                oleBuilder.QuotePrefix = "" + CXDatabase.SqlPrefix;
                                oleBuilder.QuoteSuffix = "" + CXDatabase.SqlSuffix;
                                oleAdapter.FillSchema(InternalDataSet, SchemaType.Source);
                                if (!readOnly)
                                {
                                    oleAdapter.InsertCommand = oleBuilder.GetInsertCommand();
                                    if (Database.CommandTimeout > 0) oleAdapter.InsertCommand.CommandTimeout = Database.CommandTimeout;
                                    oleAdapter.UpdateCommand = oleBuilder.GetUpdateCommand();
                                    if (Database.CommandTimeout > 0) oleAdapter.UpdateCommand.CommandTimeout = Database.CommandTimeout;
                                    oleAdapter.DeleteCommand = oleBuilder.GetDeleteCommand();
                                    if (Database.CommandTimeout > 0) oleAdapter.DeleteCommand.CommandTimeout = Database.CommandTimeout;
                                }
                            }
                            else if (Database.Type == CXDatabaseType.MySql)
                            {
                                mySqlAdapter = new MySqlDataAdapter(adaptedQuery, Database.MySqlDB);
                                mySqlBuilder = new MySqlCommandBuilder(mySqlAdapter);
                                mySqlBuilder.QuotePrefix = "" + CXDatabase.MySqlPrefix;
                                mySqlBuilder.QuoteSuffix = "" + CXDatabase.MySqlSuffix;
                                mySqlAdapter.FillSchema(InternalDataSet, SchemaType.Source);
                                if (!readOnly)
                                {
                                    mySqlAdapter.InsertCommand = mySqlBuilder.GetInsertCommand();
                                    if (Database.CommandTimeout > 0) mySqlAdapter.InsertCommand.CommandTimeout = Database.CommandTimeout;
                                    mySqlAdapter.UpdateCommand = mySqlBuilder.GetUpdateCommand();
                                    if (Database.CommandTimeout > 0) mySqlAdapter.UpdateCommand.CommandTimeout = Database.CommandTimeout;
                                    mySqlAdapter.DeleteCommand = mySqlBuilder.GetDeleteCommand();
                                    if (Database.CommandTimeout > 0) mySqlAdapter.DeleteCommand.CommandTimeout = Database.CommandTimeout;
                                }
                            }
                            else if (Database.Type == CXDatabaseType.PostgreSql)
                            {
                                pgSqlAdapter = new NpgsqlDataAdapter(adaptedQuery, Database.PostgreSqlDB);
                                pgSqlBuilder = new NpgsqlCommandBuilder(pgSqlAdapter);
                                pgSqlBuilder.QuotePrefix = "" + CXDatabase.PostgreSqlPrefix;
                                pgSqlBuilder.QuoteSuffix = "" + CXDatabase.PostgreSqlSuffix;
                                pgSqlAdapter.FillSchema(InternalDataSet, SchemaType.Source);
                                if (!readOnly)
                                {
                                    pgSqlAdapter.InsertCommand = pgSqlBuilder.GetInsertCommand();
                                    if (Database.CommandTimeout > 0) pgSqlAdapter.InsertCommand.CommandTimeout = Database.CommandTimeout;
                                    pgSqlAdapter.UpdateCommand = pgSqlBuilder.GetUpdateCommand();
                                    if (Database.CommandTimeout > 0) pgSqlAdapter.UpdateCommand.CommandTimeout = Database.CommandTimeout;
                                    pgSqlAdapter.DeleteCommand = pgSqlBuilder.GetDeleteCommand();
                                    if (Database.CommandTimeout > 0) pgSqlAdapter.DeleteCommand.CommandTimeout = Database.CommandTimeout;
                                }
                            }
                            else
                            {
                                sqlAdapter = new SqlDataAdapter(adaptedQuery, Database.SqlDB);
                                sqlBuilder = new SqlCommandBuilder(sqlAdapter);
                                sqlBuilder.QuotePrefix = "" + CXDatabase.SqlPrefix;
                                sqlBuilder.QuoteSuffix = "" + CXDatabase.SqlSuffix;
                                sqlAdapter.FillSchema(InternalDataSet, SchemaType.Source);
                                if (!readOnly)
                                {
                                    sqlAdapter.InsertCommand = sqlBuilder.GetInsertCommand();
                                    if (Database.CommandTimeout > 0) sqlAdapter.InsertCommand.CommandTimeout = Database.CommandTimeout;
                                    sqlAdapter.UpdateCommand = sqlBuilder.GetUpdateCommand();
                                    if (Database.CommandTimeout > 0) sqlAdapter.UpdateCommand.CommandTimeout = Database.CommandTimeout;
                                    sqlAdapter.DeleteCommand = sqlBuilder.GetDeleteCommand();
                                    if (Database.CommandTimeout > 0) sqlAdapter.DeleteCommand.CommandTimeout = Database.CommandTimeout;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            CX.Error(ex.Message + " on query: " + adaptedQuery, ex);
                        }
                        Load();
                    }
                    catch (Exception ex)
                    {
                        CX.Error(ex.Message + " on query: " + adaptedQuery, ex);
                    }
                    r = State != CXDatasetState.Closed;
                }
                else
                {
                    CX.Raise("Dataset open cancelled.", false);
                    r = false;
                }
            }
            return r;
        }

        /// <summary>Open dataset with query specified in sqlQuery parameter. Return true if succeed
        /// and if the table contains at least one record.</summary>
        public bool OpenAtLeast(string _SQLSelectionQuery)
        {
            bool r = false;
            if (Open(_SQLSelectionQuery))
            {
                if (Eof) Close();
                else r = true;
            }
            return r;
        }

        /// <summary>Keep dataset database connection with database Alias property 
        /// or Database property. Return true if succeed.</summary>
        public bool OpenDatabase()
        {
            bool r = false;
            if (alias.Trim().Length > 0) Database = CXDatabase.Keep(alias);
            else if (Database != null) Database.Keep();
            if (Database == null) CX.Raise("Null database error.", false);
            else
            {
                r = Database.Active;
                if (!r)
                {
                    CX.DoEvents();
                    r = Database.Open();
                }
                if (!r) CX.Raise("Error opening database.", false);
            }
            return r;
        }

        #endregion

        /* */

        #region Methods - Commands

        /*  --------------------------------------------------------------------
         *  Methods - Commands
         *  --------------------------------------------------------------------
         */

        /// <summary>Return string containing SQL parameterized syntax for dataset delete command.
        /// Require use of unique id field (ID).</summary>
        public static string DeleteCommand(CXDataset _Dataset)
        {
            return "DELETE FROM " + CX.SqlQuoteId(_Dataset.TableName, _Dataset.Database.Type)
                + " WHERE " + CX.SqlQuoteId("ID", _Dataset.Database.Type) + "=@ID";
        }

        /// <summary>Return string containing SQL parameterized syntax for dataset insert command.
        /// Require use of unique id field (ID).</summary>
        public static string InsertCommand(CXDataset _Dataset)
        {
            string comma = "";
            StringBuilder r = new StringBuilder(), v = new StringBuilder();
            r.Append("INSERT INTO ");
            r.Append(CX.SqlQuoteId(_Dataset.TableName, _Dataset.Database.Type));
            r.Append(" (");
            for (int i = 0; i < _Dataset.Table.Columns.Count; i++)
            {
                if (!_Dataset.Table.Columns[i].AutoIncrement)
                {
                    r.Append(comma);
                    r.Append(CX.SqlQuoteId(_Dataset.Table.Columns[i].ColumnName, _Dataset.Database.Type));
                    v.Append(comma);
                    v.Append('@');
                    v.Append(_Dataset.Table.Columns[i].ColumnName);
                    comma = ",";
                }
            }
            r.Append(") VALUES(");
            r.Append(v.ToString());
            r.Append(')');
            return r.ToString();
        }

        /// <summary>Set OleDb command names with char @ followed by field name wich related parameter (SourceColumn).</summary>
        public static void ParametersByName(OleDbCommand _OleDbCommand)
        {
            int i;
            string s;
            for (i = 0; i < _OleDbCommand.Parameters.Count; i++)
            {
                s = "@" + _OleDbCommand.Parameters[i].SourceColumn;
                if (_OleDbCommand.Parameters.IndexOf(s) < 0) _OleDbCommand.Parameters[i].ParameterName = s;
            }
        }

        /// <summary>Set SQL command names with char @ followed by field name wich related parameter (SourceColumn).</summary>
        public static void ParametersByName(SqlCommand _SqlCommand)
        {
            int i;
            string s;
            for (i = 0; i < _SqlCommand.Parameters.Count; i++)
            {
                s = "@" + _SqlCommand.Parameters[i].SourceColumn;
                if (_SqlCommand.Parameters.IndexOf(s) < 0) _SqlCommand.Parameters[i].ParameterName = s;
            }
        }

        /// <summary>Set MySQL command names with char @ followed by field name wich related parameter (SourceColumn).</summary>
        public static void ParametersByName(MySqlCommand _MySqlCommand)
        {
            int i;
            string s;
            for (i = 0; i < _MySqlCommand.Parameters.Count; i++)
            {
                s = "@" + _MySqlCommand.Parameters[i].SourceColumn;
                if (_MySqlCommand.Parameters.IndexOf(s) < 0) _MySqlCommand.Parameters[i].ParameterName = s;
            }
        }

        /// <summary>Set PostgreSQL command names with char @ followed by field name wich related parameter (SourceColumn).</summary>
        public static void ParametersByName(NpgsqlCommand _PgSqlCommand)
        {
            int i;
            string s;
            for (i = 0; i < _PgSqlCommand.Parameters.Count; i++)
            {
                s = "@" + _PgSqlCommand.Parameters[i].SourceColumn;
                if (_PgSqlCommand.Parameters.IndexOf(s) < 0) _PgSqlCommand.Parameters[i].ParameterName = s;
            }
        }

        /// <summary>Return string containing SQL parameterized syntax for dataset update command.
        /// Require use of unique id field (ID).</summary>
        public static string UpdateCommand(CXDataset _Dataset)
        {
            string q = "";
            StringBuilder r = new StringBuilder();
            r.Append("UPDATE ");
            r.Append(CX.SqlQuoteId(_Dataset.TableName, _Dataset.Database.Type));
            r.Append(" SET ");
            for (int i = 0; i < _Dataset.Table.Columns.Count; i++)
            {
                if (!_Dataset.Table.Columns[i].AutoIncrement)
                {
                    r.Append(q);
                    r.Append(CX.SqlQuoteId(_Dataset.Table.Columns[i].ColumnName, _Dataset.Database.Type));
                    r.Append(@"=@");
                    r.Append(_Dataset.Table.Columns[i].ColumnName);
                    q = ",";
                }
            }
            r.Append(" WHERE ");
            r.Append(CX.SqlQuoteId("ID", _Dataset.Database.Type));
            r.Append(@"=@ID");
            return r.ToString();
        }

        #endregion

        /* */

        #region Methods - Fields

        /*  --------------------------------------------------------------------
         *  Methods - Fields
         *  --------------------------------------------------------------------
         */

        /// <summary>Return correct blank value for data column type.</summary>
        public static object Blank(DataColumn _DataColumn)
        {
            if (_DataColumn.AutoIncrement) return 0;
            else if ((_DataColumn.ColumnName == "ID") && (_DataColumn.MaxLength == CX.UniqueIdLength)) return CX.UniqueId();
            else if (_DataColumn.DataType == CXDataType.Boolean) return false;
            else if (_DataColumn.DataType == CXDataType.Byte) return 0;
            else if (_DataColumn.DataType == CXDataType.Char) return ' ';
            else if (_DataColumn.DataType == CXDataType.DateTime) return DBNull.Value;
            else if (_DataColumn.DataType == CXDataType.Decimal) return 0.0d;
            else if (_DataColumn.DataType == CXDataType.Double) return 0.0d;
            else if (_DataColumn.DataType == CXDataType.Int16) return 0;
            else if (_DataColumn.DataType == CXDataType.Int32) return 0;
            else if (_DataColumn.DataType == CXDataType.Int64) return 0;
            else if (_DataColumn.DataType == CXDataType.SByte) return 0;
            else if (_DataColumn.DataType == CXDataType.Single) return 0.0f;
            else if (_DataColumn.DataType == CXDataType.String) return "";
            else if (_DataColumn.DataType == CXDataType.TimeSpan) return DBNull.Value;
            else if (_DataColumn.DataType == CXDataType.UInt16) return 0;
            else if (_DataColumn.DataType == CXDataType.UInt32) return 0;
            else if (_DataColumn.DataType == CXDataType.UInt64) return 0;
            else return DBNull.Value;
        }

        /// <summary>Return object related to field of current active record.</summary>
        public object Field(string _FieldName)
        {
            int i;
            try
            {
                if (State == CXDatasetState.Read)
                {
                    if (Database.Type == CXDatabaseType.Access)
                    {
                        i = oleReader.GetOrdinal(_FieldName);
                        if (i > -1) return oleReader[i];
                        else return null;
                    }
                    else if (Database.Type == CXDatabaseType.Sql)
                    {
                        i = sqlReader.GetOrdinal(_FieldName);
                        if (i > -1) return sqlReader[i];
                        else return null;
                    }
                    else if (Database.Type == CXDatabaseType.MySql)
                    {
                        i = mySqlReader.GetOrdinal(_FieldName);
                        if (i > -1) return mySqlReader[i];
                        else return null;
                    }
                    else if (Database.Type == CXDatabaseType.PostgreSql)
                    {
                        i = pgSqlReader.GetOrdinal(_FieldName);
                        if (i > -1) return pgSqlReader[i];
                        else return null;
                    }
                    else if (Database.Type == CXDatabaseType.DBase4)
                    {
                        i = oleReader.GetOrdinal(_FieldName);
                        if (i > -1) return oleReader[i];
                        else return null;
                    }
                    else return null;
                }
                else if (Row != null)
                {
                    if ((Row.RowState != DataRowState.Deleted) && (Row.RowState != DataRowState.Detached)) return Row[_FieldName];
                    else return null;
                }
                else return null;
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                return null;
            }
        }

        /// <summary>Return char related to field value of current active record.</summary>
        public char FieldChar(string _FieldName)
        {
            object o = Field(_FieldName);
            if (o != null) return (o.ToString().Trim() + " ")[0];
            else return ' ';
        }

        /// <summary>Return string related to field value of current active record.</summary>
        public string FieldStr(string _FieldName)
        {
            object o = Field(_FieldName);
            if (o != null) return o.ToString();
            else return "";
        }

        /// <summary>Return string related to field value of current active record
        /// and formatted with format specifications.</summary>
        public string FieldStr(string _FieldName, string _FormatString)
        {
            object o = Field(_FieldName);
            if (o != null) return CX.Str(o.ToString(), _FormatString);
            else return "";
        }

        /// <summary>Return integer related to field value of current active record.</summary>
        public int FieldInt(string _FieldName)
        {
            object o = Field(_FieldName);
            if (o != null) return CX.ToInt(o.ToString());
            else return 0;
        }

        /// <summary>Return long integer related to field value of current active record.</summary>
        public long FieldLong(string _FieldName)
        {
            object o = Field(_FieldName);
            if (o != null) return CX.ToLong(o.ToString());
            else return 0;
        }

        /// <summary>Return double related to field value of current active record.</summary>
        public double FieldDouble(string _FieldName)
        {
            object o = Field(_FieldName);
            if (o != null) return CX.ToDouble(o.ToString());
            else return 0.0d;
        }

        /// <summary>Return date related to field value of current active record.</summary>
        public DateTime FieldDate(string _FieldName)
        {
            object o = Field(_FieldName);
            if (o != null) return CX.Date(o.ToString(), CX.DateFormat, false);
            else return DateTime.MinValue;
        }

        /// <summary>Return datetime related to field value of current active record.</summary>
        public DateTime FieldDateTime(string _FieldName)
        {
            object o = Field(_FieldName);
            if (o != null) return CX.Date(o.ToString(), CX.DateFormat, true);
            else return DateTime.MinValue;
        }

        /// <summary>Return time related to field value of current active record.</summary>
        public DateTime FieldTime(string _FieldName)
        {
            object o = Field(_FieldName);
            if (o != null) return CX.Time(o.ToString());
            else return DateTime.MinValue;
        }

        /// <summary>Return bool related to field value of current active record.</summary>
        public bool FieldBool(string _FieldName)
        {
            object o = Field(_FieldName);
            if (o != null)
            {
                if (o is bool) return (bool)o;
                else return CX.Bool(o.ToString());
            }
            else return false;
        }

        /// <summary>Return byte array with blob content of field of current active record.</summary>
        public byte[] FieldBlob(string _FieldName)
        {
            object o = Field(_FieldName);
            if (o != DBNull.Value) return (byte[])Field(_FieldName);
            else return null;
        }

        /// <summary>Load blob content of field from file. Return blob size or -1 if fail.</summary>
        public int FieldBlobLoad(string _FieldName, string _FileName)
        {
            int r = -1;
            byte[] b;
            try
            {
                if (CX.FileExists(_FileName))
                {
                    b = CX.FileLoad(_FileName);
                    if (Assign(_FieldName, b))
                    {
                        if (b != null) r = b.Length;
                        else r = 0;
                    }
                }
                else r = -1;
            }
            catch (Exception ex)
            {
                r = -1;
                CX.Error(ex);
            }
            return r;
        }

        /// <summary>Save blob content of field which name is fieldName of current active record 
        /// in to file fileName. Return blob size or -1 if fail.</summary>
        public int FieldBlobSave(string _FieldName, string _FileName)
        {
            int r;
            byte[] b;
            object o;
            FileStream fs;
            BinaryWriter bw;
            try
            {
                o = Field(_FieldName);
                if (o != DBNull.Value)
                {
                    b = (byte[])Field(_FieldName);
                    fs = new FileStream(_FileName, System.IO.FileMode.Create);
                    bw = new BinaryWriter(fs);
                    try
                    {
                        bw.Write(b);
                        r = b.Length;
                    }
                    catch (Exception ex)
                    {
                        r = -1;
                        CX.Error(ex);
                    }
                    bw.Close();
                    fs.Close();
                    fs.Dispose();
                }
                else r = 0;
            }
            catch (Exception ex)
            {
                r = -1;
                CX.Error(ex);
            }
            return r;
        }

        /// <summary>Return Image with blob content of field of current active record.</summary>
        public Image FieldImage(string _FieldName)
        {
            byte[] b;
            object o = Field(_FieldName);
            MemoryStream ms;
            Image r = null;
            if (o != DBNull.Value)
            {
                b = (byte[])Field(_FieldName);
                if (b != null)
                {
                    ms = new MemoryStream(b);
                    if (b.Length > 0) r = Image.FromStream(ms);
                    ms.Dispose();
                }
            }
            return r;
        }

        /// <summary>Return integer array with fields map.</summary>
        public int[] FieldsMap(string[] _KeyFields)
        {
            int i;
            int[] r = null;
            if (_KeyFields != null)
            {
                r = new int[_KeyFields.Length];
                for (i = 0; i < _KeyFields.Length; i++) r[i] = Table.Columns.IndexOf(_KeyFields[i]);
            }
            return r;
        }

        #endregion

        /* */

        #region Methods - Browsing

        /*  --------------------------------------------------------------------
         *  Methods - Browsing
         *  --------------------------------------------------------------------
         */

        /// <summary>Return true if current record is not deleted and not detached.</summary>
        public bool DataReady()
        {
            if ((State != CXDatasetState.Closed) && (Row != null))
            {
                return (Row.RowState != DataRowState.Deleted) && (Row.RowState != DataRowState.Detached);
            }
            else return false;
        }

        /// <summary>Return true if current record is deleted or detached.</summary>
        public bool Deleted()
        {
            if (Row != null)
            {
                return (Row.RowState == DataRowState.Deleted) || (Row.RowState == DataRowState.Detached);
            }
            else return true;
        }

        /// <summary>Perform current record data fetch related.</summary>
        private void Fetch()
        {
            if (!disposing)
            {
                if (FetchTimer != null)
                {
                    FetchTimer.Enabled = false;
                    if (FetchDelay > 0)
                    {
                        FetchTimer.Interval = FetchDelay;
                        FetchTimer.Enabled = true;
                    }
                    else if (FetchDelay == 0) FetchTimerTick(this, null);
                }
                else if (FetchDelay == 0) FetchTimerTick(this, null);
                else if (FetchDelay > 0) CX.Raise("Fetch timer delay not initialized (null).", true);
            }
        }

        /// <summary>Fetch timer tick event.</summary>
        private void FetchTimerTick(object _Sender, ElapsedEventArgs _Args)
        {
            FetchTimer.Enabled = false;
            if (BindingRead != null) BindingRead(this);
            if (AfterFetch != null) AfterFetch(this);
        }

        /// <summary>Moves to the first record in the dataset. Return true if succeed.</summary>
        public bool First()
        {
            RecordIndex = -1;
            Row = null;
            Bof = true;
            Eof = true;
            if (Table != null)
            {
                if (Table.Rows != null)
                {
                    if (Table.Rows.Count > 0)
                    {
                        RecordIndex = 0;
                        Row = Table.Rows[RecordIndex];
                        Eof = false;
                        Bof = !SkipDeletedForward();
                    }
                    Fetch();
                }
            }
            return (RecordIndex > -1) && !Eof;
        }

        /// <summary>Moves to the record with recordIndex position in the dataset. Return true if succeed.</summary>
        public bool Goto(int _RecordIndex)
        {
            bool r = false;
            if (Table != null)
            {
                if (Table.Rows != null)
                {
                    if ((_RecordIndex > -1) && (_RecordIndex < Table.Rows.Count))
                    {
                        RecordIndex = _RecordIndex;
                        Row = Table.Rows[RecordIndex];
                        Bof = false;
                        Eof = false;
                        r = SkipDeletedForward();
                    }
                    Fetch();
                }
            }
            return r;
        }

        /// <summary>Moves to the last record in the dataset. Return true if succeed.</summary>
        public bool Last()
        {
            RecordIndex = -1;
            Row = null;
            Bof = true;
            Eof = true;
            if (Table != null)
            {
                if (Table.Rows != null)
                {
                    if (Table.Rows.Count > 0)
                    {
                        RecordIndex = Table.Rows.Count - 1;
                        Row = Table.Rows[RecordIndex];
                        Bof = false;
                        Eof = false;
                        SkipDeletedForward();
                    }
                    Fetch();
                }
            }
            return (RecordIndex > -1) && !Eof;
        }

        /// <summary>Moves to the next record in the dataset. Return true if succeed.</summary>
        public bool Next()
        {
            bool retValue = false;
            if (State == CXDatasetState.Read) retValue = Read();
            else if (Table != null)
            {
                if (Table.Rows != null)
                {
                    RecordIndex++;
                    if (RecordIndex < Table.Rows.Count)
                    {
                        Row = Table.Rows[RecordIndex];
                        Bof = false;
                        Eof = false;
                        retValue = SkipDeletedForward();
                    }
                    else Eof = true;
                    Fetch();
                }
            }
            return retValue;
        }

        /// <summary>Moves to the previous record in the dataset. Return true if succeed.</summary>
        public bool Previous()
        {
            bool retValue = false;
            if (Table != null)
            {
                if (Table.Rows != null)
                {
                    if (RecordIndex > 0)
                    {
                        RecordIndex--;
                        if (RecordIndex < Table.Rows.Count)
                        {
                            Row = Table.Rows[RecordIndex];
                            Bof = false;
                            Eof = false;
                            retValue = SkipDeletedBackward();
                        }
                        else Eof = true;
                    }
                    else Bof = true;
                    Fetch();
                }
            }
            return retValue;
        }

        /// <summary>Start a readonly session with sqlQuery reading first record 
        /// on current database connection. Returns true if succeed.</summary>
        public bool Read(string _SqlQuery)
        {
            bool retValue = false;
            Close();
            try
            {
                if (Database != null)
                {
                    if (Database.Keep())
                    {
                        if (Database.Type == CXDatabaseType.Access)
                        {
                            oleCommand = new OleDbCommand(CX.SqlDelimiters(CX.SqlMacros(_SqlQuery, Database.Type), Database.Type), Database.OleDB);
                            oleReader = oleCommand.ExecuteReader();
                        }
                        else if (Database.Type == CXDatabaseType.Sql)
                        {
                            sqlCommand = new SqlCommand(CX.SqlDelimiters(CX.SqlMacros(_SqlQuery, Database.Type), Database.Type), Database.SqlDB);
                            sqlReader = sqlCommand.ExecuteReader();
                        }
                        else if (Database.Type == CXDatabaseType.MySql)
                        {
                            mySqlCommand = new MySqlCommand(CX.SqlDelimiters(CX.SqlMacros(_SqlQuery, Database.Type), Database.Type), Database.MySqlDB);
                            mySqlReader = mySqlCommand.ExecuteReader();
                        }
                        else if (Database.Type == CXDatabaseType.PostgreSql)
                        {
                            pgSqlCommand = new NpgsqlCommand(CX.SqlDelimiters(CX.SqlMacros(_SqlQuery, Database.Type), Database.Type), Database.PostgreSqlDB);
                            pgSqlReader = pgSqlCommand.ExecuteReader();
                        }
                        else if (Database.Type == CXDatabaseType.DBase4)
                        {
                            oleCommand = new OleDbCommand(CX.SqlDelimiters(CX.SqlMacros(_SqlQuery, Database.Type), Database.Type), Database.OleDB);
                            oleReader = oleCommand.ExecuteReader();
                        }
                        retValue = Read();
                        if (retValue)
                        {
                            ChangeState(CXDatasetState.Read);
                            Eof = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CX.Error(ex);
            }
            return retValue;
        }

        /// <summary>Continue readonly session with sqlQuery reading next record 
        /// on current database connection. Returns true if succeed.</summary>
        public bool Read()
        {
            bool retValue = false;
            try
            {
                if (Database != null)
                {
                    if (Database.Keep())
                    {
                        if (Database.Type == CXDatabaseType.Access) retValue = oleReader.Read();
                        else if (Database.Type == CXDatabaseType.Sql) retValue = sqlReader.Read();
                        else if (Database.Type == CXDatabaseType.MySql) retValue = mySqlReader.Read();
                        else if (Database.Type == CXDatabaseType.PostgreSql) retValue = pgSqlReader.Read();
                        else if (Database.Type == CXDatabaseType.DBase4) retValue = oleReader.Read();
                        if (retValue) Bof = false;
                        else Eof = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CX.Error(ex);
            }
            return retValue;
        }

        /// <summary>Indicates the total number of records associated with the dataset.</summary>
        public int RecordCount()
        {
            if (Active && (Table != null))
            {
                if (Table.Rows != null) return Table.Rows.Count;
                else return -1;
            }
            return -1;
        }

        /// <summary>Indicates the percentage of record position on total number of dataset records.</summary>
        public double RecordPercent()
        {
            int i = RecordIndex, h = RecordCount() - 1;
            if (i > -1)
            {
                if (i == h) return 100.0d;
                else if (i < h) return Convert.ToDouble(i) * 100.0d / Convert.ToDouble(h);
                else return 0.0d;
            }
            else return 0.0d;
        }

        /// <summary>Returns a string representing values of current record fields
        /// specified in array. Blob fields will be stored as base64.</summary>
        public string RecordToString(string[] _FieldNamesList, bool _IncludeBlobs = true)
        {
            int i, j;
            string c, r = "";
            if ((_FieldNamesList != null) && (Row != null))
            {
                for (i = 0; i < _FieldNamesList.Length; i++)
                {
                    j = Table.Columns.IndexOf(_FieldNamesList[i]);
                    if (j > -1)
                    {
                        c = Table.Columns[j].ColumnName;
                        if (Table.Columns[j].DataType == CXDataType.BytesArray)
                        {
                            if (_IncludeBlobs) r = CX.TagSet(r, c, CX.Base64EncodeBytes((byte[])Field(c)), false);
                        }
                        else r = CX.TagSet(r, c, FieldStr(c));
                    }
                }
                r = CX.TagSet(r, "codeXRAD:CXDataset", r, false);
            }
            return r;
        }

        /// <summary>Skip backward all deleted or detached records from the current index. Return true if not BOF.</summary>
        public bool SkipDeletedBackward()
        {
            if (Row != null)
            {
                while (!Bof && ((Row.RowState == DataRowState.Deleted) || (Row.RowState == DataRowState.Detached)))
                {
                    if (RecordIndex > 0)
                    {
                        RecordIndex--;
                        Row = Table.Rows[RecordIndex];
                    }
                    else
                    {
                        Bof = true;
                        Row = null;
                    }
                }
            }
            return !Bof;
        }

        /// <summary>Skip forward all deleted or detached records from the current index. Return true if not EOF.</summary>
        public bool SkipDeletedForward()
        {
            if (Row != null)
            {
                while (!Eof && ((Row.RowState == DataRowState.Deleted) || (Row.RowState == DataRowState.Detached)))
                {
                    if (RecordIndex < Table.Rows.Count - 1)
                    {
                        RecordIndex++;
                        Row = Table.Rows[RecordIndex];
                    }
                    else
                    {
                        Eof = true;
                        Row = null;
                    }
                }
            }
            return !Eof;
        }

        #endregion

        /* */

        #region Methods - Appending

        /*  --------------------------------------------------------------------
         *  Methods - Appending
         *  --------------------------------------------------------------------
         */

        /// <summary>Adds a new, empty record to the dataset. Return true if succeed.</summary>
        public bool Append()
        {
            int i;
            bool r = false, cancel = false;
            DataRow row;
            if (BeforeInsert != null) BeforeInsert(this, ref cancel);
            if (cancel) CX.Raise("Append operation cancelled.", false);
            else if (readOnly) CX.Raise("Append cannot performed on readonly dataset.", false);
            else if (State != CXDatasetState.Browse) CX.Raise("Append can performed only on browsing state dataset.", false);
            else
            {
                try
                {
                    row = Table.NewRow();
                    for (i = 0; i < Table.Columns.Count; i++)
                    {
                        if (!Table.Columns[i].AutoIncrement) row[i] = Blank(Table.Columns[i]);
                    }
                    Table.Rows.Add(row);
                    RecordIndex = Table.Rows.Count - 1;
                    Row = Table.Rows[RecordIndex];
                    Bof = false;
                    Eof = false;
                    ChangeState(CXDatasetState.Insert);
                    if (AfterInsert != null) AfterInsert(this);
                    r = true;
                }
                catch (Exception ex)
                {
                    CX.Error(ex);
                }
            }
            return r;
        }

        /// <summary>Append a record replacing fields with ds source dataset fields values included in fv 
        /// array with format ( "fieldTarget1", "fieldSource1", ... "fieldTargetN", "fieldSourceN" )</summary>
        public bool Append(CXDataset _DataSet, string[] _FieldsList)
        {
            bool r = false;
            int i = 0;
            if (_DataSet != null)
            {
                if (_DataSet.State == CXDatasetState.Browse)
                {
                    if (Append())
                    {
                        r = true;
                        while (r && (i < _FieldsList.Length - 2))
                        {
                            r = Assign(_FieldsList[i], _DataSet.Field(_FieldsList[i + 1]));
                            i += 2;
                        }
                        if (r) r = Post();
                        if (!r) Cancel();
                    }
                }
            }
            return r;
        }

        /// <summary>Append to dataset all record of ds from current active.
        /// If ignoreError is true all errors will be ignored. Return true if succeed.</summary>
        public bool Append(CXDataset _DataSet, bool _IgnoreErrors)
        {
            bool r = false;
            if (readOnly) CX.Raise("Append cannot performed on readonly dataset.", false);
            else if (State != CXDatasetState.Browse) CX.Raise("Append can performed only on browsing state dataset.", false);
            else if (_DataSet != null)
            {
                _DataSet.First();
                r = true;
                while ((_IgnoreErrors || r) && !_DataSet.Eof)
                {
                    if (Append())
                    {
                        if (CopyRow(_DataSet))
                        {
                            if (!Post())
                            {
                                Cancel();
                                r = false;
                            }
                        }
                        else
                        {
                            Cancel();
                            r = false;
                        }
                    }
                    else r = false;
                    _DataSet.Next();
                }
            }
            return r;
        }

        /// <summary>Append to dataset all record of ds from current active. During operation
        /// gauge progress bar will be updated from-to values. Return true if succeed.</summary>
        public bool Append(CXDataset _DataSet, bool _IgnoreErrors, CXOnProgress _ProgressEvent)
        {
            bool r = false, stop = false;
            int i, max;
            if (readOnly) CX.Raise("Append cannot performed on readonly dataset.", false);
            else if (State != CXDatasetState.Browse) CX.Raise("Append can performed only on browsing state dataset.", false);
            else
            {
                i = 0;
                max = _DataSet.RecordCount();
                _DataSet.First();
                r = true;
                while (!stop && (_IgnoreErrors || r) && !_DataSet.Eof)
                {
                    if (Append())
                    {
                        if (CopyRow(_DataSet))
                        {
                            if (!Post())
                            {
                                Cancel();
                                r = false;
                            }
                        }
                        else
                        {
                            Cancel();
                            r = false;
                        }
                    }
                    else r = false;
                    i++;
                    if (_ProgressEvent != null) _ProgressEvent(Convert.ToDouble(CX.Percent(i, max)), ref stop);
                    _DataSet.Next();
                }
                if (stop) r = false;
            }
            return r;
        }

        #endregion

        /* */

        #region Methods - Assigning

        /*  --------------------------------------------------------------------
         *  Methods - Assigning
         *  --------------------------------------------------------------------
         */

        /// <summary>Assign value content to field named column.</summary>
        public bool Assign(string _FieldName, string _Value)
        {
            return Assign(Table.Columns.IndexOf(_FieldName), _Value);
        }

        /// <summary>Assign value content to field named column.</summary>
        public bool Assign(string _FieldName, object _Value)
        {
            return Assign(Table.Columns.IndexOf(_FieldName), _Value);
        }

        /// <summary>Assign value content to field with column index.</summary>
        public bool Assign(int _ColumnIndex, object _Value)
        {
            DataColumn co;
            if ((Row != null) && (_ColumnIndex > -1))
            {
                try
                {
                    co = Table.Columns[_ColumnIndex];
                    if (co.AutoIncrement) Row[_ColumnIndex] = 0;
                    else if (co.DataType == CXDataType.Boolean)
                    {
                        if (_Value == null) Row[_ColumnIndex] = false;
                        else Row[_ColumnIndex] = "1TVStvs".IndexOf((_Value.ToString().Trim() + " ")[0]) > -1;
                    }
                    else if ((co.DataType == CXDataType.Byte) || (co.DataType == CXDataType.SByte))
                    {
                        if (_Value == null) Row[_ColumnIndex] = 0;
                        else Row[_ColumnIndex] = Convert.ToByte(CX.ToInt(_Value.ToString()) % 256);
                    }
                    else if (co.DataType == CXDataType.Char)
                    {
                        if (_Value == null) Row[_ColumnIndex] = '\0';
                        else Row[_ColumnIndex] = (_Value.ToString() + " ")[0];
                    }
                    else if (co.DataType == CXDataType.DateTime)
                    {
                        if (_Value == null) Row[_ColumnIndex] = DBNull.Value;
                        else if (_Value is DateTime)
                        {
                            if (CX.MinDate((DateTime)_Value)) Row[_ColumnIndex] = DBNull.Value;
                            else Row[_ColumnIndex] = (DateTime)_Value;
                        }
                        else if (_Value.ToString().Trim().Length > 0) Row[_ColumnIndex] = CX.Date(_Value.ToString(), CX.DateFormat, true);
                        else Row[_ColumnIndex] = DBNull.Value;
                    }
                    else if ((co.DataType == CXDataType.Decimal) || (co.DataType == CXDataType.Double) || (co.DataType == CXDataType.Single))
                    {
                        if (_Value == null) Row[_ColumnIndex] = 0.0;
                        else Row[_ColumnIndex] = CX.Val(_Value.ToString());
                    }
                    else if ((co.DataType == CXDataType.Int32) || (co.DataType == CXDataType.Int16)
                        || (co.DataType == CXDataType.UInt32) || (co.DataType == CXDataType.UInt16))
                    {
                        if (_Value == null) Row[_ColumnIndex] = 0;
                        else Row[_ColumnIndex] = CX.ToInt(_Value.ToString());
                    }
                    else if ((co.DataType == CXDataType.Int64) || (co.DataType == CXDataType.UInt64))
                    {
                        if (_Value == null) Row[_ColumnIndex] = 0;
                        else Row[_ColumnIndex] = CX.ToLong(_Value.ToString());
                    }
                    else if (co.DataType == CXDataType.TimeSpan)
                    {
                        if (_Value == null) Row[_ColumnIndex] = DBNull.Value;
                        else if (_Value.ToString().Trim().Length > 0) Row[_ColumnIndex] = new TimeSpan(CX.Date(_Value.ToString(), CX.DateFormat, true).Ticks);
                        else Row[_ColumnIndex] = DBNull.Value;
                    }
                    else if (co.DataType == CXDataType.BytesArray)
                    {
                        if (_Value == null) Row[_ColumnIndex] = DBNull.Value;
                        else Row[_ColumnIndex] = _Value;
                    }
                    else if (_Value == null) Row[_ColumnIndex] = "";
                    else if (co.MaxLength > 0) Row[_ColumnIndex] = CX.Mid(_Value.ToString(), 0, co.MaxLength);
                    else Row[_ColumnIndex] = _Value;
                    if (BindingRead != null) BindingRead(this, co.ColumnName);
                    return true;
                }
                catch (Exception ex)
                {
                    CX.Error(ex);
                    return false;
                }
            }
            else
            {
                CX.Raise("Unknown field or invalid field index.", true);
                return false;
            }
        }

        /// <summary>Assign blank content to field named column.</summary>
        public bool Assign(string _FieldName)
        {
            int i = Table.Columns.IndexOf(_FieldName);
            if (i > -1) return Assign(i, Blank(Table.Columns[i]));
            else return false;
        }

        /// <summary>Assign fields from source dataset. fields is an array with field 
        /// names couple "target field name","source field name". Returns true if succeed.</summary>
        public bool Assign(CXDataset _SourceDataSet, string[] _FieldsList)
        {
            bool r = true;
            int i = 0;
            while (r && (i < _FieldsList.Length - 2))
            {
                r = Assign(_FieldsList[i], Field(_FieldsList[i + 1]));
                i += 2;
            }
            return r;
        }

        /// <summary>Assign image img content to field named column with format.</summary>
        public bool Assign(string _FieldName, Image _Image, System.Drawing.Imaging.ImageFormat _ImageFormat)
        {
            bool r;
            Bitmap bmp;
            MemoryStream ms;
            try
            {
                if (_Image != null)
                {
                    bmp = new Bitmap(_Image);
                    ms = new MemoryStream();
                    try
                    {
                        bmp.Save(ms, _ImageFormat);
                        r = Assign(_FieldName, ms.ToArray());
                    }
                    catch (Exception ex)
                    {
                        CX.Error(ex);
                        r = false;
                    }
                    bmp.Dispose();
                    ms.Close();
                    ms.Dispose();
                }
                else r = Assign(_FieldName, null);
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                r = false;
            }
            return r;
        }

        /// <summary>Assign new unique ID to record, matching type.</summary>
        public bool AssignNewID()
        {
            if (HasUniqueId) return Assign("ID", CX.UniqueId());
            else return false;
        }

        #endregion

        /* */

        #region Methods - Searching

        /*  --------------------------------------------------------------------
         *  Methods - Searching
         *  --------------------------------------------------------------------
         */

        /// <summary>Return index of row with key fields corresponding to map indexes field 
        /// equal to values, that must be FieldSort() formatted, or -1 if not found. 
        /// Binary if true enable binary search for keys sequence ordered recordset.</summary>
        public int Find(int[] _KeyFieldsIndexes, string[] _FieldSortFormattedValues, bool _BinarySearch)
        {
            int r = -1, i, min, mid, max;
            if (Table.Rows.Count > 0)
            {
                //
                // binary search
                //
                if (_BinarySearch)
                {
                    min = 0;
                    max = Table.Rows.Count - 1;
                    while ((r < 0) && (min <= max))
                    {
                        mid = (min + max) / 2;
                        i = FindCompare(_KeyFieldsIndexes, _FieldSortFormattedValues, mid);
                        if (i == 0) r = mid;
                        else if (i < 0) max = mid - 1;
                        else min = mid + 1;
                    }
                    //
                    // search first
                    //
                    while (r > 0)
                    {
                        if (FindCompare(_KeyFieldsIndexes, _FieldSortFormattedValues, r - 1) == 0) r--;
                        else break;
                    }
                }
                else
                {
                    //
                    // sequential search
                    //
                    i = 0;
                    while ((r < 0) && (i < Table.Rows.Count))
                    {
                        if (FindCompare(_KeyFieldsIndexes, _FieldSortFormattedValues, i) == 0) r = i;
                        i++;
                    }
                }
            }
            return r;
        }

        /// <summary>Return index of row with keys field equal to values, that must 
        /// be FieldSort() formatted, or -1 if not found. Binary if true enable 
        /// binary search for keys sequence ordered recordset.</summary>
        public int Find(string[] _KeyFields, string[] _FieldSortFormattedValues, bool _BinarySearch)
        {
            int i;
            int[] keyCols = new int[_KeyFields.Length];
            for (i = 0; i < _KeyFields.Length; i++) keyCols[i] = Table.Columns.IndexOf(_KeyFields[i]);
            return Find(keyCols, _FieldSortFormattedValues, _BinarySearch);
        }

        /// <summary>Compare fields values, that must be FieldSort() formatted, corresponding 
        /// to fields indexes of record ad index passed with values.</summary>
        private int FindCompare(int[] _KeyFieldsIndexes, string[] _FieldSortFormattedValues, int _RowIndex)
        {
            int r = 0, i = 0;
            while ((r == 0) && (i < _KeyFieldsIndexes.Length))
            {
                r = String.Compare(
                    _FieldSortFormattedValues[i],
                    CX.SqlSortable(Table.Columns[_KeyFieldsIndexes[i]], Table.Rows[_RowIndex][_KeyFieldsIndexes[i]]),
                    !Table.CaseSensitive);
                i++;
            }
            return r;
        }

        /// <summary>Return value greater than zero if opened dataset table contains another record 
        /// except current with same value of tested field. A value lesser than zero indicate error.</summary>
        public int FindDouble(string _UniqueFieldName)
        {
            int r = -1, i;
            string value;
            CXDataset ds;
            if (this.Active)
            {
                if (HasUniqueId)
                {
                    try
                    {
                        i = this.Table.Columns.IndexOf(_UniqueFieldName);
                        if (i > -1)
                        {
                            if (CXDataType.IsDate(this.Table.Columns[i].DataType)) value = CX.SqlQuote(this.FieldDateTime(_UniqueFieldName), this.Database.Type);
                            else if (CXDataType.IsNumeric(this.Table.Columns[i].DataType)) value = this.FieldStr(_UniqueFieldName);
                            else if (CXDataType.IsBoolean(this.Table.Columns[i].DataType)) value = CX.Iif(this.FieldBool(_UniqueFieldName), "TRUE", "FALSE");
                            else value = CX.Quote(this.FieldStr(_UniqueFieldName));
                            ds = new CXDataset(this.Database);
                            if (ds.OpenAtLeast("SELECT [ID],[" + _UniqueFieldName + "] FROM [" + this.TableName
                                + "] WHERE ([" + _UniqueFieldName + "]=" + value + ")AND([ID]<>" + CX.Quote(this.FieldStr("ID")) + ")"))
                            {
                                r = ds.RecordCount();
                                ds.Close();
                            }
                            ds.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        CX.Error(ex);
                        r = -1;
                    }
                }
            }
            return r;
        }

        /// <summary>Seek and go to row index with keys field equal to values, that must 
        /// be FieldSort() formatted, or -1 if not found or cannot go to row index. Binary if true enable 
        /// binary search for keys sequence ordered recordset.</summary>
        public int Seek(string[] _KeyFields, string[] _FieldSortFormattedValues, bool _BinarySearch)
        {
            int r = Find(_KeyFields, _FieldSortFormattedValues, _BinarySearch);
            if (r > -1)
            {
                if (!Goto(r)) r = -1;
            }
            return r;
        }

        /// <summary>Seek and go to row index with key fields corresponding to map indexes field 
        /// equal to values, that must be FieldSort() formatted, or -1 if not found or cannot go 
        /// to row index. Binary if true enable binary search for keys sequence ordered recordset.</summary>
        public int Seek(int[] _KeyFieldsIndexes, string[] _Values, bool _BinarySearch)
        {
            int r = Find(_KeyFieldsIndexes, _Values, _BinarySearch);
            if (r > -1)
            {
                if (!Goto(r)) r = -1;
            }
            return r;
        }

        #endregion 

        /* */

        #region Methods - Editing

        /*  --------------------------------------------------------------------
         *  Methods - Editing
         *  --------------------------------------------------------------------
         */

        /// <summary>If dataset updates buffers is full, writes changes to database. Return true if succeed.</summary>
        public bool Buffer()
        {
            changesBufferCount++;
            if (changesBufferCount < ChangesBufferSize) return true;
            else return Commit();
        }

        /// <summary>Cancels not yet posted modifications to the active record. Return true if succeed.</summary>
        public bool Cancel()
        {
            bool r = false, abort = false;
            if (BeforeCancel != null) BeforeCancel(this, ref abort);
            if (abort) CX.Raise("Cancel operation aborted.", false);
            else if (readOnly) CX.Raise("Cancel cannot performed on readonly dataset.", false);
            else if (!Modifying(false)) CX.Raise("Cancel operation can performed only on editing or appending state dataset.", false);
            else
            {
                try
                {
                    Table.RejectChanges();
                    r = true;
                }
                catch (Exception ex)
                {
                    CX.Error(ex);
                }
                if (r)
                {
                    ChangeState(CXDatasetState.Browse);
                    Goto(RecordIndex);
                    //
                    if (AfterCancel != null) AfterCancel(this);
                }
            }
            return r;
        }

        /// <summary>Writes a dataset buffered updates to the database. Return true if succeed.</summary>
        public bool Commit()
        {
            try
            {
                if ((InternalDataSet != null) && (Table != null))
                {
                    if (InternalDataSet.HasChanges())
                    {
                        if (Database.Type == CXDatabaseType.Access)
                        {
                            oleAdapter.InsertCommand.Connection = Database.OleDB;
                            oleAdapter.UpdateCommand.Connection = Database.OleDB;
                            oleAdapter.DeleteCommand.Connection = Database.OleDB;
                            oleAdapter.Update(InternalDataSet);
                        }
                        else if (Database.Type == CXDatabaseType.Sql)
                        {
                            sqlAdapter.InsertCommand.Connection = Database.SqlDB;
                            sqlAdapter.UpdateCommand.Connection = Database.SqlDB;
                            sqlAdapter.DeleteCommand.Connection = Database.SqlDB;
                            sqlAdapter.Update(InternalDataSet);
                        }
                        else if (Database.Type == CXDatabaseType.MySql)
                        {
                            mySqlAdapter.InsertCommand.Connection = Database.MySqlDB;
                            mySqlAdapter.UpdateCommand.Connection = Database.MySqlDB;
                            mySqlAdapter.DeleteCommand.Connection = Database.MySqlDB;
                            mySqlAdapter.Update(InternalDataSet);
                        }
                        else if (Database.Type == CXDatabaseType.PostgreSql)
                        {
                            pgSqlAdapter.InsertCommand.Connection = Database.PostgreSqlDB;
                            pgSqlAdapter.UpdateCommand.Connection = Database.PostgreSqlDB;
                            pgSqlAdapter.DeleteCommand.Connection = Database.PostgreSqlDB;
                            pgSqlAdapter.Update(InternalDataSet);
                        }
                        else if (Database.Type == CXDatabaseType.DBase4)
                        {
                            oleAdapter.InsertCommand.Connection = Database.OleDB;
                            oleAdapter.UpdateCommand.Connection = Database.OleDB;
                            oleAdapter.DeleteCommand.Connection = Database.OleDB;
                            oleAdapter.Update(InternalDataSet);
                        }
                    }
                    Table.AcceptChanges();
                    changesBufferCount = 0;
                    if (this.HasUniqueId)
                    {
                        if (Database.Type == CXDatabaseType.Access) oleAdapter.Fill(InternalDataSet);
                        else if (Database.Type == CXDatabaseType.Sql) sqlAdapter.Fill(InternalDataSet);
                        else if (Database.Type == CXDatabaseType.MySql) mySqlAdapter.Fill(InternalDataSet);
                        else if (Database.Type == CXDatabaseType.PostgreSql) pgSqlAdapter.Fill(InternalDataSet);
                        else if (Database.Type == CXDatabaseType.DBase4) oleAdapter.Fill(InternalDataSet);
                        else InternalDataSet.Clear();
                        if (InternalDataSet.Tables.Count > 0) Table = InternalDataSet.Tables[0];
                        else Table = null;
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                return false;
            }
        }

        /// <summary>Copy the values of corresponding fields from source dataset current row 
        /// to this dataset current row. Returns true if succeed.</summary>
        public bool CopyRow(CXDataset _SourceDataSet)
        {
            int i;
            bool r = false;
            if ((_SourceDataSet != null) && (Row != null) && (Table != null))
            {
                if (this.Modifying(false) && (_SourceDataSet.State != CXDatasetState.Closed))
                {
                    i = Table.Columns.Count;
                    r = true;
                    while (r && (i > 0))
                    {
                        i--;
                        if (!Table.Columns[i].AutoIncrement)
                        {
                            if (_SourceDataSet.IsField(Table.Columns[i].ColumnName))
                            {
                                r = this.Assign(i, _SourceDataSet.Field(Table.Columns[i].ColumnName));
                            }
                        }
                    }
                }
            }
            return r;
        }

        /// <summary>Deletes the active record in the dataset. Return true if succeed.</summary>
        public bool Delete()
        {
            bool r = false, abort = false;
            if (BeforeDelete != null) BeforeDelete(this, ref abort);
            if (abort) CX.Raise("Delete operation aborted.", false);
            else if (readOnly) CX.Raise("Delete cannot performed on readonly dataset.", false);
            else if (State != CXDatasetState.Browse) CX.Raise("Delete can performed only on browsing state dataset.", false);
            else
            {
                if (Row != null)
                {
                    ChangeState(CXDatasetState.Delete);
                    Row.Delete();
                    if (Buffer()) r = true;
                    else Table.RejectChanges();
                    if ((RecordIndex > -1) && (RecordIndex < Table.Rows.Count))
                    {
                        Row = Table.Rows[RecordIndex];
                        Bof = false;
                        Eof = false;
                        r = SkipDeletedForward();
                    }
                    else
                    {
                        Row = null;
                        Eof = true;
                        Bof = true;
                    }
                    ChangeState(CXDatasetState.Browse);
                    if (r)
                    {
                        if (ChangesNotify) Changes();
                        //
                        if (AfterDelete != null) AfterDelete(this);
                    }
                }
            }
            return r;
        }

        /// <summary>Enables data editing of dataset. Return true if succeed.</summary>
        public bool Edit()
        {
            bool r = false, abort = false;
            if (BeforeEdit != null) BeforeEdit(this, ref abort);
            if (abort) CX.Raise("Edit operation aborted.", false);
            else if (readOnly) CX.Raise("Edit cannot performed on readonly dataset.", false);
            else if (State != CXDatasetState.Browse) CX.Raise("Edit can performed only on browsing state dataset.", false);
            else
            {
                ChangeState(CXDatasetState.Edit);
                //
                if (AfterEdit != null) AfterEdit(this);
                r = true;
            }
            return r;
        }

        /// <summary>Executes SQL statement passed as parameter. 
        /// Return the number of records affected or -1 if not succeed.</summary>
        public int Exec(string _SqlStatement)
        {
            int r = 0;
            Close();
            if (OpenDatabase())
            {
                _SqlStatement = CX.SqlDelimiters(CX.SqlMacros(_SqlStatement, Database.Type), Database.Type);
                try
                {
                    if (Database.Type == CXDatabaseType.Access)
                    {
                        OleDbCommand cmd = new OleDbCommand(_SqlStatement, Database.OleDB);
                        r = cmd.ExecuteNonQuery();
                    }
                    else if (Database.Type == CXDatabaseType.Sql)
                    {
                        SqlCommand cmd = new SqlCommand(_SqlStatement, Database.SqlDB);
                        r = cmd.ExecuteNonQuery();
                    }
                    else if (Database.Type == CXDatabaseType.MySql)
                    {
                        MySqlCommand cmd = new MySqlCommand(_SqlStatement, Database.MySqlDB);
                        r = cmd.ExecuteNonQuery();
                    }
                    else if (Database.Type == CXDatabaseType.PostgreSql)
                    {
                        NpgsqlCommand cmd = new NpgsqlCommand(_SqlStatement, Database.PostgreSqlDB);
                        r = cmd.ExecuteNonQuery();
                    }
                    else if (Database.Type == CXDatabaseType.DBase4)
                    {
                        OleDbCommand cmd = new OleDbCommand(_SqlStatement, Database.OleDB);
                        r = cmd.ExecuteNonQuery();
                    }
                    if (r < 0) r = 0;
                }
                catch (Exception ex)
                {
                    CX.Error(ex.Message + "\r\n*** SQL STATEMENT ***\r\n" + _SqlStatement, ex);
                    r = -1;
                }
            }
            else r = -1;
            return r;
        }

        /// <summary>Return true if exists record changes in the buffer.</summary>
        public bool Modified()
        {
            if (InternalDataSet != null) return InternalDataSet.HasChanges();
            else return false;
        }

        /// <summary>Return true if dataset state is in insert or edit mode. If force is true, 
        /// dataset AutoEdit property is setted to true and the dataset is not in edit or insert mode
        /// the function try to set the edit mode. Returns true if succeesd.</summary>
        public bool Modifying(bool _ForceEdit)
        {
            if ((State == CXDatasetState.Insert) || (State == CXDatasetState.Edit)) return true;
            else if (_ForceEdit && (State == CXDatasetState.Browse) && (Row != null)) return Edit();
            else return false;
        }

        /// <summary>Write a modified record to the buffer. Return true if succeed.</summary>
        public bool Post()
        {
            bool r = false, abort = false;
            if (BeforePost != null) BeforePost(this, ref abort);
            //
            if (abort) CX.Raise("Post operation aborted.", false);
            else if (readOnly) CX.Raise("Post cannot performed on readonly dataset.", false);
            else if (!Modifying(false)) CX.Raise("Post can performed only on editing or appending state dataset.", false);
            else
            {
                if (BindingWrite != null) BindingWrite(this);
                try
                {
                    if (UpdateRecordSysInformation)
                    {
                        if (Row.RowState == DataRowState.Added)
                        {
                            if (HasSysInsert) Row["Sys_Insert"] = DateTime.Now;
                            if (HasSysDate) Row["Sys_Date"] = DateTime.Now;
                            if (HasSysOSUser) Row["Sys_OSUser"] = CX.User();
                            // if (HasSysUser) Row["Sys_User"] = CXUser.Current.Id;
                        }
                        else if (Row.RowState == DataRowState.Modified)
                        {
                            if (HasSysDate) Row["Sys_Date"] = DateTime.Now;
                            if (HasSysOSUser) Row["Sys_OSUser"] = CX.User();
                            // if (HasSysUser) Row["Sys_User"] = XUser.Current.Id;
                        }
                    }
                    r = Buffer();
                }
                catch (Exception ex)
                {
                    CX.Error(ex);
                    r = false;
                }
            }
            if (r)
            {
                if ((RecordIndex > -1) && (RecordIndex < Table.Rows.Count))
                {
                    Row = Table.Rows[RecordIndex];
                    Bof = false;
                    Eof = false;
                    r = SkipDeletedForward();
                }
                else
                {
                    Row = null;
                    Eof = true;
                    Bof = true;
                }
                ChangeState(CXDatasetState.Browse);
                if (r)
                {
                    if (ChangesNotify) Changes();
                    if (AfterPost != null) AfterPost(this);
                }
            }
            return r;
        }

        /// <summary>Store in the current record fields values contained on s. 
        /// If blobs is true, blob fields will be stored in temporary directory and
        /// its names will be included on strings. It's possible exclude a list of fields by name.</summary>
        public bool RecordFromString(string _TaggedString, bool _IncludeBlobs, string[] _ExcludeFields)
        {
            int i;
            bool r = false, b;
            string c, t;
            if (Row != null)
            {
                _TaggedString = CX.TagGet(_TaggedString, "codeXRAD:CXDataset", "", false).Trim();
                if (Modifying(false) && (_TaggedString.Length>0))
                {
                    i = 0;
                    while (i < Table.Columns.Count)
                    {
                        if (!Table.Columns[i].AutoIncrement)
                        {
                            c = Table.Columns[i].ColumnName;
                            if (_ExcludeFields != null) b = CX.Find(c, _ExcludeFields, true) < 0;
                            else b = true;
                            if (b)
                            {
                                b = Table.Columns[i].DataType == CXDataType.BytesArray;
                                if (b)
                                {
                                    if (_IncludeBlobs)
                                    {
                                        t = CX.TagGet(_TaggedString, c, "");
                                        Assign(c, CX.Base64DecodeBytes(t));
                                    }
                                }
                                else Assign(c, CX.TagGet(_TaggedString, c, ""));
                            }
                        }
                        i++;
                    }
                    r = true;
                }
            }
            return r;
        }

        #endregion

        /* */

    }

    /* */

}
