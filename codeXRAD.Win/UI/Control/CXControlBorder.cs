/*  ------------------------------------------------------------------------
 *  
 *  File:       CXControlBorder.cs
 *  Version:    1.0.1
 *  Date:       October 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD UI control border class.
 *
 *  ------------------------------------------------------------------------
 */

using System.ComponentModel;

namespace codeXRAD.Win
{

    /* */

    /// <summary>codeXRAD UI control box class.</summary>
    [TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public class CXControlBorder
    {

        /* */

        #region Declarations

        /*  --------------------------------------------------------------------
         *  Declarations
         *  --------------------------------------------------------------------
         */

        /// <summary>Control bottom border color.</summary>
        private Color colorBottom = Color.FromArgb(40, 0, 0, 0);

        /// <summary>Control left border color.</summary>
        private Color colorLeft = Color.FromArgb(40, 0, 0, 0);

        /// <summary>Control right border color.</summary>
        private Color colorRight = Color.FromArgb(40, 0, 0, 0);

        /// <summary>Control top border color.</summary>
        private Color colorTop = Color.FromArgb(40, 0, 0, 0);

        /// <summary>Rounded border bottom left flag.</summary>
        private bool roundedBottomLeft = false;

        /// <summary>Rounded border bottom right flag.</summary>
        private bool roundedBottomRight = false;

        /// <summary>Rounded border top left flag.</summary>
        private bool roundedTopLeft = false;

        /// <summary>Rounded border top right flag.</summary>
        private bool roundedTopRight = false;

        /// <summary>Rounded border radius.</summary>
        private int roundedRadius = 0;

        /// <summary>Control border widths.</summary>
        private Padding widths = new Padding(0,0,0,0);

        #endregion

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Class constructor.</summary>
        public CXControlBorder()
        {

        }

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get or set control bottom border color.</summary>
        [Browsable(true)]
        [Description("Get or set control bottom border color.")]
        public Color ColorBottom
        {
            get { return colorBottom; }
            set { colorBottom = value; }
        }

        /// <summary>Get or set control left border color.</summary>
        [Browsable(true)]
        [Description("Get or set control left border color.")]
        public Color ColorLeft
        {
            get { return colorLeft; }
            set { colorLeft = value; }
        }

        /// <summary>Get or set control right border color.</summary>
        [Browsable(true)]
        [Description("Get or set control right border color.")]
        public Color ColorRight
        {
            get { return colorRight; }
            set { colorRight = value; }
        }

        /// <summary>Get or set control top border color.</summary>
        [Browsable(true)]
        [Description("Get or set control top border color.")]
        public Color ColorTop
        {
            get { return colorTop; }
            set { colorTop = value; }
        }

        /// <summary>Indicate if bottom left corner has to be rounded.</summary>
        [Browsable(true)]
        [Description("Indicate if bottom left corner has to be rounded.")]
        public bool RoundedBottomLeft
        {
            get { return roundedBottomLeft; }
            set { roundedBottomLeft = value; }
        }

        /// <summary>Indicate if bottom right corner has to be rounded.</summary>
        [Browsable(true)]
        [Description("Indicate if bottom right corner has to be rounded.")]
        public bool RoundedBottomRight
        {
            get { return roundedBottomRight; }
            set { roundedBottomRight = value; }
        }

        /// <summary>Indicate if top left corner has to be rounded.</summary>
        [Browsable(true)]
        [Description("Indicate if top left corner has to be rounded.")]
        public bool RoundedTopLeft
        {
            get { return roundedTopLeft; }
            set { roundedTopLeft = value; }
        }

        /// <summary>Indicate if top right corner has to be rounded.</summary>
        [Browsable(true)]
        [Description("Indicate if top right corner has to be rounded.")]
        public bool RoundedTopRight
        {
            get { return roundedTopRight; }
            set { roundedTopRight = value; }
        }

        /// <summary>Read or set rounded corner radius.</summary>
        [Browsable(true)]
        [Description("Read or set rounded corner radius.")]
        public int RoundedRadius
        {
            get { return roundedRadius; }
            set { roundedRadius = value; }
        }

        /// <summary>Get or set control border widths.</summary>
        [Browsable(true)]
        [Description("Get or set control border widths.")]
        public Padding Widths
        {
            get { return widths; }
            set { widths = value; }
        }

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Draw control border.</summary>
        public void Draw(Control _Control, Graphics _Graphics, bool _RoundedTopLeft,
            bool _RoundedTopRight, bool _RoundedBottomLeft, bool _RoundedBottomRight)
        {
            int b = _Control.Height - 1, i, r = _Control.Width - 1, p1, p2;
            RectangleF q;
            Pen pn; 
            if (roundedRadius > 0)
            {
                //
                // top border
                //
                if (widths.Top > 0)
                {
                    pn = new Pen(colorTop);
                    if (_RoundedTopLeft) p1 = roundedRadius;
                    else p1 = 0;
                    if (_RoundedTopRight) p2 = r - roundedRadius;
                    else p2 = r;
                    i = widths.Top;
                    while (i > 0)
                    {
                        i--;
                        _Graphics.DrawLine(pn, p1, i, p2, i);
                    }
                    pn.Dispose();
                }
                //
                // bottom border
                //
                if (widths.Bottom > 0)
                {
                    pn = new Pen(colorBottom);
                    if (_RoundedBottomLeft) p1 = roundedRadius;
                    else p1 = 0;
                    if (_RoundedBottomRight) p2 = r - roundedRadius;
                    else p2 = r;
                    i = widths.Bottom;
                    while (i > 0)
                    {
                        i--;
                        _Graphics.DrawLine(pn, p1, b - i - 0.5f, p2, b - i - 0.5f);
                    }
                    pn.Dispose();
                }
                //
                // left border
                //
                if (widths.Left>0)
                {
                    pn = new Pen(colorLeft);
                    if (_RoundedTopLeft) p1 = widths.Top + roundedRadius;
                    else p1 = widths.Top;
                    if (_RoundedBottomLeft) p2 = b - widths.Bottom - roundedRadius;
                    else p2 = b - widths.Bottom;
                    i = widths.Left;
                    while (i > 0)
                    {
                        i--;
                        _Graphics.DrawLine(pn, i, p1, i, p2);
                    }
                    pn.Dispose();
                }
                //
                // right border
                //
                if (widths.Right > 0)
                {
                    pn = new Pen(colorRight);
                    if (_RoundedTopRight) p1 = widths.Top + roundedRadius;
                    else p1 = widths.Top;
                    if (_RoundedBottomRight) p2 = b - widths.Bottom - roundedRadius;
                    else p2 = b - widths.Bottom;
                    i = widths.Right;
                    while (i > 0)
                    {
                        i--;
                        _Graphics.DrawLine(pn, r - i - 0.5f, p1, r - i - 0.5f, p2);
                    }
                    pn.Dispose();
                }
                //
                // round arc rectangle
                //
                q = new RectangleF(0, 0, 2 * roundedRadius, 2 * roundedRadius);
                //
                // round arc top left
                //
                if (_RoundedTopLeft && (widths.Top>0))
                {
                    pn = new Pen(colorTop);
                    i = widths.Top;
                    while (i > 0)
                    {
                        i--;
                        q.X = i + 0.5f;
                        q.Y = i + 0.5f;
                        _Graphics.DrawArc(pn, q, 180, 90);
                    }
                    pn.Dispose();
                }
                //
                // round arc top right
                //
                if (_RoundedTopRight && (widths.Top > 0))
                {
                    pn = new Pen(colorTop);
                    i = widths.Top;
                    while (i > 0)
                    {
                        i--;
                        q.X = r - q.Width - i - 0.5f;
                        q.Y = i + 0.5f;
                        _Graphics.DrawArc(pn, q, 270, 90);
                    }
                    pn.Dispose();
                }
                //
                // round arc bottom left
                //
                if (_RoundedBottomLeft && (widths.Bottom > 0))
                {
                    pn = new Pen(colorBottom);
                    i = widths.Bottom;
                    while (i > 0)
                    {
                        i--;
                        q.X = i + 0.5f;
                        q.Y = b - q.Height - i - 0.5f;
                        _Graphics.DrawArc(pn, q, 90, 90);
                    }
                    pn.Dispose();
                }
                //
                // round arc bottom right
                //
                if (_RoundedBottomRight && (widths.Bottom > 0))
                {
                    pn = new Pen(colorBottom);
                    i = widths.Bottom;
                    while (i > 0)
                    {
                        i--;
                        q.X = r - q.Width - i - 0.5f;
                        q.Y = b - q.Height - i - 0.5f;
                        _Graphics.DrawArc(pn, q, 0, 90);
                    }
                    pn.Dispose();
                }
            }
            else
            {
                //
                // top border
                //
                if (widths.Top > 0)
                {
                    pn = new Pen(colorTop);
                    i = widths.Top;
                    while (i > 0)
                    {
                        i--;
                        _Graphics.DrawLine(pn, 0, i, r, i);
                    }
                    pn.Dispose();
                }
                //
                // bottom border
                //
                if (widths.Bottom > 0)
                {
                    pn = new Pen(colorBottom);
                    i = widths.Bottom;
                    while (i > 0)
                    {
                        i--;
                        _Graphics.DrawLine(pn, 0, b - i, r, b - i);
                    }
                    pn.Dispose();
                }
                //
                // left border
                //
                if (widths.Left > 0)
                {
                    pn = new Pen(colorLeft);
                    i = widths.Left;
                    while (i > 0)
                    {
                        i--;
                        _Graphics.DrawLine(pn, i, widths.Top, i, b - widths.Bottom);
                    }
                    pn.Dispose();
                }
                //
                // right border
                //
                if (widths.Right > 0)
                {
                    pn = new Pen(colorRight);
                    i = widths.Right;
                    while (i > 0)
                    {
                        i--;
                        _Graphics.DrawLine(pn, r - i, widths.Top, r - i, b - widths.Bottom);
                    }
                    pn.Dispose();
                }
            }
        }

        #endregion

        /* */

    }

    /* */

}
