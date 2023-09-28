/*  ------------------------------------------------------------------------
 *  
 *  File:       CXSimulatedAnnealing.cs
 *  Version:    1.0.0
 *  Date:       September 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD simulated annealing component.
 *  
 *  ------------------------------------------------------------------------
 */

using System.ComponentModel;

namespace codeXRAD
{

    /* */

    /// <summary>codeXRAD simulated annealing component.</summary>
    [ToolboxItem(true)]
    public partial class CXSimulatedAnnealing : Component
    {

        /* */

        #region Delegates

        /*  --------------------------------------------------------------------
         *  Delegates
         *  --------------------------------------------------------------------
         */

        /// <summary>Occurs when solution cost has to be evaluated.</summary>
        public delegate void OnEvaluateSolution(object _Sender, int[] _Solution, ref double _Cost);
        /// <summary>Occurs when solution cost has to be evaluated.</summary>
        public event OnEvaluateSolution EvaluateSolution = null;

        /// <summary>Occurs when new current solution has changed.</summary>
        public delegate void OnNewSolution(object _Sender, int[] _Solution);
        /// <summary>Occurs when new current solution has changed.</summary>
        public event OnNewSolution NewSolution = null;

        /// <summary>Occurs when random new solution must be validated and calculated.</summary>
        public delegate void OnValidateSolution(object _Sender, int[] _Solution, ref bool _Validate);
        /// <summary>Occurs when random new solution must be validated and calculated.</summary>
        public event OnValidateSolution ValidateSolution = null;

        #endregion

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Instance constructor.</summary>
        public CXSimulatedAnnealing()
        {
            InitializeComponent();
            InitializeInstance();
            Clear();
        }

