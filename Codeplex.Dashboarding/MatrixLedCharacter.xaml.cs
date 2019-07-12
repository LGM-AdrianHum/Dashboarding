//-----------------------------------------------------------------------
// <copyright file="MatrixLedCharacter.xaml.cs" company="David Black">
//      Copyright 2008 David Black
//
//      Licensed under the Apache License, Version 2.0 (the "License");
//      you may not use this file except in compliance with the License.
//      You may obtain a copy of the License at
//    
//          http://www.apache.org/licenses/LICENSE-2.0
//    
//      Unless required by applicable law or agreed to in writing, software
//      distributed under the License is distributed on an "AS IS" BASIS,
//      WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//      See the License for the specific language governing permissions and
//      limitations under the License.
// </copyright>
//-----------------------------------------------------------------------
namespace Codeplex.Dashboarding
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// <para>
    /// In marquee mode on request the character scrolls one led left, and raises
    /// the scroll out event (passing the coloumn that has left it). Down stream characters can 
    /// subscribe to this event and scroll themseleves. The data passed from the led before is the placed in column 5.
    /// This behavious causes a cascade scroll
    /// </para>
    /// <para>Note this control only scrolls content given to it. It is up to the controller to passint
    /// single columns that in total constitute a character in a language (indeed the
    /// columns cou;ld represent an extremely low resolution picture!
    /// </para>
    /// </summary>
    public partial class MatrixLedCharacter : PlatformIndependentDashboard
    {
        #region Fields

        /// <summary>
        /// The dependancy property for the LedOffColor
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty LedOffColorProperty = 
            DependencyProperty.Register("LedOffColor", typeof(Color), typeof(MatrixLedCharacter), new PropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));

        /// <summary>
        /// The dependancy property for the LedOn colr
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public static readonly DependencyProperty LedOnColorProperty = 
            DependencyProperty.Register("LedOnColor", typeof(Color), typeof(MatrixLedCharacter), new PropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));

        /// <summary>
        /// The dependancy property for the Text
        /// </summary>
        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register("Text", typeof(string), typeof(MatrixLedCharacter), new PropertyMetadata(new PropertyChangedCallback(TextPropertyChanged)));

        /// <summary>
        /// stores a list of columns (5 of them) which are in turn lists of row states
        /// </summary>
        private List<List<bool>> columns = new List<List<bool>>(5);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixLedCharacter"/> class.
        /// </summary>
        public MatrixLedCharacter()
        {
            InitializeComponent();
            Clear();

            LedOffColor = Color.FromArgb(0x22, 0xFF, 0x00, 0x00);
            LedOnColor = Color.FromArgb(0xFF, 0xFF, 0x00, 0x00);
            Loaded += new RoutedEventHandler(MatrixLedCharacter_Loaded);
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// This cell is scrolling out a column of data
        /// </summary>
        internal event EventHandler<MatrixScrollEventArgs> ScrollOut;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
        public bool DashboardLoaded
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the color of the when led off.
        /// </summary>
        /// <value>The color of the led off.</value>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public Color LedOffColor
        {
            get => (Color)GetValue(LedOffColorProperty);

            set => SetValue(LedOffColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the color of the led when on.
        /// </summary>
        /// <value>The color of the led on.</value>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        public Color LedOnColor
        {
            get => (Color)GetValue(LedOnColorProperty);

            set => SetValue(LedOnColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the text (single character) of the control.Setting the text initializes
        /// all leds.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);

            set => SetValue(TextProperty, value);
        }

        #endregion Properties
        /// <summary>
        /// Gets the resource root. This allow us to access the Storyboards in a Silverlight/WPf
        /// neutral manner
        /// </summary>
        /// <value>The resource root.</value>
        protected override FrameworkElement ResourceRoot => LayoutRoot;

        /// <summary>
        /// Gets or sets the led on brush, cached for efficiency (Cheers Inferno).
        /// </summary>
        /// <value>The led on brush.</value>
        private SolidColorBrush LedOnBrush { get; set; }

        /// <summary>
        /// Gets or sets the led off brush, cached for efficiency (Cheers Inferno).
        /// </summary>
        /// <value>The led on brush.</value>
        private SolidColorBrush LedOffBrush { get; set; }

        #region Methods

        /// <summary>
        /// Forces this cell to scroll one column and passes the next column. This will cascade to any connected
        /// cells. Usually on a marquee style control this means only the right most
        /// character gets ticked manually the rest do through chained events
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Codeplex.Dashboarding.MatrixScrollEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "I need this to chain the leds")]
        public void ScrollOne(object sender, MatrixScrollEventArgs args)
        {
            var toPassOver = columns[0];
            columns.RemoveAt(0);
            columns.Add(args.Column);
            OnScrollOut(toPassOver);
            UpdateLedsFromState();
        }

        /// <summary>
        /// Clear the control down to all leds are off
        /// </summary>
        internal void Clear()
        {
            columns.Clear();
            for (var i = 0; i < 5; i++)
            {
                columns.Add(new List<bool> { false, false, false, false, false, false, false });
            }

            UpdateLedsFromState();
        }

        /// <summary>
        /// The color property has changed, deal with it
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ColorPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            var instance = dependancy as MatrixLedCharacter;
            instance.LedColorChanged();

            if (instance != null && instance.DashboardLoaded)
            {
                if (instance.LedOnColor != null)
                {
                    instance.UpdateLedsFromState();
                }
            }
        }

        /// <summary>
        /// The text property changed, deal with it
        /// </summary>
        /// <param name="dependancy">The dependancy.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void TextPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            if (dependancy is MatrixLedCharacter instance)
            {
                if (instance.Text != null && instance.DashboardLoaded)
                {
                    instance.UpdateLedsFromCharacter();
                }
            }
        }

        /// <summary>
        /// Manifests the changes to the Character.
        /// </summary>
        private void ManifestChanges()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                UpdateLedsFromCharacter();
            }
        }

        /// <summary>
        /// Handles the Loaded event of the MatrixLedCharacter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MatrixLedCharacter_Loaded(object sender, RoutedEventArgs e)
        {
            ManifestChanges();
            DashboardLoaded = true;
        }

        /// <summary>
        /// Called when a scroll out is happening.
        /// </summary>
        /// <param name="toPassOver">To pass over.</param>
        private void OnScrollOut(List<bool> toPassOver)
        {
            if (ScrollOut != null)
            {
                ScrollOut(this, new MatrixScrollEventArgs(toPassOver));
            }
        }

        /// <summary>
        /// Switches on or off leds fr a single column
        /// </summary>
        /// <param name="x">the column number</param>
        private void ProcessColumn(int x)
        {
            for (var y = 0; y < columns[x].Count; y++)
            {
                var on = columns[x][y];
                var el = LayoutRoot.FindName(string.Format("_l{0}_{1}", x, y)) as Ellipse;
                TureLedOnOrOff(on, el);
            }
        }

        /// <summary>
        /// Sets the led color for a single led
        /// </summary>
        /// <param name="on">true if the led is on false otherwise</param>
        /// <param name="el">The ellipse repesesenting the led</param>
        private void TureLedOnOrOff(bool on, Ellipse el)
        {
            if (el != null)
            {
                el.Fill = on ? LedOnBrush : LedOffBrush;
            }
        }

        /// <summary>
        /// Initialize all columns from a definition in the Character defintion
        /// </summary>
        private void UpdateLedsFromCharacter()
        {
            var bytes = MatrixLedCharacterDefinitions.GetDefinition(Text);
            columns.Clear();
            for (var i = 0; i < bytes.Length - 1; i++)
            {
                var n = new List<bool>
                {
                    (bytes[i] & 0x40) != 0,
                    (bytes[i] & 0x20) != 0,
                    (bytes[i] & 0x10) != 0,
                    (bytes[i] & 0x08) != 0,
                    (bytes[i] & 0x04) != 0,
                    (bytes[i] & 0x02) != 0,
                    (bytes[i] & 0x01) != 0,
                };
                columns.Add(n);
            }

            UpdateLedsFromState();
        }

        /// <summary>
        /// One of the LED colors changed update the cached brushes
        /// </summary>
        private void LedColorChanged()
        {
            if (LedOnBrush == null || (LedOnBrush != null && LedOnBrush.Color != LedOnColor))
            {
                LedOnBrush = new SolidColorBrush(LedOnColor);
                Freeze(LedOnBrush);
            }

            if (LedOffBrush == null || (LedOffBrush != null && LedOffBrush.Color != LedOffColor))
            {
                LedOffBrush = new SolidColorBrush(LedOffColor);
                Freeze(LedOffBrush);
            }
        }

        /// <summary>
        /// Set the leds on or off acording to the buffer state
        /// </summary>
        private void UpdateLedsFromState()
        {
            for (var x = 0; x < columns.Count; x++)
            {
                ProcessColumn(x);
            }
        }

        #endregion Methods
    }
}