/*  ------------------------------------------------------------------------
 *  
 *  File:       CXPartitaIVA.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD italian "partita IVA" static class.
 *  
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD italian "partita IVA" static class.</summary>
    static public class CXPartitaIVA
    {

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Return true if "partita IVA" passed is valid. If passed string is empty will return specified value.</summary>
        public static bool Validate(string _PartitaIVA, bool _ValidateIfEmpty)
        {
            bool r;
            int s, j, n;
            _PartitaIVA = _PartitaIVA.Trim().ToUpper();
            if (_PartitaIVA.Length > 0)
            {
                j = 0;
                r = true;
                while (r && (j < _PartitaIVA.Length))
                {
                    if ((_PartitaIVA[j] < '0') || (_PartitaIVA[j] > '9')) r = false;
                    else j++;
                }
                if (r && (_PartitaIVA.Length > 10))
                {
                    s = 0;
                    for (j = 1; j < _PartitaIVA.Length - 1; j += 2)
                    {
                        n = CX.ChrToInt(_PartitaIVA[j]) * 2;
                        s += (n / 10) + (n % 10);
                    }
                    for (j = 0; j < _PartitaIVA.Length - 1; j += 2) s += CX.ChrToInt(_PartitaIVA[j]);
                    r = CX.ChrToInt(_PartitaIVA[_PartitaIVA.Length - 1]) == (10 - (s % 10)) % 10;
                }
                else r = false;
                return r;
            }
            else return _ValidateIfEmpty;
        }

        #endregion

        /* */

    }

    /* */

}
