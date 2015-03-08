using Liftit.Common;
using Liftit.DataModel;
using Liftit.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Pivot Application template is documented at http://go.microsoft.com/fwlink/?LinkID=391641

namespace Liftit
{
    public sealed partial class PivotPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private AppDataModel appData;

        public PivotPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

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
            // TODO: Save the unique state of the page here.
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

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ExerciseModel exerciseOne = new ExerciseModel("SQ", "Squat", ExerciseModel.MuscleGroups.Quads, new List<ExerciseSetModel> { new ExerciseSetModel(90, 5), new ExerciseSetModel(110, 3) });
            ExerciseModel exerciseTwo = new ExerciseModel("OHP", "Overhead press", ExerciseModel.MuscleGroups.Shoulders, new List<ExerciseSetModel> { new ExerciseSetModel(50, 5), new ExerciseSetModel(55, 5) });
            ExerciseModel exerciseThree = new ExerciseModel("DL", "Deadlift", ExerciseModel.MuscleGroups.Back, new List<ExerciseSetModel> { new ExerciseSetModel(110, 5), new ExerciseSetModel(135, 5) });

            this.appData.AddWorkout("Test", new DateTime(2014, 6, 12), "Titan gym", new List<ExerciseModel> { exerciseOne, exerciseTwo, exerciseThree });
        }
    }
}
