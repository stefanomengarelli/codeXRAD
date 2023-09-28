/*  ------------------------------------------------------------------------
 *  
 *  File:       CXCodiceFiscale.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD italian "codice fiscale" static class.
 *
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD italian "codice fiscale" static class.</summary>
    static public class CXCodiceFiscale
    {

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Return "codice fiscale" checksum part or empty string if not valid.</summary>
        public static string Check(string _CodiceFiscale)
        {
            int i, sum, c;
            int[] odd = new int[26] { 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23 };
            string r = "";
            _CodiceFiscale = _CodiceFiscale.Trim().ToUpper();
            if (_CodiceFiscale.Length > 14)
            {
                sum = 0;
                for (i = 0; i < 15; i++)
                {
                    c = CX.Asc(_CodiceFiscale[i]);
                    if ((_CodiceFiscale[i] >= '0') && (_CodiceFiscale[i] <= '9')) c += 17;
                    c -= 65;
                    if (i % 2 != 0) sum += c;
                    else sum += odd[c];
                }
                sum %= 26;
                r = CX.Chr(sum + 65) + "";
            }
            return r;
        }

        /// <summary>Return "codice fiscale" birthday date and sex part or empty string if not valid.</summary>
        public static string Date(DateTime _BirthDay, string _Sex)
        {
            const string months = "ABCDEHLMPRST";
            int i;
            string r = "";
            if (_BirthDay != null)
            {
                _Sex = _Sex.Trim().ToUpper();
                if (_Sex.Length > 0)
                {
                    r = _BirthDay.Year.ToString().PadLeft(2, '0');
                    r += months[_BirthDay.Month - 1];
                    i = _BirthDay.Day;
                    if (_Sex[0] == 'F') i += 40;
                    r += i.ToString().PadLeft(2, '0');
                }
            }
            return r;
        }

        /// <summary>Return "codice fiscale" first name part or empty string if not valid.</summary>
        public static string FirstName(string _FirstName)
        {
            string c, v;
            _FirstName = _FirstName.Trim().ToUpper();
            if (_FirstName.Length > 0)
            {
                c = CX.GetConsonants(_FirstName);
                v = CX.GetVocals(_FirstName);
                if (c.Length > 3) c = c.Substring(0, 1) + c.Substring(2);
                if (v.Length > 0) return (c + v + "XX").Substring(0, 3);
                else return "";
            }
            else return "";
        }

        /// <summary>Return "codice fiscale" last name part or empty string if not valid.</summary>
        public static string LastName(string _LastName)
        {
            string c, v;
            _LastName = _LastName.Trim().ToUpper();
            if (_LastName.Length > 0)
            {
                c = CX.GetConsonants(_LastName);
                v = CX.GetVocals(_LastName);
                if (v.Length > 0) return (c + v + "XX").Substring(0, 3);
                else return "";
            }
            else return "";
        }

        #endregion

        /* */

    }

    /* */

}
