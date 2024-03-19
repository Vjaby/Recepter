using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace src
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			List<Ingredient> ingredients = new List<Ingredient>
			{
				new Ingredient("potat", 10, "mg"),
				new Ingredient("ham", 20, "kg")
			};
			IngredientsItemsControl.ItemsSource = ingredients;
		}

		// Close, minimize, maximize buttons and draging
		#region controls
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void MinButton_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void MaxButton_Click(object sender, RoutedEventArgs e)
		{
			if (WindowState == WindowState.Maximized)
			{
				WindowState = WindowState.Normal;
				return;
			}
			WindowState = WindowState.Maximized;
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		#endregion

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("save");
		}
		private void OpenButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("open");
		}

		private void NewButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("new");
		}

		private void SaveAsButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("saveAs");
		}
	}

	public class Ingredient
	{
        public Ingredient(string name, int amount, string unit)
        {
            Name = name;
			Amount = amount;
			Unit = unit;

        }

        public string Name { get; set; }
		public int Amount { get; set; }
		public string Unit { get; set; }
	}

}
