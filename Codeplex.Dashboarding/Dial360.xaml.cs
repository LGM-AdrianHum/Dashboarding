//-----------------------------------------------------------------------
// <copyright file="Dial360.xaml.cs" company="David Black">
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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// A Dial360  displays as a traditional circular gauge with numbers from 0 to 100. The
    /// needle sweeps through approximately 240 degrees.
    /// </summary>
    public partial class Dial360 : BidirectionalDial
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dial360"/> class.
        /// </summary>
        public Dial360()
        {
            InitializeComponent();
            SetValue(FaceTextColorProperty, Colors.White);
            SetValue(ValueTextColorProperty, Colors.White);
            RegisterGrabHandle(_grabHandle);
        }

        #region proteced properties
        /// <summary>
        /// Gets the shape used to highlight the grab control
        /// </summary>
        protected override Shape GrabHighlight => _grabHighlight;

        /// <summary>
        /// Gets the resource root. This allow us to access the Storyboards in a Silverlight/WPf
        /// neutral manner
        /// </summary>
        /// <value>The resource root.</value>
        protected override FrameworkElement ResourceRoot => LayoutRoot;

        #endregion

        #region Protected methods

        /// <summary>
        /// Requires that the control hounours all appearance setting as specified in the
        /// dependancy properties (at least the supported ones). No dependancy property handling
        /// is performed until all dependancy properties are set and the control is loaded.
        /// </summary>
        protected override void ManifestChanges()
        {
            UpdateFaceColor();
            UpdateNeedleColor();
            UpdateTextColor();
            UpdateTextFormat();
            UpdateTextVisibility();
            UpdateFontStyle();

        }

        /// <summary>
        /// Animate our Dial360. We calculate the needle position, color and the face color
        /// </summary>
        protected override void Animate()
        {
            UpdateFaceColor();
            UpdateNeedleColor();
            UpdateTextFormat();
            ShowIfBiDirectional();

            if (!IsBidirectional || (IsBidirectional && !IsGrabbed))
            {
                SetPointerByAnimationOverSetTime(NormalizedValue, Value, AnimationDuration);
            }
            else
            {
                SetPointerByAnimationOverSetTime(CurrentNormalizedValue, CurrentValue, TimeSpan.Zero);
            }
        }

        /// <summary>
        /// Set the face color from the range
        /// </summary>
        protected override void UpdateFaceColor()
        {
            var c = FaceColorRange.GetColor(Value);
            if (c != null)
            {
                _colourRangeStart.Color = c.HiColor;
                _colourRangeEnd.Color = c.LowColor;
            }
        }

        /// <summary>
        /// Set the needle color from the range
        /// </summary>
        protected override void UpdateNeedleColor()
        {
            var c = NeedleColorRange.GetColor(Value);
            if (c != null)
            {
                _needleHighColour.Color = c.HiColor;
                _needleLowColour.Color = c.LowColor;
            }
        }

        /// <summary>
        /// Sets the text to the color in the TextColorProperty
        /// </summary>
        protected override void UpdateTextColor()
        {
            for (var i = 0; i <= 10; i++)
            {
                if (LayoutRoot.FindName("_txt" + i) is TextBlock tb)
                {
                    tb.Foreground = new SolidColorBrush(FaceTextColor);
                }
            }

            if (_txt11 != null)
            {
                _txt11.Foreground = new SolidColorBrush(ValueTextColor);
            }
        }

        /// <summary>
        /// Updates the font style for both face and value text.
        /// </summary>
        protected override void UpdateFontStyle()
        {
            for (var i = 0; i <= 12; i++)
            {
                var tb = ResourceRoot.FindName("_txt" + i) as TextBlock;
                CopyFontDetails(tb);
            }
        }

        /// <summary>
        /// The format string for the value has changed
        /// </summary>
        protected override void UpdateTextFormat()
        {
            for (var i = 0; i <= 10; i++)
            {
                if (LayoutRoot.FindName("_txt" + i) is TextBlock tb && FaceTextFormat != null)
                {
                    tb.Text = string.Format(FaceTextFormat, RealMinimum + (i * ((RealMaximum - RealMinimum) / 10)));
                }
            }

            if (_txt11 != null)
            {
                _txt11.Text = IsGrabbed ? FormattedCurrentValue : FormattedValue;
            }
        }

        /// <summary>
        /// Sets the text visibility to that of the TextVisibility property
        /// </summary>
        protected override void UpdateTextVisibility()
        {
            for (var i = 0; i <= 10; i++)
            {
                if (LayoutRoot.FindName("_txt" + i) is TextBlock tb)
                {
                    tb.Visibility = FaceTextVisibility;
                }
            }

            if (_txt11 != null)
            {
                _txt11.Visibility = ValueTextVisibility;
            }
        }

        /// <summary>
        /// Converts the angle specified into a 0..1 normalized value. The proposed value
        /// if foreced into the range 0..1 by rounding if necessary
        /// </summary>
        /// <param name="cv">The current normalized value candidate</param>
        protected override void SetCurrentNormalizedValue(double cv)
        {
            if (cv < -150)
            {
                cv = -150;
            }

            if (cv > 150)
            {
                cv = 150;
            }

            CurrentNormalizedValue = (cv + 150) / 300;
        }

        /// <summary>
        /// Based on the current position calculates what angle the current mouse
        /// position represents relative to the rotation point of the needle
        /// </summary>
        /// <param name="currentPoint">Current point</param>
        /// <returns>Angle in degrees</returns>
        protected override double CalculateRotationAngle(Point currentPoint)
        {
            var opposite = currentPoint.Y - (ActualHeight / 2);
            var adjacent = currentPoint.X - (ActualWidth / 2);
            var tan = opposite / adjacent;
            var angleInDegrees = Math.Atan(tan) * (180.0 / Math.PI);

            if (currentPoint.X >= (ActualWidth / 2) && currentPoint.Y <= (ActualHeight / 2))
            {
                angleInDegrees = 180 + angleInDegrees;
            }
            else if (currentPoint.X >= (ActualWidth / 2) && currentPoint.Y > (ActualHeight / 2))
            {
                angleInDegrees = 180 + angleInDegrees;
            }

            angleInDegrees = (angleInDegrees - 90) % 360;

            return angleInDegrees;
        }
        #endregion

        #region privates
        /// <summary>
        /// Sets the pointer animation to execute and sets the time to animate. This allow the same
        /// code to handle normal operation using the Dashboard.AnimationDuration or for dragging the
        /// needle during bidirectional operation (TimeSpan.Zero)
        /// </summary>
        /// <param name="normalizedValue">The normalized Value.</param>
        /// <param name="value">The value.</param>
        /// <param name="duration">The duration.</param>
        private void SetPointerByAnimationOverSetTime(double normalizedValue, double value, TimeSpan duration)
        {
            UpdateTextFormat();

            var point = -150 + (3 * (normalizedValue * 100));

            var needle = SetFirstChildSplineDoubleKeyFrameTime(AnimateIndicatorStoryboard, point);
            needle.KeyTime = KeyTime.FromTimeSpan(duration);
            Start(AnimateIndicatorStoryboard);

            var handle = SetFirstChildSplineDoubleKeyFrameTime(AnimateGrabHandleStoryboard, point);
            handle.KeyTime = KeyTime.FromTimeSpan(duration);
            Start(AnimateGrabHandleStoryboard);
        }

        /// <summary>
        /// If we are Bidirectional show the grab handle and highlight
        /// </summary>
        private void ShowIfBiDirectional()
        {
            var val = IsBidirectional ? Visibility.Visible : Visibility.Collapsed;

            _grabHandle.Visibility = val;
            _grabHighlight.Visibility = val;
        }
        #endregion
    }
}
