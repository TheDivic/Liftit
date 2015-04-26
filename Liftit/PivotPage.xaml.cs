using Liftit.Common;
using Liftit.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

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

            List<MonthStatistics> stats = new List<MonthStatistics>() { new MonthStatistics(1, 2015, 500), new MonthStatistics(2, 2015, 700), new MonthStatistics(3,2015, 600)};
            this.DefaultViewModel["statistics"] = stats;

            RenderWorkoutsGrayArc();
            RenderPercentageGrayArc();
            RenderWorkoutsArc();
            RenderPercentageArc();
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

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewWorkoutPage));
        }

        private void WorkoutName_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBlock senderPanel = (TextBlock)sender;
            Frame.Navigate(typeof(FinishedWorkoutPage), senderPanel.DataContext);
        }


        private async void DeleteWorkoutButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            WorkoutModel workoutToDelete = (WorkoutModel)button.DataContext;
            await appData.DeleteWorkout(workoutToDelete);
        }

        private double Radius = 80;
        private double WorkoutsAngle;
        private double CircleThickness = 6;
        private double PercentageAngle;
        private double CircleMargin = 10;


        public void RenderWorkoutsArc()
        {

            WorkoutsAngle = (80 * 360) / 100;

            Point startPoint = new Point(Radius, 0);
            Point endPoint = ComputeCartesianCoordinate(WorkoutsAngle, Radius);
            endPoint.X += Radius;
            endPoint.Y += Radius;

            WorkoutsCircle.Width = Radius * 2 + CircleThickness;
            WorkoutsCircle.Height = Radius * 2 + CircleThickness;
            WorkoutsCircle.Margin = new Thickness(CircleThickness, CircleThickness + CircleMargin, 0, 0);

            bool largeArc = WorkoutsAngle > 180.0;

            Size outerArcSize = new Size(Radius, Radius);

            WorkoutsPathFigure.StartPoint = startPoint;

            if (startPoint.X == Math.Round(endPoint.X) && startPoint.Y == Math.Round(endPoint.Y))
                endPoint.X -= 0.01;

            WorkoutsArcSegment.Point = endPoint;
            WorkoutsArcSegment.Size = outerArcSize;
            WorkoutsArcSegment.IsLargeArc = largeArc;
        }

        public void RenderWorkoutsGrayArc()
        {
            Point startPoint = new Point(Radius, 0);
            Point endPoint = ComputeCartesianCoordinate(360, Radius);
            endPoint.X += Radius;
            endPoint.Y += Radius;

            WorkoutsGrayCircle.Width = Radius * 2 + CircleThickness;
            WorkoutsGrayCircle.Height = Radius * 2 + CircleThickness;
            WorkoutsGrayCircle.Margin = new Thickness(CircleThickness, CircleThickness + CircleMargin, 0, 0);

            bool largeArc = 360 > 180.0;

            Size outerArcSize = new Size(Radius, Radius);

            WorkoutsGrayPathFigure.StartPoint = startPoint;

            if (startPoint.X == Math.Round(endPoint.X) && startPoint.Y == Math.Round(endPoint.Y))
                endPoint.X -= 0.01;

            WorkoutsGrayArcSegment.Point = endPoint;
            WorkoutsGrayArcSegment.Size = outerArcSize;
            WorkoutsGrayArcSegment.IsLargeArc = largeArc;
        }

        public void RenderPercentageGrayArc()
        {
            Point startPoint = new Point(Radius, 0);
            Point endPoint = ComputeCartesianCoordinate(360, Radius);
            endPoint.X += Radius;
            endPoint.Y += Radius;

            PercentageGrayCircle.Width = Radius * 2 + CircleThickness;
            PercentageGrayCircle.Height = Radius * 2 + CircleThickness;
            PercentageGrayCircle.Margin = new Thickness(CircleThickness, CircleThickness + CircleMargin, 0, 0);

            bool largeArc = 360 > 180.0;

            Size outerArcSize = new Size(Radius, Radius);

            PercentageGrayPathFigure.StartPoint = startPoint;

            if (startPoint.X == Math.Round(endPoint.X) && startPoint.Y == Math.Round(endPoint.Y))
                endPoint.X -= 0.01;

            PercentageGrayArcSegment.Point = endPoint;
            PercentageGrayArcSegment.Size = outerArcSize;
            PercentageGrayArcSegment.IsLargeArc = largeArc;
        }

        public void RenderPercentageArc()
        {

            PercentageAngle = (60 * 360) / 100;

            Point startPoint = new Point(Radius, 0);
            Point endPoint = ComputeCartesianCoordinate(PercentageAngle, Radius);
            endPoint.X += Radius;
            endPoint.Y += Radius;

            PercentageCircle.Width = Radius * 2 + CircleThickness;
            PercentageCircle.Height = Radius * 2 + CircleThickness;
            PercentageCircle.Margin = new Thickness(CircleThickness, CircleThickness + CircleMargin, 0, 0);

            bool largeArc = PercentageAngle > 180.0;

            Size outerArcSize = new Size(Radius, Radius);

            PercentagePathFigure.StartPoint = startPoint;

            if (startPoint.X == Math.Round(endPoint.X) && startPoint.Y == Math.Round(endPoint.Y))
                endPoint.X -= 0.01;

            PercentageArcSegment.Point = endPoint;
            PercentageArcSegment.Size = outerArcSize;
            PercentageArcSegment.IsLargeArc = largeArc;
        }


        private Point ComputeCartesianCoordinate(double angle, double radius)
        {
            double angleRad = (Math.PI / 180.0) * (angle - 90);

            double x = radius * Math.Cos(angleRad);
            double y = radius * Math.Sin(angleRad);

            return new Point(x, y);
        }
    }

    public class MonthStatistics
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public float WeightLifted { get; set; }
        public string MonthName { get; set; }



        public MonthStatistics(int month, int year, float weight)
        {
            this.Month = month;
            this.Year = year;
            this.WeightLifted = weight;
            System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
            this.MonthName = mfi.GetAbbreviatedMonthName(this.Month);
        }

        private void CalculateStatistics()
        {
            Random rand = new Random();
            this.WeightLifted = rand.Next(0, 1000);
        }
    }

}
