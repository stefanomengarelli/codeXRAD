/*  ------------------------------------------------------------------------
 *  
 *  File:       Sql.cs
 *  Version:    1.0.0
 *  Date:       October 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD database SQL management functions static class.
 *  
 *  ------------------------------------------------------------------------
 */

using MySql.Data.MySqlClient;
using Npgsql;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Text;

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD database SQL management functions static class.</summary>
    public static partial class CX
    {

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Add to SQL expression an AND condition with field name, operator and quoted date value.</summary>
        public static string SqlAndDate(string _SQL, string _FieldName, string _Operator, string _Value, CXDatabaseType _DatabaseType)
        {
            return CX.Cat(_SQL, '(' + _FieldName.Trim() + _Operator.Trim()
                + CX.SqlQuote(CX.Date(_Value), _DatabaseType), "AND");
        }

        /// <summary>Add to SQL expression an AND condition with field name, operator and unquoted number value.</summary>
        public static string SqlAndNumber(string _SQL, string _FieldName, string _Operator, string _Value)
        {
            return CX.Cat(_SQL, '(' + _FieldName.Trim() + _Operator.Trim()
                + CX.ToDouble(_Value).ToString("####################.####################").Replace(CX.DecimalSeparator, '.') + ')', "AND");
        }

        /// <summary>Add to SQL expression an AND condition with field name, operator and quoted string value.</summary>
        public static string SqlAndExprString(string _SQL, string _FieldName, string _Operator, string _Value)
        {
            return CX.Cat(_SQL, '(' + _FieldName.Trim() + _Operator.Trim() + CX.Quote(_Value) + ')', "AND");
        }

        /// <summary>Add to string list all id of parent id value. Return id added count or -1 if fail. If specified delete records founded.</summary>
        public static int SqlChildIdList(CXDatabase _Database, string _Table, string _IdField, string _ParentIdField, string _ParentIdValue, List<string> _IdList, bool _DeleteRecords)
        {
            int i = 0, q, r = -1;
            string id;
            CXDataset ds;
            if ((_Database != null) && (_IdField.Trim().Length > 0) && (_ParentIdField.Trim().Length > 0) && (_ParentIdValue.Trim().Length > 0) && (_IdList != null))
            {
                try
                {
                    ds = new CXDataset(_Database);
                    if (ds.Open("SELECT " + _IdField + "," + _ParentIdField + " FROM " + _Table + " WHERE " + _ParentIdField + "=" + CX.Quote(_ParentIdValue)))
                    {
                        i = _IdList.Count;
                        r = 0;
                        while (!ds.Eof && (r > -1))
                        {
                            id = ds.FieldStr(_IdField);
                            if (id.Trim().Length > 0)
                            {
                                _IdList.Add(id);
                                if (_DeleteRecords)
                                {
                                    if (!ds.Delete()) r = -1;
                                }
                                else ds.Next();
                            }
                            else ds.Next();
                        }
                    }
                    ds.Close();
                    ds.Dispose();
                    while ((r > -1) && (i < _IdList.Count))
                    {
                        q = SqlChildIdList(_Database, _Table, _IdField, _ParentIdField, _IdList[i], _IdList, _DeleteRecords);
                        if (q < 0) r = q;
                        else r += q;
                        i++;
                    }
                }
                catch (Exception ex)
                {
                    CX.Error(ex);
                    r = -1;
                }
            }
            return r;
        }

        /// <summary>Returns SQL column type definition syntax for creation of database type.</summary>
        public static string SqlColumnType(DataColumn _DataColumn, CXDatabaseType _Type)
        {
            if ((_Type == CXDatabaseType.Access) || (_Type == CXDatabaseType.Sql))
            {
                if (_DataColumn.AutoIncrement) return "BIGINT IDENTITY (1,1) NOT NULL";
                else if (_DataColumn.DataType == System.Type.GetType("System.Boolean")) return "BIT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Byte")) return "TINYINT UNSIGNED";
                else if (_DataColumn.DataType == System.Type.GetType("System.Char")) return "VARCHAR(1)";
                else if (_DataColumn.DataType == System.Type.GetType("System.DateTime")) return "DATETIME";
                else if (_DataColumn.DataType == System.Type.GetType("System.Decimal")) return "DECIMAL(32,8)";
                else if (_DataColumn.DataType == System.Type.GetType("System.Double")) return "FLOAT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Int16")) return "SMALLINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Int32")) return "INT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Int64")) return "BIGINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.SByte")) return "TINYINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Single")) return "FLOAT";
                else if (_DataColumn.DataType == System.Type.GetType("System.String"))
                {
                    if (_DataColumn.MaxLength > 255) return "VARCHAR(MAX)";
                    else return "VARCHAR(" + _DataColumn.MaxLength.ToString() + ")";
                }
                else if (_DataColumn.DataType == System.Type.GetType("System.UInt16")) return "SMALLINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.UInt32")) return "INT";
                else if (_DataColumn.DataType == System.Type.GetType("System.UInt64")) return "BIGINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Byte[]")) return "VARBINARY(MAX)";
                else return "VARCHAR(255)";
            }
            else if (_Type == CXDatabaseType.MySql)
            {
                if (_DataColumn.AutoIncrement) return "BIGINT UNSIGNED NOT NULL AUTO_INCREMENT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Boolean")) return "BOOLEAN";
                else if (_DataColumn.DataType == System.Type.GetType("System.Byte")) return "TINYINT UNSIGNED";
                else if (_DataColumn.DataType == System.Type.GetType("System.Char")) return "VARCHAR(1)";
                else if (_DataColumn.DataType == System.Type.GetType("System.DateTime")) return "DATETIME";
                else if (_DataColumn.DataType == System.Type.GetType("System.Decimal")) return "DECIMAL(32,8)";
                else if (_DataColumn.DataType == System.Type.GetType("System.Double")) return "DOUBLE";
                else if (_DataColumn.DataType == System.Type.GetType("System.Int16")) return "SMALLINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Int32")) return "INT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Int64")) return "BIGINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.SByte")) return "TINYINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Single")) return "DOUBLE";
                else if (_DataColumn.DataType == System.Type.GetType("System.String"))
                {
                    if (_DataColumn.MaxLength > 255) return "MEDIUMTEXT";
                    else return "VARCHAR(" + _DataColumn.MaxLength.ToString() + ")";
                }
                else if (_DataColumn.DataType == System.Type.GetType("System.UInt16")) return "SMALLINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.UInt32")) return "INT";
                else if (_DataColumn.DataType == System.Type.GetType("System.UInt64")) return "BIGINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Byte[]")) return "MEDIUMBLOB";
                else return "VARCHAR(255)";
            }
            else if (_Type == CXDatabaseType.PostgreSql)
            {
                if (_DataColumn.AutoIncrement) return "BIGSERIAL";
                else if (_DataColumn.DataType == System.Type.GetType("System.Boolean")) return "BOOLEAN";
                else if (_DataColumn.DataType == System.Type.GetType("System.Byte")) return "SMALLINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Char")) return "VARCHAR(1)";
                else if (_DataColumn.DataType == System.Type.GetType("System.DateTime")) return "TIMESTAMP";
                else if (_DataColumn.DataType == System.Type.GetType("System.Decimal")) return "DECIMAL";
                else if (_DataColumn.DataType == System.Type.GetType("System.Double")) return "DOUBLE PRECISION";
                else if (_DataColumn.DataType == System.Type.GetType("System.Int16")) return "SMALLINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Int32")) return "INTEGER";
                else if (_DataColumn.DataType == System.Type.GetType("System.Int64")) return "BIGINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.SByte")) return "SMALLINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Single")) return "REAL";
                else if (_DataColumn.DataType == System.Type.GetType("System.String"))
                {
                    if (_DataColumn.MaxLength > 255) return "TEXT";
                    else return "VARCHAR(" + _DataColumn.MaxLength.ToString() + ")";
                }
                else if (_DataColumn.DataType == System.Type.GetType("System.UInt16")) return "SMALLINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.UInt32")) return "INTEGER";
                else if (_DataColumn.DataType == System.Type.GetType("System.UInt64")) return "BIGINT";
                else if (_DataColumn.DataType == System.Type.GetType("System.Byte[]")) return "BYTEA";
                else return "VARCHAR(255)";
            }
            else return "";
        }

        /// <summary>Return string containing sql statement with [ ] Microsoft SQL delimiters 
        /// turned in to ty type database delimiters.</summary>
        public static string SqlDelimiters(string _SqlStatement, CXDatabaseType _Type)
        {
            int i;
            bool b;
            char c;
            StringBuilder r;
            if (_Type == CXDatabaseType.MySql)
            {
                b = false;
                r = new StringBuilder();
                for (i = 0; i < _SqlStatement.Length; i++)
                {
                    c = _SqlStatement[i];
                    if (c == '\'') b = !b;
                    else if ((c == CXDatabase.SqlPrefix) && !b) c = CXDatabase.MySqlPrefix;
                    else if ((c == CXDatabase.SqlSuffix) && !b) c = CXDatabase.MySqlSuffix;
                    r.Append(c);
                }
                return r.ToString();
            }
            else if (_Type == CXDatabaseType.PostgreSql)
            {
                b = false;
                r = new StringBuilder();
                for (i = 0; i < _SqlStatement.Length; i++)
                {
                    c = _SqlStatement[i];
                    if (c == '\'') b = !b;
                    else if ((c == CXDatabase.SqlPrefix) && !b) c = CXDatabase.PostgreSqlPrefix;
                    else if ((c == CXDatabase.SqlSuffix) && !b) c = CXDatabase.PostgreSqlSuffix;
                    r.Append(c);
                }
                return r.ToString();
            }
            else return _SqlStatement;
        }

        /// <summary>Execute the query specified in sql statement on database alias 
        /// and returns the number of records affected or -1 if the function fails.</summary>
        public static int SqlExec(string _Alias, string _SqlStatement)
        {
            return SqlExec(CXDatabase.Keep(_Alias), _SqlStatement, true);
        }

        /// <summary>Executes SQL statement passed as parameter. Is statement start by SELECT
        /// function will return integer value of result of first column of first row 
        /// else will return the number of records affected or -1 if not succeed.</summary>
        public static int SqlExec(CXDatabase _Database, string _SqlStatement, bool _ErrorManagement)
        {
            bool q;
            int r = -1;
            if (_Database != null)
            {
                if (_Database.Keep())
                {
                    _SqlStatement = SqlDelimiters(SqlMacros(_SqlStatement, _Database.Type), _Database.Type).Trim();
                    q = _SqlStatement.ToUpper().StartsWith("SELECT ");
                    try
                    {
                        if (_Database.Type == CXDatabaseType.Access)
                        {
                            OleDbCommand cmd = new OleDbCommand(_SqlStatement, _Database.OleDB);
                            if (q) r = CX.ToInt(cmd.ExecuteScalar().ToString());
                            else r = cmd.ExecuteNonQuery();
                            if (r < 0) r = 0;
                        }
                        else if (_Database.Type == CXDatabaseType.Sql)
                        {
                            SqlCommand cmd = new SqlCommand(_SqlStatement, _Database.SqlDB);
                            if (q) r = CX.ToInt(cmd.ExecuteScalar().ToString());
                            else r = cmd.ExecuteNonQuery();
                            if (r < 0) r = 0;
                        }
                        else if (_Database.Type == CXDatabaseType.MySql)
                        {
                            MySqlCommand cmd = new MySqlCommand(_SqlStatement, _Database.MySqlDB);
                            if (q) r = CX.ToInt(cmd.ExecuteScalar().ToString());
                            else r = cmd.ExecuteNonQuery();
                            if (r < 0) r = 0;
                        }
                        else if (_Database.Type == CXDatabaseType.PostgreSql)
                        {
                            NpgsqlCommand cmd = new NpgsqlCommand(_SqlStatement, _Database.PostgreSqlDB);
                            if (q) r = CX.ToInt(cmd.ExecuteScalar().ToString());
                            else r = cmd.ExecuteNonQuery();
                            if (r < 0) r = 0;
                        }
                        else if (_Database.Type == CXDatabaseType.DBase4)
                        {
                            OleDbCommand cmd = new OleDbCommand(_SqlStatement, _Database.OleDB);
                            if (q) r = CX.ToInt(cmd.ExecuteScalar().ToString());
                            else r = cmd.ExecuteNonQuery();
                            if (r < 0) r = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (_ErrorManagement)
                        {
                            CX.Error(ex.Message + "\r\n\r\n*** SQL STATEMENT ***\r\n" + _SqlStatement, ex);
                        }
                        r = -1;
                    }
                }
            }
            return r;
        }

        /// <summary>Return optimized SQL field list, if one of field is * return *.</summary>
        public static string SqlFieldList(string _FieldList)
        {
            int i;
            string r = "", c;
            List<string> f = new List<string>();
            while (_FieldList.Trim().Length > 0)
            {
                c = CX.Extract(ref _FieldList, ",; ").Trim();
                if (c == "*")
                {
                    r = "*";
                    _FieldList = "";
                }
                else if ((c.Length > 0) && (f.IndexOf(c) < 0)) f.Add(c);
            }
            if (r == "")
            {
                for (i = 0; i < f.Count; i++)
                {
                    if (i > 0) r += ",";
                    r += f[i];
                }
            }
            return r;
        }

        /// <summary>Return string with SQL expression for ISNULL, below database type.</summary>
        public static string SqlIsNull(string _SqlExpression, CXDatabaseType _Type)
        {
            if (_SqlExpression.Trim().Length > 0)
            {
                if (_Type == CXDatabaseType.Access) return "IsNull(" + _SqlExpression + ")";
                else if (_Type == CXDatabaseType.Sql) return "((" + _SqlExpression.Trim() + ") IS NULL)";
                else if (_Type == CXDatabaseType.MySql) return "((" + _SqlExpression.Trim() + ") IS NULL)";
                else if (_Type == CXDatabaseType.PostgreSql) return "(" + _SqlExpression.Trim() + " = NULL)";
                else return _SqlExpression;
            }
            else return "";
        }

        /// <summary>Return string with SQL expression for LOWERCASE, below database type.</summary>
        public static string SqlLower(string _SqlExpression, CXDatabaseType _Type)
        {
            if (_SqlExpression.Trim().Length > 0)
            {
                if (_Type == CXDatabaseType.Access) return "LCase(" + _SqlExpression + ")";
                else if (_Type == CXDatabaseType.Sql) return "LOWER(" + _SqlExpression.Trim() + ")";
                else if (_Type == CXDatabaseType.MySql) return "LCASE(" + _SqlExpression.Trim() + ")";
                else if (_Type == CXDatabaseType.PostgreSql) return "LOWER(" + _SqlExpression.Trim() + ")";
                else return _SqlExpression;
            }
            else return "";
        }

        /// <summary>Return string replacing SM SQL Macros with corresponding database SQL statement.
        /// Macros: X_ISNULL(expr), X_UPPER(expr), X_LOWER(expr), X_TRIM(expr), X_INSTR(expr,substring),
        /// X_DATETIME(yyyy-mm-dd hh:nn:ss), X_USER(), X_NOW().</summary>
        public static string SqlMacros(string _SqlStatement, CXDatabaseType _Type)
        {
            _SqlStatement = SqlMacrosReplace(_SqlStatement, "CX_ISNULL", SqlIsNull("%0%", _Type), _Type);
            _SqlStatement = SqlMacrosReplace(_SqlStatement, "CX_UPPER", SqlUpper("%0%", _Type), _Type);
            _SqlStatement = SqlMacrosReplace(_SqlStatement, "CX_LOWER", SqlLower("%0%", _Type), _Type);
            _SqlStatement = SqlMacrosReplace(_SqlStatement, "CX_TRIM", SqlTrim("%0%", _Type), _Type);
            _SqlStatement = SqlMacrosReplace(_SqlStatement, "CX_DATETIME", "%DATETIME%", _Type);
            _SqlStatement = _SqlStatement.Replace("CX_USER()", CX.Quote(CX.User()));
            _SqlStatement = _SqlStatement.Replace("CX_MACHINE()", CX.Quote(CX.Machine()));
            _SqlStatement = _SqlStatement.Replace("CX_NOW()", CX.SqlQuote(DateTime.Now, _Type));
            return _SqlStatement;
        }

        /// <summary>Return string replacing specified macro function with new expression.
        /// New expression substring %0% will be replaced with old macro function argument.</summary>
        private static string SqlMacrosReplace(string _String, string _MacroToReplace, string _ReplaceExpression, CXDatabaseType _Type)
        {
            string a,b;
            _MacroToReplace = _MacroToReplace.Trim(new char[] { ' ', '(', ')' });
            while (_String.IndexOf(_MacroToReplace+'(')>-1)
            {
                a = CX.Btw(_String, _MacroToReplace + '(', ")");
                if (_ReplaceExpression == "%DATETIME%") b = SqlQuote(CX.Date(a, CXDateFormat.yyyymmdd, true), _Type);
                else b = _ReplaceExpression.Replace("%0%", a);
                _String = _String.Replace(_MacroToReplace + '(' + a + ')', b);
            }
            return _String;            
        }

        /// <summary>Return dateValue with dbType database SQL syntax delimiters.</summary>
        public static string SqlQuote(DateTime _Value, CXDatabaseType _Type)
        {
            if (_Type == CXDatabaseType.Access)
            {
                // #mm-dd-yyyy hh.nn.ss#
                return '#' + _Value.Month.ToString().PadLeft(2, '0')
                    + '-' + _Value.Day.ToString().PadLeft(2, '0')
                    + '-' + _Value.Year.ToString().PadLeft(4, '0')
                    + ' ' + _Value.Hour.ToString().PadLeft(2, '0')
                    + '.' + _Value.Minute.ToString().PadLeft(2, '0')
                    + '.' + _Value.Second.ToString().PadLeft(2, '0')
                    + '#';
            }
            else if (_Type == CXDatabaseType.MySql)
            {
                // yyyy-mm-dd hh:nn:ss
                return '\'' + _Value.Year.ToString().PadLeft(4, '0')
                    + '-' + _Value.Month.ToString().PadLeft(2, '0')
                    + '-' + _Value.Day.ToString().PadLeft(2, '0')
                    + ' ' + _Value.Hour.ToString().PadLeft(2, '0')
                    + ':' + _Value.Minute.ToString().PadLeft(2, '0')
                    + ':' + _Value.Second.ToString().PadLeft(2, '0')
                    + '\'';
            }
            else if (_Type == CXDatabaseType.PostgreSql)
            {
                // yyyy-mm-dd hh:nn:ss
                return '\'' + _Value.Year.ToString().PadLeft(4, '0')
                    + '-' + _Value.Month.ToString().PadLeft(2, '0')
                    + '-' + _Value.Day.ToString().PadLeft(2, '0')
                    + ' ' + _Value.Hour.ToString().PadLeft(2, '0')
                    + ':' + _Value.Minute.ToString().PadLeft(2, '0')
                    + ':' + _Value.Second.ToString().PadLeft(2, '0')
                    + '\'';
            }
            else
            {
                // ISO 8601 yyyy-mm-ddThh:nn:ss
                return '\'' + _Value.Year.ToString().PadLeft(4, '0')
                    + '-' + _Value.Month.ToString().PadLeft(2, '0')
                    + '-' + _Value.Day.ToString().PadLeft(2, '0')
                    + 'T' + _Value.Hour.ToString().PadLeft(2, '0')
                    + ':' + _Value.Minute.ToString().PadLeft(2, '0')
                    + ':' + _Value.Second.ToString().PadLeft(2, '0')
                    + '\'';
            }
        }

        /// <summary>Return database identifier with type database SQL syntax delimiters.</summary>
        public static string SqlQuoteId(string _Identifier)
        {
            return '[' + _Identifier.Trim() + ']';
        }

        /// <summary>Return database identifier with type database SQL syntax delimiters.</summary>
        public static string SqlQuoteId(string _Identifier, CXDatabaseType _Type)
        {
            if ((_Type == CXDatabaseType.Access) || (_Type == CXDatabaseType.Sql)) return CXDatabase.SqlPrefix + _Identifier.Trim() + CXDatabase.SqlSuffix;
            else if (_Type == CXDatabaseType.MySql) return CXDatabase.MySqlPrefix + _Identifier.Trim() + CXDatabase.MySqlSuffix;
            else if (_Type == CXDatabaseType.PostgreSql) return CXDatabase.PostgreSqlPrefix + _Identifier.Trim() + CXDatabase.PostgreSqlSuffix;
            else return _Identifier.Trim();
        }

        /// <summary>Return database identifier with type database SQL syntax delimiters.</summary>
        public static string SqlQuoteIdList(string _Identifiers, CXDatabaseType _Type)
        {
            string r = "", c;
            _Identifiers = _Identifiers.Trim();
            while (_Identifiers.Length > 0)
            {
                c = CX.Extract(ref _Identifiers, ";,").Trim();
                if (c.Length > 0)
                {
                    if (r != "") r += ',';
                    r += SqlQuoteId(c, _Type);
                }
            }
            return r;
        }

        /// <summary>Return result field value of table first record with field greather than value related to database alias.</summary>
        public static string SqlRecordNext(string _Alias, string _TableName, string _FieldName, string _Value, string _ResultField)
        {
            string r = "";
            CXDataset ds;
            if (_Alias.Trim().Length < 1) _Alias = "MAIN";
            ds = new CXDataset(_Alias);
            if (ds.Open("SELECT " + SqlFieldList("ID," + _FieldName + "," + _ResultField) 
                + " FROM " + _TableName
                + " WHERE " + _FieldName + ">" + CX.Quote(_Value) 
                + " ORDER BY " + _FieldName))
            {
                if (!ds.Eof) r = ds.FieldStr(_ResultField);
            }
            ds.Close();
            ds.Dispose();
            return r;
        }

        /// <summary>Return result field value of table first record with field less than value related to database alias.</summary>
        public static string SqlRecordPrior(string _Alias, string _TableName, string _FieldName, string _Value, string _ResultField)
        {
            string r = "";
            CXDataset ds;
            if (_Alias.Trim().Length < 1) _Alias = "MAIN";
            ds = new CXDataset(_Alias);
            if (ds.Open("SELECT " + SqlFieldList("ID," + _FieldName + "," + _ResultField)
                + " FROM " + _TableName
                + " WHERE " + _FieldName + "<" + CX.Quote(_Value)
                + " ORDER BY " + _FieldName + " DESC"))
            {
                if (!ds.Eof) r = ds.FieldStr(_ResultField);
            }
            ds.Close();
            ds.Dispose();
            return r;
        }

        /// <summary>Return a sortable string representing datetime value.</summary>
        public static string SqlSortable(DateTime _Value)
        {
            return _Value.ToString(@"yyyy-MM-ddTHH:mm:ss");
        }

        /// <summary>Return a sortable string representing double value.</summary>
        public static string SqlSortable(double _Value)
        {
            return _Value.ToString("0000000000000000.0000000000");
        }

        /// <summary>Return a sortable string representing integer value.</summary>
        public static string SqlSortable(int _Value)
        {
            return _Value.ToString("0000000000000000");
        }

        /// <summary>Returns sortable string representing object field content according with data column type.</summary>
        public static string SqlSortable(DataColumn _DataColumn, object _Object)
        {
            if (CXDataType.IsDate(_DataColumn.DataType)) return CX.Date(_Object.ToString(), CX.DateFormat, true).ToString(@"yyyy-MM-ddTHH:mm:ss");
            else if (CXDataType.IsNumeric(_DataColumn.DataType)) return CX.Val(_Object.ToString()).ToString("0000000000000000.0000000000");
            else return _Object.ToString();
        }

        /// <summary>Return table name from SQL selection statement.</summary>
        public static string SqlTableName(string _SqlSelectStatement)
        {
            return SqlUnquote(CX.BtwU(_SqlSelectStatement + " ", " from ", " ").Trim()).Trim();
        }

        /// <summary>Return string with SQL expression for TRIM function corresponding to database type.</summary>
        public static string SqlTrim(string _SqlExpression, CXDatabaseType _DatabaseType)
        {
            _SqlExpression = _SqlExpression.Trim();
            if (_SqlExpression.Length > 0)
            {
                if (_DatabaseType == CXDatabaseType.Access) return "Trim(" + _SqlExpression + ")";
                else if (_DatabaseType == CXDatabaseType.Sql) return "LTRIM(RTRIM(" + _SqlExpression + "))";
                else if (_DatabaseType == CXDatabaseType.MySql) return "TRIM(" + _SqlExpression + ")";
                else if (_DatabaseType == CXDatabaseType.PostgreSql) return "TRIM(" + _SqlExpression + ")";
                else return _SqlExpression;
            }
            else return "";
        }

        /// <summary>Return database identifier without database SQL syntax delimiters.</summary>
        public static string SqlUnquote(string _Identifier)
        {
            _Identifier = _Identifier.Trim();
            if (_Identifier.Length > 1)
            {
                if ((_Identifier[0] == '[') || (_Identifier[0] == CXDatabase.SqlPrefix) || (_Identifier[0] == CXDatabase.MySqlPrefix))
                {
                    _Identifier = _Identifier.Substring(1).Trim();
                }
                if ((_Identifier[_Identifier.Length - 1] == ']')
                    || (_Identifier[_Identifier.Length - 1] == CXDatabase.SqlSuffix)
                    || (_Identifier[_Identifier.Length - 1] == CXDatabase.MySqlSuffix))
                {
                    if (_Identifier.Length > 1) _Identifier = _Identifier.Substring(0, _Identifier.Length - 1).Trim();
                    else _Identifier = "";
                }
            }
            return _Identifier;
        }

        /// <summary>Return string with SQL expression for UPPERCASE function corresponding to database type.</summary>
        public static string SqlUpper(string _SqlExpression, CXDatabaseType _Type)
        {
            _SqlExpression = _SqlExpression.Trim();
            if (_SqlExpression.Length > 0)
            {
                if (_Type == CXDatabaseType.Access) return "UCase(" + _SqlExpression + ")";
                else if (_Type == CXDatabaseType.Sql) return "UPPER(" + _SqlExpression + ")";
                else if (_Type == CXDatabaseType.MySql) return "UCASE(" + _SqlExpression + ")";
                else if (_Type == CXDatabaseType.PostgreSql) return "UPPER(" + _SqlExpression + ")";
                else return _SqlExpression;
            }
            else return "";
        }

        /// <summary>Return string SQL format for float numeric value.</summary>
        public static string SqlValue(double _Value)
        {
            return _Value.ToString("###############0.############").Replace(",", ".");
        }

        /// <summary>Return string SQL format for float numeric value.</summary>
        public static string SqlValue(string _Value)
        {
            return SqlValue(CX.Val(_Value));
        }

        #endregion

        /* */

    }

    /* */

}
