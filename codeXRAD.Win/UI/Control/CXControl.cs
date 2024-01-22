/*  ------------------------------------------------------------------------
 *  
 *  File:       CXControl.cs
 *  Version:    1.0.1
 *  Date:       November 2023
 *  Author:     Stefano Mengarelli  
 *  E-mail:     info@stefanomengarelli.it
 *  
 *  Copyright (C) 2023 by Stefano Mengarelli - All rights reserved - Use, permission and restrictions under license.
 *
 *  codeXRAD controls management functions static class.
 *  
 *  ------------------------------------------------------------------------
 */

using System.Drawing.Imaging;

namespace codeXRAD.Win
{

    /* */

    /// <summary>codeXRAD controls management functions static class.</summary>
    public static class CXControl
    {

        /* */

        #region Declarations

        /*  --------------------------------------------------------------------
         *  Declarations
         *  --------------------------------------------------------------------
         */

        /// <summary>Popup control instance.</summary>
        private static Control popupControl = null;

        /// <summary>Popup menu instance.</summary>
        private static ContextMenuStrip popupMenu = null;

        /// <summary>Control rendering flag.</summary>
        private static bool rendering = false;

        /// <summary>Current controls theme.</summary>
        private static CXControlThemes theme;

        #endregion

        /* */

        #region Delegates

        /*  --------------------------------------------------------------------
         *  Delegates and Events
         *  --------------------------------------------------------------------
         */

        /// <summary>Event click delegate OnEventClick(object _Sender, EventArgs _EventArgs).</summary>
        public delegate void OnEventClick(object _Sender, EventArgs _EventArgs);

        #endregion

        /* */

        #region Properties

        /*  --------------------------------------------------------------------
         *  Properties
         *  --------------------------------------------------------------------
         */

        /// <summary>Get horizontal scroll bar height.</summary>
        public static int ScrollBarHeight
        {
            get { return System.Windows.Forms.SystemInformation.HorizontalScrollBarHeight; }
        }

        /// <summary>Get vertical scroll bar width.</summary>
        public static int ScrollBarWidth
        {
            get { return System.Windows.Forms.SystemInformation.VerticalScrollBarWidth; }
        }

        /// <summary>Get or set current theme.</summary>
        public static CXControlThemes Theme
        {
            get { return theme; }
            set { theme = value; }
        }

        /// <summary>Get themes id array.</summary>
        public static string[] Themes { get; private set; }

        #endregion

        /* */

        #region Initialization

        /*  --------------------------------------------------------------------
         *  Initialization
         *  --------------------------------------------------------------------
         */

        /// <summary>Initialize controls variables.</summary>
        public static void Initialize()
        {
            CXIni ini = new CXIni("");
            ini.WriteDefault = true;
            Themes = new string[] {
                @"Light",       // default theme
                @"Light",       // flat theme
                @"Dark"        // dark theme
            };
            theme = ThemeFromStr(ini.ReadString("SETUP", "THEME", Themes[0]));
            ini.Save();
        }

        #endregion

        /* */

        #region Methods

        /*  --------------------------------------------------------------------
         *  Methods
         *  --------------------------------------------------------------------
         */

