using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Globalization;

using Codeplex.Dashboarding;

namespace WpfDashBoard
{
    /// <summary>
    /// SevenSegmentLED.xaml
    /// </summary>
    public partial class SevenSegmentLED : PlatformIndependentDashboard
    {
        private bool hasInitialized;
        private readonly Dictionary<Leds, Path> leds = new Dictionary<Leds, Path>();
        internal enum Leds
        {
            TH,   //Top
            THL,  //Top Vertical Left
            THR,  //Top Vertical Right
            MH,   //Middle
            BHL,  //Bottom horizontal left
            BHR,  //Bottom horizontal Right
            BH,   //Bottom
            PO    //Point
        }
        private static Dictionary<string, List<Leds>> characterLeds = new Dictionary<string, List<Leds>>
        {
            { "0", new List<Leds> { Leds.TH, Leds.THL, Leds.THR,Leds.BHL,Leds.BHR,Leds.BH}},
            { "1", new List<Leds> { Leds.THR, Leds.BHR } },
            { "2", new List<Leds> { Leds.TH,Leds.THR,Leds.MH,Leds.BHL,Leds.BH} },
            { "3", new List<Leds> { Leds.TH,Leds.THR,Leds.MH,Leds.BHR,Leds.BH} },
            { "4", new List<Leds> { Leds.THL,Leds.THR,Leds.MH,Leds.BHR} },
            { "5", new List<Leds> { Leds.TH,Leds.THL,Leds.MH,Leds.BHR,Leds.BH} },
            { "6", new List<Leds> { Leds.TH,Leds.THL,Leds.MH,Leds.BH,Leds.BHL,Leds.BHR } },
            { "7", new List<Leds> { Leds.TH,Leds.THR,Leds.BHR } },
            { "8", new List<Leds> { Leds.TH,Leds.THL,Leds.THR,Leds.MH,Leds.BHL,Leds.BHR,Leds.BH } },
            { "9", new List<Leds> { Leds.TH,Leds.THL,Leds.THR,Leds.MH,Leds.BHR,Leds.BH } },
        };
        private void StoreLedInformation()
        {
            leds.Add(Leds.TH,  Path_6);
            leds.Add(Leds.THL, Path_1);
            leds.Add(Leds.THR, Path_5);
            leds.Add(Leds.MH,  Path_0);
            leds.Add(Leds.BHL, Path_2);
            leds.Add(Leds.BHR, Path_4);
            leds.Add(Leds.BH,  Path_3);
            leds.Add(Leds.PO,  Path_8);
        }

        /// <summary>
        /// 
        /// </summary>
        public SevenSegmentLED()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(SixteenSegmentLED_Loaded);

            LedOffColor = Color.FromArgb(0x50, 0x5e, 0x57, 0x57);
            LedOnColor = Color.FromArgb(0xFF, 0x00, 0x99, 0x00);
        }
        private void SixteenSegmentLED_Loaded(object sender, RoutedEventArgs e)
        {
            if (!hasInitialized)
            {
                hasInitialized = true;
                StoreLedInformation();
            }

            Animate();
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DisplayCharacterProperty =
            DependencyProperty.Register("DisplayCharacter", typeof(string), typeof(SevenSegmentLED), new PropertyMetadata(new PropertyChangedCallback(DisplayCharacterPropertyChanged)));
        private static void DisplayCharacterPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args) {
            if (!(dependancy is SevenSegmentLED instance)) return;
            instance.Animate();
        }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayCharacter
        {
            get => (string)GetValue(DisplayCharacterProperty);
            set => SetValue(DisplayCharacterProperty, value);
        }
        private static void ColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args) {
            if (!(dependancy is SevenSegmentLED instance)) return;
            instance.Animate();
        }
        #region decimal point 

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PointOnProperty =
            DependencyProperty.Register("PointOn", typeof(bool), typeof(SevenSegmentLED), new PropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));

        /// <summary>
        /// 
        /// </summary>
        public bool PointOn
        {
            get => (bool)GetValue(PointOnProperty);
            set => SetValue(PointOnProperty, value);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LedOffColorProperty =
            DependencyProperty.Register("LedOffColor", typeof(Color), typeof(SevenSegmentLED), new PropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));

        /// <summary>
        /// 
        /// </summary>
        public Color LedOffColor
        {
            get => (Color)GetValue(LedOffColorProperty);
            set => SetValue(LedOffColorProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LedOnColorProperty =
            DependencyProperty.Register("LedOnColor", typeof(Color), typeof(SevenSegmentLED), new PropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));

        /// <summary>
        /// 
        /// </summary>
        public Color LedOnColor
        {
            get => (Color)GetValue(LedOnColorProperty);
            set => SetValue(LedOnColorProperty, value);
        }

        private void Animate()
        {
            SetAllLedsOff();
            SetRequiresLedsOn();
        }

        private void SetAllLedsOff()
        {
            foreach (var path in leds.Values)
            {
                path.Fill = new SolidColorBrush(LedOffColor);
            }
        }

        private void SetRequiresLedsOn()
        {
            if (leds.Count == 0 || string.IsNullOrEmpty(DisplayCharacter) || DisplayCharacter == " ")
            {
                return;
            }

            if (DisplayCharacter.Length > 1)
            {
                ShowError();
            }

            if (characterLeds.ContainsKey(DisplayCharacter.ToUpper(CultureInfo.CurrentCulture)))
            {
                var l = characterLeds[DisplayCharacter.ToUpper(CultureInfo.CurrentCulture)];
                
                foreach (var led in l)
                {
                    leds[led].Fill = new SolidColorBrush(LedOnColor);
                }
            }
            //show decimal point
            if (PointOn == true)
            {
                leds[Leds.PO].Fill = new SolidColorBrush(LedOnColor);
            }
        }
        private void ShowError()
        {
            foreach (var path in leds.Values)
            {
                path.Fill = new SolidColorBrush(LedOnColor);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        protected override FrameworkElement ResourceRoot => IndicatorLEDs;
    }
}
