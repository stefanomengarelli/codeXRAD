/*  ------------------------------------------------------------------------
 *  
 *  File:       UniqueId.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD static basic class: unique id.
 *  
 *  ------------------------------------------------------------------------
 */

namespace codeXRAD
{

    /* */

    public static partial class CX
    {

        /* */

        #region Declarations

        /*  --------------------------------------------------------------------
         *  Declarations
         *  --------------------------------------------------------------------
         */

        #endregion

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Initialize 12 chars unique id static class environment.</summary>
        static public void InitializeUniqueId()
        {
            int i;
            UniqueIdBaseYear = 2010;
            LastUniqueId = new string[8];
            LastUniqueIdIndex = 0;
            for (i = 0; i < LastUniqueId.Length; i++) LastUniqueId[i] = "";
            UniqueIdLength = 12;
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Last unique id generated.</summary>
        private static string[] LastUniqueId { get; set; } = null;

        /// <summary>Last unique id generated array index.</summary>
        private static int LastUniqueIdIndex { get; set; } = 0;

        /// <summary>Get unique id base year.</summary>
        public static int UniqueIdBaseYear { get; private set; } = 2010;

        /// <summary>Get unique id length.</summary>
        public static int UniqueIdLength { get; private set; } = 12;

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Returns date-time represented by unique id.</summary>
        static public DateTime UniqueIdDate(string _String)
        {
            DateTime r = DateTime.MinValue;
            if (ValidateUniqueId(_String))
            {
                r = new DateTime((int)BaseToInt(_String.Substring(0, 2), Base32) + UniqueIdBaseYear, 1, 1, 0, 0, 0, 0);
                r = r.AddTicks(BaseToInt(_String.Substring(2, 7), Base32) * TimeSpan.TicksPerMillisecond);
            }
            return r;
        }

        /// <summary>Returns a string representing a new high probability of unicity time related 12 chars id.</summary>
        static public string UniqueId()
        {
            int q;
            bool v;
            long n;
            string a, b;
            DateTime d = DateTime.Now, tout = d.AddTicks(TimeSpan.TicksPerMinute * 2), yst = new DateTime(d.Year, 1, 1);
            DoEvents();
            //
            //  validate current datetime
            //
            if ((d.Year < UniqueIdBaseYear) || (d.Year > UniqueIdBaseYear + 240))
            {
                Raise("System time not properly setted. Cannot generate a unique id.", true);
                return "";
            }
            else
            {
                //
                // year base calc
                //
                a = "0" + IntToBase(d.Year - UniqueIdBaseYear, Base32);
                a = a.Substring(a.Length - 2, 2);
                //
                // calc milliseconds from first of the year
                //
                q = Base32.Length - 1;
                n = (d.Ticks - yst.Ticks) / TimeSpan.TicksPerMillisecond;
                b = a + Right("0000000" + IntToBase(n, Base32), 7)
                    + Base32[Rnd(q)]
                    + Base32[Rnd(q)]
                    + Base32[Rnd(q)];
                //
                // if calculated id is next to prior recalc adding 1 milliseconds
                //
                v = NewUniqueId(b);
                while (!v && (DateTime.Now < tout))
                {
                    n++;
                    b = a + Right("0000000" + IntToBase(n, Base32), 7)
                        + Base32[Rnd(q)]
                        + Base32[Rnd(q)]
                        + Base32[Rnd(q)];
                    v = NewUniqueId(b);
                    DoEvents();
                }
                //
                // if calculated id is valid return value
                //
                if (v) return b;
                else return "";
            }
        }

        /// <summary>Return true if string passed is a valid unique id.</summary>
        public static bool ValidateUniqueId(string _String)
        {
            if (_String.Length == UniqueIdLength) return IsCharSet(_String, Base32);
            else return false;
        }

        /// <summary>Return true if unique id passed are not already present in last unique id array.</summary>
        private static bool NewUniqueId(string _String)
        {
            int i = LastUniqueId.Length;
            bool r = true;
            while (r && (i > 0)) r = _String.CompareTo(LastUniqueId[--i]) > 0;
            if (r)
            {
                LastUniqueId[LastUniqueIdIndex] = _String;
                LastUniqueIdIndex = (LastUniqueIdIndex + 1) % LastUniqueId.Length;
            }
            return r;
        }

        #endregion

        /* */

    }

    /* */

}
