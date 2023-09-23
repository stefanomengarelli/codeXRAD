/*  ------------------------------------------------------------------------
 *  
 *  File:       Errors.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD static basic class: errors.
 *
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD static basic class: errors.</summary>
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

        /// <summary>Initialize static error management class environment.</summary>
        static public void InitializeErrors()
        {
            ErrorMessage = "";
            Exception = null;
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get or set last error message.</summary>
        public static string ErrorMessage { get; set; }

        /// <summary>Get or set last exception.</summary>
        public static Exception Exception { get; set; }

        /// <summary>Get or set last exception message as string.</summary>
        public static string ExceptionMessage 
        { 
            get
            {
                if (Exception == null) return "";
                else return Exception.Message;
            }
        }

        /// <summary>Return true if last error instance contains error.</summary>
        public static bool IsError
        {
            get { return (ErrorMessage.Trim().Length > 0) || (Exception != null); }
        }

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Clear last error.</summary>
        public static void Error()
        {
            ErrorMessage = "";
            Exception = null;
        }

        /// <summary>Set last error.</summary>
        public static void Error(string _Error, Exception _Exception)
        {
            ErrorMessage = _Error;
            Exception = _Exception;
        }

        /// <summary>Set last error exception.</summary>
        public static void Error(Exception _Exception)
        {

            if (_Exception == null) ErrorMessage = "";
            else if (_Exception.Message == null) ErrorMessage = "";
            else ErrorMessage = _Exception.Message;
            Exception = _Exception;
        }

        /// <summary>Set last error and throw exception if specified.</summary>
        public static void Raise(string _ErrorMessage, bool _RaiseException)
        {
            ErrorMessage = _ErrorMessage;
            Exception = new Exception(_ErrorMessage);
            if (_RaiseException) throw Exception;
        }

        /// <summary>Get stack trace.</summary>
        public static string StackTrace()
        {
            try
            {
                if (Environment.StackTrace == null) return "";
                else return Environment.StackTrace;
            }
            catch
            {
                return "";
            }
        }

        #endregion

        /* */

    }

    /* */

}