        /// <summary>Align control to parent.</summary>
        public static void Align(Control _Control, CXControlAlignment _Alignment)
        {
            if ((_Control != null) && (_Alignment != CXControlAlignment.None))
            {
                if (_Control.Parent != null)
                {
                    //
                    // vertical alignment
                    //
                    if ((_Alignment == CXControlAlignment.Top)||(_Alignment==CXControlAlignment.TopLeft)
                        || (_Alignment == CXControlAlignment.TopCenter) || (_Alignment == CXControlAlignment.TopRight))
                    {
                        _Control.Top = _Control.Margin.Top;
                    }
                    else if ((_Alignment == CXControlAlignment.Mid) || (_Alignment == CXControlAlignment.MidLeft)
                        || (_Alignment == CXControlAlignment.MidCenter) || (_Alignment == CXControlAlignment.MidRight))
                    {
                        _Control.Top = (_Control.Parent.ClientSize.Height - _Control.Height) / 2;
                    }
                    else if ((_Alignment == CXControlAlignment.Bottom) || (_Alignment == CXControlAlignment.BottomLeft)
                        || (_Alignment == CXControlAlignment.BottomCenter) || (_Alignment == CXControlAlignment.BottomRight))
                    {
                        _Control.Top = _Control.Parent.ClientSize.Height - _Control.Height - _Control.Margin.Bottom;
                    }
                    //
                    // horizontal alignment
                    //
                    if ((_Alignment == CXControlAlignment.Left)||(_Alignment == CXControlAlignment.TopLeft)
                        || (_Alignment == CXControlAlignment.MidLeft) || (_Alignment == CXControlAlignment.BottomLeft))
                    {
                        _Control.Left = _Control.Margin.Left;
                    }
                    else if ((_Alignment == CXControlAlignment.Center) || (_Alignment == CXControlAlignment.TopCenter)
                        || (_Alignment == CXControlAlignment.MidCenter) || (_Alignment == CXControlAlignment.BottomCenter))
                    {
                        _Control.Left = (_Control.Parent.ClientSize.Width - _Control.Width) / 2;
                    }
                    else if ((_Alignment == CXControlAlignment.Right) || (_Alignment == CXControlAlignment.TopRight)
                        || (_Alignment == CXControlAlignment.MidRight) || (_Alignment == CXControlAlignment.BottomRight))
                    {
                        _Control.Left = _Control.Parent.ClientSize.Width - _Control.Width - _Control.Margin.Right;
                    }
                }
            }
        }

        /// <summary>Align controls to parent.</summary>
        public static void Align(Control[] _Controls, CXControlAlignment _Alignment)
        {
            int i, q, x;
            if ((_Controls != null) && (_Alignment != CXControlAlignment.None))
            {
                if (_Alignment == CXControlAlignment.HorizontalArrangement)
                {
                    q = 0;
                    for (i = 0; i < _Controls.Length; i++) q += _Controls[i].Width;
                    x = (_Controls[0].Parent.Width - q) / (_Controls.Length + 1);
                    q = x;
                    for (i = 0; i < _Controls.Length; i++)
                    {
                        _Controls[i].Left = q;
                        q += _Controls[i].Width + x;
                    }
                }
                else if (_Alignment == CXControlAlignment.VerticalArrangement)
                {
                    q = 0;
                    for (i = 0; i < _Controls.Length; i++) q += _Controls[i].Height;
                    x = (_Controls[0].Parent.Height - q) / (_Controls.Length + 1);
                    q = x;
                    for (i = 0; i < _Controls.Length; i++)
                    {
                        _Controls[i].Top = q;
                        q += _Controls[i].Height + x;
                    }
                }
                else if (_Alignment == CXControlAlignment.CenterHorizontally)
                {
                    q = 0;
                    for (i = 0; i < _Controls.Length; i++) q += _Controls[i].Width;
                    q = (_Controls[0].Parent.Width - q) / 2;
                    for (i = 0; i < _Controls.Length; i++)
                    {
                        _Controls[i].Left = q;
                        q += _Controls[i].Width;
                    }
                }
                else if (_Alignment == CXControlAlignment.CenterVertically)
                {
                    q = 0;
                    for (i = 0; i < _Controls.Length; i++) q += _Controls[i].Height;
                    q = (_Controls[0].Parent.Height - q) / 2;
                    for (i = 0; i < _Controls.Length; i++)
                    {
                        _Controls[i].Top = q;
                        q += _Controls[i].Height;
                    }
                }
                else for (i = 0; i < _Controls.Length; i++) Align(_Controls[i], _Alignment);
            }
        }

        /// <summary>Returns control if named name or find it recursively on childs.</summary>
        public static Control Find(Control _Control, string _Name)
        {
            Control r = null;
            if (_Name != _Control.Name)
            {
                int i = 0;
                while ((r == null) && (i < _Control.Controls.Count))
                {
                    r = Find(_Control.Controls[i], _Name);
                    i++;
                }
            }
            else r = _Control;
            return r;
        }

