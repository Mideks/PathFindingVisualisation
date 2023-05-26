using PathFindingVisualization.View;
using PathFindingVisualization.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PathFindingVisualization
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем объект VisualViewModel
            var viewModel = new VisualViewModel();

            // Создаем объект VisualView
            var view = new VisualView
            {
                // Привязываем VisualView к VisualViewModel
                DataContext = viewModel
            };

            // Отображаем VisualView
            view.Show();
        }
    }
}
