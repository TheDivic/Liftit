using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liftit.Common;

// TODO: REFACTOR and add coments to code!
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
            private set;
        }

        public Dictionary<string, List<WorkoutModel>> WorkoutsByWeek { get; private set; }
        public ObservableCollection<ExerciseModel> KnownExercises {get; private set; }
        
        // TODO: Calculate this every time the app is started
        public int WorkoutsThisMonth { get; private set; }
        public int WorkoutsBehindSchedule { get; private set; }

        public AppDataModel()
        {
            this.User = new UserModel("Anonymous", 0, "nil", 0);
            this.TrackedWorkouts = new ObservableCollection<WorkoutModel>();
            this.WorkoutsByWeek = new Dictionary<string,List<WorkoutModel>>();
            // "Deadlift", "Overhead press", "Lunges", "Bench press", "Biceps curl", "Triceps rope extension", "Lat pulldowns"
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
            this.WorkoutsThisMonth = 5;
            this.WorkoutsBehindSchedule = 2;
        }

        public void LoadDataFromMemory()
        {
            LoadTestData();
        }

        public void AddWorkout(string workoutName, DateTime workoutDate, string location, List<FinishedExerciseModel> finishedExercises)
        {
            this.TrackedWorkouts.Add(new WorkoutModel(workoutName, workoutDate, location, finishedExercises));
            this.WorkoutsByWeek = GroupWorkoutsByWeek(this.TrackedWorkouts);
            OnPropertyChanged("WorkoutsByWeek");
        }

        private Dictionary<string, List<WorkoutModel>> GroupWorkoutsByWeek(ObservableCollection<WorkoutModel> workoutsModel)
        {
            var workoutsByWeekQuery = workoutsModel.Select(workout => new { Week = GetWeekFromDate(workout.WorkoutDate), Workout = workout });

            var workouts = new Dictionary<string, List<WorkoutModel>>();
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

        //TODO: fix start of week, it doesn't work correctly. Add timezone support!
        private string GetWeekFromDate(DateTime dateTime)
        {
            var day = dateTime.Day;
            var dayOfWeek = (int)dateTime.DayOfWeek;
            var startOfWeek = dateTime.AddDays(-dayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);
            return String.Format("Week {0} to {1}", startOfWeek.ToString("dd.MM"), endOfWeek.ToString("dd.MM"));
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
            FinishedExerciseModel exerciseOne = new FinishedExerciseModel("SQ", "Squat", ExerciseModel.MuscleGroups.Quads, new List<ExerciseSetModel> { new ExerciseSetModel(90, 5), new ExerciseSetModel(110, 3) });
            FinishedExerciseModel exerciseTwo = new FinishedExerciseModel("OHP", "Overhead press", ExerciseModel.MuscleGroups.Shoulders, new List<ExerciseSetModel> { new ExerciseSetModel(50, 5), new ExerciseSetModel(55, 5) });
            FinishedExerciseModel exerciseThree = new FinishedExerciseModel("DL", "Deadlift", ExerciseModel.MuscleGroups.Back, new List<ExerciseSetModel> { new ExerciseSetModel(110, 5), new ExerciseSetModel(135, 5) });
            FinishedExerciseModel exerciseFour = new FinishedExerciseModel("BP", "Bench press", ExerciseModel.MuscleGroups.Chest, new List<ExerciseSetModel> { new ExerciseSetModel(80, 5), new ExerciseSetModel(90, 3) });
            FinishedExerciseModel exerciseFive = new FinishedExerciseModel("BC", "Bicep curl", ExerciseModel.MuscleGroups.Biceps, new List<ExerciseSetModel> { new ExerciseSetModel(20, 8), new ExerciseSetModel(20, 5) });
            FinishedExerciseModel exerciseSix = new FinishedExerciseModel("BC", "Bicep curl", ExerciseModel.MuscleGroups.Biceps, new List<ExerciseSetModel> { new ExerciseSetModel(20, 8), new ExerciseSetModel(20, 5) });
            FinishedExerciseModel exerciseSeven = new FinishedExerciseModel("BC", "Bicep curl", ExerciseModel.MuscleGroups.Biceps, new List<ExerciseSetModel> { new ExerciseSetModel(20, 8), new ExerciseSetModel(20, 5) });
            FinishedExerciseModel exerciseEight = new FinishedExerciseModel("BC", "Bicep curl", ExerciseModel.MuscleGroups.Biceps, new List<ExerciseSetModel> { new ExerciseSetModel(20, 8), new ExerciseSetModel(20, 5) });

            // create Workouts
            this.AddWorkout("Cucanj ludnica", new DateTime(2014, 5, 12), "Titan gym", new List<FinishedExerciseModel> { exerciseOne, exerciseTwo, exerciseFive, exerciseSix, exerciseSeven, exerciseEight });
            this.AddWorkout("Pokidao mrtvo", new DateTime(2014, 5, 30), "Titan gym", new List<FinishedExerciseModel> { exerciseThree, exerciseFour });
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
