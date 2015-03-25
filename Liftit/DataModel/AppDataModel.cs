using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liftit.Common;

namespace Liftit.DataModel
{
    /// <summary>
    /// The data model for the application
    /// </summary>
    public class AppDataModel : INotifyPropertyChanged
    {
        public UserModel User { get; private set; }
        public ObservableCollection<WorkoutModel> TrackedWorkouts { get; private set; }
        // Workouts grouped by week
        public Dictionary<string, List<WorkoutModel>> WorkoutsByWeek { get; private set; }
        public ObservableCollection<ExerciseModel> KnownExercises {get; private set; }
        
        public int WorkoutsThisMonth { get; private set; }
        public int WorkoutsBehindSchedule { get; private set; }

        public AppDataModel()
        {
            this.User = new UserModel("Anonymous", 0, "nil", 0);
            this.TrackedWorkouts = new ObservableCollection<WorkoutModel>();
            this.WorkoutsByWeek = new Dictionary<string,List<WorkoutModel>>();
            this.KnownExercises = new ObservableCollection<ExerciseModel>() 
            { 
                new ExerciseModel("SQ", "Squat", ExerciseModel.MuscleGroups.Quads),
                new ExerciseModel("DL", "Deadlift", ExerciseModel.MuscleGroups.Back), 
                new ExerciseModel("OHP", "Overhead press", ExerciseModel.MuscleGroups.Shoulders),
                new ExerciseModel("BP", "Bench press", ExerciseModel.MuscleGroups.Chest), 
                new ExerciseModel("BC", "Bicep curls", ExerciseModel.MuscleGroups.Biceps),
                new ExerciseModel("LU", "Lunges", ExerciseModel.MuscleGroups.Quads),
                new ExerciseModel("TRE", "Triceps rope extensions", ExerciseModel.MuscleGroups.Triceps)
            };
        }

        public void LoadDataFromMemory()
        {
            LoadTestData();
            this.WorkoutsThisMonth = TrackedWorkouts.Where(x => x.WorkoutDate.Year == DateTime.Now.Year && x.WorkoutDate.Month == DateTime.Now.Month).Count() ;
            this.WorkoutsBehindSchedule = this.User.WorkoutsPerWeek * 4 - this.WorkoutsThisMonth;
        }

        private void LoadTestData()
        {
            // Create user
            this.User = new UserModel("Divic", 92, "kg", 4);

            // create personal records
            this.User.DisplayedPersonalRecords.Add(new PersonalRecord("SQ", "Squat", new DateTime(2014, 5, 12), 130, 2));
            this.User.DisplayedPersonalRecords.Add(new PersonalRecord("DL", "Deadlift", new DateTime(2014, 5, 30), 135, 5));
            this.User.DisplayedPersonalRecords.Add(new PersonalRecord("BP", "Bench press", new DateTime(2014, 10, 23), 105, 2));
            this.User.DisplayedPersonalRecords.Add(new PersonalRecord("OHP", "Overhead press", new DateTime(2014,10,25), 70, 2));

            //create exercises
            FinishedExerciseModel exerciseOne = new FinishedExerciseModel("SQ", "Squat", ExerciseModel.MuscleGroups.Quads, new List<ExerciseSetModel> { new ExerciseSetModel(90, 5), new ExerciseSetModel(110, 3) });
            FinishedExerciseModel exerciseTwo = new FinishedExerciseModel("OHP", "Overhead press", ExerciseModel.MuscleGroups.Shoulders, new List<ExerciseSetModel> { new ExerciseSetModel(50, 5), new ExerciseSetModel(55, 5) });
            FinishedExerciseModel exerciseThree = new FinishedExerciseModel("DL", "Deadlift", ExerciseModel.MuscleGroups.Back, new List<ExerciseSetModel> { new ExerciseSetModel(110, 5), new ExerciseSetModel(135, 5) });
            FinishedExerciseModel exerciseFour = new FinishedExerciseModel("BP", "Bench press", ExerciseModel.MuscleGroups.Chest, new List<ExerciseSetModel> { new ExerciseSetModel(80, 5), new ExerciseSetModel(90, 3) });
            FinishedExerciseModel exerciseFive = new FinishedExerciseModel("BC", "Bicep curl", ExerciseModel.MuscleGroups.Biceps, new List<ExerciseSetModel> { new ExerciseSetModel(20, 8), new ExerciseSetModel(20, 5) });
            FinishedExerciseModel exerciseSix = new FinishedExerciseModel("BC", "Bicep curl", ExerciseModel.MuscleGroups.Biceps, new List<ExerciseSetModel> { new ExerciseSetModel(20, 8), new ExerciseSetModel(20, 5) });
            FinishedExerciseModel exerciseSeven = new FinishedExerciseModel("BC", "Bicep curl", ExerciseModel.MuscleGroups.Biceps, new List<ExerciseSetModel> { new ExerciseSetModel(20, 8), new ExerciseSetModel(20, 5) });
            FinishedExerciseModel exerciseEight = new FinishedExerciseModel("BC", "Bicep curl", ExerciseModel.MuscleGroups.Biceps, new List<ExerciseSetModel> { new ExerciseSetModel(20, 8), new ExerciseSetModel(20, 5) });

            // create Workouts
            this.AddWorkout("Cucanj ludnica", DateTime.Now, "Titan gym", new List<FinishedExerciseModel> { exerciseOne, exerciseTwo, exerciseFive, exerciseSix, exerciseSeven, exerciseEight });
            this.AddWorkout("Pokidao mrtvo", new DateTime(2014, 5, 30), "Titan gym", new List<FinishedExerciseModel> { exerciseThree, exerciseFour });
        }

