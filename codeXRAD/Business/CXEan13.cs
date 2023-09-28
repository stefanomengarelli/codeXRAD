/*  ------------------------------------------------------------------------
 *  
 *  File:       CXEan13.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD EAN 13 barcode static class.
 *  
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>Microrun XRad EAN 13 barcode static class.</summary>
    static public class CXEan13
    {

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Return checksum for EAN code.</summary>
        public static string Check(string _EanCode)
        {
            int i = 11, s = 0, t = 0;
            string c = CX.Right(@"000000000000" + CX.Mid(_EanCode, 0, 12), 12);
            while (i > -1)
            {
                s += CX.ChrToInt(c[i]);
                i -= 2;
            }
            i = 10;
            while (i > -1)
            {
                t += CX.ChrToInt(c[i]);
                i -= 2;
            }
            s = s * 3 + t;
            while (s > 10) s -= 10;
            return (10 - s).ToString();
        }

        /// <summary>Return EAN code with right checksum.</summary>
        public static string Fix(string _EanCode)
        {
            string r = CX.Right(@"000000000000" + CX.Mid(_EanCode.Trim(), 0, 12), 12);
            return r + Check(r);
        }

        /// <summary>Return true if EAN code is valid (with right checksum).</summary>
        public static bool Validate(string _EanCode)
        {
            bool r = true;
            int i = 0;
            while (r && (i < _EanCode.Length))
            {
                if ((_EanCode[i] < '0') || (_EanCode[i] > '9')) r = false;
                else i++;
            }
            if (r) r = _EanCode == Fix(_EanCode);
            return r;
        }

        #endregion

        /* */

    }

    /* */

}