        /// <summary>Return a control hard-copy image and send it to printer if parameters is setted.
        /// If control is null perform current active form hard-copy.</summary>
        public static Image HardCopy(Control _Control, bool _SendToPrinter)
        {
            string f;
            Graphics g;
            Image r = null;
            if (_Control == null)
            {
                _Control = Form.ActiveForm;
                if (_Control != null)
                {
                    ((Form)_Control).Refresh();
                    ((Form)_Control).BringToFront();
                }
            }
            if (_Control != null)
            {
                g = _Control.CreateGraphics();
                r = new Bitmap(_Control.ClientRectangle.Width, _Control.ClientRectangle.Height, g);
                g.Dispose();
                g = Graphics.FromImage(r);
                g.CopyFromScreen(_Control.PointToScreen(new Point(_Control.ClientRectangle.Left, _Control.ClientRectangle.Top)),
                    new Point(0, 0), new Size(_Control.ClientRectangle.Width, _Control.ClientRectangle.Height));
                g.Dispose();
                if (_SendToPrinter)
                {
                    f = CX.Merge(CX.TempPath,"~controlhardcopy.png");
                    CX.FileDelete(f);
                    r.Save(f, ImageFormat.Png);
                    CX.RunShellVerb(f, "print", false, false);
                }
            }
            return r;
        }

        /// <summary>Load controls values on a ini file.
        /// (** Warning! Method source code must be updated if new controls implemented.)</summary>
        public static bool LoadValues(Control[] _Controls, string _IniFileName, string _Password)
        {
            int i;
            bool r = false;
            const string sz = "CONTROLS_VALUES";
            Control c;
            CXIni ini;
            try
            {
                if (_Controls != null)
                {
                    ini = new CXIni(_IniFileName, _Password);
                    i = 0;
                    while (i < _Controls.Length)
                    {
                        c = _Controls[i];
                        if (c is Form) ((Form)c).Text = ini.ReadString(sz, c.Name.ToUpper(), ((Form)c).Text);
                        else if (c is CheckBox) ((CheckBox)c).Checked = ini.ReadBool(sz, c.Name.ToUpper(), ((CheckBox)c).Checked);
                        else if (c is TextBox) ((TextBox)c).Text = ini.ReadString(sz, c.Name.ToUpper(), ((TextBox)c).Text);
                        else if (c is Button) ((Button)c).Text = ini.ReadString(sz, c.Name.ToUpper(), ((Button)c).Text);
                        else if (c is RadioButton) ((RadioButton)c).Checked = ini.ReadBool(sz, c.Name.ToUpper(), ((RadioButton)c).Checked);
                        else if (c is NumericUpDown) ((NumericUpDown)c).Value = Convert.ToDecimal(ini.ReadDouble(sz, c.Name.ToUpper(), Convert.ToDouble(((NumericUpDown)c).Value)));
                        //else if (c is XUIButton) ((XUIButton)c).Text = ini.ReadString(sz, c.Name.ToUpper(), ((XUIButton)c).Text);
                        //else if (c is XUICheckBox) ((XUICheckBox)c).Checked = ini.ReadBool(sz, c.Name.ToUpper(), ((XUICheckBox)c).Checked);
                        //else if (c is XUINumericUpDown) ((XUINumericUpDown)c).Value = Convert.ToDecimal(ini.ReadDouble(sz, c.Name.ToUpper(), Convert.ToDouble(((XUINumericUpDown)c).Value)));
                        //else if (c is XUIRadioButton) ((XUIRadioButton)c).Checked = ini.ReadBool(sz, c.Name.ToUpper(), ((XUIRadioButton)c).Checked);
                        //else if (c is XUIRating) ((XUIRating)c).Value = ini.ReadInteger(sz, c.Name.ToUpper(), ((XUIRating)c).Value);
                        //else if (c is XUISlider) ((XUISlider)c).Value = ini.ReadInteger(sz, c.Name.ToUpper(), ((XUISlider)c).Value);
                        //else if (c is XUITextBox) ((XUITextBox)c).Text = ini.ReadString(sz, c.Name.ToUpper(), ((XUITextBox)c).Text);
                        //else if (c is XUITrackBar) ((XUITrackBar)c).Value = ini.ReadInteger(sz, c.Name.ToUpper(), ((XUITrackBar)c).Value);
                        i++;
                    }
                    r = true;
                }
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                r = false;
            }
            return r;
        }

