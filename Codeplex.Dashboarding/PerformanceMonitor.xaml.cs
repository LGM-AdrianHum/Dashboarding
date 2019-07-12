//-----------------------------------------------------------------------
// <copyright file="PerformanceMonitor.xaml.cs" company="David Black">
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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// The performance monitor control was inspired by the graph in the 
    /// performance tab of the windows TaskManager. It maintains a historical
    /// high and displays a filled shaded graph.
    /// <para>There are many color properties to allow you to customize the
    /// display of the control</para>
    /// </summary>
    public partial class PerformanceMonitor : Dashboard
    {
        #region Fields

        /// <summary>
        /// Dependancy property for GridLine color
        /// </summary>
        public static readonly DependencyProperty AxisProperty = 
            DependencyProperty.Register("Axis", typeof(Color), typeof(PerformanceMonitor), new PropertyMetadata(new PropertyChangedCallback(AxisChanged)));

        /// <summary>
        /// Dependancy property for GraphLine color
        /// </summary>
        public static readonly DependencyProperty GraphFillFromProperty = 
            DependencyProperty.Register("GraphFillFrom", typeof(Color), typeof(PerformanceMonitor), new PropertyMetadata(new PropertyChangedCallback(GraphFillFromChanged)));

        /// <summary>
        /// Dependancy property for GraphLine color
        /// </summary>
        public static readonly DependencyProperty GraphFillToProperty = 
            DependencyProperty.Register("GraphFillTo", typeof(Color), typeof(PerformanceMonitor), new PropertyMetadata(new PropertyChangedCallback(GraphFillToChanged)));

        /// <summary>
        /// Dependancy property for GraphLine color
        /// </summary>
        public static readonly DependencyProperty GraphLineProperty = 
            DependencyProperty.Register("GraphLine", typeof(Color), typeof(PerformanceMonitor), new PropertyMetadata(new PropertyChangedCallback(GraphLineColorChanged)));

        /// <summary>
        /// Dependancy property for GridLine color
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GridLine", Justification = "It is correctly cased")]
        public static readonly DependencyProperty GridLineProperty = 
            DependencyProperty.Register("GridLine", typeof(Color), typeof(PerformanceMonitor), new PropertyMetadata(new PropertyChangedCallback(GridLineColorChanged)));

        /// <summary>
        /// Dependancy property for Historical values
        /// </summary>
        public static readonly DependencyProperty HistoricalValuesProperty = 
            DependencyProperty.Register("HistoricalValues", typeof(List<double>), typeof(PerformanceMonitor), new PropertyMetadata(new PropertyChangedCallback(HistoricalValuesChanged)));

        /// <summary>
        /// Largest value yet seen
        /// </summary>
        private int historicalMax;

        /// <summary>
        /// Smallest value yet seen
        /// </summary>
        private int historicalMin;

        /// <summary>
        /// The lines to draw
        /// </summary>
        private List<Line> lines = new List<Line>();

        /// <summary>
        /// One graph full of points
        /// </summary>
        private List<double> values = new List<double>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceMonitor"/> class.
        /// </summary>
        public PerformanceMonitor()
        {
            InitializeComponent();
            GraphLine = Colors.Cyan;
            GridLine = Colors.White;
            Axis = Colors.Green;
            ValueTextColor = Colors.Green;
            GraphFillTo = Colors.Gray;
            GraphFillFrom = Colors.DarkGray;

            SizeChanged += new SizeChangedEventHandler(PerformanceMonitor_SizeChanged);
            Loaded += new RoutedEventHandler(PerformanceMonitor_Loaded);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the color of the grid lines on the background of the graph
        /// </summary>
        public Color Axis
        {
            get => (Color)GetValue(AxisProperty);
            set => SetValue(AxisProperty, value);
        }

        /// <summary>
        /// Gets or sets the color of the grid lines on the background of the graph
        /// </summary>
        public Color GraphFillFrom
        {
            get => (Color)GetValue(GraphFillFromProperty);
            set => SetValue(GraphFillFromProperty, value);
        }

        /// <summary>
        /// Gets or sets the color of the grid lines on the background of the graph
        /// </summary>
        public Color GraphFillTo
        {
            get => (Color)GetValue(GraphFillToProperty);
            set => SetValue(GraphFillToProperty, value);
        }

        /// <summary>
        /// Gets or sets the color of the grid lines on the background of the graph
        /// </summary>
        public Color GraphLine
        {
            get => (Color)GetValue(GraphLineProperty);
            set => SetValue(GraphLineProperty, value);
        }

        /// <summary>
        /// Gets or sets the color of the grid lines on the background of the graph
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GridLine", Justification = "This is the correct casing")]
        public Color GridLine
        {
            get => (Color)GetValue(GridLineProperty);

            set => SetValue(GridLineProperty, value);
        }

        /// <summary>
        /// Gets or sets the color of the grid lines on the background of the graph
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is bound to xaml and the colection does change!")]
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Bound to XAML")]
        public List<double> HistoricalValues
        {
            get => (List<double>)GetValue(HistoricalValuesProperty);
            set => SetValue(HistoricalValuesProperty, value);
        }

        /// <summary>
        /// Gets the resource root. This allow us to access the Storyboards in a Silverlight/WPf
        /// neutral manner
        /// </summary>
        /// <value>The resource root.</value>
        protected override FrameworkElement ResourceRoot => LayoutRoot;

        /// <summary>
        /// Gets or sets a value indicating whether the grid needs to redraw
        /// </summary>
        private bool GridRedrawRequired
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Display the control according the the current value
        /// </summary>
        protected override void Animate()
        {
            UpdateColours();
            StoreValue();
            DrawLine();
            UpdateMinMxValues();
        }

        /// <summary>
        /// Requires that the control hounours all appearance setting as specified in the
        /// dependancy properties (at least the supported ones). No dependancy property handling
        /// is performed until all dependancy properties are set and the control is loaded.
        /// </summary>
        protected override void ManifestChanges()
        {
            UpdateAxisColor();
            UpdateColours();
            UpdateGraphFill();
            UpdateGraphLineColors();
            UpdateGridLineColor();
            UpdateHistoricalValues();
            UpdateMinMxValues();
            UpdateTextColor();
            UpdateTextFormat();
            UpdateTextVisibility();
            UpdateFontStyle();
        }

        /// <summary>
        /// Update your text colors to that of the TextColor dependancy property
        /// </summary>
        protected override void UpdateTextColor()
        {
            _lowWaterMark.Foreground = new SolidColorBrush(ValueTextColor);
            _highWaterMark.Foreground = new SolidColorBrush(ValueTextColor);
        }

        /// <summary>
        /// Updates the font style for both face and value text.
        /// </summary>
        protected override void UpdateFontStyle()
        {
            CopyFontDetails(_lowWaterMark);
            CopyFontDetails(_highWaterMark);
        }


        /// <summary>
        /// The format string for the value has changed
        /// </summary>
        protected override void UpdateTextFormat()
        {
        }

        /// <summary>
        /// Set the visibiity of your text according to that of the TextVisibility property
        /// </summary>
        protected override void UpdateTextVisibility()
        {
            _lowWaterMark.Visibility = ValueTextVisibility;
            _highWaterMark.Visibility = ValueTextVisibility;
        }

        /// <summary>
        /// Our color has changed possibly via the GridLineProperty ot via a SetValue directly
        /// on the dependancy property. We change the color to the new value
        /// </summary>
        /// <param name="dependancy">the PerformanceMonitor</param>
        /// <param name="args">old value and new value</param>
        private static void AxisChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            if (dependancy is PerformanceMonitor instance && instance.DashboardLoaded)
            {
                instance.UpdateAxisColor();
            }
        }

        /// <summary>
        /// Our color has changed possibly via the GraphLineProperty or via a SetValue directly
        /// on the dependancy property. We change the color to the new value
        /// </summary>
        /// <param name="dependancy">the PerformanceMonitor</param>
        /// <param name="args">old value and new value</param>
        private static void GraphFillFromChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            if (dependancy is PerformanceMonitor instance && instance.DashboardLoaded)
            {
                instance.UpdateGraphFill();
            }
        }

        /// <summary>
        /// Our color has changed possibly via the GraphLineProperty or via a SetValue directly
        /// on the dependancy property. We change the color to the new value
        /// </summary>
        /// <param name="dependancy">the PerformanceMonitor</param>
        /// <param name="args">old value and new value</param>
        private static void GraphFillToChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            if (dependancy is PerformanceMonitor instance && instance.DashboardLoaded)
            {
                instance.UpdateGraphFill();
            }
        }

        /// <summary>
        /// Our color has changed possibly via the GraphLineProperty or via a SetValue directly
        /// on the dependancy property. We change the color to the new value
        /// </summary>
        /// <param name="dependancy">the PerformanceMonitor</param>
        /// <param name="args">old value and new value</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        private static void GraphLineColorChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            if (dependancy is PerformanceMonitor instance && instance.DashboardLoaded)
            {
                instance.UpdateGraphLineColors();
            }
        }

        /// <summary>
        /// Our color has changed possibly via the GridLineProperty ot via a SetValue directly
        /// on the dependancy property. We change the color to the new value
        /// </summary>
        /// <param name="dependancy">the PerformanceMonitor</param>
        /// <param name="args">old value and new value</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Color", Justification = "We support U.S. naming in a British project")]
        private static void GridLineColorChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            if (dependancy is PerformanceMonitor instance && instance.DashboardLoaded)
            {
                instance.GridRedrawRequired = true;
                instance.UpdateGridLineColor();
            }
        }

        /// <summary>
        /// Initializes the control to a set of historical values to pre form a graph.
        /// </summary>
        /// <param name="dependancy">the PerformanceMonitor</param>
        /// <param name="args">old value and new value</param>
        private static void HistoricalValuesChanged(DependencyObject dependancy, DependencyPropertyChangedEventArgs args)
        {
            if (dependancy is PerformanceMonitor instance && instance.DashboardLoaded)
            {
                instance.UpdateHistoricalValues();
            }
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        private void DrawLine()
        {
            var normalised = new List<double>();

            var ch = _canvas.ActualHeight;
            var cw = _canvas.ActualWidth;

            // if the line is from 0 to 99 in one pixel then back to 0 again the
            // path over extends and escapes the canvas, we clip to prevent this
            _canvas.Clip = new RectangleGeometry { Rect = new Rect(0, 0, cw, ch) };

            var max = values.Max();
            var min = values.Min();
            if (max > historicalMax)
            {
                historicalMax = (int)max;
            }

            if (min < historicalMin)
            {
                historicalMin = (int)min;
            }

            foreach (int val in values)
            {
                if (historicalMax == 0)
                {
                    normalised.Add(0);
                }
                else
                {
                    normalised.Add(((double)val) / historicalMax);
                }
            }

            var startPoint = cw - values.Count;

            var pg = new PathGeometry();
            pg.FillRule = FillRule.Nonzero;
            pg.Figures = new PathFigureCollection();
            var pf = new PathFigure();
            pf.IsClosed = true;

            pf.StartPoint = new Point(startPoint, ch);
            pf.Segments = new PathSegmentCollection();

            var idx = 0;

            for (var i = (int)startPoint; i < cw; i++)
            {
                var y = ch - (normalised[idx] * ch);

                pf.Segments.Add(new LineSegment { Point = new Point(i + 1, y) });
                idx++;
                if (idx >= normalised.Count) break;
            }

            pf.Segments.Add(new LineSegment { Point = new Point(cw, ch) });
            pg.Figures.Add(pf);
            _path.Data = pg;
            _path.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Draws the lines.
        /// </summary>
        /// <param name="spacing">The spacing.</param>
        /// <param name="maxSpacing">The max spacing.</param>
        private void DrawLines(int spacing, int maxSpacing)
        {
            double lineY = 0;
            var remainder = _canvas.ActualHeight % maxSpacing;
            if (remainder > 0)
            {
                lineY = -(remainder / 2);
            }

            while (lineY <= _canvas.ActualHeight)
            {
                var l = new Line { X1 = 0, Y1 = lineY, X2 = _canvas.ActualWidth, Y2 = lineY, Opacity = 0.15, Stroke = new SolidColorBrush(GridLine) };
                _canvas.Children.Add(l);
                lineY += spacing;
                lines.Add(l);
                Panel.SetZIndex(l, 0);
            }

            double lineX = 0;
            remainder = _canvas.ActualWidth % maxSpacing;
            if (remainder > 0)
            {
                lineX = -(remainder / 2);
            }

            while (lineX <= _canvas.ActualWidth)
            {
                var l = new Line { X1 = lineX, Y1 = 0, X2 = lineX, Y2 = _canvas.ActualHeight, Opacity = 0.15, Stroke = new SolidColorBrush(GridLine) };
                _canvas.Children.Add(l);
                lineX += spacing;
                lines.Add(l);
                Panel.SetZIndex(l, 0);
            }
        }

        /// <summary>
        /// Handles the Loaded event of the PerformanceMonitor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void PerformanceMonitor_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateColours();
            StoreValue();
            DrawLine();
            UpdateMinMxValues();
        }

        /// <summary>
        /// Handles the SizeChanged event of the PerformanceMonitor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void PerformanceMonitor_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridRedrawRequired = true;
            UpdateColours();
        }

        /// <summary>
        /// Stores the value.
        /// </summary>
        private void StoreValue()
        {
            if (values.Count > 0 && values.Count == _canvas.ActualWidth)
            {
                values.RemoveAt(0);
            }

            values.Insert(values.Count, Value);
        }

        /// <summary>
        /// Sets the color of the axis.
        /// </summary>
        private void UpdateAxisColor()
        {
            _vertAxis.Stroke = new SolidColorBrush(Axis);
            _horAxis.Stroke = new SolidColorBrush(Axis);
        }

        /// <summary>
        /// Sets the colours.
        /// </summary>
        private void UpdateColours()
        {
            UpdateGridLineColor();
            UpdateGraphLineColors();
            UpdateTextColor();
            UpdateAxisColor();
            UpdateGraphFill();
        }

        /// <summary>
        /// Sets the graph fill.
        /// </summary>
        private void UpdateGraphFill()
        {
            rangeHighColour0.Color = GraphFillFrom;
            rangeLowColour0.Color = GraphFillTo;
        }

        /// <summary>
        /// Sets the graph line colors.
        /// </summary>
        private void UpdateGraphLineColors()
        {
            _path.Stroke = new SolidColorBrush(GraphLine);
        }

        /// <summary>
        /// Updates the color of the grid line.
        /// </summary>
        private void UpdateGridLineColor()
        {
            if (!GridRedrawRequired || _canvas.ActualHeight == 0 || _canvas.ActualHeight == 0)
            {
                return;
            }

            if (lines.Count > 0)
            {
                foreach (var line in lines)
                {
                    _canvas.Children.Remove(line);
                }

                lines.Clear();
            }

            DrawLines(10, 100);
            DrawLines(50, 100);
            DrawLines(100, 100);
            Panel.SetZIndex(_path, 1000);
            GridRedrawRequired = false;
        }

        /// <summary>
        /// Initializes the control to a set of historical values to pre form a graph.
        /// </summary>
        private void UpdateHistoricalValues()
        {
            if (HistoricalValues != null && HistoricalValues.Count > 0)
            {
                values.AddRange(HistoricalValues);
            }
        }

        /// <summary>
        /// Updates the min and max values.
        /// </summary>
        private void UpdateMinMxValues()
        {
            _lowWaterMark.Text = historicalMin.ToString();
            _highWaterMark.Text = historicalMax.ToString();
        }

        #endregion Methods
    }
}