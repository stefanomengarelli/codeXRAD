/*  ------------------------------------------------------------------------
 *  
 *  File:       CXParserAtomType.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD parser atom type enumeration.
 *
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD parser atom type enumeration.</summary>
    public enum CXParserAtomType
    {
        /// <summary>Variable atom.</summary>
        Variable,
        /// <summary>Value atom.</summary>
        Value,
        /// <summary>Operator atom.</summary>
        Operator,
        /// <summary>Function atom.</summary>
        Function,
        /// <summary>Result atom.</summary>
        Result,
        /// <summary>Bracket atom.</summary>
        Bracket,
        /// <summary>Comma atom.</summary>
        Comma,
        /// <summary>Error atom.</summary>
        Error,
        /// <summary>None atom.</summary>
        None
    }

    /* */

}
