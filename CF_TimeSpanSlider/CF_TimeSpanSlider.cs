// CF_TIMESPANSLIDER (c) Copyright by Christian Fenkart
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Media;
using System.Timers;
using System.Diagnostics;
using System.Globalization;

namespace CF_TimeSpanSlider
{
    public delegate void ValueChangedEventHandler(object sender, EventArgs e);
    public partial class CF_TimeSpanSlider : UserControl
    {
    #region Fields
        private static System.Timers.Timer aTimer;

        private int _Inc;
        private int _LineSize = 50;
        private int _LineWidth = 4;

        private Color _CSeperator;
        private Color _CCenter;
        private Color _CLeft;
        private Color _CRight;
        private Color _CSel1;
        private Color _CSel2;
        private Color _CSel1Highlight;
        private Color _CSel2Highlight;
        private Color _CTextCenter;
        private Color _CNow;

        private DateTime _Time1;
        private DateTime _Time2;

        private TimeSpan _TScomp;
        private TimeSpan _TSvalue;

        private DateTime _TimeStart;
        private DateTime _TimeEnd;

        private TimeSpan _TSdist;// = new TimeSpan(_TimeEnd.Ticks - _TimeStart.Ticks);

        private int _Slider1Pos = 10;
        private int _Slider2Pos = 99;

        private Point _MouseDownPos;
        private Point _MouseOffset;

        private PointF _MouseOffsetF;

        private Point _Slider1center = new Point(0,0);
        private Point _Slider2center = new Point(0,0);

        private Point[] Slider1 = null;
        private Point[] Slider2 = null;
        private Point[] Slider1big = null;
        private Point[] Slider2big = null;
        private Point[] Slider1outer = new Point[3];
        private Point[] Slider1inner = new Point[3];
        private Point[] Slider2outer = new Point[3];
        private Point[] Slider2inner = new Point[3];
        private Point[] Slider1outerBig = new Point[3];
        private Point[] Slider1innerBig = new Point[3];
        private Point[] Slider2outerBig = new Point[3];
        private Point[] Slider2innerBig = new Point[3];

        private PointF[] Marks = null;

        public bool blink = false;
        private bool nonNumberEntered = false;
        private bool _debug = false;
        private bool _Update = false;

        private TimeSpan TScomp
        {
            get { return _TScomp; }
            set{
                if (value == _TScomp)
                    return;

                _TScomp = value;

                Invalidate();
            }
        }
        private TimeSpan TSvalue
        {
            get { return _TSvalue; }
            set
            {
                if (value == _TSvalue)
                    return;

                _TSvalue = value;

                Invalidate();
            }
        }

        private TimeSpan _interval = new TimeSpan(0);

        private bool _Hovering = false;
        private bool Hovering
        {
            get { return _Hovering; }
            set
            {
                if (value == _Hovering)
                    return;

                _Hovering = value;

                Invalidate();
            }
        }

        private bool _Pressed = false;
        private bool Pressed
        {
            get { return _Pressed; }
            set
            {
                if (value == _Pressed)
                    return;

                _Pressed = value;

                Invalidate();
            }
        }

        private bool _Slider1sel = false;
        private bool Slider1sel
        {
            get { return _Slider1sel; }
            set
            {
                if (value == _Slider1sel)
                    return;

                _Slider1sel = value;

                Invalidate();
            }
        }
        private bool Slider1innerB = false;
        private bool Slider1outerB = false;

        private bool _Slider2sel = false;
        private bool Slider2sel
        {
            get { return _Slider2sel; }
            set
            {
                if (value == _Slider2sel)
                    return;

                _Slider2sel = value;

                Invalidate();
            }
        }
        private bool Slider2innerB = false;
        private bool Slider2outerB = false;

        private bool _focused = false;
        private bool focused
        {
            get { return _focused; }
            set
            {
                if (value == _focused)
                    return;

                _focused = value;

                Invalidate();
            }
        }

        private bool modSel1 = false;
        private bool modSel2 = false;

        private bool _blink_on_focus = false;
        private bool _showNow = false;

        private string Modify;

        private string STime_Sel_1;
        private string STime_Sel_2;

        private string _timeFormat = "HH:mm";
        private string debug_string;

        SizeF stringSize2LC = new SizeF();
        Point PosText2LC = new Point();

        SizeF stringSizeTextCenter = new SizeF();
        Point PosTextCenter = new Point();

        private List<TimeRange> _Highlight;
        #endregion

    #region Enum_Para
        public enum steps
        {
            none,
            Minute1,
            Minute5,
            Minute10,
            Minute15,
            Minute30,
            Hour1,
            Day,
            Week
        }
        public enum eSeperator_style
        {
            none,
            Elipse,
            Line,
            X,
            WeekNr
        }

        private steps _Step = steps.Minute5;
        [Description("Steps"),
        Category("Apperance"),
        DefaultValue(typeof(steps), "Minute10"),
        Browsable(true)]
        public steps Steps
        {
            get { return _Step; }
            set{
                if (_Step == value)
                    return;

                _Step = value;

                TSMI_1h.Checked = false;
                TSMI_30min.Checked = false;
                TSMI_15min.Checked = false;
                TSMI_10min.Checked = false;
                TSMI_5min.Checked = false;
                TSMI_1min.Checked = false;

                if (_Step == steps.none)
                    _interval = new TimeSpan(0);
                if (_Step == steps.Minute1)
                {
                    _interval = new TimeSpan(0, 1, 0);
                    TSMI_1min.Checked = true;
                }
                if (_Step == steps.Minute5)
                {
                    _interval = new TimeSpan(0, 5, 0);
                    TSMI_5min.Checked = true;
                }
                if (_Step == steps.Minute10)
                {
                    _interval = new TimeSpan(0, 10, 0);
                    TSMI_10min.Checked = true;
                }
                if (_Step == steps.Minute15)
                {
                    _interval = new TimeSpan(0, 15, 0);
                    TSMI_15min.Checked = true;
                }
                if (_Step == steps.Minute30)
                {
                    _interval = new TimeSpan(0, 30, 0);
                    TSMI_30min.Checked = true;
                }
                if (_Step == steps.Hour1)
                {
                    _interval = new TimeSpan(1, 0, 0);
                    TSMI_1h.Checked = true;
                }
                if (_Step == steps.Day)
                {
                    _interval = new TimeSpan(1, 0, 0, 0, 0);
                }
                if (_Step == steps.Week)
                {
                    _interval = new TimeSpan(7, 0, 0, 0, 0);
                }

                Invalidate();
            }
        }

        private eSeperator_style _eSeperator = eSeperator_style.Line;
        [Description("Seperator_style"),
        Category("Apperance"),
        DefaultValue(typeof(eSeperator_style), "Line"),
        Browsable(true)]
        public eSeperator_style eSeperator
        {
            get { return _eSeperator; }
            set
            {
                if (_eSeperator == value)
                    return;

                _eSeperator = value;

                Invalidate();
            }
        }
        #endregion

    #region Parameter

        [Description("Debug"),
        Category("Apperance"),
        DefaultValue(false),
        Browsable(true)]
        public bool Debug
        {
            get { return _debug; }
            set
            {
                if (value == _debug )
                    return;

                _debug = value;

                Invalidate();
            }
        }



        [Description("LineSize"),
        Category("Apperance"),
        DefaultValue(50),
        Browsable(true)]
        public int LineHight
        {
            get { return (int)_LineSize;}
            set
            {
                if (value == _LineSize | value < 0 | value > 100)
                    return;

                _LineSize = value;

                Invalidate();
            }
        }

        [Description("LineWidth"),
        Category("Apperance"),
        DefaultValue(4),
        Browsable(true)]
        public int LineWidth
        {
            get { return (int)_LineWidth; }
            set
            {
                if (value == _LineWidth | value < 0 | value > 100)
                    return;

                _LineWidth = value;

                Invalidate();
            }
        }

        [Description("Blink_on_Focus"),
        Category("Apperance"),
        DefaultValue(false),
        Browsable(true)]
        public bool blink_on_focus
        {
            get { return _blink_on_focus; }
            set
            {
                if (value == _blink_on_focus)
                    return;

                _blink_on_focus = value;

                Invalidate();
            }
        }

