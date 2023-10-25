/*  ------------------------------------------------------------------------
 *  
 *  File:       Html.cs
 *  Version:    1.0.0
 *  Date:       October 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD HTML static functions.
 *
 *  ------------------------------------------------------------------------
 */

using System.Text;
using System.Web;

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

        /// <summary>Decode string containing HTML entities in actual characters.</summary>
        public static string DecodeHtml(string _String)
        {
            return HttpUtility.HtmlDecode(_String);
        }

        /// <summary>Esegue la codifica forte in HTML rimpiazzando i singoli apici con &quot;
        /// e i doppi con &quot;.</summary>
        public static string EncodeHtml(string _Value, char _ReplaceSpecialCharsWith = '\0')
        {
            int a, i;
            char[] c;
            StringBuilder r;
            if (_Value == null) return "";
            else if (_Value.Length < 1) return "";
            else
            {
                r = new StringBuilder();
                c = HttpUtility.HtmlEncode(_Value).Replace("\"", "&quot;").Replace("'", "&apos;").ToCharArray();
                if (c != null)
                {
                    if (_ReplaceSpecialCharsWith == '\0')
                    {
                        for (i = 0; i < c.Length; i++)
                        {
                            a = Convert.ToInt32(c[i]);
                            if (a > 127) r.AppendFormat("&#{0};", a);
                            else r.Append(c[i]);
                        }
                    }
                    else
                    {
                        for (i = 0; i < c.Length; i++)
                        {
                            a = Convert.ToInt32(c[i]);
                            if (a > 127) r.Append(_ReplaceSpecialCharsWith);
                            else r.Append(c[i]);
                        }
                    }
                    return r.ToString();
                }
                else return "";
            }
        }

        #endregion

        /* */

    }

    /* */

}