        /// <summary>Return menu source control for item.</summary>
        public static Control MenuSourceControl(ToolStripMenuItem _MenuItem)
        {
            ContextMenuStrip strip;
            if (_MenuItem != null)
            {
                strip = (ContextMenuStrip)_MenuItem.Owner;
                if (strip != null) return strip.SourceControl;
                else return null;
            }
            else return null;
        }

        /// <summary>Return text of sender as ToolStripItem.</summary>
        static public string MenuStripText(object _Sender)
        {
            if (_Sender is ToolStripItem) return ((ToolStripItem)_Sender).Text;
            else return "";
        }

        /// <summary>Return tag of sender as ToolStripItem.</summary>
        static public object MenuStripTag(object _Sender)
        {
            if (_Sender is ToolStripItem) return ((ToolStripItem)_Sender).Tag;
            else return null;
        }

        /// <summary>Return tag as string of sender as ToolStripItem.</summary>
        static public string MenuStripTagStr(object _Sender)
        {
            if (_Sender is ToolStripItem)
            {
                if (((ToolStripItem)_Sender).Tag == null) return "";
                else return ((ToolStripItem)_Sender).Tag.ToString();
            }
            else return "";
        }

        /// <summary>Remove context menu strip double separators.</summary>
        public static void MenuRemoveDoubleSeparators(ContextMenuStrip _MenuStrip)
        {
            bool b = true;
            int i = 0;
            while (i < _MenuStrip.Items.Count)
            {
                if (_MenuStrip.Items[i].Visible)
                {
                    if (_MenuStrip.Items[i] is ToolStripSeparator)
                    {
                        if (b) _MenuStrip.Items[i].Visible = false;
                        else
                        {
                            _MenuStrip.Items[i].Visible = true;
                            b = true;
                        }
                    }
                    else b = false;
                }
                i++;
            }
        }

        /// <summary>Mostra il popup di selezione delle anomalie per il controllo.</summary>
        public static void Popup(Control _Control, string[] _Items)
        {
            int i = 0;
            ToolStripItem m;
            if (_Control != null)
            {
                popupControl = _Control;
                if (_Items == null)
                {
                    if (_Control.ContextMenuStrip != null) _Control.ContextMenuStrip.Show(_Control, new Point(0, _Control.Height));
                }
                else
                {
                    popupMenu = new ContextMenuStrip();
                    popupMenu.RenderMode = ToolStripRenderMode.System;
                    popupMenu.BackColor = Color.White;
                    popupMenu.ShowImageMargin = false;
                    popupMenu.ShowCheckMargin = false;
                    while (i < _Items.Length)
                    {
                        m = popupMenu.Items.Add(CX.Before(_Items[i] + '|', "|"));
                        m.Click += PopupClick;
                        m.Tag = CX.After(_Items[i], "|");
                        if (m.Tag.ToString().Trim().Length < 1) m.Tag = m.Text;
                        i++;
                    }
                    popupMenu.Show(_Control, new Point(0, _Control.Height));
                }
            }
        }

        /// <summary>Select item from popup.</summary>
        public static void PopupClick(object _Sender, EventArgs _EventArgs)
        {
            if (popupControl!=null) popupControl.Text = ((ToolStripMenuItem)_Sender).Tag.ToString();
            popupMenu = null;
            popupControl = null;
        }

        /// <summary>Perform render methods for control childs on graphics or create graphics if null.
        /// (** Warning! Method source code must be updated if new controls implemented.)</summary>
        public static void Render(Control _Control, Graphics _Graphics)
        {
            int i;
            if (!rendering)
            {
                rendering = true;
                if (_Graphics == null) _Graphics = _Control.CreateGraphics();
                for (i = 0; i < _Control.Controls.Count; i++)
                {
                    if (_Control.Controls[i].Visible)
                    {
                        //if (_Control.Controls[i] is XUITextBox) ((XUITextBox)_Control.Controls[i]).CustomRender(_Graphics);
                        //else if (_Control.Controls[i] is XUINumericUpDown) ((XUINumericUpDown)_Control.Controls[i]).CustomRender(_Graphics);
                        //else if (_Control.Controls[i] is XUIRichTextBox) ((XUIRichTextBox)_Control.Controls[i]).CustomRender(_Graphics);
                    }
                }
                rendering = false;
            }
        }

