/*  ------------------------------------------------------------------------
 *  
 *  File:       CXDatabaseType.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD database type enumeration.
 *
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD database type enumeration.</summary>
    public enum CXDatabaseType
    {
        /// <summary>None.</summary>
        None = -1,
        /// <summary>Microsoft Access (MDB).</summary>
        Access = 0,
        /// <summary>Microsoft SQL Server.</summary>
        Sql = 1,
        /// <summary>MySQL Server.</summary>
        MySql = 2,
        /// <summary>DBase IV.</summary>
        DBase4 = 3,
        /// <summary>PostgreSQL.</summary>
        PostgreSql = 4
    }

    /* */

}
