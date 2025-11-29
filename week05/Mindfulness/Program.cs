using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

//
// W05 Project: Mindfulness Program
//
// Extra features implemented (for exceeding requirements):
// 1) Prompts/questions are not reused until all have been used in the session.
// 2) Simple activity log saved to "mindfulness_log.txt" with timestamp, activity, duration and listing count.
// Document these extras here as requested in the assignment.
//
// To run:
//   dotnet new console -n MindfulnessApp   (if you need a new project)
//   Replace Program.cs with this file and run `dotnet run`
//

namespace MindfulnessApp
{
    // Base abstract class for activities
    abstract class Activity
    {
        private string _name;
        private string _description;
        private int _durationSeconds;

        protected readonly Random _random = new Random();

        protected Activity(string name, string description)
        {
            _name = name;
            _description = description;
            _durationSeconds = 0;
        }

        // Encapsulated property for duration
        public int DurationSeconds
        {
            get { return _durationSeconds; }
            private set { _durationSeconds = Math.Max(0, value); }
        }

        // Standard starting message (shared behavior)
        public void Start()
        {
            Console.Clear();
            Console.WriteLine($"--- {_name} ---");
            Console.WriteLine();
            Console.WriteLine(_description);
            Console.WriteLine();
            AskAndSetDuration();
            Console.WriteLine();
            Console.WriteLine("Get ready...");
            ShowSpinner(3); // pause for several seconds with spinner
            DoActivity();
            Finish();
        }

