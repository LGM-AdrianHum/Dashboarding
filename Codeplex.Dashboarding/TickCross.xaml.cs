//-----------------------------------------------------------------------
// <copyright file="TickCross.xaml.cs" company="David Black">
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
    using System.Windows;

    /// <summary>
    /// Represents a boolean value, either as a Tick or cross for true and 
    /// false respecively. The user may specify the color of the tick and cross
    /// </summary>
    public partial class TickCross : BinaryDashboard
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TickCross"/> class.
        /// </summary>
        public TickCross()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the resource root. This allow us to access the Storyboards in a Silverlight/WPf
        /// neutral manner
        /// </summary>
        /// <value>The resource root.</value>
        protected override FrameworkElement ResourceRoot => LayoutRoot;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Display the control according the the current value
        /// </summary>
        protected override void Animate()
        {
            if (LayoutRoot != null)
            {
                PerformCommonBinaryAnimation(_true, _false, AnimateIndicatorStoryboard);
            }
        }

        #endregion Methods
    }
}