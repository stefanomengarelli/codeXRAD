/*  ------------------------------------------------------------------------
 *  
 *  File:       CXFileMode.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD file mode enumeration.
 *
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD file mode enumeration.</summary>
    public enum CXFileMode
    {
        /// <summary>None.</summary>
        None = -1,
        /// <summary>Open read mode.</summary>
        Read = 0,
        /// <summary>Open write mode.</summary>
        Write = 1,
        /// <summary>Open append mode.</summary>
        Append = 2,
        /// <summary>Random access (read/write) mode.</summary>
        RandomAccess = 3
    }

    /* */

}
