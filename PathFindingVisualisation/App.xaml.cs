using PathFindingVisualisation.View;
using PathFindingVisualisation.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PathFindingVisualisation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаём объект VisualViewModel
            var viewModel = new VisualViewModel();

            // Создаём объект VisualView
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
