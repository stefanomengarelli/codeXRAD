/*  ------------------------------------------------------------------------
 *  
 *  File:       CXDatasetState.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD dataset state enumeration.
 *
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD dataset state enumeration.</summary>
    public enum CXDatasetState
    {
        /// <summary>Closed.</summary>
        Closed = 0,
        /// <summary>Browse.</summary>
        Browse = 1,
        /// <summary>Insert.</summary>
        Insert = 2,
        /// <summary>Edit.</summary>
        Edit = 3,
        /// <summary>Delete.</summary>
        Delete = 4,
        /// <summary>Read.</summary>
        Read = 5
    };

    /* */

}
