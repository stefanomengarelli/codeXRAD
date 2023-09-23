/*  ------------------------------------------------------------------------
 *  
 *  File:       Delegates.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2022 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD delegates.
 *
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>CXOnCompare delegate.</summary>
    public delegate int CXOnCompare(object _A, object _B);

    /* */

    /// <summary>CXOnDoEvents delegate.</summary>
    public delegate void CXOnDoEvents();

    /* */

    /// <summary>CXOnProgress delegate.</summary>
    public delegate void CXOnProgress(double _Percent, ref bool _Stop);

    /* */

}