        /// <summary>Instance constructor with container</summary>
        public CXSimulatedAnnealing(IContainer _Container)
        {
            _Container.Add(this);
            InitializeComponent();
            InitializeInstance();
            Clear();
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get or set solution search around size.</summary>
        [Browsable(true)]
        [Description("Get or set solution search around size.")]
        public int Around { get; set; }

        /// <summary>Get or set best found solution indexes array.</summary>
        [Browsable(false)]
        [Description("Get or set best found solution indexes array.")]
        public int[] Best { get; set; }

        /// <summary>Get or set solution minimize flag (if false solution will be maximized).</summary>
        [Browsable(true)]
        [Description("Get or set solution minimize flag (if false solution will be maximized).")]
        public bool Minimize { get; set; }

        /// <summary>Get or set discarded solutions counter.</summary>
        [Browsable(false)]
        [Description("Get or set discarded solutions counter.")]
        public int Cooling { get; set; }

        /// <summary>Get or set application process events interval.</summary>
        [Browsable(true)]
        [Description("Get or set application process events interval.")]
        public double EventsInterval { get; set; }

        /// <summary>Get or set how many discarded solutions need to stop algorithm or 0 for infinite (continuos).</summary>
        [Browsable(true)]
        [Description("Get or set how many discarded solutions need to stop algorithm or 0 for infinite (continuos).")]
        public int Freezing { get; set; }

        /// <summary>Get original object collection.</summary>
        [Browsable(true)]
        [Description("Get original object collection.")]
        public List<object> Items { get; set; }

        /// <summary>Get or set algorithm iterations counter.</summary>
        [Browsable(false)]
        [Description("Get or set algorithm iterations counter.")]
        public int Iterations { get; set; }

        /// <summary>Get or set probability to assign pejorative solutions.</summary>
        [Browsable(true)]
        [Description("Get or set probability to assign pejorative solutions.")]
        public int Probability { get; set; }

        /// <summary>Get or set current solution indexes array.</summary>
        [Browsable(false)]
        [Description("Get or set current solution indexes array.")]
        public int[] Solution { get; set; }

        /// <summary>Get or set current solution cost value.</summary>
        [Browsable(false)]
        [Description("Get or set current solution cost value.")]
        public double Cost { get; set; }

        /// <summary>Get or set solving state flag.</summary>
        [Browsable(false)]
        [Description("Get or set solving state flag.")]
        public bool Solving { get; set; }

        /// <summary>Get or set timeout in seconds (0 = disabled timeout).</summary>
        [Browsable(true)]
        [Description("Get or set timeout in seconds (0 = disabled timeout).")]
        public long TimeOut { get; set; }

        /// <summary>Get or set current solution trial indexes array.</summary>
        [Browsable(false)]
        [Description("Get or set current solution trial indexes array.")]
        public int[] Trial { get; set; }

        /// <summary>Get or set best found cost value.</summary>
        [Browsable(false)]
        [Description("Get or set best found cost value.")]
        public double Value { get; set; }

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Initialize and reset properties variables.</summary>
        public void Clear()
        {
            Items.Clear();
            Solution = null;
            Trial = null;
        }

        /// <summary>Initialize instance properties.</summary>
        public void InitializeInstance()
        {
            CX.Initialize();
            Around = 1;
            Best = null;
            Minimize = true;
            Cooling = 0;
            EventsInterval = 0.1d;
            Freezing = 250;
            Items = new List<object>();
            Iterations = 0;
            Probability = 40;
            Solution = null;
            Cost = 0.0d;
            Solving = false;
            TimeOut = 0;
            Trial = null;
            Value = 0.0d;
        }

        /// <summary>Initialize and reset properties variables.</summary>
        public void Solve()
        {
            int i, j, h = Items.Count, a, b, p = Probability;
            bool validate, changed;
            double cost;
            Random rnd = new Random(DateTime.Now.Day * 1000 + DateTime.Now.Month * 10 + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
            DateTime start, events;
            long tout = TimeOut * TimeSpan.TicksPerSecond;

            // test if already solving
            if (!Solving)
            {
                // initialize solving
                Solving = true;
                if (Around < 1) Around = 1;
                Best = null;
                Cooling = 0;
                Cost = 0.0d;
                Iterations = 0;
                Solution = null;
                Trial = null;
                Value = 0.0d;

                // test if items exists
                if (h > 0)
                {
                    // create solution array with loaded solution
                    start = DateTime.Now;
                    events = DateTime.Now.AddSeconds(EventsInterval);
                    Best = new int[h];
                    Solution = new int[h];
                    Trial = new int[h];
                    for (i = 0; i < h; i++)
                    {
                        Best[i] = i;
                        Solution[i] = i;
                        Trial[i] = i;
                    }

                    // evaluate starting solution
                    cost = 0.0d;
                    EvaluateSolution?.Invoke(this, Best, ref cost);
                    Cost = cost;
                    Value = cost;

                    // if items are more than one start solving
                    if (h > 1)
                    {
                        // solving loop
                        while (Solving && ((Freezing < 1) || (Cooling < Freezing))
                            && ((tout < 1) || (tout > (DateTime.Now.Ticks - start.Ticks))))
                        {
                            changed = false;

                            // modulate probability and increase iterations counter
                            if (p > 0)
                            {
                                p -= Iterations / 10;
                                if (p < 0) p = 0;
                            }
                            Iterations++;

                            // around finding loop
                            i = 0;
                            while (i < Around)
                            {

                                // initialize new solution
                                for (j = 0; j < h; j++) Trial[j] = Solution[j];

                                // find random positions to swap
                                a = rnd.Next(h);
                                b = rnd.Next(h);
                                while ((a == b) || (a >= h) || (b >= h))
                                {
                                    a = rnd.Next(h);
                                    b = rnd.Next(h);
                                }

                                // swap items corresponding to random positions
                                j = Trial[a];
                                Trial[a] = Trial[b];
                                Trial[b] = j;

                                // validate current solution
                                validate = true;
                                cost = 0.0d;
                                ValidateSolution?.Invoke(this, Trial, ref validate);
                                if (validate)
                                {
                                    // evaluate current solution
                                    EvaluateSolution?.Invoke(this, Trial, ref cost);

                                    // detect if new solution must be assigned
                                    if (Minimize) validate = cost < Cost;
                                    else validate = cost > Cost;
                                    if (!validate) validate = rnd.Next(100) < p;

                                    // assign new solution
                                    if (validate)
                                    {
                                        Cost = cost;
                                        for (j = 0; j < h; j++) Solution[j] = Trial[j];
                                        changed = true;
                                        if (Minimize) validate = Cost < Value;
                                        else validate = Cost > Value;
                                        if (validate)
                                        {
                                            Value = Cost;
                                            for (j = 0; j < h; j++) Best[j] = Solution[j];
                                            NewSolution?.Invoke(this, Best);
                                        }
                                    }
                                }

                                // next around
                                i++;

                                // process events
                                if (EventsInterval > 0.0d)
                                {
                                    if (events < DateTime.Now)
                                    {
                                        CX.DoEvents();
                                        events = DateTime.Now.AddSeconds(EventsInterval);
                                    }
                                }
                                else CX.DoEvents();
                            }

                            // update freeze counter
                            if (changed) Cooling = 0;
                            else Cooling++;

                        }
                    }
                }

                // end of solving
                Solving = false;
            }
        }

        #endregion

        /* */

    }

    /* */

}
