/*  ------------------------------------------------------------------------
 *  
 *  File:       CXParserAtoms.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD parser atoms collection class.
 *  
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD parser atoms collection class.</summary>
    public class CXParserAtoms
    {

        /* */

        #region Declarations

        /*  --------------------------------------------------------------------
         *  Declarations
         *  --------------------------------------------------------------------
         */

        /// <summary>Max atoms number.</summary>
        private const int maxAtoms = 512;

        #endregion

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Atoms constructor.</summary>
        public CXParserAtoms()
        {
            Items = new CXParserAtom[maxAtoms];
            Count = 0;
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get or set atoms count.</summary>
        public int Count { get; set; }

        /// <summary>Get atoms array.</summary>
        public CXParserAtom[] Items { get; private set; }

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Add atom with name, value and type.</summary>
        public void Add(string _Name, double _Value, CXParserAtomType _Type)
        {
            if (Count < maxAtoms)
            {
                Items[Count] = new CXParserAtom(_Name, _Value, _Type);
                Count++;
            }
        }

        /// <summary>Reset atoms.</summary>
        public void Clear()
        {
            Count = 0;
        }

        /// <summary>Returns index of atom with name, -1 if not found.</summary>
        public int Find(string _Name)
        {
            int i = 0, r = -1;
            while (r < 0 && i < Count) if (_Name == Items[i].Name) r = i; else i++;
            return r;
        }

        /// <summary>Returns value of atom with name, 0 if not found.</summary>
        public double Get(string _Name)
        {
            int i = Find(_Name);
            if (i > -1)
            {
                if (Items[i].Type == CXParserAtomType.Variable) return Items[i].Value;
                else return 0.0d;
            }
            else return 0.0d;
        }

        /// <summary>Set value of atom with name.</summary>
        public void Set(string _Name, double _Value)
        {
            int i = Find(_Name);
            if (i > -1)
            {
                if (Items[i].Type == CXParserAtomType.Variable) Items[i].Value = _Value;
            }
            else Add(_Name, _Value, CXParserAtomType.Variable);
        }

        #endregion

        /* */

    }

    /* */

}
