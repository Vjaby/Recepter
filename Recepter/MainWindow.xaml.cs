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
using System.Windows.Interop;
using System.Windows.Controls.Primitives;

namespace Recepter {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        List<Ingredient> Ingredients = new List<Ingredient>();
        List<Step> Steps = new List<Step>();
        List<Note> Notes = new List<Note>();
        Recipe SavedRecipe = new Recipe();

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

            SavedRecipe.Ingredients = Ingredients;
            SavedRecipe.Steps = Steps;
            SavedRecipe.Notes = Notes;
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
            if (IsUnsaved()) {
                return;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Recipe));

            // I took this and did my best https://learn.microsoft.com/cs-cz/dotnet/api/system.windows.forms.savefiledialog?view=windowsdesktop-8.0
            Stream stream = null;
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Filter = "xml|*.xml"
            };

            if ((bool)openFileDialog.ShowDialog()) {
                if ((stream = openFileDialog.OpenFile()) != null) {
                    SavedRecipe = (Recipe)(serializer.Deserialize(stream));
                    stream.Close();
                }
            }

            stream.Dispose(); // maybe not needed especialy after "stream.Close() but just to be safe

            Ingredients = SavedRecipe.Ingredients;
            Steps = SavedRecipe.Steps;
            Notes = SavedRecipe.Notes;
            ResetItemsControl();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e) {

            if (IsUnsaved()) {
                return;
            }

            Ingredients.Clear();
            Steps.Clear();
            Notes.Clear();

            ResetItemsControl();

            SavedRecipe = new Recipe {
                Ingredients = Ingredients,
                Steps = Steps,
                Notes = Notes
            };
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

            SavedRecipe = recipe;
        }
        #endregion

        //adding a new ingredient, step, note
        #region Add Buttons
        private void AddIngredientButton_Click(object sender, RoutedEventArgs e) {
            Ingredients.Add(new Ingredient());
            ResetItemsControl();
        }

        private void AddStepButton_Click(object sender, RoutedEventArgs e) {
            Steps.Add(new Step() { StepId = Steps.Count + 1});
            ResetItemsControl();
        }

        private void AddNoteButton_Click(object sender, RoutedEventArgs e) {
            Notes.Add(new Note());
            ResetItemsControl();
        }
        #endregion

        //deleting an ingredient, step, note
        #region Delete Buttons
        private void NoteDeleteButton_Click(object sender, RoutedEventArgs e) {
            //inspiration here: https://stackoverflow.com/questions/16342885/access-other-xaml-controls-data-in-another-controls-event?rq=3
            Button btn = (Button)sender;
            Note btnDataContext = (Note)btn.DataContext;
            Notes.Remove(btnDataContext);
            ResetItemsControl();
        }

        private void StepDeleteButton_Click(object sender, RoutedEventArgs e) {
            Button btn = (Button)sender;
            Step btnDataContext = (Step)btn.DataContext;

            Steps.Remove(btnDataContext);
            for (int i = 0; i < Steps.Count(); i++) {
                Steps[i].StepId = i + 1;
            }
            ResetItemsControl();
        }

        private void IngredientDeleteButton_Click(object sender, RoutedEventArgs e) {
            Button btn = (Button)sender;
            Ingredient btnDataContext = (Ingredient)btn.DataContext;

            Ingredients.Remove(btnDataContext);
            ResetItemsControl();
        }
        #endregion

        /*
        Makes sure the ItemsControls are showing, what their supposed to
        especialy after adding or deleting
        */
        private void ResetItemsControl() {
            IngredientsItemsControl.ItemsSource = null;
            IngredientsItemsControl.ItemsSource = Ingredients;
            StepsItemsControl.ItemsSource = null;
            StepsItemsControl.ItemsSource = Steps;
            NotesItemsControl.ItemsSource = null;
            NotesItemsControl.ItemsSource = Notes;
            //if it works...
        }

        /*
         chcecks for unsaved changes
         if the curent recipe is the same as the last "SavedRecipe" -> false
         if not -> ask "wanna save?" -> yes -> save -> false
                                     -> no -> false
                                     -> cancel -> true
         false is like saying "go ahead"
         true is like saying "stop! I still have work to do"
         */
        private bool IsUnsaved() {
            Recipe recipe = new Recipe() {
                Ingredients = Ingredients, // oh boy
                Steps = Steps,
                Notes = Notes
            };

            //if the last saved recipe isnt the same as the recipe now...
            if (SavedRecipe != recipe) /* ****DOESNT WORK, IS ALWAYS TRUE**** */
            {
                var result = MessageBox.Show("Do you want to save this recipe?",
                                             "UNSAVED CHANGES",
                                             MessageBoxButton.YesNoCancel,
                                             MessageBoxImage.Warning,
                                             MessageBoxResult.Cancel);
                if (result == MessageBoxResult.Yes) {
                    SaveButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); //click on the save button
                    return false;
                }
                else if (result == MessageBoxResult.No) {
                    return false;
                }
            }
            return true;
        }
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
