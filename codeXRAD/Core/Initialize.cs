/*  ------------------------------------------------------------------------
 *  
 *  File:       Initialize.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD static basic class: initialization.
 *  
 *  ------------------------------------------------------------------------
 */

using System.Diagnostics;

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD static basic class: initialization.</summary>
    static public partial class CX
    {

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Initialize static instance values with custom OEM identifier.</summary>
        static public void Initialize(string[] _Arguments = null, string _OEM = "")
        {
            //Ini ini;
            if (!Initialized && !Initializing)
            {
                Initializing = true;
                //
                // Preliminary initializations
                //
                Arguments = _Arguments;
                InternalPassword = @"MNG5FN:c0d3#XR4D";
                OEM = _OEM;
                //
                // Core classes initializations
                //
                InitializeStrings();
                InitializeMath();
                InitializeErrors();
                InitializeDate();
                InitializeIO();
                InitializeSystem();
                InitializePath();
                InitializeUniqueId();
                InitializeCSV();
                ////
                //// RAD classes initializations
                ////
                //XLocalization.Initialize();
                //XApplication.Initialize(_Company, _Product); // To be completed...
                //XZip.Initialize();
                //XCSV.Initialize();
                //XForm.Initialize();
                //XHolidays.Initialize();
                ////
                //// Ini settings
                ////
                //try
                //{
                //    ini = new XIni("");
                //    XDatabase.ClientMode = ini.ReadBool("SETUP", "CLIENT_MODE", false);
                //    XPath.Data = ini.ReadString("SETUP", "DATA_PATH", XPath.Data);
                //    XLocalization.Language = (XLanguage)ini.ReadInteger("SETUP", "LANGUAGE", (int)XLocalization.Language);
                //    XDatabase.DefaultCommandTimeout = ini.ReadInteger("DATABASE_SETTINGS", "DEFAULT_COMMAND_TIMEOUT", 0);
                //    XDatabase.DefaultConnectionTimeout = ini.ReadInteger("DATABASE_SETTINGS", "DEFAULT_CONNECTION_TIMEOUT", 30);
                //    XDatabase.DefaultFetchDelay = ini.ReadInteger("DATABASE_SETTINGS", "DEFAULT_FETCH_DELAY", 330);
                //    XError.Verbose = ini.ReadBool("ERROR_SETTINGS", "VERBOSE", Debug);
                //}
                //catch
                //{

                //}
                ////
                //// Resources
                ////
                //Resources = new XResources("Resources.zip");
                ////
                //// RAD classes initializations
                ////
                //XControl.Initialize();
                //XPalette.Initialize();
                //XCalendar.Initialize();
                //XHelp.Initialize();
                //XTextViewer.Recents = null;
                //
                // Cleaning operation and maintenance
                //
                WipeTemp();
                //
                // End of initialization
                //
                Initializing = false;
                Initialized = true;
            }
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get or set application passed arguments array (parameters).</summary>
        public static string[] Arguments { get; set; } = null;

        /// <summary>Get application passed arguments array (parameters) count.</summary>
        public static int ArgumentsCount
        {
            get
            {
                if (Arguments == null) return 0;
                else return Arguments.Length;
            }
        }

        /// <summary>Indicate if application use database.</summary>
        public static bool Databased 
        { 
            get 
            {
                return false;
                // return XDatabase.Databases.Count > 0; 
            } 
        }

        /// <summary>Ultramarine static core class initialized flag.</summary>
        public static bool Initialized { get; private set; }

        /// <summary>Ultramarine static core class initializing flag.</summary>
        public static bool Initializing { get; private set; }

        /// <summary>Generic internal password.</summary>
        public static string InternalPassword { get; set; }

        /// <summary>OEM id.</summary>
        public static string OEM { get; set; }

        /// <summary>Internal resources.</summary>
        // public static XResources Resources { get; set; } = null;


        #endregion

        /* */

        #region Events

        /*  --------------------------------------------------------------------
         *  Events
         *  --------------------------------------------------------------------
         */

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Return true if debugger attached.</summary>
        public static bool IsDebug()
        {
            return Debugger.IsAttached;
        }

        #endregion

        /* */

    }

    /* */

}
