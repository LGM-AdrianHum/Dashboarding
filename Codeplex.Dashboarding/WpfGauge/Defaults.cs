using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfGauge
{
    /// <summary>
    /// 
    /// </summary>
    public static class Defaults
    {
        /// <summary>
        /// Default style for the gauge's value text
        /// </summary>
        public static Style DefaultValueTextStyle => GetDefaultValueTextStyle();

        private static Style GetDefaultValueTextStyle()
        {
            var style = new Style(typeof(TextBlock));

            style.Setters.Add
                (
                    new Setter { Property = TextBlock.ForegroundProperty, Value = new SolidColorBrush(Color.FromArgb(255,248,248,255)) }
                );
            style.Setters.Add
                (
                    new Setter { Property = TextBlock.FontSizeProperty, Value = 22d }
                );
            style.Setters.Add
                (
                    new Setter { Property = FrameworkElement.MarginProperty, Value = new Thickness(0, 100, 0, 0) }
                );
            style.Setters.Add
                (
                    new Setter { Property = FrameworkElement.HorizontalAlignmentProperty, Value = HorizontalAlignment.Center }
                );
            style.Setters.Add
                (
                    new Setter { Property = FrameworkElement.VerticalAlignmentProperty, Value = VerticalAlignment.Center }
                );
            style.Setters.Add
                (
                    new Setter { Property = TextBlock.FontWeightProperty, Value = FontWeights.Bold }
                );
            style.Setters.Add
                (
                    new Setter { Property = TextBlock.FontFamilyProperty, Value = new FontFamily("Agency FB") }
                );
            style.Setters.Add
                (
                    new Setter { Property = UIElement.EffectProperty, Value = new System.Windows.Media.Effects.DropShadowEffect() 
                    { ShadowDepth = 0.0, BlurRadius = 5.0, Color = Colors.Black, Opacity = 1.0,

#if !SILVERLIGHT
                        RenderingBias = System.Windows.Media.Effects.RenderingBias.Quality
#endif
                    } }
                );
            return style;
        }

        /// <summary>
        ///  Default value for the gauge's Tick Mark Labels
        /// </summary>
        public static Style DefaultTickLabelStyle => GetDefaultTickLabelStyle();

        private static Style GetDefaultTickLabelStyle()
        {
            var style = new Style(typeof(TextBlock));

            style.Setters.Add
                (
                    new Setter { Property = TextBlock.ForegroundProperty, Value = GradientBrushes.GaugeValueForeground }
                );
            style.Setters.Add
                (
                    new Setter { Property = TextBlock.FontSizeProperty, Value = 14d }
                );
            style.Setters.Add
                (
                    new Setter { Property = TextBlock.FontWeightProperty, Value = FontWeights.Bold }
                );
            style.Setters.Add
                (
                    new Setter { Property = TextBlock.FontFamilyProperty, Value = new FontFamily("Agency FB") }
                );
            
            return style;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public static class GradientBrushes
        {
            /// <summary>
            /// 
            /// </summary>
            public static LinearGradientBrush GaugeValueForeground => GetGaugeValueForeground();

            private static LinearGradientBrush GetGaugeValueForeground()
            {
                var gradients = new GradientStopCollection();
                gradients.Add(new GradientStop() {Color = Colors.Gray, Offset = 0.0});
                gradients.Add(new GradientStop() { Color= Color.FromArgb(255, 248, 248, 255), Offset=0.5});
                var gradient = new LinearGradientBrush(gradients, 90.0);

                return gradient;
            }

            /// <summary>
            /// 
            /// </summary>
            public static LinearGradientBrush GaugeOuterEllipseBackground => GetGaugeOuterEllipseBackground();

            private static LinearGradientBrush GetGaugeOuterEllipseBackground()
            {
                var gradients = new GradientStopCollection();
                gradients.Add(new GradientStop() { Color = Color.FromArgb(255, 220, 220, 220), Offset = 0.462 });
                gradients.Add(new GradientStop() { Color = Color.FromArgb(255, 192, 192, 192), Offset = 0.552 });
                var gradient = new LinearGradientBrush(gradients, 90.0);

                return gradient;
            }

        }
    }
}
