using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liftit.Common;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Windows.Storage;
using System.IO;
using System.Diagnostics;


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
        public ObservableCollection<ExerciseModel> KnownExercises { get; private set; }

        public int WorkoutsThisMonth { get; private set; }
        public int WorkoutsBehindSchedule { get; private set; }

        private string USER_FILENAME = "user.json";
        private string WORKOUTS_FILENAME = "workouts.json";

        public AppDataModel()
        {
            this.User = new UserModel("Anonymous", 0, "nil", 0);
            this.TrackedWorkouts = new ObservableCollection<WorkoutModel>();
            this.WorkoutsByWeek = new Dictionary<string, List<WorkoutModel>>();
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

        public async Task LoadDataFromLocalFolder()
        {
            //LoadTestData();
            LoadTestUser();

            //await DeserializeUser();
            await DeserializeWorkouts();
            this.WorkoutsThisMonth = TrackedWorkouts.Where(x => x.WorkoutDate.Year == DateTime.Now.Year && x.WorkoutDate.Month == DateTime.Now.Month).Count();
            this.WorkoutsBehindSchedule = this.User.WorkoutsPerWeek * 4 - this.WorkoutsThisMonth;
        }

        private void LoadTestUser()
        {
            // Create user
            this.User = new UserModel("Divic", 92, "kg", 4);

            // create personal records
            // TODO: implement the personal records mechanism
            this.User.DisplayedPersonalRecords.Add(new PersonalRecord("SQ", "Squat", new DateTime(2014, 5, 12), 130, 2));
            this.User.DisplayedPersonalRecords.Add(new PersonalRecord("DL", "Deadlift", new DateTime(2014, 5, 30), 135, 5));
            this.User.DisplayedPersonalRecords.Add(new PersonalRecord("BP", "Bench press", new DateTime(2014, 10, 23), 105, 2));
            this.User.DisplayedPersonalRecords.Add(new PersonalRecord("OHP", "Overhead press", new DateTime(2014, 10, 25), 70, 2));
        }

        public async Task WriteDataToLocalFolder()
        {
            var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<WorkoutModel>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
               WORKOUTS_FILENAME,
               CreationCollisionOption.ReplaceExisting
               ))
            {
                serializer.WriteObject(stream, this.TrackedWorkouts);
            }
        }

        #region Serialization methods
        private async Task SerializeUser()
        {
            var serializer = new DataContractJsonSerializer(typeof(UserModel));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
               USER_FILENAME,
               CreationCollisionOption.ReplaceExisting
               ))
            {
                serializer.WriteObject(stream, this.User);
            }
        }

        private async Task DeserializeUser()
        {
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            var userFile = files.Where(x => x.Name == USER_FILENAME).FirstOrDefault();

            if (userFile != null)
            {
                var stream = await userFile.OpenStreamForReadAsync();
                var serializer = new DataContractJsonSerializer(typeof(UserModel));

                using (StreamReader reader = new StreamReader(stream))
                {
                    this.User = (UserModel)serializer.ReadObject(stream);
                }
            }
        }

        private async Task SerializeWorkouts()
        {
            var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<WorkoutModel>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
               WORKOUTS_FILENAME,
               CreationCollisionOption.ReplaceExisting
               ))
            {
                serializer.WriteObject(stream, this.TrackedWorkouts);
            }
        }

        private async Task DeserializeWorkouts()
        {
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            var workoutFile = files.Where(x => x.Name == WORKOUTS_FILENAME).FirstOrDefault();

            if (workoutFile != null)
            {
                //var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(WORKOUTS_FILENAME);]
                var stream = await workoutFile.OpenStreamForReadAsync();
                var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<WorkoutModel>));

                using (StreamReader reader = new StreamReader(stream))
                {
                    this.TrackedWorkouts = (ObservableCollection<WorkoutModel>)serializer.ReadObject(stream);
                    this.WorkoutsByWeek = GroupWorkoutsByWeek(this.TrackedWorkouts);
                }
            }
        }
        #endregion

        /// <summary>
        /// Adds a new workout, regroups the workouts by week and raises the propertyChanged event
        /// </summary>
        public async Task AddWorkout(string workoutNote, DateTime workoutDate, string location, List<FinishedExerciseModel> finishedExercises)
        {
            this.TrackedWorkouts.Add(new WorkoutModel(workoutNote, workoutDate, location, finishedExercises));
            await this.SerializeWorkouts();
            this.WorkoutsByWeek = GroupWorkoutsByWeek(this.TrackedWorkouts);
            // increment workouts this month if necessary
            if (workoutDate.Year == DateTime.Now.Year && workoutDate.Month == DateTime.Now.Month)
            {
                WorkoutsThisMonth++;
                OnPropertyChanged("WorkoutsThisMonth");
            }
            OnPropertyChanged("WorkoutsByWeek");
        }

        public async Task DeleteWorkout(WorkoutModel workout)
        {
            this.TrackedWorkouts.Remove(workout);
            await this.SerializeWorkouts();
            this.WorkoutsByWeek = GroupWorkoutsByWeek(this.TrackedWorkouts);
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
            var dayOfWeek = ((int)dateTime.DayOfWeek - 1) % 7;
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

        public UserModel() { }
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

        public ExerciseModel() { }

        public ExerciseModel(ExerciseModel other) : this(other.ExerciseId, other.ExerciseName, other.PrimaryMuscleGroup) { }
    }

    /// <summary>
    /// A finished exercise contains the data about the exercise and the data about the finished sets
    /// </summary>
    public class FinishedExerciseModel : ExerciseModel
    {
        public List<ExerciseSetModel> Sets { get; set; }

        public FinishedExerciseModel() { }

        public FinishedExerciseModel(string exerciseId, string fullName, MuscleGroups muscleGroup, List<ExerciseSetModel> sets)
            : base(exerciseId, fullName, muscleGroup)
        {
            this.Sets = sets;
        }

        public FinishedExerciseModel(ExerciseModel exercise, List<ExerciseSetModel> sets)
            : base(exercise)
        {
            this.Sets = sets;
        }
    }

    public class ExerciseSetModel
    {
        public double Weight { get; set; }
        public int Reps { get; set; }

        public ExerciseSetModel() { }

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

        public PersonalRecord() { }

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
        public string WorkoutNote { get; set; }
        public DateTime WorkoutDate { get; set; }
        public string WorkoutLocation { get; set; }
        public List<FinishedExerciseModel> WorkoutFinishedExercises { get; set; }

        public List<string> WorkoutMuscleGroups { get; set; }

        public WorkoutModel() { }

        public WorkoutModel(string workoutNote, DateTime date, string location, List<FinishedExerciseModel> exercises)
        {
            this.WorkoutNote = workoutNote;
            this.WorkoutDate = date;
            this.WorkoutLocation = location;
            this.WorkoutFinishedExercises = exercises;
            this.WorkoutMuscleGroups = this.WorkoutFinishedExercises.Select(exercise => "#" + exercise.PrimaryMuscleGroupName).Distinct().ToList();
        }
    }
}
