/*  ------------------------------------------------------------------------
 *  
 *  File:       CXParserAtom.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD parser atom class.
 *
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD parser atom class.</summary>
    public class CXParserAtom
    {

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Atom constructor.</summary>
        public CXParserAtom()
        {
            Name = "";
            Value = 0.0d;
            Type = CXParserAtomType.None;
        }

        /// <summary>Atom constructor.</summary>
        public CXParserAtom(string _Name, double _Value, CXParserAtomType _Type)
        {
            Name = _Name;
            Value = _Value;
            Type = _Type;
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get or set atom name.</summary>
        public string Name { get; set; }

        /// <summary>Get or set atom value.</summary>
        public double Value { get; set; }

        /// <summary>Get or set atom type.</summary>
        public CXParserAtomType Type { get; set; }

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Set atom error.</summary>
        public void Error(string _Name)
        {
            Name = _Name;
            Type = CXParserAtomType.Error;
        }

        #endregion

        /* */

    }

    /* */

}
