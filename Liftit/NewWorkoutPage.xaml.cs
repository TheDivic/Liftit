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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Liftit
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.DefaultViewModel["appData"] = appData;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
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

        private void AddExerciseButton_Click(object sender, RoutedEventArgs e)
        {
            StackPanel newExercisePanel = new StackPanel();
            newExercisePanel.Orientation = Orientation.Horizontal;
            newExercisePanel.HorizontalAlignment = HorizontalAlignment.Stretch;

            int exerciseNumber = FinishedExercisesPanel.Children.Count + 1;
            TextBlock numberTextBlock = new TextBlock();
            numberTextBlock.FontSize = 30;
            numberTextBlock.Text = exerciseNumber + ". ";
            numberTextBlock.Margin = new Thickness(0, 10, 10, 0);
            newExercisePanel.Children.Add(numberTextBlock);

            ComboBox exerciseNamesComboBox = new ComboBox();
            exerciseNamesComboBox.ItemsSource = appData.KnownExercises;
            object comboItemTemplateObject;
            this.Resources.TryGetValue("ComboItemTemplate", out comboItemTemplateObject);
            exerciseNamesComboBox.ItemTemplate = (DataTemplate)comboItemTemplateObject;

            exerciseNamesComboBox.Margin = new Thickness(0,0,0,0);

            newExercisePanel.Children.Add(exerciseNamesComboBox);

            FinishedExercisesPanel.Children.Add(newExercisePanel);
        }

    }
}