        [Description("show aktual Time"),
        Category("Apperance"),
        DefaultValue(false),
        Browsable(true)]
        public bool showNow
        {
            get { return _showNow; }
            set
            {
                if (value == _showNow)
                    return;

                _showNow = value;
                Invalidate();
            }
        }



        [Description("Inc"),
        Category("Apperance"),
        DefaultValue(100),
        Browsable(true)]
        public int Inc
        {
            get { return _Inc; }
            set
            {
                if (value == _Inc)
                    return;

                _Inc = value;

                Invalidate();
            }
        }

        [Description("IncPos1"),
        Category("Apperance"),
        //DefaultValue(1),
        Browsable(true)]
        public int IncPos1
        {
            get { return _Slider1Pos; }
            set
            {
                if (value == _Slider1Pos)
                    return;

                _Slider1Pos = value;

                Invalidate();
            }
        }

        [Description("IncPos2"),
        Category("Apperance"),
        //DefaultValue(_Inc),
        Browsable(true)]
        public int IncPos2
        {
            get { return _Slider2Pos; }
            set
            {
                if (value == _Slider2Pos)
                    return;

                _Slider2Pos = value;

                Invalidate();
            }
        }

        [Description("Text"),
        Category("Apperance"),
        DefaultValue("Text"),
        Browsable(true)]
        public new string Text
        {
            get { return base.Text;}
            set
            {
                if (value == base.Text)
                    return;

                base.Text = value;

                Invalidate();
            }
        }

        [Description("TimeFormat"),
        Category("Apperance"),
        DefaultValue("HH:mm"),
        Browsable(true)]
        public string TimeFormat
        {
            get { return _timeFormat; }
            set
            {
                if (value == _timeFormat)
                    return;

                _timeFormat = value;

                Invalidate();
            }
        }

        [Description("TimePos1"),
        Category("Apperance"),
        Browsable(true)]
        public DateTime Time1
        {
            get { return _Time1; }
            set
            {
                if (value == _Time1 || value < _TimeStart || value > _TimeEnd)
                    return;

                _Time1 = value;
                _Slider1Pos = set_Select_to_pos(_Time1, _Slider1Pos);

                ValueChangedVoid();
                Invalidate();
            }
        }

        [Description("TimePos2"),
        Category("Apperance"),
        Browsable(true)]
        public DateTime Time2
        {
            get { return _Time2; }
            set
            {
                if (value == _Time2 || value < _TimeStart || value > _TimeEnd)
                    return;

                _Time2 = value;
                _Slider2Pos = set_Select_to_pos(_Time2, _Slider2Pos);

                ValueChangedVoid();
                Invalidate();
            }
        }

        [Description("TimeStart"),
        Category("Apperance"),
        Browsable(true)]
        public DateTime TimeStart
        {
            get { return _TimeStart; }
            set
            {
                if (value == _TimeStart)
                    return;

                if (value >= _TimeEnd)
                    _TimeStart = value.Subtract(new TimeSpan(1,0,0));

                _TimeStart = value;
                _TSdist = new TimeSpan(_TimeEnd.Ticks - _TimeStart.Ticks);

                ValueChangedVoid();
                Invalidate();
            }
        }

        [Description("TimeEnd"),
        Category("Apperance"),
        Browsable(true)]
        public DateTime TimeEnd
        {
            get { return _TimeEnd; }
            set
            {
                if (value == _TimeEnd)
                    return;
                if (value <= _TimeStart)
                    _TimeEnd = _TimeStart.AddHours(1);

                _TimeEnd = value;
                _TSdist = new TimeSpan(_TimeEnd.Ticks - _TimeStart.Ticks);

                ValueChangedVoid();
                Invalidate();
            }
        }

        [Description("TSdist"),
        Category("Apperance"),
        Browsable(true)]
        public TimeSpan TSdist
        {
            get { return _TSdist; }
            set
            {
                if (value == _TSdist)
                    return;

                _TSdist = value;
                //_TSdist = new TimeSpan(_TSdist.Ticks - _TimeStart.Ticks);

                Invalidate();
            }
        }


        [Description("Highlight"),
        Category("Apperance"),
        Browsable(true)]
        public List<TimeRange> Highlight
        {
            get { return _Highlight; }
            set
            {
                if (value == _Highlight)
                    return;

                Invalidate();
            }
        }

        #region COLOR
        [Description("Color of Seperators"),
        Category("Colors"),
        DefaultValue(typeof(Color), "PaleTurquoise"),
        Browsable(true)]
        public Color CSeperator
        {
            get { return _CSeperator; }
            set
            {
                if (value == _CSeperator)
                    return;

                _CSeperator = value;

                Invalidate();
            }
        }

        [Description("Color of Center"),
        Category("Colors"),
        DefaultValue(typeof(Color), "CornflowerBlue"),
        Browsable(true)]
        public Color CCenter
        {
            get { return _CCenter; }
            set
            {
                if (value == _CCenter)
                    return;

                _CCenter = value;

                Invalidate();
            }
        }

        [Description("Color Left"),
        Category("Colors"),
        DefaultValue(typeof(Color), "Black"),
        Browsable(true)]
        public Color CLeft
        {
            get { return _CLeft; }
            set
            {
                if (value == _CLeft)
                    return;

                _CLeft = value;

                Invalidate();
            }
        }

        [Description("Color Right"),
        Category("Colors"),
        DefaultValue(typeof(Color), "Black"),
        Browsable(true)]
        public Color CRight
        {
            get { return _CRight; }
            set
            {
                if (value == _CRight)
                    return;

                _CRight = value;

                Invalidate();
            }
        }

        [Description("Color Sel1"),
        Category("Colors"),
        DefaultValue(typeof(Color), "LemonChiffon"),
        Browsable(true)]
        public Color CSel1
        {
            get { return _CSel1; }
            set
            {
                if (value == _CSel1)
                    return;

                _CSel1 = value;

                Invalidate();
            }
        }

        [Description("Color Sel2"),
        Category("Colors"),
        DefaultValue(typeof(Color), "MidnightBlue"),
        Browsable(true)]
        public Color CSel2
        {
            get { return _CSel2; }
            set
            {
                if (value == _CSel2)
                    return;

                _CSel2 = value;

                Invalidate();
            }
        }

        [Description("Color Sel1 Highlight"),
        Category("Colors"),
        DefaultValue(typeof(Color), "LemonChiffon"),
        Browsable(true)]
        public Color CSel1Highlight
        {
            get { return _CSel1Highlight; }
            set
            {
                if (value == _CSel1Highlight)
                    return;

                _CSel1Highlight = value;

                Invalidate();
            }
        }

        [Description("Color Sel2 Highlight"),
        Category("Colors"),
        DefaultValue(typeof(Color), "MidnightBlue"),
        Browsable(true)]
        public Color CSel2Highlight
        {
            get { return _CSel2Highlight; }
            set
            {
                if (value == _CSel2Highlight)
                    return;

                _CSel2Highlight = value;

                Invalidate();
            }
        }

        [Description("Color CTextCenter"),
        Category("Colors"),
        DefaultValue(typeof(Color), "DarkMagenta"),
        Browsable(true)]
        public Color CTextCenter
        {
            get { return _CTextCenter; }
            set
            {
                if (value == _CTextCenter)
                    return;

                _CTextCenter = value;

                Invalidate();
            }
        }

        [Description("Color Now"),
        Category("Colors"),
        DefaultValue(typeof(Color), "DarkMagenta"),
        Browsable(true)]
        public Color CNow
        {
            get { return _CNow; }
            set
            {
                if (value == _CNow)
                    return;

                _CNow = value;

                Invalidate();
            }
        }
        #endregion

        #endregion

        #region Event
        // Declare an event 
        public event ValueChangedEventHandler ValueChanged;
        protected virtual void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }
        private void ValueChangedVoid()
        {
            //int value = 0;
            if (! Pressed)
                OnValueChanged(new EventArgs());
        }
        #endregion

        public CF_TimeSpanSlider()
        {
            InitializeComponent();

            this.SetStyle(
                        ControlStyles.UserPaint |
                        ControlStyles.AllPaintingInWmPaint |
                        ControlStyles.DoubleBuffer, true);


            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

        }