        /// <summary>
        /// Adds a new workout, regroups the workouts by week and raises the propertyChanged event
        /// </summary>
        public void AddWorkout(string workoutName, DateTime workoutDate, string location, List<FinishedExerciseModel> finishedExercises)
        {
            this.TrackedWorkouts.Add(new WorkoutModel(workoutName, workoutDate, location, finishedExercises));
            this.WorkoutsByWeek = GroupWorkoutsByWeek(this.TrackedWorkouts);
            // increment workouts this month if necessary
            if (workoutDate.Year == DateTime.Now.Year && workoutDate.Month == DateTime.Now.Month)
            {
                WorkoutsThisMonth++;
                OnPropertyChanged("WorkoutsThisMonth");
            }
            OnPropertyChanged("WorkoutsByWeek");
        }

        /// <summary>
        /// Groups the tracked workouts by week, because that's the way I want to display them to the user
        /// (LINQ MAGIC)
        /// </summary>
        private Dictionary<string, List<WorkoutModel>> GroupWorkoutsByWeek(ObservableCollection<WorkoutModel> workoutsModel)
        {
            // First I group the workouts by week using the GetWeekFromDate method to form the date string
            var groupByWeek = workoutsModel.Select(workout => new { Week = GetWeekFromDate(workout.WorkoutDate), Workout = workout }).GroupBy(x => x.Week);
            // Then I make the dictionary
            return groupByWeek.ToDictionary(x => x.Key, x => x.Select(y => y.Workout).ToList()); ;
        }

        /// <summary>
        // A method that takes a DateTime and outputs a string representing the week of that date
        // in the following format: "Week dd.MM to dd.MM"
        // for example: Week 09.03 to 15.03
        /// </summary>
        private string GetWeekFromDate(DateTime dateTime)
        {
            var day = dateTime.Day;
            // I did this because the default DayOfWeek index starts at sunday as 0, and I want monday to be index 0
            var dayOfWeek = ((int)dateTime.DayOfWeek - 1)%7;
            var startOfWeek = dateTime.AddDays(-dayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);
            return String.Format("Week {0} to {1}", startOfWeek.ToString("dd.MM"), endOfWeek.ToString("dd.MM"));
        }

        #region PropertyChanged stuff
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }

    /// <summary>
    /// Data about the user
    /// </summary>
    public class UserModel
    {
        public string Username { get; private set; }
        public double Weight { get; set; }
        public string WeightUnit { get; set; }
        public int WorkoutsPerWeek { get; set; }
        public ObservableCollection<PersonalRecord> DisplayedPersonalRecords { get; private set; }

        public UserModel(string username, double weight, string weightUnit, int workoutsPerWeek)
        {
            this.Username = username;
            this.Weight = weight;
            this.WeightUnit = weightUnit;
            this.WorkoutsPerWeek = workoutsPerWeek;
            this.DisplayedPersonalRecords = new ObservableCollection<PersonalRecord>();
        }
    }

