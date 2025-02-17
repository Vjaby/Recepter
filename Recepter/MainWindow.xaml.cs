using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.Globalization;
using System.ComponentModel;

namespace Recepter {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        List<Ingredient> Ingredients = new List<Ingredient>();
        List<Step> Steps = new List<Step>();
        List<Note> Notes = new List<Note>();
        Recipe SavedRecipe = new Recipe();
        string SavedPath = "";


        public MainWindow() {
            InitializeComponent();

            ChangeLang(Properties.Settings.Default.lang);

            IngredientsItemsControl.ItemsSource = Ingredients;
            StepsItemsControl.ItemsSource = Steps;
            NotesItemsControl.ItemsSource = Notes;

            SavedRecipe.Ingredients = Ingredients.ToList();
            SavedRecipe.Steps = Steps.ToList();
            SavedRecipe.Notes = Notes.ToList();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                // opening app by draging a file onto it 
                // or file.xml -> open file with -> this thing
                OpenFileMethod(args[1].ToString());
            }
            else
            {
                // add some placeholders
                Ingredients.Add(new Ingredient()
                {
                    Name = "Pasta",
                    Amount = 200,
                    Unit = "g"
                });
                Steps.Add(new Step()
                {
                    StepId = 1,
                    StepContent = "Boil water in a pot"
                });
                Notes.Add(new Note() { NoteContent = "Helpfull tip" });
            }

