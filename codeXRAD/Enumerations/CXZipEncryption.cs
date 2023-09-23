/*  ------------------------------------------------------------------------
 *  
 *  File:       CXZipEncryption.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD zip encryption mode enumeration.
 *
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD zip encryption mode enumeration.</summary>
    public enum CXZipEncryption
    {
        /// <summary>Default encryption.</summary>
        Default,
        /// <summary>AES 128 bit encryption.</summary>
        AES128,
        /// <summary>AES 256 bit encryption.</summary>
        AES256
    };

    /* */

}
