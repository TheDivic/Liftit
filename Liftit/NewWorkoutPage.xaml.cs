using Liftit.Common;
using Liftit.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Liftit
{
    public sealed partial class NewWorkoutPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public AppDataModel appData;

        public NewWorkoutPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.appData = (App.Current as App).appData;
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.DefaultViewModel["appData"] = appData;
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        // TODO: add validation for user input
        /// <summary>
        /// Saves the tracked workout to the model
        /// </summary>
        private void SaveWorkoutButton_Click(object sender, RoutedEventArgs e)
        {
            List<FinishedExerciseModel> finishedExercises = new List<FinishedExerciseModel>();
            
            foreach (StackPanel sp in FinishedExercisesPanel.Children)
            {
                var combo = (ComboBox)sp.Children.OfType<ComboBox>().FirstOrDefault();
                if (combo != null && combo.SelectedItem != null)
                {
                    finishedExercises.Add(new FinishedExerciseModel((ExerciseModel)combo.SelectedItem, new List<ExerciseSetModel>()));
                }
            }

            appData.AddWorkout(WorkoutName.Text, WorkoutDate.Date.DateTime, WorkoutLocation.Text, finishedExercises);
            Frame.Navigate(typeof(PivotPage));
        }

        /// <summary>
        /// Adds a new exercise to the FinishedExercisesPanel
        /// </summary>
        private void AddExerciseButton_Click(object sender, RoutedEventArgs e)
        {
            var newExercisePanel = new StackPanel();
            newExercisePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            newExercisePanel.Margin = new Thickness(0, 0, 0, 10);

            ComboBox exerciseNamesComboBox = new ComboBox();
            exerciseNamesComboBox.ItemsSource = appData.KnownExercises;
            object comboItemTemplateObject;
            this.Resources.TryGetValue("ComboItemTemplate", out comboItemTemplateObject);
            exerciseNamesComboBox.ItemTemplate = (DataTemplate)comboItemTemplateObject;
            exerciseNamesComboBox.PlaceholderText = "Tap to choose an exercise";
            exerciseNamesComboBox.SelectionChanged += new SelectionChangedEventHandler(ExerciseNamesComboBox_SelectionChanged);
            exerciseNamesComboBox.Margin = new Thickness(0, 0, 0, 0);
            newExercisePanel.Children.Add(exerciseNamesComboBox);

            var setsPanel = new StackPanel();
            setsPanel.Tag = "SetsPanel";
            newExercisePanel.Children.Add(setsPanel);

            FinishedExercisesPanel.Children.Add(newExercisePanel);
        }

        //TODO: fix setsPanel positioning
        /// <summary>
        /// Display the form for entering the sets when the exercise name is chosen
        /// </summary>
        private void ExerciseNamesComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox senderBox = (ComboBox)sender;
            StackPanel parentPanel = (StackPanel)senderBox.Parent;

            StackPanel SetsPanel;

            // find the set list panel if it exists
            var panelChildren = parentPanel.Children.OfType<StackPanel>();
            SetsPanel = panelChildren.Where(panel=>(string)panel.Tag == "SetsPanel").FirstOrDefault();

            if (SetsPanel != null && SetsPanel.Children.Count == 0)
            {
                TextBlock setsHeader = new TextBlock();
                setsHeader.Text = "My sets:";
                setsHeader.FontSize = 24;
                SetsPanel.Children.Add(setsHeader);

                var SetsListPanel = new StackPanel();
                SetsPanel.Children.Add(SetsListPanel);

                SetsListPanel.Children.Add(CreateOneSetStackPanel(0));

                Button newSetButton = new Button();
                newSetButton.Content = "Add a set";
                newSetButton.Click += new RoutedEventHandler(newSetButton_Click);

                SetsPanel.Children.Add(newSetButton);
            }
        }

        private void newSetButton_Click(object sender, RoutedEventArgs e)
        {
            StackPanel SetsListPanel;
            StackPanel SetsPanel = (StackPanel)((Button)sender).Parent;

            SetsListPanel = SetsPanel.Children.OfType<StackPanel>().FirstOrDefault();

            if (SetsListPanel != null)
            {
                SetsListPanel.Children.Add(CreateOneSetStackPanel(SetsListPanel.Children.Count));
            }
        }

        private StackPanel CreateOneSetStackPanel(int panelIndex)
        {
            StackPanel oneSetStackPanel = new StackPanel();
            oneSetStackPanel.Orientation = Orientation.Horizontal;

            int setNumber = panelIndex;
            TextBlock setNumberTextBlock = new TextBlock();
            setNumberTextBlock.FontSize = 24;
            setNumberTextBlock.Text = setNumber + ". ";
            setNumberTextBlock.Margin = new Thickness(0, 15, 5, 0);
            oneSetStackPanel.Children.Add(setNumberTextBlock);

            InputScope repsScope = new InputScope();
            InputScopeName numberScopeName = new InputScopeName();
            numberScopeName.NameValue = InputScopeNameValue.Number;
            repsScope.Names.Add(numberScopeName);

            TextBox repsTextBox = new TextBox();
            repsTextBox.InputScope = repsScope;
            repsTextBox.PlaceholderText = "reps";
            oneSetStackPanel.Children.Add(repsTextBox);

            TextBlock timesText = new TextBlock();
            timesText.FontSize = 24;
            timesText.Margin = new Thickness(5, 15, 5, 0);
            timesText.Text = "times";
            oneSetStackPanel.Children.Add(timesText);

            InputScope weightScope = new InputScope();
            InputScopeName weightScopeName = new InputScopeName();
            weightScopeName.NameValue = InputScopeNameValue.Number;
            weightScope.Names.Add(weightScopeName);

            TextBox weightTextBox = new TextBox();
            weightTextBox.InputScope = weightScope;
            weightTextBox.PlaceholderText = "weight";
            oneSetStackPanel.Children.Add(weightTextBox);

            Button deleteSetButton = new Button();
            deleteSetButton.Content = "delete";
            deleteSetButton.Margin = new Thickness(5, 0, 0, 0);
            deleteSetButton.Click += deleteSetButton_Click;
            oneSetStackPanel.Children.Add(deleteSetButton);

            return oneSetStackPanel;
        }

        void deleteSetButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var setPanel = (StackPanel)button.Parent;
            var SetsListPanel = (StackPanel)setPanel.Parent;
            SetsListPanel.Children.Remove(setPanel);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Flip");
            
        }
    }
}