            // https://stackoverflow.com/a/8398176
            // (the rest is in "Other methods" region)
            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
        }


        // Close, minimize, maximize buttons and draging
        #region controls

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
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


        // Save, open, new file
        #region File Buttons

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            /*
            if you opend or "save as-ed", made some changes and now want to "save", it should just save (duh)
            if you click on "save" and you're NOT working on an already existing recipe, it should do "save as"

            if SavedPath is empty -> save as
            else check if SavedPath exists if yes -> save
                                           if not -> save as
            */
            if (File.Exists(SavedPath) && NameTextBox.Text == System.IO.Path.GetFileName(SavedPath).Replace(".rcpt", ""))
            {
                Stream stream = File.Open(SavedPath, FileMode.Create);

                // this check maybe dosn't need to happen since we already checked File.Exists
                if (stream != null) {
                    Recipe recipe = new Recipe() {
                        Ingredients = Ingredients.ToList(), // oh boy
                        Steps = Steps.ToList(),
                        Notes = Notes.ToList()
                    };
                    XmlSerializer serializer = new XmlSerializer(typeof(Recipe));

                    serializer.Serialize(stream, recipe);
                    stream.Close();

                    SavedRecipe = recipe;
                }
            }
            else
            {
                SaveAsButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); //click on the save as button
            }
        }


        private void OpenButton_Click(object sender, RoutedEventArgs e) {
            // I took this and did my best https://learn.microsoft.com/cs-cz/dotnet/api/system.windows.forms.savefiledialog?view=windowsdesktop-8.0
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Filter = "Recipe File|*.rcpt"
            };

            if ((bool)openFileDialog.ShowDialog()) {
                string filepath = openFileDialog.FileName;
                if (!string.IsNullOrEmpty(filepath)) {
                    OpenFileMethod(filepath);
                }
            }
        }


        private void NewButton_Click(object sender, RoutedEventArgs e) {
            if (IsUnsaved()) {
                return;
            }

            Ingredients.Clear();
            Steps.Clear();
            Notes.Clear();
            NameTextBox.Text = FindResource("NewRecipe").ToString();

            ResetItemsControl();

            SavedRecipe = new Recipe {
                Ingredients = Ingredients.ToList(),
                Steps = Steps.ToList(),
                Notes = Notes.ToList()
            };
            SavedPath = "";
        }


        private void SaveAsButton_Click(object sender, RoutedEventArgs e) {
            // I took this and did my best https://learn.microsoft.com/cs-cz/dotnet/api/system.windows.forms.savefiledialog?view=windowsdesktop-8.0
            Stream stream;
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Filter = "Recipe File|*.rcpt",
                FileName = NameTextBox.Text
            };

            if ((bool)saveFileDialog.ShowDialog()) {
                if ((stream = saveFileDialog.OpenFile()) != null) {
                    Recipe recipe = new Recipe() {
                        Ingredients = Ingredients.ToList(), // oh boy
                        Steps = Steps.ToList(),
                        Notes = Notes.ToList()
                    };
                    XmlSerializer serializer = new XmlSerializer(typeof(Recipe));

                    serializer.Serialize(stream, recipe);
                    stream.Close();

                    SavedRecipe = recipe;
                    NameTextBox.Text = (saveFileDialog.SafeFileName).Replace(".rcpt", "");
                    SavedPath = saveFileDialog.FileName;
                }
            }
        }

        #endregion


        // adding a new ingredient, step, note
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


        // deleting an ingredient, step, note
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

        // Helpers and such
        #region Other methods

        //Makes sure the ItemsControls are showing what their supposed to
        //especialy after adding or deleting
        private void ResetItemsControl() {
            IngredientsItemsControl.ItemsSource = null;
            IngredientsItemsControl.ItemsSource = Ingredients;
            StepsItemsControl.ItemsSource = null;
            StepsItemsControl.ItemsSource = Steps;
            NotesItemsControl.ItemsSource = null;
            NotesItemsControl.ItemsSource = Notes;
            //if it works...
        }


        /// <summary>
        ///     Chcecks for unsaved changes.
        ///     False is like saying "go ahead".
        ///     True is like saying "Stop! I still have work to do".
        /// </summary>
        /// <returns>
        ///     <list type="bullet">
        ///         <item>
        ///         False if the curent recipe is the same as the last "SavedRecipe"
        ///         </item>
        ///         <item>
        ///         if it's not -> ask "wanna save?"
        ///         </item>
        ///         
        ///         <list type="bullet">
        ///             <item>
        ///             False if the user saves
        ///             </item>
        ///             <item>
        ///             False if the user doesn't want to save
        ///             </item>
        ///             <item>
        ///             True if the user wants to cancle
        ///             </item>
        ///         </list>
        ///     </list>
        /// </returns>
        private bool IsUnsaved() {
            string recipeString;
            string savedRecipeString;

            Recipe recipe = new Recipe() {
                Ingredients = Ingredients.ToList(), // oh boy
                Steps = Steps.ToList(),
                Notes = Notes.ToList()
            };

            //create strings out of the recipe and SavedRecipe objects and comapre those
            //because .Equals (or ==) doesn't work (because of complex objects (i guess))
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Recipe));
            using (StringWriter textWriter = new StringWriter()) {
                xmlSerializer.Serialize(textWriter, recipe);
                recipeString = textWriter.ToString();
            }
            using (StringWriter textWriter = new StringWriter()) {
                xmlSerializer.Serialize(textWriter, SavedRecipe);
                savedRecipeString = textWriter.ToString();
            }

            //if the last saved recipe isnt the same as the recipe now...
            if (savedRecipeString != recipeString)
            {
                var result = MessageBox.Show(FindResource("WantToSave").ToString(),
                                             FindResource("UnsavedChanges").ToString(),
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
                else {
                    return true;
                }
            }
            else {
                return false;
            }
        }


        #region Language

        private void LangButtons_Click(object sender, RoutedEventArgs e) {
            //https://www.youtube.com/watch?v=FJSJLf76mBM
            //https://www.azulcoding.com/wpf-multilingual/

            ChangeLang(((Button)sender).Tag.ToString());
        }


        private void ChangeLang(string lang) {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            Application.Current.Resources.MergedDictionaries.Clear();
            ResourceDictionary resdict = new ResourceDictionary() {
                Source = new Uri($"/Dictionary-{lang}.xaml", UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Add(resdict);

            CSButton.IsEnabled = true;
            DEButton.IsEnabled = true;
            ENButton.IsEnabled = true;

            switch (lang) {
                case "cs-CZ":
                    CSButton.IsEnabled = false;
                    break;
                case "de-DE":
                    DEButton.IsEnabled = false;
                    break;
                case "en-GB":
                    ENButton.IsEnabled = false;
                    break;
                default:
                    break;
            }

            Properties.Settings.Default.lang = lang;
            Properties.Settings.Default.Save();
        }

        #endregion


        private void OpenFileMethod(string filepath) {
            // https://learn.microsoft.com/cs-cz/dotnet/api/system.windows.forms.savefiledialog?view=windowsdesktop-8.0
            if (IsUnsaved()) {
                return;
            }

            if (Path.GetExtension(filepath) != ".rcpt") {
                MessageBox.Show(FindResource("FileTypeError").ToString(),
                                "",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Recipe));
            Stream st = File.Open(filepath, FileMode.Open);
            SavedRecipe = (Recipe)(serializer.Deserialize(st));

            Ingredients = SavedRecipe.Ingredients.ToList();
            Steps = SavedRecipe.Steps.ToList();
            Notes = SavedRecipe.Notes.ToList();
            NameTextBox.Text = (Path.GetFileNameWithoutExtension(filepath));
            SavedPath = filepath;

            ResetItemsControl();
            st.Close();
        }


        private void Window_Drop(object sender, DragEventArgs e) {
            // https://stackoverflow.com/questions/5662509/drag-and-drop-files-into-wpf
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                string filepath = Path.GetFullPath(files[0]);
                OpenFileMethod(filepath);
            }
        }


        // https://stackoverflow.com/a/8398176
        // (the first bit is in "public MainWindow()")
        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (IsUnsaved())
            {
                // cancle the closing event
                e.Cancel = true;
            }
        }

        #endregion
    }


    // to use xmlserializer the constructor must be parameterless
    // so in this case, nothing
    // otherwise it could've been usefull
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