        private void CF_TimeSpanSlider_Load(object sender, EventArgs e)
        {
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 500;
            aTimer.Enabled = true;

            if (Inc <= 0)
                Inc = 100;

            //int i = Slider1[99].X;

            //Time2 = get_Time_from_pos(Slider2Pos - 1);
            //Time1 = get_Time_from_pos(Slider1Pos + 1);


            _Slider1Pos = 50;
            _Slider2Pos = ClientRectangle.Width - 50;

            if (_Time1 == null)
                _Time1 = DateTime.Now;
            if (_Time2 == null)
                _Time2 = DateTime.Now + TimeSpan.FromHours(5);

            if (_CCenter == Color.FromArgb(0, 0, 0, 0))
                _CCenter = Color.CornflowerBlue;
            if (CSeperator == Color.FromArgb(0, 0, 0, 0))
                _CSeperator = Color.PaleTurquoise;
            if (_CLeft == Color.FromArgb(0, 0, 0, 0))
                _CLeft = Color.Black;
            if (_CRight == Color.FromArgb(0, 0, 0, 0))
                _CRight = Color.Black;
            if (_CSel1 == Color.FromArgb(0, 0, 0, 0))
                _CSel1 = Color.LemonChiffon;
            if (_CSel2 == Color.FromArgb(0, 0, 0, 0))
                _CSel2 = Color.MidnightBlue;
            if (_CTextCenter == Color.FromArgb(0, 0, 0, 0))
                _CTextCenter = Color.DarkMagenta;

            _Slider1Pos = set_Select_to_pos(_Time1, _Slider1Pos);
            _Slider2Pos = set_Select_to_pos(_Time2, _Slider2Pos);

            _TSdist = new TimeSpan(_TimeEnd.Ticks - _TimeStart.Ticks);

            Invalidate();
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            blink = !blink;

            if (showNow || blink_on_focus)
                Invalidate();
        }

        public void BeginUpdate()
        {
            _Update = true;
        }
        public void EndUpdate()
        {
            _Update = false;
            this.Invalidate();
        }

        public void init(DateTime start, DateTime end, DateTime T1, DateTime T2)
        {
            if (start < end && T1 <= T2 && start <= T1 && T2 <= end)
            {
                _TimeStart = start;
                _TimeEnd = end;
                _Time1 = T1;
                _Time2 = T2;

                _Slider1Pos = set_Select_to_pos(_Time1, _Slider1Pos);
                //_Time1 = get_Time_from_pos(_Slider1Pos);

                _Slider2Pos = set_Select_to_pos(_Time2, _Slider2Pos);
                //_Time2 = get_Time_from_pos(_Slider2Pos);

                this.Invalidate();
            }
        }
        public void setHighlight(List<TimeRange> HL)
        {
            _Highlight = HL;
        }


        #region Override
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            float startPos = ClientRectangle.Height / 2;
            float endPos = ClientRectangle.Width; // -ClientRectangle.Height / 2;
            float distPos = endPos - startPos;
            float distPosPro = distPos / _Inc;
            float pos;

            //float center = (((ClientRectangle.Width - ClientRectangle.Height) /100 * pos + (ClientRectangle.Height / 2)));

            Hovering = true;
            PointF SelPos = subtrPointF(e.Location, _MouseOffsetF);
            pos = SelPos.X; //(SelPos.X / distPosPro); //- ClientRectangle.Height / 2f) / distPosPro; 

            if (Slider1sel & Pressed)
            {
                if (_Step != steps.none)
                {
                    _Time1 = get_Time_from_pos(e.Location.X, true);
                    _Slider1Pos = set_Select_to_pos(_Time1, _Slider1Pos);
                }
                else
                {
                    _Slider1Pos = e.Location.X;//(int)pos;
                    _Time1 = get_Time_from_pos(_Slider1Pos);

                    if (_debug)
                    {
                        STime_Sel_1 = _Time1.Date.ToString() + ": " + _Time1.TimeOfDay.ToString();
                    }
                }

                Invalidate();
            }
            if (Slider2sel & Pressed)
            {
                if (_Step != steps.none)
                {
                    _Time2 = get_Time_from_pos(e.Location.X, true);
                    _Slider2Pos = set_Select_to_pos(_Time2, _Slider2Pos);
                }
                else
                {
                    _Slider2Pos = e.Location.X;//(int)pos;
                    _Time2 = get_Time_from_pos(_Slider2Pos);
                }

                if (_debug)
                {
                    STime_Sel_2 = _Time2.Date.ToString() + ": " + _Time2.TimeOfDay.ToString();
                }

                Invalidate();
            }

            base.OnMouseMove(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Hovering = false;
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && _Step != steps.Week)
            {
                Point pt = e.Location;
                Point pscreen = System.Windows.Forms.Cursor.Position;

                contextMenuStrip1.Enabled = true;
                contextMenuStrip1.Show(pscreen);

                //else { }
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {

            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Pressed = true;
                _MouseDownPos = e.Location;
            }
            if (Slider1 != null)
            {
                //if (e.Location.X < Slider1[2].X & e.Location.X > Slider1[1].X & e.Location.Y < Slider1[3].Y & e.Location.Y > Slider1[1].Y)
                if (!Slider1innerB && !Slider1outerB)
                {
                    if (PointIsInPolygon(Slider1, e.Location))
                    {
                        Slider1sel = true;
                        _MouseOffset = new Point(e.X - Slider1[0].X, e.Y - Slider1[0].Y);
                        _MouseOffsetF = new PointF(e.X - Slider1[0].X, e.Y - Slider1[0].Y);
                    }
                    else
                        Slider1sel = false;
                }
                if (Slider1innerB)
                {
                    if (PointIsInPolygon(Slider1inner, e.Location))
                    {
                        Slider1sel = true;
                        _MouseOffset = new Point(e.X - Slider1[0].X, e.Y - Slider1[0].Y);
                        _MouseOffsetF = new PointF(e.X - Slider1[0].X, e.Y - Slider1[0].Y);
                    }
                    else
                        Slider1sel = false;
                }
                if (Slider1outerB)
                {
                    if (PointIsInPolygon(Slider1outer, e.Location))
                    {
                        Slider1sel = true;
                        _MouseOffset = new Point(e.X - Slider1[0].X, e.Y - Slider1[0].Y);
                        _MouseOffsetF = new PointF(e.X - Slider1[0].X, e.Y - Slider1[0].Y);
                    }
                    else
                        Slider1sel = false;
                }
            }

            if (Slider2 != null)
            {
                //if (e.Location.X < Slider2[2].X & e.Location.X > Slider2[1].X & e.Location.Y < Slider2[3].Y & e.Location.Y > Slider2[1].Y)
                if (!Slider2innerB && !Slider2outerB)
                {
                    if (PointIsInPolygon(Slider2, e.Location))
                    {
                        Slider2sel = true;
                        _MouseOffset = new Point(e.X - Slider2[0].X, e.Y - Slider2[0].Y);
                        _MouseOffsetF = new PointF(e.X - Slider2[0].X, e.Y - Slider2[0].Y);
                    }
                    else
                        Slider2sel = false;
                }
                if (Slider2outerB)
                {
                    if (PointIsInPolygon(Slider2outer, e.Location))
                    {
                        Slider2sel = true;
                        _MouseOffset = new Point(e.X - Slider2[0].X, e.Y - Slider2[0].Y);
                        _MouseOffsetF = new PointF(e.X - Slider2[0].X, e.Y - Slider2[0].Y);
                    }
                    else
                        Slider2sel = false;
                }
                if (Slider2innerB)
                {
                    if (PointIsInPolygon(Slider2inner, e.Location))
                    {
                        Slider2sel = true;
                        _MouseOffset = new Point(e.X - Slider2[0].X, e.Y - Slider2[0].Y);
                        _MouseOffsetF = new PointF(e.X - Slider2[0].X, e.Y - Slider2[0].Y);
                    }
                    else
                        Slider2sel = false;
                }
            }

            if (!Slider1sel & !Slider2sel)
                _MouseOffset = new Point(0,0);

            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Pressed = false;
                if (Slider1sel || Slider2sel)
                    ValueChangedVoid();
            }

            base.OnMouseUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
             // Boolean flag used to determine when a character other than a number is entered. 
            nonNumberEntered = false;

            //Text = e.KeyData.ToString();

