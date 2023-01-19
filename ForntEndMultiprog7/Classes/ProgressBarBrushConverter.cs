using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ForntEndMultiprog7.Model
{
    public class ProgressBarBrushConverter : IMultiValueConverter
    {
        // Methods
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Type type = typeof(double);
            if (((((values == null) || (values.Length != 5)) || ((values[0] == null) || (values[1] == null))) || (((values[2] == null) || (values[3] == null)) || ((values[4] == null) || !typeof(Brush).IsAssignableFrom(values[0].GetType())))) || ((!typeof(bool).IsAssignableFrom(values[1].GetType()) || !type.IsAssignableFrom(values[2].GetType())) || (!type.IsAssignableFrom(values[3].GetType()) || !type.IsAssignableFrom(values[4].GetType()))))
            {
                return null;
            }
            Brush brush = (Brush)values[0];
            bool flag = (bool)values[1];
            double d = (double)values[2];
            double num2 = (double)values[3];
            double num3 = (double)values[4];
            if ((((d <= 0.0) || double.IsInfinity(d)) || (double.IsNaN(d) || (num2 <= 0.0))) || (double.IsInfinity(num2) || double.IsNaN(num2)))
            {
                return null;
            }
            DrawingBrush brush2 = new DrawingBrush();
            brush2.Viewport = brush2.Viewbox = new Rect(0.0, 0.0, d, num2);
            brush2.ViewportUnits = brush2.ViewboxUnits = BrushMappingMode.Absolute;
            brush2.TileMode = TileMode.None;
            brush2.Stretch = Stretch.None;
            DrawingGroup group = new DrawingGroup();
            DrawingContext context = group.Open();
            double x = 0.0;
            double width = 6.0;
            double num6 = 2.0;
            double num7 = width + num6;
            if (flag)
            {
                int num8 = (int)Math.Ceiling((double)(d / num7));
                double num9 = -num8 * num7;
                double num10 = d * 0.3;
                brush2.Viewport = brush2.Viewbox = new Rect(num9, 0.0, num10 - num9, num2);
                TranslateTransform transform = new TranslateTransform();
                double num11 = num8 * 100;
                DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(num11));
                animation.RepeatBehavior = RepeatBehavior.Forever;
                for (int i = 1; i <= num8; i++)
                {
                    double num13 = i * num7;
                    animation.KeyFrames.Add(new DiscreteDoubleKeyFrame(num13, KeyTime.Uniform));
                }
                transform.BeginAnimation(TranslateTransform.XProperty, animation);
                brush2.Transform = transform;
                while ((x + width) < num10)
                {
                    context.DrawRectangle(brush, null, new Rect(num9 + x, 0.0, width, num2));
                    x += num7;
                }
                d = num10;
                x = 0.0;
            }
            while ((x + width) < d)
            {
                context.DrawRectangle(brush, null, new Rect(x, 0.0, width, num2));
                x += num7;
            }
            double num14 = d - x;
            if ((!flag && (num14 > 0.0)) && (Math.Abs((double)(d - num3)) < 1E-05))
            {
                context.DrawRectangle(brush, null, new Rect(x, 0.0, num14, num2));
            }
            context.Close();
            brush2.Drawing = group;
            return brush2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
