/*  ------------------------------------------------------------------------
 *  
 *  File:       Tags.cs
 *  Version:    1.0.0
 *  Date:       October 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD tags helper static functions.
 *
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    public static partial class CX
    {

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Return string value included between opening and closing tag
        /// and decoded for html entities if specified. If tag not present return default value.</summary>
        public static string TagGet(string _String, string _Tag, string _Default, bool _DecodeEntities = true)
        {
            int i = CX.PosU(_String, "<" + _Tag), j, k = -1;
            string t;
            if ((i > -1) && (_Tag.Trim().Length > 0))
            {
                j = i + 1;
                while ((k < 0) && (j < _String.Length))
                {
                    if (_String[j] == '>') k = j;
                    j++;
                }
                if (k > -1)
                {
                    t = CX.BtwU(_String, CX.Mid(_String, i, k - i + 1), "</" + _Tag);
                    if (_DecodeEntities) return DecodeHtml(t);
                    else return t;
                }
                else return _Default;
            }
            else return _Default;
        }

        /// <summary>Return boolean value included between opening and closing tag.</summary>
        public static bool TagGetBool(string _String, string _Tag, bool _Default)
        {
            string r = CX.TagGet(_String, _Tag, CX.Unknown, true);
            if (r == CX.Unknown) return _Default;
            else return CX.Bool(r);
        }

        /// <summary>Return bytes array value included between opening and closing tag.</summary>
        public static byte[] TagGetBytes(string _String, string _Tag)
        {
            string r = CX.TagGet(_String, _Tag, CX.Unknown, true);
            if (r == CX.Unknown) return null;
            else return CX.Base64DecodeBytes(r);
        }

        /// <summary>Return datetime value included between opening and closing tag.</summary>
        public static DateTime TagGetDateTime(string _String, string _Tag, DateTime _Default)
        {
            string r = TagGet(_String, _Tag, CX.Unknown, true);
            if (r == CX.Unknown) return _Default;
            else return CX.Date(_String, CXDateFormat.iso8601, true);
        }

        /// <summary>Return double value included between opening and closing tag.</summary>
        public static double TagGetDouble(string _String, string _Tag, double _Default)
        {
            string r = TagGet(_String, _Tag, CX.Unknown, true);
            if (r == CX.Unknown) return _Default;
            else return CX.ToDouble(r.Replace('.', CX.DecimalSeparator));
        }

        /// <summary>Return hex masked value included between opening and closing tag.</summary>
        public static string TagGetHexMask(string _String, string _Tag, string _Default)
        {
            string r = TagGet(_String, _Tag, CX.Unknown, true);
            if (r == CX.Unknown) return _Default;
            else return CX.FromHexMask(r, CX.InternalPassword);
        }

        /// <summary>Return int value included between opening and closing tag.</summary>
        public static int TagGetInt(string _String, string _Tag, int _Default)
        {
            string r = TagGet(_String, _Tag, CX.Unknown, true);
            if (r == CX.Unknown) return _Default;
            else return CX.ToInt(r);
        }

        /// <summary>Return string adding tagged string value encoded with html entities if specified.</summary>
        public static string TagSet(string _String, string _Tag, string _Value, bool _EncodeEntities = true)
        {
            int i, j, k = -1;
            _String = _String.Trim();
            if (_Tag.Trim().Length > 0)
            {
                i = CX.PosU(_String, "<" + _Tag);
                if (_EncodeEntities) _Value = CX.EncodeHtml(_Value);
                if (i > -1)
                {
                    j = i + 1 + _Tag.Length;
                    while ((k < 0) && (j < _String.Length))
                    {
                        if (_String[j] == '>') k = j;
                        j++;
                    }
                    if (k > -1)
                    {
                        j = CX.PosU(_String.Substring(k), "</" + _Tag);
                        if (j > -1) return CX.Mid(_String, 0, k + 1) + _Value + CX.Mid(_String, j + k, _String.Length);
                        else _String = CX.Mid(_String, 0, i);
                    }
                    else _String = CX.Mid(_String, 0, i);
                }
                if (!_String.EndsWith("\r\n")) _String += "\r\n";
                return _String + '<' + _Tag + '>' + _Value + "</" + _Tag + '>';
            }
            else return _String;
        }

        /// <summary>Return string adding tagged boolean value.</summary>
        public static string TagSetBool(string _String, string _Tag, bool _Value)
        {
            return TagSet(_String, _Tag, CX.Bool(_Value), true);
        }

        /// <summary>Return string adding tagged bytes array value.</summary>
        public static string TagSetBytes(string _String, string _Tag, byte[] _Value)
        {
            return TagSet(_String, _Tag, CX.Base64EncodeBytes(_Value), true);
        }

        /// <summary>Return string adding tagged datetime value.</summary>
        public static string TagSetDateTime(string _String, string _Tag, DateTime _Value)
        {
            return TagSet(_String, _Tag, CX.Str(_Value, CXDateFormat.iso8601, true), true);
        }

        /// <summary>Return string adding tagged double value.</summary>
        public static string TagSetDouble(string _String, string _Tag, double _Value)
        {
            return TagSet(_String, _Tag, CX.Str(_Value).Replace(CX.DecimalSeparator, '.'), true);
        }

        /// <summary>Return string adding tagged hex masked value.</summary>
        public static string TagSetHexMask(string _String, string _Tag, string _Value)
        {
            return TagSet(_String, _Tag, CX.ToHexMask(_Value, CX.InternalPassword), true);
        }

        /// <summary>Return string adding tagged integer value.</summary>
        public static string TagSetInt(string _String, string _Tag, int _Value)
        {
            return TagSet(_String, _Tag, _Value.ToString(), true);
        }

        #endregion

        /* */

    }

    /* */

}
