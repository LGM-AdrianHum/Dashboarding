﻿//-----------------------------------------------------------------------
// <copyright file="Dial90.cs" company="David Black">
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
    /// Base class for all quadrant based 90 degree dials
    /// </summary>
    public abstract class Dial90 : BidirectionalDial
    {
        /// <summary>
        /// Canvas to rotate to move the grab handle
        /// </summary>
        private Canvas grabHandleCanvas;

        /// <summary>
        /// Shape used to highlight that the mouse is in the grab area
        /// </summary>
        private Shape grabHighlightShape;

        /// <summary>
        /// The text block used to display the percentage
        /// </summary>
        private TextBlock textBlock;

        /// <summary>
        /// Blend start for the face of the dial
        /// </summary>
        private GradientStop faceHighColorGradientStop;

        /// <summary>
        /// Blend end for the face of the dial
        /// </summary>
        private GradientStop faceLowColorGradientStop;

        /// <summary>
        /// Blend start for the face of the dial
        /// </summary>
        private GradientStop needleHighColorGradientStop;

        /// <summary>
        /// Blend end for the face of the dial
        /// </summary>
        private GradientStop needleLowColorGradientStop;

        /// <summary>
        /// Gets the shape used to highlight the grab control
        /// </summary>
        protected override Shape GrabHighlight => grabHighlightShape;

        /// <summary>
        /// Requires that the control honors all appearance setting as specified in the
        /// dependency properties (at least the supported ones). No dependency property handling
        /// is performed until all dependency properties are set and the control is loaded.
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
        /// Set the defaults for our dependency properties and register the
        /// grab handle
        /// </summary>
        protected void InitializeDial90()
        {
            InitialiseRefs();
            SetValue(FaceTextColorProperty, Colors.White);
            SetValue(ValueTextColorProperty, Colors.Black);
            SetValue(FontSizeProperty, 7.0);
            RegisterGrabHandle(grabHandleCanvas);
        }

        /// <summary>
        /// Sets the text visibility to that of the TextVisibility property
        /// </summary>
        protected override void UpdateTextVisibility()
        {
            textBlock.Visibility = ValueTextVisibility;
            for (var i = 0; i < 5; i++)
            {
                if (ResourceRoot.FindName("_txt" + i) is TextBlock tb)
                {
                    tb.Visibility = FaceTextVisibility;
                }
            }
        }

        /// <summary>
        /// Updates the font style for both face and value text.
        /// </summary>
        protected override void UpdateFontStyle()
        {
            for (var i = 0; i <= 4; i++)
            {
                var tb = ResourceRoot.FindName("_txt" + i) as TextBlock;
                CopyFontDetails(tb);
            }
            CopyFontDetails(textBlock);
        }

        /// <summary>
        /// Set our text color to that of the TextColorProperty
        /// </summary>
        protected override void UpdateTextColor()
        {
            textBlock.Foreground = new SolidColorBrush(ValueTextColor);
            for (var i = 0; i < 5; i++)
            {
                if (ResourceRoot.FindName("_txt" + i) is TextBlock tb)
                {
                    tb.Foreground = new SolidColorBrush(FaceTextColor);
                }
            }
        }

        /// <summary>
        /// The format string for the value has changed
        /// </summary>
        protected override void UpdateTextFormat()
        {
            if (textBlock != null)
            {
                textBlock.Text = IsGrabbed ? FormattedCurrentValue : FormattedValue;
            }

            for (var i = 0; i < 5; i++)
            {
                if (ResourceRoot.FindName("_txt" + i) is TextBlock tb)
                {
                    tb.Text = string.Format(FaceTextFormat, RealMinimum + (i * ((RealMaximum - RealMinimum) / 4)));
                }
            }
        }

        /// <summary>
        /// Sets the face color from the color range
        /// </summary>
        protected override void UpdateFaceColor()
        {
            var c = FaceColorRange.GetColor(Value);
            if (c != null)
            {
                faceHighColorGradientStop.Color = c.HiColor;
                faceLowColorGradientStop.Color = c.LowColor;
            }
        }

        /// <summary>
        /// Sets the needle color from the color range
        /// </summary>
        protected override void UpdateNeedleColor()
        {
            var c = NeedleColorRange.GetColor(Value);
            if (c != null)
            {
                needleHighColorGradientStop.Color = c.HiColor;
                needleLowColorGradientStop.Color = c.LowColor;
            }
        }

        /// <summary>
        /// Based on the rotation angle, set the normalized current value
        /// </summary>
        /// <param name="cv">rotation angle</param>
        protected override void SetCurrentNormalizedValue(double cv)
        {
            cv = (cv < 0) ? 0 : cv;
            cv = (cv > 90) ? 90 : cv;
            CurrentNormalizedValue = cv / 90;
        }

        /// <summary>
        /// Shows the grab handle if this control is bidirectional
        /// </summary>
        protected void ShowHandleIfBidirectional()
        {
            var val = IsBidirectional ? Visibility.Visible : Visibility.Collapsed;

            grabHandleCanvas.Visibility = val;
            grabHighlightShape.Visibility = val;
        }

        /// <summary>
        /// Move the needle and set the needle and face colors to suite the value
        /// </summary>
        protected override void Animate()
        {
            UpdateFaceColor();
            UpdateNeedleColor();

            ShowHandleIfBidirectional();

            if (!IsBidirectional || (IsBidirectional && !IsGrabbed))
            {
                SetPointerByAnimationOverSetTime(CalculatePointFromNormalisedValue(), CurrentValue, AnimationDuration);
            }
            else
            {
                SetPointerByAnimationOverSetTime(CalculatePointFromCurrentNormalisedValue(), CurrentValue, TimeSpan.Zero);
            }
        }
 
        /// <summary>
        /// Calculate the rotation angle from the normalised actual value
        /// </summary>
        /// <returns>angle in degrees to position the transform</returns>
        protected abstract double CalculatePointFromNormalisedValue();

        /// <summary>
        /// Calculate the rotation angle from the normalised current value
        /// </summary>
        /// <returns>angle in degrees to position the transform</returns>
        protected abstract double CalculatePointFromCurrentNormalisedValue();

        /// <summary>
        /// Initialize references to controls we expect to find in the child
        /// </summary>
        private void InitialiseRefs()
        {
            grabHandleCanvas = ResourceRoot.FindName("_grabHandle") as Canvas;
            grabHighlightShape = ResourceRoot.FindName("_grabHighlight") as Shape;
            textBlock = ResourceRoot.FindName("_text") as TextBlock;
            faceHighColorGradientStop = ResourceRoot.FindName("_colourRangeStart") as GradientStop;
            faceLowColorGradientStop = ResourceRoot.FindName("_colourRangeEnd") as GradientStop;
            needleHighColorGradientStop = ResourceRoot.FindName("_needleHighColour") as GradientStop;
            needleLowColorGradientStop = ResourceRoot.FindName("_needleLowColour") as GradientStop;
        }

        /// <summary>
        /// Sets the pointer animation to execute and sets the time to animate. This allow the same
        /// code to handle normal operation using the Dashboard.AnimationDuration or for dragging the
        /// needle during bidirectional operation (TimeSpan.Zero)
        /// </summary>
        /// <param name="point">The point to animate to.</param>
        /// <param name="value">The value to display.</param>
        /// <param name="duration">The duration.</param>
        private void SetPointerByAnimationOverSetTime(double point, double value, TimeSpan duration)
        {
            UpdateTextFormat();

            var needle = SetFirstChildSplineDoubleKeyFrameTime(AnimateIndicatorStoryboard, point);
            needle.KeyTime = KeyTime.FromTimeSpan(duration);
            Start(AnimateIndicatorStoryboard);

            var handle = SetFirstChildSplineDoubleKeyFrameTime(AnimateGrabHandleStoryboard, point);
            handle.KeyTime = KeyTime.FromTimeSpan(duration);
            Start(AnimateGrabHandleStoryboard);
        }
    }
}