        // Ask user for duration in seconds and set property
        private void AskAndSetDuration()
        {
            while (true)
            {
                Console.Write("Enter the duration of the activity in seconds: ");
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int seconds) && seconds > 0)
                {
                    DurationSeconds = seconds;
                    break;
                }
                Console.WriteLine("Please enter a positive integer for seconds.");
            }
        }

        // Standard finishing message (shared behavior)
        private void Finish()
        {
            Console.WriteLine();
            Console.WriteLine("Well done!");
            ShowSpinner(3); // pause
            Console.WriteLine($"You have completed the {GetName()} for {DurationSeconds} seconds.");
            ShowSpinner(2);
            // allow derived classes to perform any final actions, such as logging
            OnFinish();
        }

        // Hook for derived classes
        protected virtual void OnFinish() { }

        // Name access for base class
        protected string GetName() => _name;

        // Abstract method: each activity implements its own flow
        protected abstract void DoActivity();

        #region Utility methods for animations / pauses

        // Spinner animation for given number of seconds
        protected void ShowSpinner(int seconds)
        {
            char[] sequence = new[] { '|', '/', '-', '\\' };
            DateTime end = DateTime.Now.AddSeconds(seconds);
            int i = 0;
            while (DateTime.Now < end)
            {
                Console.Write(sequence[i % sequence.Length]);
                Thread.Sleep(200);
                Console.Write('\b');
                i++;
            }
        }

        
        protected void ShowCountdown(int count)
        {
            for (int i = count; i >= 1; i--)
            {
                Console.Write(i);
                Thread.Sleep(1000);
                Console.Write('\b'); // erase
            }
            Console.Write(" "); // clear
            Console.Write('\b');
        }

        
        protected void DotPause(int seconds)
        {
            for (int i = 0; i < seconds; i++)
            {
                Console.Write('.');
                Thread.Sleep(1000);
            }
            Console.WriteLine();
        }

        #endregion
    }

    
    class BreathingActivity : Activity
    {
        
        private readonly int _inhaleSeconds;
        private readonly int _exhaleSeconds;

        public BreathingActivity() : base(
            "Breathing Activity",
            "This activity will help you relax by walking you through breathing in and out slowly. Clear your mind and focus on your breathing.")
        {
           
            _inhaleSeconds = 4;
            _exhaleSeconds = 6;
        }

        protected override void DoActivity()
        {
            Console.WriteLine();
            Console.WriteLine($"Starting {GetName()} for {DurationSeconds} seconds.");
            Console.WriteLine();

            DateTime endTime = DateTime.Now.AddSeconds(DurationSeconds);
            while (DateTime.Now < endTime)
            {
                // Breathe in
                Console.Write("Breathe in... ");
                ShowCountdown(_inhaleSeconds);

                if (DateTime.Now >= endTime) break;

                // Breathe out
                Console.Write("Breathe out... ");
                ShowCountdown(_exhaleSeconds);
            }
        }

        protected override void OnFinish()
        {
           
            Logger.Log($"{GetName()} | Duration: {DurationSeconds} seconds");
        }
    }

    
    class ReflectionActivity : Activity
    {
        private readonly List<string> _prompts = new List<string>
        {
            "Think of a time when you stood up for someone else.",
            "Think of a time when you did something really difficult.",
            "Think of a time when you helped someone in need.",
            "Think of a time when you did something truly selfless."
        };

        private readonly List<string> _questions = new List<string>
        {
            "Why was this experience meaningful to you?",
            "Have you ever done anything like this before?",
            "How did you get started?",
            "How did you feel when it was complete?",
            "What made this time different than other times when you were not as successful?",
            "What is your favorite thing about this experience?",
            "What could you learn from this experience that applies to other situations?",
            "What did you learn about yourself through this experience?",
            "How can you keep this experience in mind in the future?"
        };

       
        private readonly ShuffleBag<string> _promptBag;
        private readonly ShuffleBag<string> _questionBag;

        public ReflectionActivity() : base(
            "Reflection Activity",
            "This activity will help you reflect on times in your life when you have shown strength and resilience. This will help you recognize the power you have and how you can use it in other aspects of your life.")
        {
            _promptBag = new ShuffleBag<string>(_prompts);
            _questionBag = new ShuffleBag<string>(_questions);
        }

        protected override void DoActivity()
        {
            Console.WriteLine();
            Console.WriteLine($"Starting {GetName()} for {DurationSeconds} seconds.");
            Console.WriteLine();

            string prompt = _promptBag.GetRandom();
            Console.WriteLine("Prompt:");
            Console.WriteLine($"  >>> {prompt}");
            Console.WriteLine();

            Console.WriteLine("When you are ready, reflect on the following questions:");
            DateTime endTime = DateTime.Now.AddSeconds(DurationSeconds);

         
            ShowSpinner(3);

            while (DateTime.Now < endTime)
            {
                string question = _questionBag.GetRandom();
                Console.WriteLine();
                Console.WriteLine($" - {question}");
                
                int pauseFor = 6;
              
                int remaining = (int)Math.Ceiling((endTime - DateTime.Now).TotalSeconds);
                if (remaining <= 0) break;
                int actualPause = Math.Min(pauseFor, remaining);
                ShowSpinner(actualPause);
            }
        }

        protected override void OnFinish()
        {
            Logger.Log($"{GetName()} | Duration: {DurationSeconds} seconds");
        }
    }

    // Listing activity class
    class ListingActivity : Activity
    {
        private readonly List<string> _prompts = new List<string>
        {
            "Who are people that you appreciate?",
            "What are personal strengths of yours?",
            "Who are people that you have helped this week?",
            "When have you felt the Holy Ghost this month?",
            "Who are some of your personal heroes?"
        };

        private readonly ShuffleBag<string> _promptBag;

        public ListingActivity() : base(
            "Listing Activity",
            "This activity will help you reflect on the good things in your life by having you list as many things as you can in a certain area.")
        {
            _promptBag = new ShuffleBag<string>(_prompts);
        }

        protected override void DoActivity()
        {
            Console.WriteLine();
            Console.WriteLine($"Starting {GetName()} for {DurationSeconds} seconds.");
            Console.WriteLine();

            string prompt = _promptBag.GetRandom();
            Console.WriteLine("Prompt:");
            Console.WriteLine($"  >>> {prompt}");
            Console.WriteLine();

            Console.WriteLine("You will have a short time to think. Prepare...");
           
            ShowCountdown(5);
            Console.WriteLine();
            Console.WriteLine("Start listing items now (press Enter after each item). The timer will stop when time is up.");

            DateTime endTime = DateTime.Now.AddSeconds(DurationSeconds);
            List<string> items = new List<string>();

            
            while (DateTime.Now < endTime)
            {
             
                int remaining = (int)Math.Ceiling((endTime - DateTime.Now).TotalSeconds);
                Console.Write($"Time left: {remaining}s > ");
             
                string? line = ReadLineWithTimeout(endTime);
                Console.WriteLine();
                if (line == null)
                {
                    
                    continue;
                }
              
                if (!string.IsNullOrWhiteSpace(line))
                {
                    items.Add(line.Trim());
                }
            }

            Console.WriteLine();
            Console.WriteLine($"You listed {items.Count} item(s):");
            for (int i = 0; i < items.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {items[i]}");
            }

            
            Logger.Log($"{GetName()} | Duration: {DurationSeconds} seconds | Items: {items.Count}");
        }

        
        private string? ReadLineWithTimeout(DateTime endTime)
        {
            var buffer = new List<char>();
            while (DateTime.Now < endTime)
            {
                while (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(intercept: true);
                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        return new string(buffer.ToArray());
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (buffer.Count > 0)
                        {
                            buffer.RemoveAt(buffer.Count - 1);
                           
                            Console.Write("\b \b");
                        }
                    }
                    else
                    {
                        buffer.Add(keyInfo.KeyChar);
                        Console.Write(keyInfo.KeyChar);
                    }
                }
                Thread.Sleep(200); 
            }
           
            return null;
        }

        protected override void OnFinish()
        {
       
        }
    }

   
    static class Logger
    {
        private static readonly string _filePath = "mindfulness_log.txt";
        private static readonly object _lock = new object();

        public static void Log(string message)
        {
            try
            {
                lock (_lock)
                {
                    string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}";
                    File.AppendAllLines(_filePath, new[] { line });
                }
            }
            catch
            {
              
            }
        }
    }

        class ShuffleBag<T>
    {
        private List<T> _items;
        private List<T> _bag;
        private readonly Random _rng = new Random();

        public ShuffleBag(IEnumerable<T> items)
        {
            _items = new List<T>(items);
            RefillBag();
        }

        // get a random item, ensuring each item used once before repeating
        public T GetRandom()
        {
            if (_bag.Count == 0) RefillBag();
            int idx = _rng.Next(_bag.Count);
            T item = _bag[idx];
            _bag.RemoveAt(idx);
            return item;
        }

        private void RefillBag()
        {
            _bag = new List<T>(_items);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Mindfulness Program";
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Mindfulness Program");
                Console.WriteLine("-------------------");
                Console.WriteLine("Menu Options:");
                Console.WriteLine("1) Breathing Activity");
                Console.WriteLine("2) Reflection Activity");
                Console.WriteLine("3) Listing Activity");
                Console.WriteLine("4) Exit");
                Console.WriteLine();
                Console.Write("Choose an option (1-4): ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        var breathing = new BreathingActivity();
                        breathing.Start();
                        PauseBeforeMenu();
                        break;
                    case "2":
                        var reflection = new ReflectionActivity();
                        reflection.Start();
                        PauseBeforeMenu();
                        break;
                    case "3":
                        var listing = new ListingActivity();
                        listing.Start();
                        PauseBeforeMenu();
                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Please select a valid option (1-4).");
                        Thread.Sleep(1200);
                        break;
                }
            }

            Console.WriteLine("Goodbye — remember to take time for yourself today!");
            Thread.Sleep(800);
        }

        private static void PauseBeforeMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Returning to menu...");
         
            var tmp = new ActivityMenuSpinner();
            tmp.Show(2);
        }

    
        private class ActivityMenuSpinner
        {
            public void Show(int seconds)
            {
                char[] seq = { '|', '/', '-', '\\' };
                DateTime end = DateTime.Now.AddSeconds(seconds);
                int i = 0;
                while (DateTime.Now < end)
                {
                    Console.Write(seq[i % seq.Length]);
                    Thread.Sleep(200);
                    Console.Write('\b');
                    i++;
                }
            }
        }
    }
}
