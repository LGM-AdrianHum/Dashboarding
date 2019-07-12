using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

namespace WpfGauge
{
    /// <summary>
    /// Interaction logic for TickMarker.xaml
    /// </summary>
    public partial class TickMarker : UserControl, INotifyPropertyChanged
    {
        #region Dependency Properties

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty TickLabelStyleProperty =
            DependencyProperty.Register("TickLabelStyle", typeof(Style), typeof(TickMarker),
            new PropertyMetadata(Defaults.DefaultTickLabelStyle, OnTickLabelStyleChanged));

        private static void OnTickLabelStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var t = d as TickMarker;
            var o = t.TickLabelStyle;
        }

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty TickMarkColorProperty =
            DependencyProperty.Register("TickMarkColor", typeof(Brush), typeof(TickMarker),
            new PropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty LineMarginProperty =
            DependencyProperty.Register("LineMargin", typeof(Thickness), typeof(TickMarker),
            new PropertyMetadata(new Thickness(0,0,0,0)));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty LabelMarginProperty =
            DependencyProperty.Register("LabelMargin", typeof(Thickness), typeof(TickMarker),
            new PropertyMetadata(new Thickness(0, 20, 0, 0)));

        #endregion Dependency Properties

        #region Private Vars

        private double _labelAngle;
        private string _label;
        private double _angle;
        private double _strokeThickness;
        private double _lineHeight;

        #endregion Private Vars

        #region Public Vars

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
        public Style TickLabelStyle
        {
            get => (Style)GetValue(TickLabelStyleProperty);
            set => SetValue(TickLabelStyleProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Brush TickMarkColor
        {
            get => (Brush)GetValue(TickMarkColorProperty);
            set => SetValue(TickMarkColorProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public double LabelAngle
        {
            get => _labelAngle;
            set
            {
                _labelAngle = value;
                OnPropertyChanged("LabelAngle");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Label
        {
            get => _label;
            set
            {
                _label = value;
                OnPropertyChanged("Label");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Angle
        {
            get => _angle;
            set
            {
                _angle = value;
                OnPropertyChanged("Angle");
                LabelAngle = value - 90;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double StrokeThickness
        {
            get => _strokeThickness;
            set
            {
                _strokeThickness = value;
                OnPropertyChanged("StrokeThickness");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double LineHeight
        {
            get => _lineHeight;
            set
            {
                _lineHeight = value;
                OnPropertyChanged("LineHeight");
                LineMargin = new Thickness(55 - LineHeight, 0, 0, 0);
            }
        }

        #endregion Public Vars

        /// <summary>
        /// 
        /// </summary>
        public TickMarker()
        {
            InitializeComponent();
        }

        #region INotifyPropertyChanged

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged

    }
}