    /// <summary>
    /// Data about one exercise
    /// </summary>
    public class ExerciseModel
    {
        public Dictionary<MuscleGroups, string> MuscleGroupNames
        {
            get;
            private set;
        }
        public enum MuscleGroups
        {
            Shoulders,
            Chest,
            Arms,
            Biceps,
            Triceps,
            Back,
            Traps,
            Lats,
            Delts,
            Core,
            Abs,
            Legs,
            Quads,
            Glutes,
            Calves,
        }

        public string ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public MuscleGroups PrimaryMuscleGroup { get; set; }
        public string PrimaryMuscleGroupName { get; set; }

        public ExerciseModel(string exerciseId, string fullName, MuscleGroups muscleGroup)
        {
            this.ExerciseId = exerciseId;
            this.ExerciseName = fullName;
            this.PrimaryMuscleGroup = muscleGroup;
            

            this.MuscleGroupNames = new Dictionary<MuscleGroups, string>()
            {
                {MuscleGroups.Shoulders, "Shoulders"},
                {MuscleGroups.Chest, "Chest"},
                {MuscleGroups.Arms, "Arms"},
                {MuscleGroups.Biceps, "Biceps"},
                {MuscleGroups.Triceps, "Triceps"},
                {MuscleGroups.Back, "Back"},
                {MuscleGroups.Traps, "Traps"},
                {MuscleGroups.Lats, "Lats"},
                {MuscleGroups.Delts, "Delts"},
                {MuscleGroups.Core, "Core"},
                {MuscleGroups.Abs, "Abs"},
                {MuscleGroups.Legs, "Legs"},
                {MuscleGroups.Quads, "Quads"},
                {MuscleGroups.Glutes, "Glutes"},
                {MuscleGroups.Calves, "Calves"}
            };

            this.PrimaryMuscleGroupName = this.MuscleGroupNames[this.PrimaryMuscleGroup];
        }

        public ExerciseModel(ExerciseModel other) : this(other.ExerciseId, other.ExerciseName, other.PrimaryMuscleGroup) { }
    }

    /// <summary>
    /// A finished exercise contains the data about the exercise and the data about the finished sets
    /// </summary>
    public class FinishedExerciseModel : ExerciseModel
    {
        public List<ExerciseSetModel> Sets { get; set; }
       
        public FinishedExerciseModel(string exerciseId, string fullName, MuscleGroups muscleGroup, List<ExerciseSetModel> sets) : base(exerciseId, fullName, muscleGroup)
        {
            this.Sets = sets;
        }

        public FinishedExerciseModel(ExerciseModel exercise, List<ExerciseSetModel> sets) : base(exercise)
        {
            this.Sets = sets;
        }
    }

    public class ExerciseSetModel
    {
        public double Weight { get; set; }
        public int Reps { get; set; }

        public ExerciseSetModel(double weight, int reps)
        {
            this.Weight = weight;
            this.Reps = reps;
        }
    }

    public class PersonalRecord
    {
        public string ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public DateTime DateOfRecord { get; set; }
        public double Weight { get; set; }
        public int Reps { get; set; }

        public PersonalRecord(string exerciseId, string exerciseName, DateTime date, double weight, int reps)
        {
            this.ExerciseId = exerciseId;
            this.ExerciseName = exerciseName;
            this.DateOfRecord = date;
            this.Weight = weight;
            this.Reps = reps;
        }
    }

    /// <summary>
    /// One workout
    /// </summary>
    public class WorkoutModel
    {
        public string WorkoutName { get; set; }
        public DateTime WorkoutDate { get; set; }
        public string WorkoutLocation { get; set; }
        public List<FinishedExerciseModel> WorkoutFinishedExercises { get; set; }

        public List<string> WorkoutMuscleGroups { get; set; }

        public WorkoutModel(string workoutName, DateTime date, string location, List<FinishedExerciseModel> exercises)
        {
            this.WorkoutName = workoutName;
            this.WorkoutDate = date;
            this.WorkoutLocation = location;
            this.WorkoutFinishedExercises = exercises;
            this.WorkoutMuscleGroups = this.WorkoutFinishedExercises.Select(exercise => "#" + exercise.PrimaryMuscleGroupName).Distinct().ToList();
        }
    }
}
