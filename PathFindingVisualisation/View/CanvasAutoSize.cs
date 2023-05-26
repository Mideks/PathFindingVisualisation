using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PathFindingVisualisation.View
{
    public class CanvasAutoSize : Canvas
    {
        protected override Size MeasureOverride(Size constraint)
        {
            double width = 0;
            double height = 0;

            foreach (FrameworkElement element in InternalChildren)
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                double right = GetLeft(element) + element.DesiredSize.Width;
                double bottom = GetTop(element) + element.DesiredSize.Height;

                if (!double.IsNaN(right))
                {
                    width = Math.Max(width, right);
                }

                if (!double.IsNaN(bottom))
                {
                    height = Math.Max(height, bottom);
                }
            }

            return new Size(width, height);
        }
    }
}
