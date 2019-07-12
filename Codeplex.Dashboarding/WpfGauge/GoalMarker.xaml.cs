using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfGauge
{
    /// <summary>
    /// Interaction logic for GoalMarker.xaml
    /// </summary>
    public partial class GoalMarker : UserControl
    {
        internal Gauge ParentGauge { get; set; }

        #region Dependency Properties

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(GoalMarker), new PropertyMetadata(OnValueChanged));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(GoalMarker), new PropertyMetadata(0.0));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty GaugeMaxProperty =
            DependencyProperty.Register("GaugeMax", typeof(double), typeof(GoalMarker), new PropertyMetadata(0.0));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty GaugeStartAngleProperty =
            DependencyProperty.Register("GaugeStartAngle", typeof(double), typeof(GoalMarker), new PropertyMetadata(0.0));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty GaugeEndAngleProperty =
            DependencyProperty.Register("GaugeEndAngle", typeof(double), typeof(GoalMarker), new PropertyMetadata(0.0));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty MarkerWidthProperty =
            DependencyProperty.Register("MarkerWidth", typeof(double), typeof(GoalMarker), new PropertyMetadata(10.0d));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty MarkerHeightProperty =
            DependencyProperty.Register("MarkerHeight", typeof(double), typeof(GoalMarker), new PropertyMetadata(10.0d));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty MarkerColorProperty =
            DependencyProperty.Register("MarkerColor", typeof(Brush), typeof(GoalMarker), new PropertyMetadata(new SolidColorBrush(Colors.Orange)));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(GoalMarker), new PropertyMetadata(null));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var goal = d as GoalMarker;

            goal.ParentGauge.UpdateAngle();

            // Update the Angle Property
            var v = (double)e.NewValue;
            var valueInPercent = v / goal.GaugeMax;
            var valueInDegrees = valueInPercent * (goal.GaugeEndAngle - goal.GaugeStartAngle) + goal.GaugeStartAngle;
            goal.Angle = valueInDegrees;
        }

        #endregion Dependency Properties

        #region Public Vars

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
        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public double GaugeMax
        {
            get => (double)GetValue(GaugeMaxProperty);
            set => SetValue(GaugeMaxProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public double GaugeStartAngle
        {
            get => (double)GetValue(GaugeStartAngleProperty);
            set => SetValue(GaugeStartAngleProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public double GaugeEndAngle
        {
            get => (double)GetValue(GaugeEndAngleProperty);
            set => SetValue(GaugeEndAngleProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public double MarkerWidth
        {
            get => (double)GetValue(MarkerWidthProperty);
            set => SetValue(MarkerWidthProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public double MarkerHeight
        {
            get => (double)GetValue(MarkerHeightProperty);
            set => SetValue(MarkerHeightProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Brush MarkerColor
        {
            get => (Brush)GetValue(MarkerColorProperty);
            set => SetValue(MarkerColorProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion Public Vars

        /// <summary>
        /// 
        /// </summary>
        public GoalMarker()
        {
            InitializeComponent();
        }
    }
}
