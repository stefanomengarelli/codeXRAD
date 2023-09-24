/*  ------------------------------------------------------------------------
 *  
 *  File:       CSV.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD CSV management static functions.
 *
 *  ------------------------------------------------------------------------
 */

using System;

namespace codeXRAD
{

    /* */

    public static partial class CX
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

        /// <summary>Initialize SMSharp CSV management static functions.</summary>
        public static void InitializeCSV()
        {
            CSVSeparator = ';';
            CSVDelimiter = '"';
            LoadCSVSettings();
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get or set CSV delimiter char.</summary>
        public static char CSVDelimiter { get; set; }

        /// <summary>Get or set CSV separator char.</summary>
        public static char CSVSeparator { get; set; }

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Return CSV string adding value.</summary>
        public static string AddCSV(string _CSV, string _Value)
        {
            _CSV = _CSV.Trim();
            if (_CSV.Length > 0) _CSV += CSVSeparator;
            _Value = _Value.Replace("" + CSVDelimiter, @"\" + CSVDelimiter);
            return _CSV + CSVDelimiter + _Value + CSVDelimiter;
        }

        /// <summary>Return CSV string adding value with delimiter if specified.</summary>
        public static string AddCSV(string _CSV, string _Value, bool _Delimiter)
        {
            _CSV = _CSV.Trim();
            if (_CSV.Length > 0) _CSV += CSVSeparator;
            _Value = _Value.Replace("" + CSVDelimiter, @"\" + CSVDelimiter);
            if (_Delimiter) return _CSV + CSVDelimiter + _Value + CSVDelimiter;
            else return _CSV + _Value;
        }

        /// <summary>Return CSV string adding value.</summary>
        public static string AddCSV(string _CSV, int _Value)
        {
            _CSV = _CSV.Trim();
            if (_CSV.Length > 0) _CSV += CSVSeparator;
            return _CSV + _Value.ToString();
        }

        /// <summary>Return CSV string adding value.</summary>
        public static string AddCSV(string _CSV, double _Value)
        {
            _CSV = _CSV.Trim();
            if (_CSV.Length > 0) _CSV += CSVSeparator;
            return _CSV + _Value.ToString("####################.################").Replace(DecimalSeparator, '.');
        }

        /// <summary>Return CSV string adding value.</summary>
        public static string AddCSV(string _CSV, DateTime _Value)
        {
            _CSV = _CSV.Trim();
            if (_CSV.Length > 0) _CSV += CSVSeparator;
            return _CSV + Str(_Value, CXDateFormat.iso8601, true);
        }

        /// <summary>Estract first CSV value of string.</summary>
        public static string ExtractCSV(ref string _CSV)
        {
            int i = 0;
            bool q, h = false, w = true;
            string r = "";
            _CSV = _CSV.Trim();
            if (_CSV.Length > 0)
            {
                if (_CSV[0] == CSVSeparator)
                {
                    if (_CSV.Length > 1) _CSV = _CSV.Substring(1);
                    else _CSV = "";
                }
                else
                {
                    q = _CSV[0] == CSVDelimiter;
                    if (q) i++;
                    while (w && (i < _CSV.Length))
                    {
                        if (!q && (_CSV[i] == CSVSeparator))
                        {
                            if (h)
                            {
                                r += _CSV[i];
                                h = false;
                            }
                            else
                            {
                                i++;
                                if (_CSV.Length > i) _CSV = _CSV.Substring(i);
                                else _CSV = "";
                                w = false;
                            }
                        }
                        else if (q && (_CSV[i] == '\\'))
                        {
                            if (h)
                            {
                                r += _CSV[i];
                                h = false;
                            }
                            else h = true;
                        }
                        else if (_CSV[i] == CSVDelimiter)
                        {
                            if (h)
                            {
                                r += _CSV[i];
                                h = false;
                            }
                            else if (q) q = false;
                            else r += _CSV[i];
                        }
                        else
                        {
                            r += _CSV[i];
                            h = false;
                        }
                        i++;
                    }
                    if (w) _CSV = "";
                }
            }
            return r;
        }

        /// <summary>Load CSV settings from default application INI file.</summary>
        public static void LoadCSVSettings()
        {
            CXIni ini = new CXIni("");
            CSVDelimiter = (ini.ReadString("CSV_SETTINGS", "DELIMITER", CSVDelimiter + "").Trim() + ';')[0];
            CSVSeparator = (ini.ReadString("CSV_SETTINGS", "SEPARATOR", CSVSeparator + "").Trim() + '"')[0];
        }

        /// <summary>Save CSV settings to default application INI file.</summary>
        public static bool SaveCSVSettings()
        {
            CXIni ini = new CXIni("");
            ini.WriteString("CSV_SETTINGS", "DELIMITER", CSVDelimiter + "");
            ini.WriteString("CSV_SETTINGS", "SEPARATOR", CSVSeparator + "");
            return ini.Save();
        }

        #endregion

        /* */

    }

    /* */

}
