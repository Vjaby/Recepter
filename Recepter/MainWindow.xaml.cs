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

namespace Recepter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<Ingredient> Ingredients = new List<Ingredient>
		{
			new Ingredient("potat", 10, "mg"), // for easier testing, REMOVE LATER
			new Ingredient("ham", 20, "kg")
		};

		List<Step> Steps = new List<Step>
		{
			new Step(1, "do this"),
			new Step(2, "do that"),
		};

		List<Note> Notes = new List<Note>
		{
			new Note("Oh boy"),
			new Note("Hmmmm")
		};

		public MainWindow()
		{
			InitializeComponent();
			IngredientsItemsControl.ItemsSource = Ingredients;
			StepsItemsControl.ItemsSource = Steps;
			NotesItemsControl.ItemsSource = Notes;

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

		#region File Buttons
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
        #endregion

        #region Add Buttons
        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
		{
			Ingredients.Add(new Ingredient("", 0, ""));
			IngredientsItemsControl.ItemsSource = null;
			IngredientsItemsControl.ItemsSource = Ingredients; //if it works...
		}

		private void AddStepButton_Click(object sender, RoutedEventArgs e)
		{
			Steps.Add(new Step(Steps.Count + 1, ""));
			StepsItemsControl.ItemsSource = null;
			StepsItemsControl.ItemsSource = Steps; //if it works...
		}

		private void AddNoteButton_Click(object sender, RoutedEventArgs e)
		{
			Notes.Add(new Note(""));
			NotesItemsControl.ItemsSource = null;
			NotesItemsControl.ItemsSource = Notes; //if it works...
		}
		#endregion

		#region Delete Buttons
		private void NoteDeleteButton_Click(object sender, RoutedEventArgs e)
		{
			//I have never seen this syntax (though that isnt saying much)
			//anyway, taken from here: https://stackoverflow.com/questions/16342885/access-other-xaml-controls-data-in-another-controls-event?rq=3
			Button btn = (Button)sender;
			Note btnDataContext = (Note)btn.DataContext;

			/* a bit of history
			at first I thought I would use RemoveAt for this, but then
			this simpler thing came out at random out of IntelliSense/Code

			just a reminder to thank our Microsoft overlords
			*/
			Notes.Remove(btnDataContext);
			NotesItemsControl.ItemsSource = null;
			NotesItemsControl.ItemsSource = Notes;
		}

		private void StepDeleteButton_Click(object sender, RoutedEventArgs e)
		{
			Button btn = (Button)sender;
			Step btnDataContext = (Step)btn.DataContext;

			Steps.Remove(btnDataContext);
			for (int i = 0; i < Steps.Count(); i++)
			{
				Steps[i].StepId = i + 1;
			}
			StepsItemsControl.ItemsSource = null;
			StepsItemsControl.ItemsSource = Steps;
		}

		private void IngredientDeleteButton_Click(object sender, RoutedEventArgs e)
		{
			Button btn = (Button)sender;
			Ingredient btnDataContext = (Ingredient)btn.DataContext;

			Ingredients.Remove(btnDataContext);
			IngredientsItemsControl.ItemsSource = null;
			IngredientsItemsControl.ItemsSource = Ingredients;
		} 
		#endregion
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

	public class Step
	{
		public Step(int stepId, string stepContent)
		{
			StepId = stepId;
			StepContent = stepContent;
		}

		public int StepId { get; set; }
		public string StepContent { get; set; }

	}

	public class Note
	{
		public Note(string noteContent)
		{
			NoteContent = noteContent;
		}

		public string NoteContent { get; set; }
	}
}
