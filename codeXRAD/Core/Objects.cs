/*  ------------------------------------------------------------------------
 *  
 *  File:       Objects.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD static basic class: objects.
 *
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD static basic class: objects.</summary>
    static public partial class CX
    {

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Return index of object matching with value according to compare method, or -1 if fails.</summary>
        public static int Find(object _Value, List<object> _Objects, CXOnCompare _CompareMethod, bool _BinarySearch = true)
        {
            int i, max, mid, min, r = -1;
            if ((_Value != null) && (_Objects != null))
            {
                if (_BinarySearch)
                {
                    min = 0;
                    max = _Objects.Count - 1;
                    while ((r < 0) && (min <= max))
                    {
                        mid = (min + max) / 2;
                        i = _CompareMethod(_Value, _Objects[mid]);
                        if (i == 0) r = mid;
                        else if (i < 0) max = mid - 1;
                        else min = mid + 1;
                    }
                    while (r > 0)
                    {
                        if (_CompareMethod(_Value, _Objects[r - 1]) == 0) r--;
                        else break;
                    }
                }
                else
                {
                    i = 0;
                    while ((r < 0) && (i < _Objects.Count))
                    {
                        if (_CompareMethod(_Value, _Objects[i]) == 0) r = i;
                        i++;
                    }
                }
            }
            return r;
        }

        /// <summary>Return index of object matching with value according to compare method and index, or -1 if fails.</summary>
        public static int Find(object _Value, List<object> _Objects, List<int> _Index, CXOnCompare _CompareMethod)
        {
            int i, max, mid, min, r = -1;
            if ((_Value != null) && (_Objects != null) && (_Index != null))
            {
                min = 0;
                max = _Index.Count - 1;
                while ((r < 0) && (min <= max))
                {
                    mid = (min + max) / 2;
                    i = _CompareMethod(_Value, _Objects[_Index[mid]]);
                    if (i == 0) r = mid;
                    else if (i < 0) max = mid - 1;
                    else min = mid + 1;
                }
                while (r > 0)
                {
                    if (_CompareMethod(_Value, _Objects[_Index[r - 1]]) == 0) r--;
                    else break;
                }
                if (r > -1) r = _Index[r];
            }
            return r;
        }

        /// <summary>Sort object list by passed compare method.</summary>
        public static void Sort(List<object> _Objects, CXOnCompare _CompareMethod, bool _SortOnAddToSortedArray = false)
        {
            int i;
            bool b = true;
            object swap;
            while (b)
            {
                b = false;
                i = _Objects.Count - 1;
                while (i > 0)
                {
                    if (_CompareMethod(_Objects[i], _Objects[i - 1]) < 0)
                    {
                        b = true;
                        swap = _Objects[i];
                        _Objects[i] = _Objects[i - 1];
                        _Objects[i - 1] = swap;
                    }
                    else if (_SortOnAddToSortedArray)
                    {
                        b = false;
                        i = 0;
                    }
                    i--;
                }
            }
        }

        /// <summary>Sort index of object list by passed compare method.</summary>
        public static void Sort(List<int> _Index, List<object> _Objects, CXOnCompare _CompareMethod, bool _SortOnAddToSortedArray = false)
        {
            int i, swap;
            bool b = true;
            while (b)
            {
                b = false;
                i = _Index.Count - 1;
                while (i > 0)
                {
                    if (_CompareMethod(_Objects[_Index[i]], _Objects[_Index[i - 1]]) < 0)
                    {
                        b = true;
                        swap = _Index[i];
                        _Index[i] = _Index[i - 1];
                        _Index[i - 1] = swap;
                    }
                    else if (_SortOnAddToSortedArray)
                    {
                        b = false;
                        i = 0;
                    }
                    i--;
                }
            }
        }

        #endregion

        /* */

    }

    /* */

}
