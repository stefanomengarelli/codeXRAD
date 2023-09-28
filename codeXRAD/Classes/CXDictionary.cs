/*  ------------------------------------------------------------------------
 *  
 *  File:       CXDictionary.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD dictionary class.
 *
 *  ------------------------------------------------------------------------
 */

using System.Text;

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD dictionary class.</summary>
    public class CXDictionary
    {

        /* */

        #region Declarations

        /*  --------------------------------------------------------------------
         *  Declarations
         *  --------------------------------------------------------------------
         */

        /// <summary>Dictionary items collection.</summary>
        private List<CXDictionaryItem> items = new List<CXDictionaryItem>();

        /// <summary>Last index found.</summary>
        private int lastIndex = -1;

        /// <summary>Last key found.</summary>
        private string lastKey = "";

        /// <summary>Sorted list flag.</summary>
        private bool sorted = true;

        #endregion

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Class constructor.</summary>
        public CXDictionary()
        {
            Clear();
        }

        /// <summary>Class constructor.</summary>
        public CXDictionary(CXDictionary _Dictionary)
        {
            Assign(_Dictionary);
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Quick access declaration.</summary>
        public CXDictionaryItem this[int _Index]
        {
            get { return items[_Index]; }
            set { items[_Index] = value; }
        }

        /// <summary>Get items count.</summary>
        public int Count { get { return items.Count; } }

        /// <summary>Get or set ignore case flag.</summary>
        public bool IgnoreCase { get; set; } = false;

        /// <summary>Get or set sorted list.</summary>
        public bool Sorted
        {
            get { return sorted; }
            set
            {
                if (sorted!=value)
                {
                    sorted = value;
                    if (sorted) Sort();
                }
            }
        }

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Add dictionary item and sort collection.</summary>
        public void Add(CXDictionaryItem _DictionaryItem)
        {
            lastIndex = -1;
            lastKey = "";
            items.Add(_DictionaryItem);
            if (sorted) Sort(true);
        }

        /// <summary>Add dictionary item and sort collection.</summary>
        public void Add(string _Key, string _Value, object _Tag)
        {
            Add(new CXDictionaryItem(_Key, _Value, _Tag)); ;
        }

        /// <summary>Assign instance properties from another.</summary>
        public void Assign(CXDictionary _Dictionary)
        {
            int i;
            lastIndex = -1;
            lastKey = "";
            sorted = _Dictionary.sorted;
            IgnoreCase = _Dictionary.IgnoreCase;
            items.Clear();
            if (_Dictionary.items != null)
            {
                for (i = 0; i < _Dictionary.items.Count; i++)
                {
                    items.Add(_Dictionary.items[i]);
                }
            }
        }

        /// <summary>Clear item.</summary>
        public void Clear()
        {
            lastIndex = -1;
            lastKey = "";
            items.Clear();
        }

        /// <summary>Find first item with passed key. It possible to indicate 
        /// if search must be binary otherwise sequential. 
        /// Return item index or -1 if not found.</summary>
        public int Find(string _Key)
        {
            int i, max, mid, min, r = -1;
            if (items.Count > 0)
            {
                if ((lastKey == _Key) && (lastIndex > -1)) r = lastIndex;
                else if (sorted)
                {
                    //
                    // binary search
                    //
                    min = 0;
                    max = items.Count - 1;
                    while ((r < 0) && (min <= max))
                    {
                        mid = (min + max) / 2;
                        i = String.Compare(_Key, items[mid].Key, IgnoreCase);
                        if (i == 0) r = mid;
                        else if (i < 0) max = mid - 1;
                        else min = mid + 1;
                    }
                    //
                    // search first
                    //
                    while (r > 0)
                    {
                        if (String.Compare(_Key, items[r - 1].Key, IgnoreCase) == 0) r--;
                        else break;
                    }
                    lastIndex = r;
                    lastKey = _Key;
                }
                else
                {
                    //
                    // sequential search
                    //
                    i = 0;
                    while ((r < 0) && (i < items.Count))
                    {
                        if (String.Compare(_Key, items[i].Key, IgnoreCase) == 0) r = i;
                        i++;
                    }
                    lastIndex = r;
                    lastKey = _Key;
                }
            }
            return r;
        }

        /// <summary>Load dictionary from the text file specified by file name
        /// with specified text encoding. Returns true if succeed.</summary>
        public bool Load(string _FileName, string _Password = null, Encoding _Encoding = null)
        {
            StreamReader sr;
            bool p = !CX.Empty(_Password);
            string k;
            items.Clear();
            if (System.IO.File.Exists(_FileName))
            {
                try
                {
                    if (_Encoding == null) _Encoding = CX.TextEncoding;
                    sr = new System.IO.StreamReader(_FileName, _Encoding);
                    while (!sr.EndOfStream)
                    {
                        k = sr.ReadLine();
                        if (k.Trim().Length > 0)
                        {
                            if (p) k = CX.FromHexMask(k, _Password);
                            Add(new CXDictionaryItem(k));
                        }
                    }
                    sr.Close();
                    sr.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    CX.Error(ex);
                    return false;
                }
            }
            else return false;
        }

        /// <summary>Save dictionary in the text file specified by file name
        /// with specified text encoding. Returns true if succeed.</summary>
        public bool Save(string _FileName, string _Password = null, Encoding _Encoding = null)
        {
            int i = 0;
            bool p = !CX.Empty(_Password);
            StreamWriter sw;
            try
            {
                if (_Encoding == null) _Encoding = CX.TextEncoding;
                sw = new StreamWriter(_FileName, false, _Encoding);
                sw.NewLine = CX.CR;
                while (i < items.Count)
                {
                    if (p) sw.WriteLine(CX.ToHexMask(items[i].ToCSV(), _Password));
                    else sw.WriteLine(items[i].ToCSV());
                    i++;
                }
                sw.Close();
                sw.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                return false;
            }
        }

        /// <summary>Sort list.</summary>
        public void Sort(bool _AppendOnSortedList = false)
        {
            bool b = true;
            int i = items.Count;
            CXDictionaryItem swap;
            while (b)
            {
                b = false;
                while (i > 1)
                {
                    i--;
                    if (String.Compare(items[i].Key, items[i - 1].Key, IgnoreCase) < 0)
                    {
                        b = true;
                        swap = items[i];
                        items[i] = items[i - 1];
                        items[i - 1] = swap;
                    }
                    else if (_AppendOnSortedList) i = 0;
                }
            }
        }

        /// <summary>Return tag of first items with passed key.
        /// Return null if not found.</summary>
        public object TagOf(string _Key)
        {
            int i = Find(_Key);
            if (i > -1) return items[i].Tag;
            else return null;
        }

        /// <summary>Return value of first items with passed key.
        /// Return empty string if not found.</summary>
        public string ValueOf(string _Key)
        {
            int i = Find(_Key);
            if (i > -1) return items[i].Value;
            else return "";
        }

        #endregion

        /* */

    }

    /* */

}