        /// <summary>Save controls values on a ini file.
        /// (** Warning! Method source code must be updated if new controls implemented.)</summary>
        public static bool SaveValues(Control[] _Controls, string _IniFileName, string _Password)
        {
            int i;
            bool r = false;
            const string sz = "CONTROLS_VALUES";
            Control c;
            CXIni ini;
            try
            {
                if (_Controls != null)
                {
                    ini = new CXIni(_IniFileName, _Password);
                    i = 0;
                    while (i < _Controls.Length)
                    {
                        c = _Controls[i];
                        if (c is Form) ini.WriteString(sz, c.Name.ToUpper(), ((Form)c).Text);
                        else if (c is CheckBox) ini.WriteBool(sz, c.Name.ToUpper(), ((CheckBox)c).Checked);
                        else if (c is TextBox) ini.WriteString(sz, c.Name.ToUpper(), ((TextBox)c).Text);
                        else if (c is Button) ini.WriteString(sz, c.Name.ToUpper(), ((Button)c).Text);
                        else if (c is RadioButton) ini.WriteBool(sz, c.Name.ToUpper(), ((RadioButton)c).Checked);
                        else if (c is NumericUpDown) ini.WriteDouble(sz, c.Name.ToUpper(), Convert.ToDouble(((NumericUpDown)c).Value));
                        //else if (c is XUIButton) ini.WriteString(sz, c.Name.ToUpper(), ((XUIButton)c).Text);
                        //else if (c is XUICheckBox) ini.WriteBool(sz, c.Name.ToUpper(), ((XUICheckBox)c).Checked);
                        //else if (c is XUINumericUpDown) ini.WriteDouble(sz, c.Name.ToUpper(), Convert.ToDouble(((XUINumericUpDown)c).Value));
                        //else if (c is XUIRadioButton) ini.WriteBool(sz, c.Name.ToUpper(), ((XUIRadioButton)c).Checked);
                        //else if (c is XUIRating) ini.WriteInteger(sz, c.Name.ToUpper(), ((XUIRating)c).Value);
                        //else if (c is XUISlider) ini.WriteInteger(sz, c.Name.ToUpper(), ((XUISlider)c).Value);
                        //else if (c is XUITextBox) ini.WriteString(sz, c.Name.ToUpper(), ((XUITextBox)c).Text);
                        //else if (c is XUITrackBar) ini.WriteInteger(sz, c.Name.ToUpper(), ((XUITrackBar)c).Value);
                        i++;
                    }
                    r = ini.Save();
                }
            }
            catch (Exception ex)
            {
                CX.Error(ex);
                r = false;
            }
            return r;
        }

        /// <summary>Return theme from string name.</summary>
        public static CXControlThemes ThemeFromStr(string _Theme)
        {
            int i = 1;
            CXControlThemes r = CXControlThemes.Default;
            _Theme = _Theme.Trim().ToUpper();
            while ((i < Themes.Length) && (r == CXControlThemes.Default))
            {
                if (_Theme == Themes[i].Trim().ToUpper()) r = (CXControlThemes)i;
                i++;
            }
            return r;
        }

        /// <summary>Return string with name of theme.</summary>
        public static string ThemeToStr(CXControlThemes _Theme)
        {
            int i = (int)_Theme;
            if ((i > -1) && (i < Themes.Length)) return Themes[i];
            else return "";
        }

        /// <summary>Change theme settings and restart application if specified.</summary>
        public static void ThemeSet(CXControlThemes _Theme, bool _Restart)
        {
            CXIni ini = new CXIni("");
            theme = _Theme;
            ini.WriteString("SETUP", "THEME", ThemeToStr(theme));
            ini.Save();
            if (_Restart)
            {
                CX.ForceExit = true;
                CX.RunShell(Application.ExecutablePath, "", false, false);
                Application.Exit();
            }
        }

        #endregion

        /* */

    }

    /* */

}