            // Determine whether the keystroke is a number from the top of the keyboard. 
            if (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9)
            {
                // Determine whether the keystroke is a number from the keypad. 
                if (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9)
                {
                    // Determine whether the keystroke is a backspace. 
                    if (e.KeyCode != Keys.Back)
                    {
                        // A non-numerical keystroke was pressed. 
                        // Set the flag to true and evaluate in KeyPress event.
                        nonNumberEntered = true;
                    }
                }
            }
            //If shift key was pressed, it's not a number. 
            if (Control.ModifierKeys == Keys.Shift)
            {
                nonNumberEntered = true;
            }

            if (e.KeyData == Keys.Space & !modSel1 & !modSel2)
            {
                if (_Slider1sel)
                {
                    Slider1sel = false;
                    Slider2sel = true;
                    Invalidate();
                    return;
                }
                if (_Slider2sel | (!_Slider1sel & !_Slider2sel))
                {
                    Slider2sel = false;
                    Slider1sel = true;
                    Invalidate();
                    return;
                }
            }
            //else
            //    base.OnKeyDown(e);

            if (modSel1)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    modSel1 = false;
                    _Slider1sel = false;
                    //Debug.Print(Time1.Date.ToString() + " " + Modify);
                    _Time1 = get_Time_from_String(_Time1.Date.ToString().Substring(0, 10) + " " + Modify);
                    //Modify = Time1.TimeOfDay.ToString();
                }
            }

            if (modSel2)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    modSel2 = false;
                    _Slider2sel = false;
                    //Debug.Print(Time2.Date.ToString() + " " + Modify);
                    _Time2 = get_Time_from_String(_Time2.Date.ToString().Substring(0,10) + " " + Modify);
                    //Modify = Time2.TimeOfDay.ToString();
                }
            }

            if (modSel1 | modSel2)
            {
                if (!nonNumberEntered & (e.KeyCode != Keys.Back))
                {
                    if (Modify.Length < 5)
                    {
                        Modify += Char_from_KeyCode(e);
                        if (Modify.Length == 2)
                            Modify += ":";
                    }
                }

                if (e.KeyCode == Keys.Back)
                {
                    //MessageBox.Show(mod_temp);
                    Modify = del_from_String(Modify);
                    if (Modify.Length == 3)
                        Modify = del_from_String(Modify);

                    
                }
            }

            if (_Slider1sel & e.KeyCode == Keys.Enter)
            {
                modSel1 = true;
                Modify = "";
                //Time1 = get_Time_from_String(Time1.Date.ToString()+ " " + Modify);
                //Time1 = DateTime.Parse(Time1.Date.ToString() + " " + Modify);
            }

            if (_Slider2sel & e.KeyCode == Keys.Enter)
            {
                modSel2 = true;
                Modify = "";
                //Time2 = get_Time_from_String(Time2.Date.ToString() + " " + Modify);
            }

            Invalidate();
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            //base.OnKeyPress(e);
        }
        protected override void OnGotFocus(EventArgs e)
        {
            _focused = true;
            _Slider1sel = true;
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            _focused = false;
            base.OnLostFocus(e);
        }
        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if (m.WParam == (IntPtr)Keys.Right)// && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                return true;
            }
            else if (m.WParam == (IntPtr)Keys.Left)// && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                return true;
            }
            else if (m.WParam == (IntPtr)Keys.Up)// && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                return true;
            }
            else if (m.WParam == (IntPtr)Keys.Down)// && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref m, keyData);
            }
        }
    #endregion

    #region Calc
        private bool PointIsInPolygon(Point[] polygon, Point target_point)
        {
            // Make a GraphicsPath containing the polygon.
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(polygon);

            // See if the point is inside the path.
            return path.IsVisible(target_point);
        }


        Point subtrPoint(Point P1, Point P2)
        {
            Point returnPoint = new Point(0, 0);

            if (P1 != null && P2 != null)
                returnPoint = new Point(P1.X - P2.X, P1.Y - P2.Y);
            return returnPoint;
        }

        PointF subtrPointF(PointF P1, PointF P2)
        {
            PointF returnPointF = new PointF(0, 0);

            if (P1 != null && P2 != null)
                returnPointF = new PointF(P1.X - P2.X, P1.Y - P2.Y);
            return returnPointF;
        }

        private Point[] xPointArray(int pos, Rectangle outline, float ratio, float offset)
        {
            float offpr = 0;
            if (offset > 0)
                offpr = outline.Height * offset;

            if (outline == null)
                outline = new Rectangle(0, 0, 2, 2);
            if (outline.Height <= 0)
                outline.Height = 2;
            if (outline.Width <= 0)
                outline.Width = 2;

            if (pos < 0)
                pos = 1;
            if (pos > ClientRectangle.Width)
                pos = ClientRectangle.Width;

            int edge = (int)(outline.Height / 2 * ratio);
            //float center = (((outline.Width - outline.Height) / _Inc * pos + (outline.Height / 2)));
            float center = pos;//outline.Width / _Inc * pos;

            Point[] output = new Point[5];
            output[0] = new Point((int)center, outline.Height / 2); //center
            output[1] = new Point((int)(output[0].X - edge - offpr), output[0].Y - edge); //left top
            output[2] = new Point((int)(output[0].X + edge + offpr), output[0].Y - edge); //right top
            output[3] = new Point((int)(output[0].X - edge - offpr), output[0].Y + edge); //left bottom
            output[4] = new Point((int)(output[0].X + edge + offpr), output[0].Y + edge); //right bottom

            return output;
        }

        private PointF[] LinesPointArray(float size)
        {
            PointF[] outLines = null;
            float halfH = ClientRectangle.Height / 2f;
            float iStart = 0; //ClientRectangle.Height / 2;
            float iEnd = ClientRectangle.Width; //- (ClientRectangle.Height / 2);
            float iDist = iEnd - iStart;
            int iStep = 1;
            float idistX = 1;
            float iY1 = halfH - (int)(halfH * size);
            float iY2 = halfH + (int)(halfH * size);
            float rX = 0;

            //iY1 = 0;
            //iY2 = 20;

            if (_Step == steps.none)
                return null;

            //if (_Step == steps.Minute1)
            //    iStep = (int)(_TSdist.Ticks / new TimeSpan(0, 1, 0).Ticks);


            if (_Step == steps.Minute1)
                rX = new TimeSpan(0, 1, 0).Ticks;
            if (_Step == steps.Minute5)
                rX = new TimeSpan(0, 5, 0).Ticks;
            if (_Step == steps.Minute10)
                rX = new TimeSpan(0, 10, 0).Ticks;
            if (_Step == steps.Minute15)
                rX = new TimeSpan(0, 15, 0).Ticks;
            if (_Step == steps.Minute30)
                rX = new TimeSpan(0, 30, 0).Ticks;
            if (_Step == steps.Hour1)
                rX = new TimeSpan(1, 0, 0).Ticks;
            if (_Step == steps.Day)
                rX = new TimeSpan(1, 0, 0, 0).Ticks;
            if (_Step == steps.Week)
                rX = new TimeSpan(7, 0, 0, 0).Ticks;

            if (rX > 0 && _TSdist.Ticks > 0)
            {
                iStep = (int)(_TSdist.Ticks / rX);


                if (iStep != 0)
                    idistX = CF_calc.Kurventransformation(rX, 0, _TSdist.Ticks, iStart, iEnd);

                if (iStep > 0 & iStep < 32000)
                {
                    outLines = new PointF[iStep];//[iStep * 2];
                    for (int i = 0; i < iStep; i++)
                    {
                        outLines[i] = new PointF(iStart + (idistX * i), halfH);
                        //outLines[i * 2 + 1] = new Point(iStart + (idistX * i), halfH);
                    }
                }
            }

            return outLines;
        }

        private int set_Select_to_pos(DateTime Zeit, int ActPos)
        {
            int posOut = ActPos;

            double dPixelStart  = 0d;
            double dPixelEnd    = ClientRectangle.Width;
            double dTicksStart  = _TimeStart.Ticks;
            double dTicksEnd    = _TimeEnd.Ticks;
            double dTicksZeit   = Zeit.Ticks;

            if (dTicksZeit < dTicksStart)
                dTicksZeit = dTicksStart;
            if (dTicksZeit > dTicksEnd)
                dTicksZeit = dTicksEnd;

            debug_string = "set_Select_to_pos : Zeit Ticks : " + Zeit.Ticks.ToString();

            if (dTicksStart <= dTicksZeit && dTicksZeit <= dTicksEnd)
            {
                posOut = (int)CF_calc.doubleKurventransformation(dTicksZeit, dTicksStart, dTicksEnd, dPixelStart, dPixelEnd);
            }
            else
            {
                debug_string = "Parameter outside Limitation->  Start: " + _TimeStart.ToString() + " IN: " + Zeit.ToString() + " End: " + _TimeEnd.ToString();
                debug_string = Zeit.ToString();
            }
            return posOut;
        }

        private DateTime get_Time_from_pos(int Pos)
        {
            return get_Time_from_pos(Pos, false);
        }
        private DateTime get_Time_from_pos(int Pos, bool round2Step)
        {
            DateTime TimeOut = new DateTime();

            double dPixelPos = Pos;
            double dPixelStart = 0d;
            double dPixelEnd = ClientRectangle.Width;
            double dTicksStart = _TimeStart.Ticks;
            double dTicksEnd = _TimeEnd.Ticks;
            double dTimeTicks = dTicksStart;

            if (dPixelPos < dPixelStart)
                dPixelPos = dPixelStart;
            if (dPixelPos > dPixelEnd)
                dPixelPos = dPixelEnd;

            if (dPixelStart <= dPixelPos && dPixelPos <= dPixelEnd)
            {
                dTimeTicks = CF_calc.doubleKurventransformation(dPixelPos, dPixelStart, dPixelEnd, dTicksStart, dTicksEnd);
                long lTime = Convert.ToInt64(dTimeTicks);
                TimeOut = new DateTime(lTime);

                if (round2Step && _interval.Ticks > 0)
                {
                    TimeOut = Round(TimeOut, _interval);
                }
            }

            return TimeOut;
        }

        private DateTime get_Time_from_String(string input)
        {
            DateTime TimeOut = new DateTime();
            try
            {
                //MessageBox.Show(input);
                if (input.Length < 16)
                {
                    while (input.Length < 16)
                    {
                        input += "0";
                    }

                    //MessageBox.Show(input);
                }

                TimeOut = DateTime.Parse(input);
                //TimeOut = DateTime.ParseExact(input, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { }

            return TimeOut;
        }

        private char Char_from_KeyCode(KeyEventArgs e)
        {
            char CharOut = new char();

            if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
            {
                switch (e.KeyCode)
                {
                    case Keys.NumPad0:
                        CharOut = Convert.ToChar("0");
                        break;
                    case Keys.NumPad1:
                        CharOut = Convert.ToChar("1");
                        break;
                    case Keys.NumPad2:
                        CharOut = Convert.ToChar("2");
                        break;
                    case Keys.NumPad3:
                        CharOut = Convert.ToChar("3");
                        break;
                    case Keys.NumPad4:
                        CharOut = Convert.ToChar("4");
                        break;
                    case Keys.NumPad5:
                        CharOut = Convert.ToChar("5");
                        break;
                    case Keys.NumPad6:
                        CharOut = Convert.ToChar("6");
                        break;
                    case Keys.NumPad7:
                        CharOut = Convert.ToChar("7");
                        break;
                    case Keys.NumPad8:
                        CharOut = Convert.ToChar("8");
                        break;
                    case Keys.NumPad9:
                        CharOut = Convert.ToChar("9");
                        break;
                }
            }
            else
               CharOut = Convert.ToChar(e.KeyValue);

            return CharOut;
        }

        private string del_from_String(string input)
        {
            string StringOut = "";

            if (input.Length > 0)
            {
                //StringOut = input.Substring(0, input.Length - 1);
                StringOut = input.Remove(input.Length - 1);
            }

            return StringOut;
        }

        private int getWeekNr(DateTime dt)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            System.Globalization.Calendar cal = dfi.Calendar;

            int iOut = cal.GetWeekOfYear(dt.Date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

            return iOut;
        }

        void set_Text_sel()
        {
            if (_debug)
                return;
            if (_Step == steps.Week)
            {
                int KW1 = getWeekNr(_Time1);
                int KW2 = getWeekNr(_Time2);

                int Y1 = _Time1.Year;
                int Y2 = _Time2.Year;

                TimeSpan ts = _Time2 - _Time1;

                int distI = ts.Days / 7;

                STime_Sel_1 = "KW " + KW1.ToString() + Environment.NewLine + Y1.ToString();
                STime_Sel_2 = "KW " + KW2.ToString() + Environment.NewLine + Y2.ToString();

                Text = distI.ToString();
            }
            else
            {
                STime_Sel_1 = _Time1.ToString(_timeFormat); //.Substring(0, 8);
                STime_Sel_2 = _Time2.ToString(_timeFormat); //.Substring(0, 8);

                if (_Time2.Ticks > _Time1.Ticks)
                {
                    TimeSpan ts = new TimeSpan(_Time2.Ticks - _Time1.Ticks);
                    DateTime dt = new DateTime() + ts;
                    Text = dt.ToString(_timeFormat); //.Substring(0, 8);
                                                     //_TSdist = new TimeSpan(_Time2.Ticks - _Time1.Ticks);
                }
            }
        }

        Color col_inv(Color c)
        {
            int R = 255 - c.R;
            int G = 255 - c.G;
            int B = 255 - c.B;

            Color cOut = Color.FromArgb(R, G, B);
            return cOut;
        }

        private static DateTime Floor(DateTime dateTime, TimeSpan interval)
        {
            return dateTime.AddTicks(-(dateTime.Ticks % interval.Ticks));
        }
        private static DateTime Ceiling(DateTime dateTime, TimeSpan interval)
        {
            var overflow = dateTime.Ticks % interval.Ticks;

            return overflow == 0 ? dateTime : dateTime.AddTicks(interval.Ticks - overflow);
        }
        private static DateTime Round(DateTime dateTime, TimeSpan interval)
        {
            var halfIntervelTicks = ((interval.Ticks + 1) >> 1);

            return dateTime.AddTicks(halfIntervelTicks - ((dateTime.Ticks + halfIntervelTicks) % interval.Ticks));
        }

        #endregion

        #region PAINT
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_Update)
                return;

            _Slider1Pos = set_Select_to_pos(_Time1, _Slider1Pos);
            _Slider2Pos = set_Select_to_pos(_Time2, _Slider2Pos);

            #region set default Color
            if (_CCenter == Color.FromArgb(0, 0, 0, 0))
                _CCenter = Color.CornflowerBlue;
            if (CSeperator == Color.FromArgb(0, 0, 0, 0))
                _CSeperator = Color.PaleTurquoise;
            if (_CLeft == Color.FromArgb(0, 0, 0, 0))
                _CLeft = Color.Black;
            if (_CRight == Color.FromArgb(0, 0, 0, 0))
                _CRight = Color.Black;
            if (_CSel1 == Color.FromArgb(0, 0, 0, 0))
                _CSel1 = Color.LemonChiffon;
            if (_CSel2 == Color.FromArgb(0, 0, 0, 0))
                _CSel2 = Color.MidnightBlue;
            if (_CTextCenter == Color.FromArgb(0, 0, 0, 0))
                _CTextCenter = Color.DarkMagenta;


            #endregion

            #region Constant
            _TSdist = new TimeSpan(_TimeEnd.Ticks - _TimeStart.Ticks);
            //_Inc = ClientRectangle.Width;

            //this.BackColor = Color.FromArgb(0, 0, 0, 0);
            Graphics gfx = e.Graphics;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            Marks = LinesPointArray((float)_LineSize / 100f);

            int edge = (int)(ClientRectangle.Height / 2f * 0.82f);


            if (_Slider1Pos + (_Inc / 100f * 2f) >= _Slider2Pos)
                _Slider1Pos = _Slider2Pos;// -(int)(Inc / 100f * 2f);
            if (_Slider1Pos <= 0)
                _Slider1Pos = 1;
            //if (Slider1Pos >= _Inc)
                //Slider1Pos = _Inc;
            if (_Slider1Pos >= ClientRectangle.Width)
                _Slider1Pos = ClientRectangle.Width;

            if (_Slider2Pos <= 0)
                _Slider2Pos = 1;//Slider2Pos =(int)(_Inc / 100f * 1f);
            //if (Slider2Pos >= _Inc)
                //Slider2Pos = (_Inc);
            if (_Slider2Pos >= ClientRectangle.Width)
                _Slider2Pos = ClientRectangle.Width;

         
            Slider1 = xPointArray(_Slider1Pos, ClientRectangle, 0.8f, 0f);
            Slider2 = xPointArray(_Slider2Pos, ClientRectangle, 0.8f, 0f);

            Slider1big = xPointArray(_Slider1Pos, ClientRectangle, 0.8f, 0.1f);
            Slider2big = xPointArray(_Slider2Pos, ClientRectangle, 0.8f, 0.1f);  

            Slider1outer[0] = Slider1[0];
            Slider1outer[1] = Slider1[1];
            Slider1outer[2] = Slider1[3];
            Slider1outerBig[0] = Slider1big[0];
            Slider1outerBig[1] = Slider1big[1]; //l
            Slider1outerBig[2] = Slider1big[3]; //l

            Slider1inner[0] = Slider1[0];
            Slider1inner[1] = Slider1[2]; //r
            Slider1inner[2] = Slider1[4]; //r
            Slider1innerBig[0] = Slider1big[0];
            Slider1innerBig[1] = Slider1big[2];
            Slider1innerBig[2] = Slider1big[4];

            Slider2outer[0] = Slider2[0];
            Slider2outer[1] = Slider2[2];
            Slider2outer[2] = Slider2[4];
            Slider2outerBig[0] = Slider2big[0];
            Slider2outerBig[1] = Slider2big[2];
            Slider2outerBig[2] = Slider2big[4];

            Slider2inner[0] = Slider2[0];
            Slider2inner[1] = Slider2[1];
            Slider2inner[2] = Slider2[3];
            Slider2innerBig[0] = Slider2big[0];
            Slider2innerBig[1] = Slider2big[1];
            Slider2innerBig[2] = Slider2big[3];

            int s1centers2 = (Slider2[0].X - Slider1[0].X);

            Rectangle rc = ClientRectangle;
            Rectangle rcc1 = new Rectangle(Slider1[0].X, Slider1[0].Y - (int)(edge * 0.7),  s1centers2, (int)(edge * 0.7f));
            Rectangle rcc2 = new Rectangle(Slider1[0].X, Slider1[0].Y,                      s1centers2, (int)(edge * 0.7f));

            rc.X -= 1;
            rc.Y -= 1;
            rc.Width += 2;
            rc.Height += 2;

            Rectangle rcc1br = rcc1;
            Rectangle rcc2br = rcc2;

            rcc1br.X -= 2;
            rcc1br.Y -= 2;
            rcc1br.Width += 4;
            rcc1br.Height += 4;

            rcc2br.X -= 2;
            rcc2br.Y -= 2;
            rcc2br.Width += 4;
            rcc2br.Height += 4;

            Rectangle rcl1 = new Rectangle(0, Slider1[0].Y - (int)(edge * 0.7f), Slider1[0].X, (int)(edge * 0.7f));
            Rectangle rcl2 = new Rectangle(0, Slider1[0].Y,                      Slider1[0].X, (int)(edge * 0.7f));

            Rectangle rcl1br = rcl1;
            Rectangle rcl2br = rcl2;

            rcl1br.X -= 2;
            rcl1br.Y -= 2;
            rcl1br.Width += 4;
            rcl1br.Height += 4;

            rcl2br.X -= 2;
            rcl2br.Y -= 2;
            rcl2br.Width += 4;
            rcl2br.Height += 4;

            Rectangle rcr1 = new Rectangle(Slider2[0].X, Slider2[0].Y - (int)(edge * 0.7f),  ClientRectangle.Width - Slider2[0].X, (int)(edge * 0.7f));
            Rectangle rcr2 = new Rectangle(Slider2[0].X, Slider2[0].Y,                      ClientRectangle.Width - Slider2[0].X, (int)(edge * 0.7f));

            Rectangle rcr1br = rcr1;
            Rectangle rcr2br = rcr2;

            rcr1br.X -= 2;
            rcr1br.Y -= 2;
            rcr1br.Width += 4;
            rcr1br.Height += 4;

            rcr2br.X -= 2;
            rcr2br.Y -= 2;
            rcr2br.Width += 4;
            rcr2br.Height += 4;

            // Parent.BackColor
            Color alpha = Parent.BackColor;


            LinearGradientBrush brush = new LinearGradientBrush(rcc1br, alpha, _CCenter, LinearGradientMode.Vertical);
            LinearGradientBrush brush2 = new LinearGradientBrush(rcc2br, _CCenter, alpha, LinearGradientMode.Vertical);

            LinearGradientBrush brushle = new LinearGradientBrush(rcl1br, alpha, _CLeft, LinearGradientMode.Vertical);
            LinearGradientBrush brushle2 = new LinearGradientBrush(rcl2br, _CLeft, alpha, LinearGradientMode.Vertical);

            LinearGradientBrush brushre = new LinearGradientBrush(rcr1br, alpha, _CRight, LinearGradientMode.Vertical);
            LinearGradientBrush brushre2 = new LinearGradientBrush(rcr2br, _CRight, alpha, LinearGradientMode.Vertical);

            if (_focused & blink & _blink_on_focus)
            {
                Color _CCenter_INV = col_inv(_CCenter);
                Color _CLeft_INV = col_inv(_CLeft);
                Color _CRight_INV = col_inv(_CRight);

                brush = new LinearGradientBrush(rcc1br, alpha, _CCenter_INV, LinearGradientMode.Vertical);
                brush2 = new LinearGradientBrush(rcc2br, _CCenter_INV, alpha, LinearGradientMode.Vertical);

                brushle = new LinearGradientBrush(rcl1br, alpha, _CLeft_INV, LinearGradientMode.Vertical);
                brushle2 = new LinearGradientBrush(rcl2br, _CLeft_INV, alpha, LinearGradientMode.Vertical);

                brushre = new LinearGradientBrush(rcr1br, alpha, _CRight_INV, LinearGradientMode.Vertical);
                brushre2 = new LinearGradientBrush(rcr2br, _CRight_INV, alpha, LinearGradientMode.Vertical);
            }

            gfx.FillRectangle(new SolidBrush(alpha), rc);
            gfx.FillRectangle(brush, rcc1);
            gfx.FillRectangle(brush2, rcc2);

            gfx.FillRectangle(brushle, rcl1);
            gfx.FillRectangle(brushle2, rcl2);

            gfx.FillRectangle(brushre, rcr1);
            gfx.FillRectangle(brushre2, rcr2);

            LinearGradientBrush pos1b = new LinearGradientBrush(new Rectangle(Slider1[0].X, Slider1[0].Y, edge, edge), _CSel1, _CSel2, LinearGradientMode.ForwardDiagonal);
            LinearGradientBrush pos2b = new LinearGradientBrush(new Rectangle(Slider2[0].X, Slider2[0].Y, edge, edge), _CSel2, _CSel1, LinearGradientMode.ForwardDiagonal);

            LinearGradientBrush pos1bsel = new LinearGradientBrush(new Rectangle(Slider1big[1].X - 1, Slider1big[0].Y - 1, Slider1big[2].X - Slider1big[1].X + 2, Slider1big[3].Y - Slider1big[1].Y + 2), _CSel1Highlight, _CSel2Highlight, LinearGradientMode.Horizontal);
            LinearGradientBrush pos2bsel = new LinearGradientBrush(new Rectangle(Slider2big[1].X - 1, Slider2big[0].Y - 1, Slider2big[2].X - Slider2big[1].X + 2, Slider2big[3].Y - Slider2big[1].Y + 2), _CSel2Highlight, _CSel1Highlight, LinearGradientMode.Horizontal);

            #endregion

            #region Draw



            #region Highlight
            if (_Highlight != null)
            {
                foreach (TimeRange item in _Highlight)
                {
                    DrawHighlight(gfx, item);
                }
            }
            #endregion

            #region Select
            int dist_S1_to_S2 = _Slider2Pos - _Slider1Pos;
             if (dist_S1_to_S2 < 0)
                 dist_S1_to_S2 = dist_S1_to_S2 * -1;

             if (_Slider1sel & _focused)
             {
                 if (Slider1innerB)
                     gfx.FillPolygon(pos1bsel, Slider1innerBig);
                 if (Slider1outerB)
                     gfx.FillPolygon(pos1bsel, Slider1outerBig);
                 if (!Slider1outerB & !Slider1innerB)
                     gfx.FillPolygon(pos1bsel, Slider1big);

             }
             if (_Slider2sel & _focused)
             {
                 if (Slider2innerB)
                     gfx.FillPolygon(pos2bsel, Slider2innerBig);
                 if (Slider2outerB)
                     gfx.FillPolygon(pos2bsel, Slider2outerBig);
                 if (!Slider2outerB & !Slider2innerB)
                     gfx.FillPolygon(pos2bsel, Slider2big);

             }

             if (_Slider1Pos <= edge)
             {
                 gfx.FillPolygon(pos1b, Slider1inner);
                 Slider1innerB = true;
             }
             else if (((_Slider1Pos >= ClientRectangle.Width - edge) || (dist_S1_to_S2 <= edge * 2)) && ! Slider1innerB)
             {
                 gfx.FillPolygon(pos1b, Slider1outer);
                 Slider1outerB = true;
             }
             else
             {
                 gfx.FillPolygon(pos1b, Slider1);
                 Slider1innerB = false;
                 Slider1outerB = false;
             }

             if (_Slider2Pos >= ClientRectangle.Width - edge)
             {
                 gfx.FillPolygon(pos2b, Slider2inner);
                 Slider2innerB = true;
             }
             else if (((_Slider2Pos <= edge) || (dist_S1_to_S2 <= edge * 2)) && !Slider2innerB)
             {
                 gfx.FillPolygon(pos2b, Slider2outer);
                 Slider2outerB = true;
             }
             else
             {
                 gfx.FillPolygon(pos2b, Slider2);
                 Slider2outerB = false;
                 Slider2innerB = false;
             }
             #endregion

             #region Seperator
             Pen P1 = new Pen(_CSeperator, 2.0f);
             try
             {
                 if (Marks != null)
                 {
                     if (Marks.Length > 0 & Marks.Length < 32000)
                     {
                         for (int i = 0; i < Marks.Length; i++)
                         {
                             float iLineSize = ClientRectangle.Height / 100f * _LineSize;
                             float LineSizeHalf = iLineSize / 2;
                             float iLineWidth = ClientRectangle.Height / 100f * _LineWidth;

                             PointF EX = new PointF(Marks[i].X - iLineWidth, Marks[i].Y - (ClientRectangle.Height / 2f / 100f * _LineSize));
                             PointF PX = new PointF(Marks[i].X - iLineWidth, Marks[i].Y - _LineSize / 2f);


                             PointF X11 = new PointF(Marks[i].X - LineSizeHalf, Marks[i].Y - LineSizeHalf);
                             PointF X12 = new PointF(Marks[i].X + LineSizeHalf, Marks[i].Y + LineSizeHalf);

                             PointF X21 = new PointF(Marks[i].X - LineSizeHalf, Marks[i].Y + LineSizeHalf);
                             PointF X22 = new PointF(Marks[i].X + LineSizeHalf, Marks[i].Y - LineSizeHalf);

                             P1 = new Pen(_CSeperator, (float)_LineWidth);

                             switch (_eSeperator)
                             {
                                 case eSeperator_style.Line:
                                     gfx.DrawLine(P1, new PointF(Marks[i].X, Marks[i].Y - (iLineSize / 2)), new PointF(Marks[i].X, Marks[i].Y + (iLineSize / 2f)));
                                     break;

                                 case eSeperator_style.Elipse:
                                     gfx.DrawEllipse(P1, EX.X - (iLineWidth / 2f), EX.Y - (iLineSize / 2f), iLineWidth, iLineSize);
                                     break;

                                 case eSeperator_style.X:
                                     gfx.DrawLine(P1, X11, X12);
                                     gfx.DrawLine(P1, X21, X22);
                                     break;

                                 case eSeperator_style.WeekNr:
                                    if (_Step == steps.Week)
                                    {
                                        gfx.DrawLine(P1, new PointF(Marks[i].X, Marks[i].Y - (iLineSize / 2)), new PointF(Marks[i].X, Marks[i].Y + (iLineSize / 2f)));

                                        TimeSpan ts_mark = new TimeSpan(_interval.Ticks * i);
                                        DateTime dt_mark = _TimeStart + ts_mark;

                                        int WeekNr = getWeekNr(dt_mark.Date);

                                        string s_mark = WeekNr.ToString();
                                        Font fnt = new Font(base.Font.FontFamily, base.Font.Size * 0.77f); // base.Font;


                                        SizeF s = e.Graphics.MeasureString(s_mark, fnt);

                                        PointF p = Marks[i];

                                        p.X -= s.Width / 2;
                                        p.Y -= (s.Height + (iLineSize / 2f));

                                        if (i != 0 && false)
                                        {
                                            p.X -= s.Width / 2;

                                            if (i == Marks.Length - 1)
                                                p.X -= s.Width / 2;
                                        }

                                        gfx.DrawString(s_mark, fnt, new SolidBrush(_CSeperator), p);
                                    }
                                     break;

                                 case eSeperator_style.none:
                                     break;
                                 default:
                                     break;

                             }



                         }
                         //Text = Marks.Length.ToString();
                     }
                 }
             }
             catch { }
            #endregion

            #region Now
            if (showNow && DateTime.Now >= _TimeStart && DateTime.Now <= _TimeEnd)
            {
                P1 = new Pen(_CNow, 2.0f);
                float iLineSize = ClientRectangle.Height / 100f * 80f;//_LineSize;
                float y = ClientRectangle.Height / 2;

                long nowTicks = DateTime.Now.Ticks - _TimeStart.Ticks;
                float posX = CF_calc.Kurventransformation(nowTicks, 0, _TSdist.Ticks, 0, ClientRectangle.Width);

                gfx.DrawLine(P1, new PointF(posX, y - (iLineSize / 2)), new PointF(posX, y + (iLineSize / 2f)));
            }


            #endregion

            #region Text
            set_Text_sel();
             SizeF stringSize1 = new SizeF();
             Point PosText1 = new Point();
             SizeF stringSize2 = new SizeF();
             Point PosText2 = new Point();

             //Text 
             //Font fnt = new System.Drawing.Font("Arial", (float)(ClientRectangle.Height * 0.3));
             //StringFormat sf = new StringFormat();
             //sf.Alignment = StringAlignment.Center;
             //gfx.DrawString(Text, fnt, new SolidBrush(Color.Red), Slider1[0]);

             //Select 1
             // Measure string.           
             Font fnt_Sel = base.Font; //new System.Drawing.Font("Arial", (float)(ClientRectangle.Height * 0.3));
             stringSize1 = e.Graphics.MeasureString(STime_Sel_1, fnt_Sel);
             PosText1 = Slider1[0];
             PosText1.Y -= (int)(ClientRectangle.Height / 100f * _LineSize);
             PosText1.X -= (int)(stringSize1.Width / 2f);
             PosText1.Y -= (int)(stringSize1.Height / 2f);

             if (PosText1.X < 0)
                 PosText1.X = 0;
             if (PosText1.X > ClientRectangle.Width - stringSize1.Width)
                 PosText1.X = (int)(ClientRectangle.Width - stringSize1.Width);
             if (PosText1.X + stringSize1.Width / 2 > PosText2LC.X - stringSize2LC.Width / 2)
                 PosText1.X = (int)(PosText2LC.X - (stringSize2LC.Width / 2) - (stringSize1.Width / 2));

             //if (PosText1.X + stringSize1.Width / 2 > PosTextCenter.X - stringSizeTextCenter.Width / 2)
             //    PosText1.X = (int)(PosTextCenter.X - (stringSizeTextCenter.Width / 2) - (stringSize1.Width / 2));

             StringFormat sf_sel = new StringFormat();
             sf_sel.Alignment = StringAlignment.Center;

             if (!modSel1)
                 gfx.DrawString(STime_Sel_1, fnt_Sel, new SolidBrush(base.ForeColor), PosText1);
             if (modSel1)
             {
                 string DrawString = Modify;
                 if (blink)
                     DrawString = Modify + "_";
                 gfx.DrawString(DrawString, fnt_Sel, new SolidBrush(base.ForeColor), PosText1);
             }

             //Select 2
             // Measure string.           
             Font fnt_Sel_2 = base.Font; //new System.Drawing.Font("Arial", (float)(ClientRectangle.Height * 0.3));
             stringSize2 = e.Graphics.MeasureString(STime_Sel_2, fnt_Sel_2);
             PosText2 = Slider2[0];
             PosText2.Y -= (int)(ClientRectangle.Height / 100f * _LineSize);
             PosText2.X -= (int)(stringSize2.Width / 2f);
             PosText2.Y -= (int)(stringSize2.Height / 2f);

             if (PosText2.X < 0)
                 PosText2.X = 0;
             if (PosText2.X > ClientRectangle.Width - stringSize2.Width)
                 PosText2.X = (int)(ClientRectangle.Width - stringSize2.Width);
             if (PosText2.X - stringSize2.Width / 2 < PosText1.X + stringSize1.Width / 2)
                 PosText2.X = (int)(PosText1.X + (stringSize1.Width / 2) + (stringSize2.Width / 2));

             //if (PosText2.X - stringSize2.Width / 2 < PosTextCenter.X + stringSizeTextCenter.Width / 2)
             //    PosText2.X = (int)(PosTextCenter.X + (stringSizeTextCenter.Width / 2) + (stringSize2.Width / 2));

             PosText2LC = PosText2;
             stringSize2LC = stringSize2;

             StringFormat sf_sel_2 = new StringFormat();
             sf_sel_2.Alignment = StringAlignment.Center;

             if (!modSel2)
                 gfx.DrawString(STime_Sel_2, fnt_Sel_2, new SolidBrush(base.ForeColor), PosText2);
             if (modSel2)
             {
                 string DrawString = Modify;
                 if (blink)
                     DrawString = Modify + "_";
                 gfx.DrawString(DrawString, fnt_Sel_2, new SolidBrush(base.ForeColor), PosText2);
             }

            // Text Center
             Font fnt_Center = base.Font; //new System.Drawing.Font("Arial", (float)(ClientRectangle.Height * 0.3));
             StringFormat sf_Center = new StringFormat();
             sf_Center.Alignment = StringAlignment.Center;

            stringSizeTextCenter =  e.Graphics.MeasureString(Text, fnt_Center);

            PosTextCenter = new Point(_Slider1Pos + (int)(stringSize1.Width / 2f) + dist_S1_to_S2 / 2 - (int)(stringSizeTextCenter.Width / 2), (int)(ClientRectangle.Height - stringSizeTextCenter.Height)); //(int)((ClientRectangle.Height / 2) - (stringSizeTextCenter.Height / 2)));
            PosTextCenter.X -= (int)(stringSizeTextCenter.Width / 2f);

             if (PosTextCenter.X  < 0)
                 PosTextCenter.X = 0;
             if (PosTextCenter.X + (int)(stringSizeTextCenter.Width) > ClientRectangle.Width)
                 PosTextCenter.X = ClientRectangle.Width - (int)(stringSizeTextCenter.Width);

             gfx.DrawString(Text, fnt_Center, new SolidBrush(_CTextCenter), PosTextCenter);

             #endregion
             #region Debug
             //if (!nonNumberEntered)
             //if (Modify != null)
             //    gfx.DrawString(Modify + "  " + Modify.Length.ToString(), fnt_Center, new SolidBrush(Color.Red), new Point(0, 0));

             if (_debug)
             {

                 gfx.DrawString(_TimeStart.ToString(), fnt_Center, new SolidBrush(Color.Red), new Point(0, 0));
                 gfx.DrawString(_TimeEnd.ToString(), fnt_Center, new SolidBrush(Color.Red), new Point(ClientRectangle.Width / 2, 0));

              //   gfx.DrawString(Time1.ToString(), fnt_Center, new SolidBrush(Color.Red), new Point(0, 20));
               //  gfx.DrawString(Time2.ToString(), fnt_Center, new SolidBrush(Color.Red), new Point(ClientRectangle.Width / 2, 20));

                 gfx.DrawString(debug_string, fnt_Center, new SolidBrush(Color.Red), new Point(0, 20));

             }

             //gfx.DrawString("P2 X: " + Slider1[2].X.ToString() + " Y: " + Slider1[2].Y.ToString(), fnt, new SolidBrush(Color.Red), Slider1[2]);
             //gfx.DrawString("P3 X: " + Slider1[3].X.ToString() + " Y: " + Slider1[3].Y.ToString(), fnt, new SolidBrush(Color.Red), Slider1[3]);
             //gfx.DrawString("P4 X: " + Slider1[4].X.ToString() + " Y: " + Slider1[4].Y.ToString(), fnt, new SolidBrush(Color.Red), Slider1[4]);


             //gfx.DrawString(Text, fnt, new SolidBrush(Color.Red), new Point(0,0));
             //Font fnt2 = new System.Drawing.Font("Arial", (float)(ClientRectangle.Height * 0.05));
             //StringFormat sf2 = new StringFormat();
            //sf2.Alignment = StringAlignment.Center;
             #endregion
            #endregion
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }

        private void DrawHighlight(Graphics gfx, TimeRange tr)
        {
            RectangleF rc = ClientRectangle;

            if (tr.startTime >= _TimeStart && tr.endTime <= _TimeEnd)
            {
                long startTicks = tr.startTime.Ticks - _TimeStart.Ticks;
                long endTicks = tr.endTime.Ticks - _TimeStart.Ticks;

                float posXstart = CF_calc.Kurventransformation(startTicks, 0, _TSdist.Ticks, 0, ClientRectangle.Width);
                float posXend   = CF_calc.Kurventransformation(endTicks, 0, _TSdist.Ticks, 0, ClientRectangle.Width);
                float Xdist = posXend - posXstart;

                float h = rc.Height / 2.0f;
                float xh = h * tr.Height;

                if (Xdist > 0.0f)
                {
                    RectangleF rcc1 = new RectangleF(posXstart, h - xh, Xdist, xh);
                    RectangleF rcc2 = new RectangleF(posXstart, h, Xdist, xh);
                    rcc2.Y = rc.Height / 2.0f;

                    RectangleF rcc1br = rcc1;
                    RectangleF rcc2br = rcc2;

                    rcc1br.X -= 2;
                    rcc1br.Y -= 2;
                    rcc1br.Width += 4;
                    rcc1br.Height += 4;

                    Color alpha = Color.FromArgb(0, 0, 0, 0);

                    LinearGradientBrush brush = new LinearGradientBrush(rcc1br, alpha, tr.Color, LinearGradientMode.Vertical);
                    LinearGradientBrush brush2 = new LinearGradientBrush(rcc2br, tr.Color, alpha, LinearGradientMode.Vertical);

                    gfx.FillRectangle(brush, rcc1);
                    gfx.FillRectangle(brush2, rcc2);
                }
            }
        }

        #endregion


        #region TOOLSTRIP
        private void TSMI_1h_Click(object sender, EventArgs e)
        {
            Steps = steps.Hour1;
        }
        private void TSMI_30min_Click(object sender, EventArgs e)
        {
            Steps = steps.Minute30;
        }
        private void TSMI_15min_Click(object sender, EventArgs e)
        {
            Steps = steps.Minute15;
        }
        private void TSMI_10min_Click(object sender, EventArgs e)
        {
            Steps = steps.Minute10;
        }
        private void TSMI_5min_Click(object sender, EventArgs e)
        {
            Steps = steps.Minute5;
        }
        private void TSMI_1min_Click(object sender, EventArgs e)
        {
            Steps = steps.Minute1;
        }


        #endregion

    }

    public class TimeRange
    {
        private DateTime _startTime;
        private DateTime _endTime;
        private Color _color;
        private float _height;

        public DateTime startTime
        {
            get { return _startTime; }
            set
            {
                if (value != _startTime)
                {
                    _startTime = value;
                    if (value > _endTime)
                        _endTime = value;
                }
            }
        }
        public DateTime endTime
        {
            get { return _endTime; }
            set
            {
                if (value != _endTime)
                {
                    _endTime = value;
                    if (value < _startTime)
                        _startTime = value;
                }
            }
        }
        public Color Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                    _color = value;
            }
        }
        public float Height
        {
            get
            {
                if (_height > 1.0f)
                    _height = 1.0f;
                if (_height < 0.01f)
                    _height = 0.01f;

                return _height;
            }
            set
            {
                if (value != _height)
                {
                    _height = value;

                    if (_height > 1.0f)
                        _height = 1.0f;
                    if (_height < 0.01f)
                        _height = 0.01f;
                }
            }
        }

        public TimeSpan Span
        {
            get
            {
                return _endTime - _startTime;
            }
        }

        public TimeRange()
        {
            Height = 0.7f;
            Color = Color.Green;
        }
    }
}
