/*  ------------------------------------------------------------------------
 *  
 *  File:       CXDictionaryItem.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD dictionary item class.
 *
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD dictionary item class.</summary>
    public class CXDictionaryItem
    {

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Class constructor.</summary>
        public CXDictionaryItem()
        {
            Clear();
        }

        /// <summary>Class constructor.</summary>
        public CXDictionaryItem(CXDictionaryItem _DictionaryItem)
        {
            Assign(_DictionaryItem);
        }

        /// <summary>Class constructor.</summary>
        public CXDictionaryItem(string _Key, string _Value, object _Tag)
        {
            this.Key = _Key;
            this.Value = _Value;
            this.Tag = _Tag;
        }

        /// <summary>Class constructor.</summary>
        public CXDictionaryItem(string _CSV)
        {
            FromCSV(_CSV);
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get or set item key.</summary>
        public string Key { get; set; }

        /// <summary>Get or set item value.</summary>
        public string Value { get; set; }

        /// <summary>Get or set item tag object.</summary>
        public object Tag { get; set; }

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Assign instance properties from another.</summary>
        public void Assign(CXDictionaryItem _DictionaryItem)
        {
            this.Key = _DictionaryItem.Key;
            this.Value = _DictionaryItem.Value;
            this.Tag = _DictionaryItem.Tag;
        }

        /// <summary>Clear item.</summary>
        public void Clear()
        {
            this.Key = "";
            this.Value = "";
            this.Tag = null;
        }

        /// <summary>Compare this dictionary item instance with passed.</summary>
        public int Compare(CXDictionaryItem _DictionaryItem)
        {
            return this.Key.CompareTo(_DictionaryItem.Key);
        }

        /// <summary>Get key and value from CSV string.</summary>
        public void FromCSV(string _CSV)
        {
            this.Key = CX.ExtractCSV(ref _CSV);
            this.Value = CX.ExtractCSV(ref _CSV);
        }

        /// <summary>Return CSV string containing key and value.</summary>
        public string ToCSV()
        {
            return CX.AddCSV(CX.AddCSV("", this.Key), this.Value);
        }

        #endregion

        /* */

    }

    /* */

}
