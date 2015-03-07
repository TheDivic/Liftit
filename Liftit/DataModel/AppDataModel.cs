using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liftit.DataModel
{
    /// <summary>
    /// All the data needed for the app
    /// </summary>
    public class AppDataModel : INotifyPropertyChanged
    {
        public UserModel User { get; private set; }
        public ObservableCollection<WorkoutModel> TrackedWorkouts
        {
            get;
            set;
        }
        public List<string> workoutNames;

        public AppDataModel()
        {
            this.User = new UserModel("Anonymous", 0, "nil", 0);
            this.TrackedWorkouts = new ObservableCollection<WorkoutModel>();
            this.workoutNames = new List<string>() { "Squat", "Deadlift", "Overhead press", "Lunges", "Bench press", "Biceps curl", "Triceps rope extension", "Lat pulldowns" };
        }

        public void LoadDataFromMemory()
        {
            LoadTestData();
        }

        private void LoadTestData()
        {
            // Create user
            this.User = new UserModel("Divic", 92, "kg", 4);
            // create personal records
            this.User.DisplayedPersonalRecords.Add(new PersonalRecord("SQ", "Squat", new DateTime(2014, 5, 12), 100, 5));
            this.User.DisplayedPersonalRecords.Add(new PersonalRecord("DL", "Deadlift", new DateTime(2014, 5, 30), 135, 5));
            this.User.DisplayedPersonalRecords.Add(new PersonalRecord("BP", "Bench press", new DateTime(2014, 10, 23), 100, 2));

            //create exercises
            ExerciseModel exerciseOne = new ExerciseModel("SQ", "Squat", ExerciseModel.MuscleGroups.Quads, new List<ExerciseSetModel> { new ExerciseSetModel(90, 5), new ExerciseSetModel(110, 3) });
            ExerciseModel exerciseTwo = new ExerciseModel("OHP", "Overhead press", ExerciseModel.MuscleGroups.Shoulders, new List<ExerciseSetModel> { new ExerciseSetModel(50, 5), new ExerciseSetModel(55, 5) });
            ExerciseModel exerciseThree = new ExerciseModel("DL", "Deadlift", ExerciseModel.MuscleGroups.Back, new List<ExerciseSetModel> { new ExerciseSetModel(110, 5), new ExerciseSetModel(135, 5) });
            ExerciseModel exerciseFour = new ExerciseModel("BP", "Bench press", ExerciseModel.MuscleGroups.Chest, new List<ExerciseSetModel> { new ExerciseSetModel(80, 5), new ExerciseSetModel(90, 3) });
            ExerciseModel exerciseFive = new ExerciseModel("BC", "Bicep curl", ExerciseModel.MuscleGroups.Biceps, new List<ExerciseSetModel> { new ExerciseSetModel(20, 8), new ExerciseSetModel(20, 5) });
            ExerciseModel exerciseSix = new ExerciseModel("BC", "Bicep curl", ExerciseModel.MuscleGroups.Biceps, new List<ExerciseSetModel> { new ExerciseSetModel(20, 8), new ExerciseSetModel(20, 5) });
            ExerciseModel exerciseSeven = new ExerciseModel("BC", "Bicep curl", ExerciseModel.MuscleGroups.Biceps, new List<ExerciseSetModel> { new ExerciseSetModel(20, 8), new ExerciseSetModel(20, 5) });
            ExerciseModel exerciseEight = new ExerciseModel("BC", "Bicep curl", ExerciseModel.MuscleGroups.Biceps, new List<ExerciseSetModel> { new ExerciseSetModel(20, 8), new ExerciseSetModel(20, 5) });

            // create Workouts
            var workoutOne = new WorkoutModel("Cucanj ludnica", new DateTime(2014, 5, 12), "Titan gym", new List<ExerciseModel> { exerciseOne, exerciseTwo, exerciseFive, exerciseSix, exerciseSeven, exerciseEight });
            var workoutTwo = new WorkoutModel("Pokidao mrtvo", new DateTime(2014, 5, 30), "Titan gym", new List<ExerciseModel> { exerciseThree, exerciseFour });

            this.TrackedWorkouts.Add(workoutOne);
            this.TrackedWorkouts.Add(workoutTwo);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
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
    /// Data about one execise
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
        public string ExerciseFullName { get; set; }
        public MuscleGroups PrimaryMuscleGroup { get; set; }
        public string PrimaryMuscleGroupName { get; set; }
        public List<ExerciseSetModel> Sets { get; set; }

        public ExerciseModel(string exerciseId, string fullName, MuscleGroups muscleGroup, List<ExerciseSetModel> sets)
        {
            this.ExerciseId = exerciseId;
            this.ExerciseFullName = fullName;
            this.PrimaryMuscleGroup = muscleGroup;
            this.Sets = sets;

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
        public List<ExerciseModel> WorkoutFinishedExercises { get; set; }

        public List<string> WorkoutMuscleGroups { get; set; }

        public WorkoutModel(string workoutName, DateTime date, string location, List<ExerciseModel> exercises)
        {
            this.WorkoutName = workoutName;
            this.WorkoutDate = date;
            this.WorkoutLocation = location;
            this.WorkoutFinishedExercises = exercises;
            this.WorkoutMuscleGroups = this.WorkoutFinishedExercises.Select(exercise => "#" + exercise.PrimaryMuscleGroupName).Distinct().ToList();
        }
    }
}
