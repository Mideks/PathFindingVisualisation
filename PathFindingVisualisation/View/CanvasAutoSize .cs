using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PathFindingVisualization.View
{
    public class CanvasAutoSize : Canvas
    {
        protected override Size MeasureOverride(Size constraint)
        {
            double width = 0;
            double height = 0;

            foreach (FrameworkElement element in InternalChildren)
            {
                double right = GetLeft(element) + element.ActualWidth;
                double bottom = GetTop(element) + element.ActualHeight;
                
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
