//-----------------------------------------------------------------------
// <copyright file="OdometerDigit.xaml.cs" company="David Black">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// A OdometerDigit represents a Single digit displayed in an Odometer
    /// control. 
    /// <para>The class has Increment and Decrement operators and raises
    /// events when ever it rolls over from 9 to 0 when incrementing and 0 to 9 when decrementing</para>
    /// </summary>
    public partial class OdometerDigit : PlatformIndependentDashboard
    {
        #region Fields

        /// <summary>
        /// Value of the digit
        /// </summary>
        private int digit;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OdometerDigit"/> class.
        /// </summary>
        public OdometerDigit()
        {
            InitializeComponent();
            LayoutRoot.Clip = new RectangleGeometry { Rect = new Rect { X = 0, Y = 0, Width = 40, Height = 50 } };
            AnimateIndicatorStoryboard.Completed += new EventHandler(_swipe_Completed);
            Loaded += new RoutedEventHandler(OdometerDigit_Loaded);
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// This digit is just about to roll under from 0 to 9, this event
        /// allows the next least significant digit to decrement by one at
        /// the same time and gives that nice Odometer behaviour
        /// </summary>
        public event EventHandler<EventArgs> DecadeMinus;

        /// <summary>
        /// This digit is just about to roll over from 9 to 0, this event
        /// allows the next most significant digit to increment by one at
        /// the same time and gives that nice Odometer behaviour
        /// </summary>
        public event EventHandler<EventArgs> DecadePlus;

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
        /// Gets the current value of the digit 0..9
        /// </summary>
        public int Digit => digit;

        /// <summary>
        /// Gets the resource root. This allow us to access the Storyboards in a Silverlight/WPf
        /// neutral manner
        /// </summary>
        /// <value>The resource root.</value>
        protected override FrameworkElement ResourceRoot => LayoutRoot;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Slides the image down by 32 to show the value decrementing.On amination complete
        /// calls MoveToDigit to ensure the digit is nailed. if the new digit
        /// is 9 we raise the DecadeMinusEvent a second OdometerDigit may listen for this event and
        /// decrment itself as a result. When decrementing from 1000 to 999 this will be 'fun'
        /// </summary>
        public void Decrement()
        {
            digit -= 1;
            if (digit < 0)
            {
                digit = 9;
                OnDecadeMinus();
            }

            var from = (double)_image.GetValue(Canvas.TopProperty);
            var to = from + 32;

            GetChildDoubleAnimation(AnimateIndicatorStoryboard, "_anim").To = to;
            GetChildDoubleAnimation(AnimateIndicatorStoryboard, "_anim").From = from;

            Start(AnimateIndicatorStoryboard);
        }

        /// <summary>
        /// Slides the image up by 32 to show the value incrementing. On amination complete
        /// calls MoveToDigit to ensure the digit is nailed. if the new digit
        /// is 0 we raise the DecadePlusEvent a second OdometerDigit may listen for this event and
        /// increment itself as a result. When incrementing from 999 to 1000 this can cascade quite
        /// some way
        /// </summary>
        public void Increment()
        {
            digit += 1;
            if (digit > 9)
            {
                digit = 0;
                OnDecadePlus();
            }

            var from = (double)_image.GetValue(Canvas.TopProperty);

             // move to the lower 0 in the animation it has the roll over ability
            if (digit == 0)
            {
                double offset = 23;
                double amountToScroll = 9 * 32;
                _image.SetValue(Canvas.TopProperty, -(offset + amountToScroll));
                from = -(offset + amountToScroll);

                var sb = GetStoryboard("_swipe");
                Start(sb);
            }

            var to = from - 32;
            GetChildDoubleAnimation(AnimateIndicatorStoryboard, "_anim").To = to;
            GetChildDoubleAnimation(AnimateIndicatorStoryboard, "_anim").From = from;

            Start(AnimateIndicatorStoryboard);
        }

        /// <summary>
        /// When chained together in an odeometer the container connects the digits togeter
        /// so when a lower order digit rolls under to 9 the upper digit (us) can decrement.
        /// </summary>
        /// <param name="sender">the lower digit</param>
        /// <param name="args">Empty args</param>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "We need this to chain digits")]
        public void LowerOrderDigitDecadeMinus(object sender, EventArgs args)
        {
            Decrement();
        }

        /// <summary>
        /// When chained together in an odeometer the container connects the digits togeter
        /// so when a lower order digit rolls over to 0 the upper digit (us) can increment.
        /// </summary>
        /// <param name="sender">the lower digit</param>
        /// <param name="args">Empty args</param>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "We need this to chain digits")]
        public void LowerOrderDigitDecadePlus(object sender, EventArgs args)
        {
            Increment();
        }

        /// <summary>
        /// Called from the odometer control to set the initial values
        /// </summary>
        /// <param name="value">The value of the digit</param>
        internal void SetInitialValue(int value)
        {
            digit = value;
            MoveToDigit();
        }

        /// <summary>
        /// Manifests the changes.
        /// </summary>
        private void ManifestChanges()
        {
        }

        /// <summary>
        /// Place the image so the specified image is revealed
        /// </summary>
        private void MoveToDigit()
        {
            double offset = 23;
            double amountToScroll = digit * 32;
            _image.SetValue(Canvas.TopProperty, -(offset + amountToScroll));
        }

        /// <summary>
        /// Handles the Loaded event of the OdometerDigit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OdometerDigit_Loaded(object sender, RoutedEventArgs e)
        {
            ManifestChanges();
            DashboardLoaded = true;
        }

        /// <summary>
        /// we are rolling over from 9 to 0
        /// </summary>
        private void OnDecadeMinus()
        {
            if (DecadeMinus != null)
            {
                DecadeMinus(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// We are rolling down from 0 to 9 inform upper digit to decrement
        /// </summary>
        private void OnDecadePlus()
        {
            if (DecadePlus != null)
            {
                DecadePlus(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the Completed event of the _swipe control.  Moves the image to the correct cannonical location,
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void _swipe_Completed(object sender, EventArgs e)
        {
            MoveToDigit();
        }

        #endregion Methods
    }
}