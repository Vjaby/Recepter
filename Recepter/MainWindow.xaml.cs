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
using System.IO;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace Recepter {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        List<Ingredient> Ingredients = new List<Ingredient> { };
        List<Step> Steps = new List<Step> { };
        List<Note> Notes = new List<Note> { };

        public MainWindow() {
            InitializeComponent();

            // to use xmlserializer the constructor must be parameterless
            // DELETE LATER (or maybe leave it like a html text placeholder or actualy do that but propperly... can you?)
            Ingredients.Add(new Ingredient() {
                Name = "potat",
                Amount = 2,
                Unit = "kg"
            });
            Ingredients.Add(new Ingredient() {
                Name = "ham",
                Amount = 220,
                Unit = "g"
            });
            Steps.Add(new Step() {
                StepId = 1,
                StepContent = "do this"
            });
            Steps.Add(new Step() {
                StepId = 2,
                StepContent = "to that"
            });
            Notes.Add(new Note() { NoteContent = "maybe" });
            Notes.Add(new Note() { NoteContent = "you can" });

            IngredientsItemsControl.ItemsSource = Ingredients;
            StepsItemsControl.ItemsSource = Steps;
            NotesItemsControl.ItemsSource = Notes;
        }

        // Close, minimize, maximize buttons and draging
        #region controls
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void MinButton_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void MaxButton_Click(object sender, RoutedEventArgs e) {
            if (WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
                return;
            }
            WindowState = WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }
        #endregion

        //Save, open, new file
        #region File Buttons
        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("save");
        }
        private void OpenButton_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("open");
        }

        private void NewButton_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("new");
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e) {

            Recipe recipe = new Recipe() {
                Ingredients = Ingredients, // oh boy
                Steps = Steps,
                Notes = Notes
            };
            XmlSerializer serializer = new XmlSerializer(typeof(Recipe));

            // I took this and did my best https://learn.microsoft.com/cs-cz/dotnet/api/system.windows.forms.savefiledialog?view=windowsdesktop-8.0
            Stream stream = null;
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Filter = "xml|*.xml"
            };

            if ((bool)saveFileDialog.ShowDialog()) {
                if ((stream = saveFileDialog.OpenFile()) != null) {
                    serializer.Serialize(stream, recipe);
                    stream.Close();
                }
            }

            stream.Dispose(); // maybe not needed especialy after "stream.Close() but just to be safe
        }
        #endregion

        //adding a new ingredient, step, note
        #region Add Buttons
        private void AddIngredientButton_Click(object sender, RoutedEventArgs e) {
            Ingredients.Add(new Ingredient());
            IngredientsItemsControl.ItemsSource = null;
            IngredientsItemsControl.ItemsSource = Ingredients; //if it works...
        }

        private void AddStepButton_Click(object sender, RoutedEventArgs e) {
            Steps.Add(new Step() { StepId = Steps.Count + 1});
            StepsItemsControl.ItemsSource = null;
            StepsItemsControl.ItemsSource = Steps; //if it works...
        }

        private void AddNoteButton_Click(object sender, RoutedEventArgs e) {
            Notes.Add(new Note());
            NotesItemsControl.ItemsSource = null;
            NotesItemsControl.ItemsSource = Notes; //if it works...
        }
        #endregion

        //deleting an ingredient, step, note
        #region Delete Buttons
        private void NoteDeleteButton_Click(object sender, RoutedEventArgs e) {
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

        private void StepDeleteButton_Click(object sender, RoutedEventArgs e) {
            Button btn = (Button)sender;
            Step btnDataContext = (Step)btn.DataContext;

            Steps.Remove(btnDataContext);
            for (int i = 0; i < Steps.Count(); i++) {
                Steps[i].StepId = i + 1;
            }
            StepsItemsControl.ItemsSource = null;
            StepsItemsControl.ItemsSource = Steps;
        }

        private void IngredientDeleteButton_Click(object sender, RoutedEventArgs e) {
            Button btn = (Button)sender;
            Ingredient btnDataContext = (Ingredient)btn.DataContext;

            Ingredients.Remove(btnDataContext);
            IngredientsItemsControl.ItemsSource = null;
            IngredientsItemsControl.ItemsSource = Ingredients;
        }
        #endregion
    }

    public class Ingredient {
        public string Name { get; set; }
        public int Amount { get; set; }
        public string Unit { get; set; }
    }

    public class Step {

        public int StepId { get; set; }
        public string StepContent { get; set; }

    }

    public class Note {
        public string NoteContent { get; set; }
    }

    public class Recipe {
        public List<Ingredient> Ingredients { get; set; }
        public List<Step> Steps { get; set; }
        public List<Note> Notes { get; set; }
    }
}
