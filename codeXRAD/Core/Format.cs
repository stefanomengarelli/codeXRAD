/*  ------------------------------------------------------------------------
 *  
 *  File:       Format.cs
 *  Version:    1.0.0
 *  Date:       October 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD string format static class.
 *  
 *  ------------------------------------------------------------------------
 */

using System;

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD string format static class.</summary>
    static public partial class CX
    {

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Returns string formatted by format parameter wich can assume special values as
        /// if start by 0 or # will be formatted as common double value number format string; 
        /// &amp;D,&amp;DATE for date format;
        /// $,&amp;CUR for currency format;
        /// &amp;CURNZ for currency format, empty if zero;
        /// !,&amp;UPPER for uppercase format; 
        /// &amp;HM for HH:MM time format; 
        /// &amp;T, &amp;HMS, &amp;TIME for HH:MM:SS time format; 
        /// &amp;DT, &amp;DATETIME form datetime format;
        /// &amp;DU, &amp;DURATION form duration format HHHHHH:MM:SS:ZZZ;
        /// &amp;DUS, &amp;DURATIONSEC form duration format HHHHHH:MM:SS;
        /// &amp;DUM, &amp;DURATIONMIN form duration format HHHHHH:MM;
        /// &amp;DUMNZ form duration format HHHHHH:MM or string empty if zero;
        /// &amp;DUC, &amp;DURATIONCENT form duration format HHHHHH.CC;
        /// &amp;DUD, &amp;DURATIONDAY form duration format DDDD.CC;
        /// &amp;LOWER for lowercase format;
        /// &amp;CUR+ for currency more accurate format;
        /// &amp;CUR+NZ for currency more accurate format, empty if zero;
        /// &amp;QTY for quantity format;
        /// &amp;QTYNZ form quantity format, empty if zero;
        /// &amp;QTY+ for quantity more accurate format;
        /// &amp;QTY+NZ form quantity more accurate format, empty if zero;
        /// &amp;EU,&amp;EUR,&amp;EURO for euro money format; 
        /// &amp;EUNZ,&amp;EURNZ,&amp;EURONZ for euro money format with empty string if zero; 
        /// &amp;USD,&amp;DOLLAR for US dollar money format; 
        /// &amp;USDNZ,&amp;DOLLARNZ for US dollar money format with empty string if zero.
        /// </summary>
        static public string Str(string _String, string _Format)
        {
            _Format = _Format.Trim(); 
            if (_Format.Length > 0)
            {
                if ((_Format[0] == '#') || (_Format[0] == '0')) return CX.Val(_String).ToString(_Format);
                else if ((_Format == @"&D") || (_Format == @"&DATE")) return CX.Str(CX.Date(_String, CX.DateFormat, false));
                else if ((_Format == @"$") || (_Format == @"&CUR")) return CX.Val(_String).ToString(CX.CurrencyFormat);
                else if (_Format == @"&CURNZ") return CX.Val(_String).ToString(CX.CurrencyFormat + ";; ");
                else if ((_Format == @"!") || (_Format == @"&UPPER")) return _String.ToUpper();
                else if (_Format == @"&HM") return CX.Mid(CX.FixTime(_String), 0, 5);
                else if ((_Format == @"&T") || (_Format == @"&HMS") || (_Format == @"&TIME")) return CX.FixTime(_String);
                //else if ((_Format == @"&DT") || (_Format == @"&DATETIME")) return XDate.Str(XDate.Date(_String, XDate.DateFormat, true), true);
                //else if ((_Format == @"&DU") || (_Format == @"&DURATION")) return XDuration.Fix(_String, XDurationFormat.HHHHHHMMSSZZZ, false);
                //else if ((_Format == @"&DUS") || (_Format == @"&DURATIONSEC")) return CX.Mid(XDuration.Fix(_String, XDurationFormat.HHHHHHMMSS, false), 0, 12);
                //else if (_Format == @"&DUMNZ") return CX.Mid(CX.dura(_String, XDurationFormat.HHHHHHMM, true), 0, 9);
                //else if ((_Format == @"&DUM") || (_Format == @"&DURATIONMIN")) return CX.Mid(XDuration.Fix(_String, XDurationFormat.HHHHHHMM, false), 0, 9);
                //else if ((_Format == @"&DUC") || (_Format == @"&DURATIONCENT")) return CX.Mid(XDuration.Fix(_String, XDurationFormat.HHHHHHCC, false), 0, 9);
                //else if ((_Format == @"&DUD") || (_Format == @"&DURATIONDAY")) return CX.Mid(XDuration.Fix(_String, XDurationFormat.DDDDCC, false), 0, 7);
                else if (_Format == @"&LOWER") return _String.ToLower();
                else if (_Format == @"&CUR+") return CX.Val(_String).ToString(CX.CurrencyPrecisionFormat);
                else if (_Format == @"&CUR+NZ") return CX.Val(_String).ToString(CX.CurrencyPrecisionFormat + ";; ");
                else if (_Format == @"&QTY") return CX.Val(_String).ToString(CX.QuantityFormat);
                else if (_Format == @"&QTYNZ") return CX.Val(_String).ToString(CX.QuantityFormat + ";; ");
                else if (_Format == @"&QTY+") return CX.Val(_String).ToString(CX.QuantityPrecisionFormat);
                else if (_Format == @"&QTY+NZ") return CX.Val(_String).ToString(CX.QuantityPrecisionFormat + ";; ");
                else if ((_Format == @"&EU") || (_Format == @"&EURO") || (_Format == @"&EUR")
                    || (_Format == @"&USD") || (_Format == @"&DOLLAR")) return CX.Val(_String).ToString("###,###,###,###,##0.00");
                else if ((_Format == @"&EUNZ") || (_Format == @"&EURONZ") || (_Format == @"&EURNZ")
                    || (_Format == @"&USDNZ") || (_Format == @"&DOLLARNZ")) return CX.Val(_String).ToString("###,###,###,###,##0.00;; ");
                else if ((_Format == @"&EUNZ") || (_Format == @"&EURONZ") || (_Format == @"&EURNZ")
                    || (_Format == @"&USDNZ") || (_Format == @"&DOLLARNZ")) return CX.Val(_String).ToString("###,###,###,###,##0.00;; ");
                else return String.Format(@"{0:" + _Format + @"}", _String);
            }
            else return _String; 
        }

        /// <summary>Return sortable string related to format type.</summary>
        static public string SqlSortable(string _String, string _Format)
        {
            _Format = _Format.Trim();
            if (_Format.Length > 0) 
            {
                if ((_Format[0] == '#') || (_Format[0] == '0') || (_Format == @"$") || (_Format == @"&CUR")
                    || (_Format == @"&CURNZ") || (_Format == @"&CUR+") || (_Format == @"&CUR+NZ") || (_Format == @"&QTY")
                    || (_Format == @"&QTYNZ") || (_Format == @"&QTY+") || (_Format == @"&QTY+NZ") || (_Format == @"&EU")
                    || (_Format == @"&EURO") || (_Format == @"&EUR") || (_Format == @"&USD") || (_Format == @"&DOLLAR")
                    || (_Format == @"&EUNZ") || (_Format == @"&EURONZ") || (_Format == @"&EURNZ")
                    || (_Format == @"&USDNZ") || (_Format == @"&DOLLARNZ"))
                {
                    return CX.Val(_String).ToString("0000000000000000.0000000000");
                }
                else if ((_Format == @"&D") || (_Format == @"&DATE"))
                {
                    return CX.Date(_String, CX.DateFormat, true).ToString(@"yyyy-MM-ddTHH:mm:ss");
                }
                else return _String;
            }
            else return _String;
        }

        /// <summary>Return value type related to format type (C=chars, N=number, D=Date).</summary>
        static public char Type(string _Format)
        {
            _Format = _Format.Trim();
            if (_Format.Length > 0)
            {
                if ((_Format[0] == '#') || (_Format[0] == '0') || (_Format == @"$") || (_Format == @"&CUR")
                    || (_Format == @"&CURNZ") || (_Format == @"&CUR+") || (_Format == @"&CUR+NZ") || (_Format == @"&QTY")
                    || (_Format == @"&QTYNZ") || (_Format == @"&QTY+") || (_Format == @"&QTY+NZ") || (_Format == @"&EU")
                    || (_Format == @"&EURO") || (_Format == @"&EUR") || (_Format == @"&USD") || (_Format == @"&DOLLAR")
                    || (_Format == @"&EUNZ") || (_Format == @"&EURONZ") || (_Format == @"&EURNZ")
                    || (_Format == @"&USDNZ") || (_Format == @"&DOLLARNZ"))
                {
                    return 'N';
                }
                else if ((_Format == @"&D") || (_Format == @"&DATE")) return 'D';
                else return 'C';
            }
            else return 'C';
        }

        #endregion

        /* */

    }

    /* */

}
