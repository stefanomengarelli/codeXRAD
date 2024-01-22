/*  ------------------------------------------------------------------------
 *  
 *  File:       CXControlEditable.cs
 *  Version:    1.0.1
 *  Date:       October 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD control editable modes enumeration.
 *
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD.Win
{

    /* */

    /// <summary>codeXRAD control editable modes enumeration.</summary>
    public enum CXControlEditable
    {
        /// <summary>Always editable.</summary>
        Always,
        /// <summary>Editable only on insert.</summary>
        Insert,
        /// <summary>Never editable (readonly).</summary>
        Never
    }

    /* */

}
