using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace WpfGauge
{
    /// <summary>
    /// Interaction logic for Gauge.xaml
    /// </summary>
    public partial class Gauge : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty TickLabelStyleProperty =
            DependencyProperty.Register("TickLabelStyle", typeof(Style), typeof(Gauge), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty AutoScaleProperty =
            DependencyProperty.Register("AutoScale", typeof(bool), typeof(Gauge), new PropertyMetadata(true));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty LineMarginProperty =
            DependencyProperty.Register("LineMargin", typeof(Thickness), typeof(Gauge),
            new PropertyMetadata(new Thickness(40,0,0,0)));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty LabelMarginProperty =
            DependencyProperty.Register("LabelMargin", typeof(Thickness), typeof(Gauge),
            new PropertyMetadata(new Thickness(0, 20, 0, 0)));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty MajorTickMarkColorProperty =
            DependencyProperty.Register("MajorTickMarkColor", typeof(Brush), typeof(Gauge),
            new PropertyMetadata(new SolidColorBrush(Colors.White), OnMajorTickMarkColorChanged));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty MinorTickMarkColorProperty =
            DependencyProperty.Register("MinorTickMarkColor", typeof(Brush), typeof(Gauge),
            new PropertyMetadata(new SolidColorBrush(Colors.White), OnMinorTickMarkColorChanged));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty InnerEllipseStrokeProperty =
            DependencyProperty.Register("InnerEllipseStroke", typeof(Brush), typeof(Gauge),
            new PropertyMetadata(OnInnerEllipseStrokeChanged));

        private static void OnInnerEllipseStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = d as Gauge;
            if (e.NewValue == null)
                gauge.InnerEllipse.Stroke = new SolidColorBrush(Color.FromArgb(255, 105, 105, 105));
            else
                gauge.InnerEllipse.Stroke = (Brush)e.NewValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty OuterEllipseStrokeProperty =
            DependencyProperty.Register("OuterEllipseStroke", typeof(Brush), typeof(Gauge),
            new PropertyMetadata(OnOuterEllipseStrokeChanged));

        private static void OnOuterEllipseStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = d as Gauge;
            if (e.NewValue == null)
                gauge.OuterEllipse.Stroke = new SolidColorBrush(Color.FromArgb(255, 105, 105, 105));
            else
                gauge.OuterEllipse.Stroke = (Brush) e.NewValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty InnerEllipseFillProperty =
            DependencyProperty.Register("InnerEllipseFill", typeof(Brush), typeof(Gauge),
            new PropertyMetadata(OnInnerEllipseFillChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnInnerEllipseFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = d as Gauge;
            if (e.NewValue == null)
                gauge.InnerEllipse.Fill = new SolidColorBrush(Colors.Black);
            else
                gauge.InnerEllipse.Fill = (Brush)e.NewValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty OuterEllipseFillProperty =
            DependencyProperty.Register("OuterEllipseFill", typeof(Brush), typeof(Gauge),
            new PropertyMetadata(OnOuterEllipseFillChanged));

        private static void OnOuterEllipseFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = d as Gauge;
            if (e.NewValue == null)
                gauge.OuterEllipse.Fill = Defaults.GradientBrushes.GaugeOuterEllipseBackground;
            else
                gauge.OuterEllipse.Fill = (Brush)e.NewValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(Gauge),
            new PropertyMetadata(double.NegativeInfinity, OnGaugeChanged));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty ValueTextStyleProperty =
            DependencyProperty.Register("ValueTextStyle", typeof(Style), typeof(Gauge), 
            new PropertyMetadata(OnValueTextStyleChanged));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(Gauge), new PropertyMetadata(double.NegativeInfinity));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(Gauge), new PropertyMetadata(0.0d));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty NeedleTooltipProperty =
            DependencyProperty.Register("NeedleTooltip", typeof(object), typeof(Gauge), 
            new PropertyMetadata(null, OnNeedleTooltipChanged));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty IsNeedleTooltipEnabledProperty =
            DependencyProperty.Register("IsNeedleTooltipEnabled", typeof(bool), typeof(Gauge), new PropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty LightVisibilityProperty =
            DependencyProperty.Register("LightVisibility", typeof(Visibility), typeof(Gauge), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty ControlGridProperty =
            DependencyProperty.Register("ControlGrid", typeof(Grid), typeof(Gauge), new PropertyMetadata(OnControlsGridChanged));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty ValueFormatProperty =
            DependencyProperty.Register("ValueFormat", typeof(string), typeof(Gauge), new PropertyMetadata(OnValueFormatChanged));
        
        /// <summary>
        /// 
        /// </summary>
        internal static DependencyProperty ValueTextMarginProperty =
            DependencyProperty.Register("ValueTextMargin", typeof(Thickness), typeof(Gauge), 
            new PropertyMetadata(new Thickness(0,100,0,0)));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty MajorTickMarksProperty =
            DependencyProperty.Register("MajorTickMarks", typeof(ObservableCollection<TickMarker>), typeof(Gauge), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty MinorTickMarksProperty =
            DependencyProperty.Register("MinorTickMarks", typeof(ObservableCollection<TickMarker>), typeof(Gauge), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(Gauge), new PropertyMetadata(-45.0, OnGaugeChanged));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(Gauge), new PropertyMetadata(225.0, OnGaugeChanged));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty MajorTickCountProperty =
            DependencyProperty.Register("MajorTickCount", typeof(int), typeof(Gauge), new PropertyMetadata(10, OnGaugeChanged));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty MinorTickCountProperty =
            DependencyProperty.Register("MinorTickCount", typeof(int), typeof(Gauge), new PropertyMetadata(4, OnGaugeChanged));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty GoalMarkersProperty =
            DependencyProperty.Register("GoalMarkers", typeof(ObservableCollection<GoalMarker>), typeof(Gauge), 
            new PropertyMetadata(new ObservableCollection<GoalMarker>()));

        private static void OnGaugeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = d as Gauge;
            gauge.UpdateAngle();
            if (!string.IsNullOrWhiteSpace(gauge.ValueFormat))
                gauge.ValueText.Text = string.Format(gauge.ValueFormat.Replace("{}",""), gauge.Value);
            else
                gauge.ValueText.Text = gauge.Value.ToString();
        }

        private static void OnNeedleTooltipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = d as Gauge;
            if (e.NewValue == null)
                gauge.IsNeedleTooltipEnabled = false;
            else
                gauge.IsNeedleTooltipEnabled = true;
        }

        private static void OnValueFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = d as Gauge;
            gauge.UpdateAngle();
        }

        private static void OnValueTextStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = d as Gauge;
            if (e.NewValue == null)
                gauge.SetValueTextDefaultStyle();
            else
                gauge.ValueText.Style = gauge.ValueTextStyle;
        }

        private static void OnMajorTickMarkColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = d as Gauge;
            if (gauge.MajorTickMarks != null)
            {
                // Set all Major Tick Fill Properties
                foreach (var tick in gauge.MajorTickMarks)
                    tick.TickMarkColor = gauge.MajorTickMarkColor;
            }
        }

        private static void OnMinorTickMarkColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = d as Gauge;
            if (gauge.MinorTickMarks != null)
            {
                // Set all Major Tick Fill Properties
                foreach (var tick in gauge.MinorTickMarks)
                    tick.TickMarkColor = gauge.MajorTickMarkColor;
            }
        }

        private static void OnControlsGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = d as Gauge;
            gauge.LayoutRoot.Children.Add((Grid)e.NewValue);
        }

        #endregion Dependency Properties

        #region Public Vars

        /// <summary>
        /// 
        /// </summary>
        public bool AutoScale
        {
            get => (bool)GetValue(AutoScaleProperty);
            set => SetValue(AutoScaleProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Style TickLabelStyle
        {
            get => (Style)GetValue(TickLabelStyleProperty);
            set => SetValue(TickLabelStyleProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Thickness LineMargin
        {
            get => (Thickness)GetValue(LineMarginProperty);
            set => SetValue(LineMarginProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Thickness LabelMargin
        {
            get => (Thickness)GetValue(LabelMarginProperty);
            set => SetValue(LabelMarginProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Brush InnerEllipseStroke
        {
            get => (Brush)GetValue(InnerEllipseStrokeProperty);
            set => SetValue(InnerEllipseStrokeProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Brush MajorTickMarkColor
        {
            get => (Brush)GetValue(MajorTickMarkColorProperty);
            set => SetValue(MajorTickMarkColorProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Brush MinorTickMarkColor
        {
            get => (Brush)GetValue(MinorTickMarkColorProperty);
            set => SetValue(MinorTickMarkColorProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Brush OuterEllipseStroke
        {
            get => (Brush)GetValue(OuterEllipseStrokeProperty);
            set => SetValue(OuterEllipseStrokeProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Brush InnerEllipseFill
        {
            get => (Brush)GetValue(InnerEllipseFillProperty);
            set => SetValue(InnerEllipseFillProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Brush OuterEllipseFill
        {
            get => (Brush)GetValue(OuterEllipseFillProperty);
            set => SetValue(OuterEllipseFillProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Style ValueTextStyle
        {
            get => (Style)GetValue(ValueTextStyleProperty);
            set => SetValue(ValueTextStyleProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public double MinValue
        {
            get => (double)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Thickness ValueTextMargin
        {
            get => (Thickness)GetValue(ValueTextMarginProperty);
            set => SetValue(ValueTextMarginProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public object NeedleTooltip
        {
            get => (object)GetValue(NeedleTooltipProperty);
            set => SetValue(NeedleTooltipProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNeedleTooltipEnabled
        {
            get => (bool)GetValue(IsNeedleTooltipEnabledProperty);
            set => SetValue(IsNeedleTooltipEnabledProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Visibility LightVisibility
        {
            get => (Visibility)GetValue(LightVisibilityProperty);
            set => SetValue(LightVisibilityProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Grid ControlGrid
        {
            get => (Grid)GetValue(ControlGridProperty);
            set => SetValue(ControlGridProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public string ValueFormat
        {
            get => (string)GetValue(ValueFormatProperty);
            set => SetValue(ValueFormatProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<TickMarker> MajorTickMarks
        {
            get => (ObservableCollection<TickMarker>)GetValue(MajorTickMarksProperty);
            set => SetValue(MajorTickMarksProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<TickMarker> MinorTickMarks
        {
            get => (ObservableCollection<TickMarker>)GetValue(MinorTickMarksProperty);
            set => SetValue(MinorTickMarksProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public int MajorTickCount
        {
            get => (int)GetValue(MajorTickCountProperty);
            set => SetValue(MajorTickCountProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public int MinorTickCount
        {
            get => (int)GetValue(MinorTickCountProperty);
            set => SetValue(MinorTickCountProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<GoalMarker> GoalMarkers
        {
            get => (ObservableCollection<GoalMarker>)GetValue(GoalMarkersProperty);
            set => SetValue(GoalMarkersProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public double StartAngle
        {
            get => (double)GetValue(StartAngleProperty);
            set => SetValue(StartAngleProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public double EndAngle
        {
            get => (double)GetValue(EndAngleProperty);
            set => SetValue(EndAngleProperty, value);
        }

        #endregion Public Vars

        /// <summary>
        /// 
        /// </summary>
        public Gauge()
        {
            // Setup events
            SizeChanged += (s, e) => { GenerateNeedlePoints(); };

            InitializeComponent();

            // Add events
            Loaded += new RoutedEventHandler(Gauge_Loaded);
            if (GoalMarkers != null)
                GoalMarkers.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(GoalMarkers_CollectionChanged);
        }

        private void Gauge_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateAngle();

            if (ValueTextStyle == null)
                SetValueTextDefaultStyle();
        }

        private void GenerateNeedlePoints()
        {
            var h = Height;
            var w = Width;
            var a = (3.5d / 300d) * h;
            var b = (90d / 300d) * w;
            var c = (1.75d / 300d) * h;
            var d = (95d / 300d) * w;

            var points = new PointCollection();
            points.Add(new Point(a, a));
            points.Add(new Point(a, -a));
            points.Add(new Point(-b, -c));
            points.Add(new Point(-d, 0));
            points.Add(new Point(-b, c));
            Needle.Points = points;
        }

        private void GoalMarkers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                foreach (GoalMarker gm in e.NewItems)
                {
                    gm.ParentGauge = this;
                    gm.GaugeMax = MaxValue;
                    gm.GaugeStartAngle = StartAngle;
                    gm.GaugeEndAngle = EndAngle;
                    if (gm.Parent is Grid)
                        ((Grid)gm.Parent).Children.Remove(gm);
                    GoalMarkerLayout.Children.Add(gm);
                }
        }

        internal void DrawElements()
        {
            // TODO: Improve this...
            // First, clear any children that may exist in the grid
            DynamicLayout.Children.Clear();

            // There are no specified Major Tick Markers so auto-create them
            // The default number of  Major Tick Markers is 11 (0 - 10)
            // The total degrees of the scale
            var totalDegrees = EndAngle - StartAngle;
            var degreeIncrement = totalDegrees / (MajorTickCount);
            var labelIncrement = (MaxValue - MinValue) / (MajorTickCount);
            var smallDegreeIncrement = degreeIncrement / (MinorTickCount + 1);

            MajorTickMarks = new ObservableCollection<TickMarker>();
            
            for (var i = 0; i <= MajorTickCount; i++)
            {
                var majorTick = new TickMarker();

                majorTick.StrokeThickness = 8;
                majorTick.LineHeight = 10;
                Panel.SetZIndex(majorTick, 1);
                if (AutoScale)
                {
                    majorTick.Angle = (i * degreeIncrement) + StartAngle;
                    majorTick.Label = (i * labelIncrement).ToString();
                }
                else
                {
                    majorTick.Angle = (i * degreeIncrement) + StartAngle;
                    majorTick.Label = ((i * labelIncrement) + MinValue).ToString();
                }
                MajorTickMarks.Add(majorTick);
                DynamicLayout.Children.Add(majorTick);

                majorTick.TickMarkColor = MajorTickMarkColor;
                majorTick.LineMargin = LineMargin;
                majorTick.LabelMargin = LabelMargin;

                if (TickLabelStyle != null)
                {
                    majorTick.TickLabelStyle = TickLabelStyle;
                }
            }

            MinorTickMarks = new ObservableCollection<TickMarker>();

            foreach (var majorTick in MajorTickMarks)
            {
                if (MajorTickMarks.IndexOf(majorTick) < MajorTickMarks.Count - 1)
                {
                    for (var i = 0; i <= MinorTickCount; i++)
                    {
                        var minorTick = new TickMarker();
                        minorTick.StrokeThickness = 4;
                        minorTick.LineHeight = 5;
                        Panel.SetZIndex(minorTick, 0);
                        minorTick.Angle = (i * smallDegreeIncrement) + smallDegreeIncrement + majorTick.Angle;
                        MinorTickMarks.Add(minorTick);
                        DynamicLayout.Children.Add(minorTick);

                        minorTick.TickMarkColor = MinorTickMarkColor;
                        minorTick.LineMargin = LineMargin;
                        minorTick.LabelMargin = LabelMargin;
                    }
                }
            }
        }

        internal void UpdateAngle()
        {
            if (Value == double.NegativeInfinity)
            {
                NeedleRotateTransform.Angle = StartAngle - 10;
                NeedleDropShadowEffect.Direction = StartAngle - 10;
                DrawElements();
                return;
            }
            Needle.Visibility = Visibility.Visible;

            if (AutoScale)
            {
                var v = GetGreatestValue();
                MaxValue = v.GetGaugeTop();
                MinValue = 0;
            }

            var valueInPercent = Value / MaxValue;
            if (AutoScale)
                valueInPercent = Value / MaxValue;
            else
                valueInPercent = (Value - MinValue) / (MaxValue - MinValue);

            var valueInDegrees = valueInPercent * (EndAngle - StartAngle) + StartAngle;

            NeedleRotateTransform.Angle = valueInDegrees;
            NeedleDropShadowEffect.Direction = valueInDegrees;

            DrawElements();

            // Set Binding on Value
            /*Binding value = new Binding("Value");
            value.ElementName = "GaugeControl";
            
            value.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            value.StringFormat = this.ValueFormat;
            ValueText.SetBinding(TextBlock.TextProperty, value);

            var b = ValueText.GetBindingExpression(TextBlock.TextProperty);
            if (b != null)
                b.UpdateTarget();*/
        }

        // Get greatest value
        // compare all gauge markers and the gauge value to determine the greatest number
        private double GetGreatestValue()
        {
            if (GoalMarkers != null && GoalMarkers.Count > 0)
            {
                var greatest = Value;
                foreach (var goal in GoalMarkers.ToList())
                {
                    if (goal.Value > greatest)
                        greatest = goal.Value;
                }
                return greatest;
            }
            else
                return Value;
        }

        internal void SetValueTextDefaultStyle()
        {
            ValueText.Style = Defaults.DefaultValueTextStyle;
            // Set up the default margin binding
            var b = new Binding("Height");
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            var relSource = new RelativeSource();
            relSource.Mode = RelativeSourceMode.FindAncestor;
            relSource.AncestorType = typeof(Gauge);
            b.RelativeSource = relSource;
            b.Converter = new Converters.ValueTextMarginConverter();
            ValueText.SetBinding(MarginProperty, b);
        }
    }
}
