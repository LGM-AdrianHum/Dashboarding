//-----------------------------------------------------------------------
// <copyright file="BidirectionalDashboard.cs" company="David Black">
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

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Codeplex.Dashboarding", MessageId = "Codeplex", Justification = "This is a trademark")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Codeplex.Dashboarding", MessageId = "Dashboarding", Justification = "This is the project name")]
namespace Codeplex.Dashboarding
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// A bidirectionalDashboard can both display and set values. Increasingly
    /// analysts claim that showing data is not enough and that interaction is key.
    /// </summary>
    public abstract class BidirectionalDashboard : Dashboard
    {
        #region public fields

        /// <summary>
        /// Identifies the IsBidirectional attached property
        /// </summary>
        public static readonly DependencyProperty IsBidirectionalProperty =
            DependencyProperty.Register("IsBidirectional", typeof(bool), typeof(BidirectionalDashboard), new PropertyMetadata(false, new PropertyChangedCallback(IsBidirectionalPropertyChanged)));

        #endregion
 
        #region private fields

        /// <summary>
        /// The point where the mouse went down
        /// </summary>
        private Point grabOrigin;

        /// <summary>
        /// Are the events registered in theis state?
        /// </summary>
        private bool eventsRegistered;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BidirectionalDashboard"/> class, 
        /// which is mostly about grabbing the mouse enter and leave events 
        /// and rendering the focus handle is necessary
        /// </summary>
        protected BidirectionalDashboard()
        {
            IsGrabbed = false; 
        }

        #region public properties
       
        /// <summary>
        /// Gets  the current value of the drag point in the range 
        /// Minimum &lt;= CurrentValue &lt;= Maximum
        /// </summary>
        /// <value>The current value.</value>
        public double CurrentValue => Minimum + ((Maximum - Minimum) * CurrentNormalizedValue);

        /// <summary>
        /// Gets or sets a value indicating whether this dashboard is bidrectional. IsBiderectional == false means that
        /// the control shows values. IsBidirectional == true means that the user can interact with the control
        /// to ser values.
        /// </summary>
        public bool IsBidirectional
        {
            get => (bool)GetValue(IsBidirectionalProperty);

            set
            {
                SetValue(IsBidirectionalProperty, value);
                SetGrabHandleEventsForGrabState();
            }
        }

        /// <summary>
        /// Gets the textural representation of the current value as specified through the TextFormat property.
        /// </summary>
        /// <value>The formatted value. If the TextFormat property null we return the value formatted
        /// using "{0:000} which is the backwards compatabilty value. If TextFormat 
        /// is not a valid format string we return "???" rather than crashing
        /// </value>
        public string FormattedCurrentValue
        {
            get 
            {
                try
                {
                    return string.Format(ValueTextFormat ?? "{0:000}", CurrentValue);
                }
                catch (FormatException)
                {
                    return "???";
                }
            }
        }
        #endregion
 
        #region internal Properties
        /// <summary>
        /// Gets or sets the grab handle.
        /// </summary>
        /// <value>The grab handle.</value>
        internal FrameworkElement GrabHandle { get; set; }
        #endregion

        #region protected properties

        /// <summary>
        /// Gets or sets a value indicating whether the handle is grabbed, child 
        /// controls should not render other than on mouse move etc
        /// </summary>
        /// <value>
        ///      <c>true</c> if this instance is grabbed; otherwise, <c>false</c>.
        /// </value>
        protected bool IsGrabbed { get; set; }

        /// <summary>
        /// Gets or sets the current normalized value while dragging.
        /// </summary>
        /// <value>The current normalized value.</value>
        protected double CurrentNormalizedValue { get; set; }

        #endregion

        #region internal methods

        /// <summary>
        /// Handles the MouseEnter event of the BidirectionalDashboard control.
        /// If we are showing focus and we are bidirectional
        /// we call animate to get the child control to render
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void BidirectionalDashboard_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IsBidirectional)
            {
                ShowGrabHandle();
            }
        }

        /// <summary>
        /// Button down exposed here for unit testing.
        /// </summary>
        /// <param name="at">The location of the ButtonDown</param>
        internal void ButtonDown(Point at)
        {
            IsGrabbed = true;
            CurrentNormalizedValue = NormalizedValue;
            if (GrabHandle != null)
            {
                GrabHandle.CaptureMouse();
            }

            grabOrigin = at;

            // user may click-release-click on the grab handle, no mouse in event occurs 
            // so we show focus here too
            ShowGrabHandle();
        }

        /// <summary>
        /// The mouse has moved to a new point. Deal with it (exposed for unit testing ony)
        /// </summary>
        /// <param name="p">THe mouse point</param>
        internal void MoveToPoint(Point p)
        {
            if (IsBidirectional && IsGrabbed)
            {
                OnMouseGrabHandleMove(grabOrigin, p);
            }
        }

        /// <summary>
        /// Mouse is up (internal for unit-tests)
        /// </summary>
        internal void MouseUpAction()
        {
            if (IsGrabbed)
            {
                IsGrabbed = false;
                Value = CurrentValue;
                HideGrabHandle();
                if (GrabHandle != null)
                {
                    GrabHandle.ReleaseMouseCapture();
                }

                Animate();
            }
            else
            {
                Animate();
            }
        }

        /// <summary>
        /// The mouse has left the control if there is no grab then remove the focus handle
        /// </summary>
        /// <param name="sender">The initiator of the event</param>
        /// <param name="e">Mouse event args</param>
        internal void GrabHandle_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsGrabbed && IsBidirectional)
            {
                HideGrabHandle();
            }
        }

        #endregion

        #region protected methods
        
        /// <summary>
        /// Manipulates the CurrentNormalizedValue to move the position by a number of screen pixels
        /// </summary>
        /// <param name="offset">Offset in pixels</param>
        protected void MoveCurrentPositionByOffset(double offset)
        {
            CurrentNormalizedValue = NormalizedValue + (offset / 100);
            if (CurrentNormalizedValue > 1)
            {
                CurrentNormalizedValue = 1;
            }

            if (CurrentNormalizedValue < 0)
            {
                CurrentNormalizedValue = 0;
            }
        }

        /// <summary>
        /// Register the control that the grab action works upon
        /// </summary>
        /// <param name="target">The FrameWorkElement representing the Grab handle</param>
        protected void RegisterGrabHandle(FrameworkElement target)
        {
            GrabHandle = target;
            SetGrabHandleEventsForGrabState();
        }

        /// <summary>
        /// The mouse has entered the control registered as the grab handle, show the focus control
        /// </summary>
        protected virtual void ShowGrabHandle()
        {
        }

        /// <summary>
        /// The mouse has exited the control registered as the grab handle, hide the focus control
        /// </summary>
        protected virtual void HideGrabHandle()
        {
        }
     
        /// <summary>
        /// We have a mouse down and move event, we pass the point where the original click happened
        /// and the current point
        /// </summary>
        /// <param name="mouseDownPosition">Point this all happend at</param>
        /// <param name="currentPosition">Where we are now</param>
        protected virtual void OnMouseGrabHandleMove(Point mouseDownPosition, Point currentPosition)
        {
        }

        #endregion

        #region private methods

        /// <summary>
        /// The value of the IsBidirectional property has changed. We call animate to allow any focus
        /// handle to be rendered
        /// </summary>
        /// <param name="dependancy">the dependancy object</param>
        /// <param name="args">The property changed event args</param>
        private static void IsBidirectionalPropertyChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            if (dependancy is BidirectionalDashboard instance)
            {
                instance.SetGrabHandleEventsForGrabState();
                if (instance.DashboardLoaded)
                {
                    instance.OnPropertyChanged("IsBidirectional");
                    instance.Animate();
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the BidirectionalDashboard control. If we 
        /// are grabbing we do the MouseUp handling 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void BidirectionalDashboard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseUpAction();
        }
 
        /// <summary>
        /// Handles the MouseLeftButtonDown event of the target control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Target_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ButtonDown(e.GetPosition(this));
        }

        /// <summary>
        /// Handles the MouseMove event of the GrabHandle control, 
        /// pass on the origin and current position
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void GrabHandle_MouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(this);
            MoveToPoint(p);
        }

        /// <summary>
        /// Sets the state of the grab handle events for Gra control, hookem up if the
        /// IsBidirectional flag is set on and removing em if it isnt.
        /// </summary>
        private void SetGrabHandleEventsForGrabState()
        {
            if (GrabHandle != null && IsBidirectional && !eventsRegistered)
            {
                GrabHandle.Cursor = IsBidirectional ? Cursors.Hand : Cursors.None;
                GrabHandle.MouseEnter += new MouseEventHandler(BidirectionalDashboard_MouseEnter);
                GrabHandle.MouseLeave += new MouseEventHandler(GrabHandle_MouseLeave);
                GrabHandle.MouseLeftButtonUp += new MouseButtonEventHandler(BidirectionalDashboard_MouseLeftButtonUp);
                GrabHandle.MouseLeftButtonDown += new MouseButtonEventHandler(Target_MouseLeftButtonDown);
                GrabHandle.MouseMove += new MouseEventHandler(GrabHandle_MouseMove);
                eventsRegistered = true;
            }
            else if (GrabHandle != null && !IsBidirectional && eventsRegistered)
            {
                GrabHandle.Cursor = IsBidirectional ? Cursors.Hand : Cursors.None;
                GrabHandle.MouseEnter -= new MouseEventHandler(BidirectionalDashboard_MouseEnter);
                GrabHandle.MouseLeave -= new MouseEventHandler(GrabHandle_MouseLeave);
                GrabHandle.MouseLeftButtonUp -= new MouseButtonEventHandler(BidirectionalDashboard_MouseLeftButtonUp);
                GrabHandle.MouseLeftButtonDown -= new MouseButtonEventHandler(Target_MouseLeftButtonDown);
                GrabHandle.MouseMove -= new MouseEventHandler(GrabHandle_MouseMove);
                eventsRegistered = false;
            }
        }

        #endregion
    }
}
