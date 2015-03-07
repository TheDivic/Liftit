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

        public PivotPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
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
            var appData = (App.Current as App).appData;
            this.DefaultViewModel["User"] = appData.User;
            this.DefaultViewModel["WorkoutsThisMonth"] = 5;
            this.DefaultViewModel["WorkoutsBehindSchedule"] = 2;
            this.DefaultViewModel["WorkoutsByWeek"] = GroupWorkoutsByWeek(appData.TrackedWorkouts);
        }

        private Dictionary<string, List<WorkoutModel>> GroupWorkoutsByWeek(ObservableCollection<WorkoutModel> workoutsModel)
        {
            var workoutsByWeekQuery = workoutsModel.Select(workout => new { Week = GetWeekFromDate(workout.WorkoutDate), Workout = workout });

            Dictionary<string, List<WorkoutModel>> workouts = new Dictionary<string, List<WorkoutModel>>();
            foreach (var workoutPair in workoutsByWeekQuery)
            {
                if (workouts.ContainsKey(workoutPair.Week))
                {
                    workouts[workoutPair.Week].Add(workoutPair.Workout);
                }
                else
                {
                    workouts.Add(workoutPair.Week, new List<WorkoutModel>() { workoutPair.Workout });
                }
            }
            return workouts;
        }

        /// <summary>
        /// Get a string representing the week of a given date
        /// </summary>
        private string GetWeekFromDate(DateTime dateTime)
        {
            var day = dateTime.Day;
            var dayOfWeek = (int)dateTime.DayOfWeek - 1;
            var startOfWeek = dateTime.AddDays(-dayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);
            return String.Format("Week {0} to {1}", startOfWeek.ToString("dd.MM"), endOfWeek.ToString("dd.MM"));
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
    }
}
