/*  ------------------------------------------------------------------------
 *  
 *  File:       CXControlArrange.cs
 *  Version:    1.0.1
 *  Date:       October 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD control arrange class.
 *  
 *  ------------------------------------------------------------------------
 */

using System.ComponentModel;

namespace codeXRAD.Win
{

    /* */

    /// <summary>codeXRAD control arrange class.</summary>
    [TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public class CXControlArrange
    {

        /* */

        #region Declarations

        /*  --------------------------------------------------------------------
         *  Declarations
         *  --------------------------------------------------------------------
         */

        /// <summary>Busy flag.</summary>
        private bool busy = false;

        /// <summary>Arrange margins.</summary>
        private Padding margins = new Padding();

        /// <summary>Minimum control height.</summary>
        private int minimumHeight = 4;

        /// <summary>Minimum control width.</summary>
        private int minimumWidth = 4;

        /// <summary>Control arrange mode.</summary>
        private CXControlArrangeMode mode = CXControlArrangeMode.None;

        #endregion

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Class constructor.</summary>
        public CXControlArrange()
        {
            Control = null;
        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get or set arrange control.</summary>
        [Browsable(false)]
        public Control Control { get; set; } = null;

        /// <summary>Get or set arrange margins.</summary>
        [Browsable(true)]
        [Description("Get or set arrange margins.")]
        public Padding Margins
        {
            get { return margins; }
            set
            {
                margins = value;
                if (Control != null)
                {
                    if (Control.Parent != null)
                    {
                        if (margins.Right < 0)
                        {
                            margins.Right = Control.Parent.Width - Control.Left - Control.Width;
                        }
                        if (margins.Bottom < 0)
                        {
                            margins.Bottom = Control.Parent.Height - Control.Top - Control.Height;
                        }
                    }
                }
                Arrange();
            }
        }

        /// <summary>Get or set minimum control height.</summary>
        [Browsable(true)]
        [Description("Get or set minimum control height.")]
        public int MinimumHeight
        {
            get { return minimumHeight; }
            set
            {
                minimumHeight = value;
                Arrange();
            }
        }

        /// <summary>Get or set minimum control width.</summary>
        [Browsable(true)]
        [Description("Get or set minimum control width.")]
        public int MinimumWidth
        {
            get { return minimumWidth; }
            set
            {
                minimumWidth = value;
                Arrange();
            }
        }

        /// <summary>Get or set arrange mode.</summary>
        [Browsable(true)]
        [Description("Get or set arrange mode.")]
        public CXControlArrangeMode Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                Arrange();
            }
        }

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Arrange control.</summary>
        public void Arrange()
        {
            int x, y, w, h, minw, minh, ph, pw;
            bool b = false;
            if (!busy && (Control != null) && (mode != CXControlArrangeMode.None))
            {
                busy = true;
                if (Control.Visible && (Control.Parent != null))
                {
                    ph = Control.Parent.ClientSize.Height;
                    pw = Control.Parent.ClientSize.Width;
                    if ((pw > minimumWidth) && (ph > minimumHeight))
                    {
                        x = Control.Left;
                        y = Control.Top;
                        w = Control.Width;
                        h = Control.Height;
                        //
                        minw = Control.MinimumSize.Width;
                        if (minw < minimumWidth) minw = minimumWidth;
                        //
                        minh = Control.MinimumSize.Height;
                        if (minh < minimumHeight) minh = minimumHeight;
                        //
                        if (mode == CXControlArrangeMode.Width)
                        {
                            w = pw - Control.Left - margins.Right;
                        }
                        else if (mode == CXControlArrangeMode.Height)
                        {
                            h = ph - Control.Top - margins.Bottom;
                        }
                        else if (mode == CXControlArrangeMode.Size)
                        {
                            w = pw - Control.Left - margins.Right;
                            h = ph - Control.Top - margins.Bottom;
                        }
                        else if (mode == CXControlArrangeMode.Right)
                        {
                            x = pw - Control.Width - margins.Right;
                        }
                        else if (mode == CXControlArrangeMode.CenterHorizontally)
                        {
                            x = (pw - Control.Width - margins.Left - margins.Right) / 2;
                        }
                        else if (mode == CXControlArrangeMode.CenterVertically)
                        {
                            y = (ph - Control.Height - margins.Top - margins.Bottom) / 2;
                        }
                        else if (mode == CXControlArrangeMode.Center)
                        {
                            x = (pw - Control.Width - margins.Left - margins.Right) / 2;
                            y = (ph - Control.Height - margins.Top - margins.Bottom) / 2;
                        }
                        //
                        if (w < minw) w = minw;
                        else if ((Control.MaximumSize.Width > 0) && (w > Control.MaximumSize.Width)) w = Control.MaximumSize.Width;
                        //
                        if (h < minh) h = minh;
                        else if ((Control.MaximumSize.Height > 0) && (h > Control.MaximumSize.Height)) h = Control.MaximumSize.Height;
                        //
                        if (Control.Left != x)
                        {
                            Control.Left = x;
                            b = true;
                        }
                        if (Control.Top != y)
                        {
                            Control.Top = y;
                            b = true;
                        }
                        if (Control.Width != w)
                        {
                            Control.Width = w;
                            b = true;
                        }
                        if (Control.Height != h)
                        {
                            Control.Height = h;
                            b = true;
                        }
                        if (b) Control.Invalidate();
                    }
                }
                busy = false;
            }
        }

        /// <summary>Arrange childs control.
        /// (** Warning! Method source code must be updated if new controls implemented.)</summary>
        public void ArrangeChilds()
        {
            int i;
            Control c;
            if (!busy && (Control != null))
            {
                busy = true;
                for (i = 0; i < Control.Controls.Count; i++)
                {
                    c = Control.Controls[i];
                    //if (c is XUIButton) ((XUIButton)c).Arrange.Arrange();
                    //else if (c is XUIButtons) ((XUIButtons)c).Arrange.Arrange();
                    //else if (c is XUICheckBox) ((XUICheckBox)c).Arrange.Arrange();
                    //else if (c is XUILabel) ((XUILabel)c).Arrange.Arrange();
                    //else if (c is XUINumericUpDown) ((XUINumericUpDown)c).Arrange.Arrange();
                    //else if (c is XUIPanel) ((XUIPanel)c).Arrange.Arrange();
                    //else if (c is XUIPictureBox) ((XUIPictureBox)c).Arrange.Arrange();
                    //else if (c is XUIProgressBar) ((XUIProgressBar)c).Arrange.Arrange();
                    //else if (c is XUIRadioButton) ((XUIRadioButton)c).Arrange.Arrange();
                    //else if (c is XUIRating) ((XUIRating)c).Arrange.Arrange();
                    //else if (c is XUIRichTextBox) ((XUIRichTextBox)c).Arrange.Arrange();
                    //else if (c is XUISlider) ((XUISlider)c).Arrange.Arrange();
                    //else if (c is XUITextBox) ((XUITextBox)c).Arrange.Arrange();
                    //else if (c is XUITrackBar) ((XUITrackBar)c).Arrange.Arrange();
                }
                busy = false;
            }
        }

        #endregion

        /* */

    }

    /* */

}
