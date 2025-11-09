using System;
using System.Globalization;

public class Program
{
    // EXCEEDING REQUIREMENTS:
    // - Saves and loads as JSON (safe for special characters).
    // - Stores mood rating (1-10) with each entry and includes stats (count, average mood).
    // - Supports search by keyword or by date (yyyy-MM-dd).
    // - Exports CSV suitable for Excel (simple quoting).
    // - PromptGenerator can load prompts from file and accept added prompts at runtime.
    // - Uses solid OO design: Entry, Journal, PromptGenerator, Program.
    public static void Main(string[] args)
    {
        var journal = new Journal();
        var prompts = new PromptGenerator();

        bool running = true;
        while (running)
        {
            PrintMenu();
            Console.Write("Choose an option: ");
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    WriteNewEntry(journal, prompts);
                    break;
                case "2":
                    journal.DisplayAll();
                    break;
                case "3":
                    SaveJson(journal);
                    break;
                case "4":
                    LoadJson(journal);
                    break;
                case "5":
                    SearchEntries(journal);
                    break;
                case "6":
                    ShowStats(journal);
                    break;
                case "7":
                    ExportCsv(journal);
                    break;
                case "8":
                    ManagePrompts(prompts);
                    break;
                case "9":
                    SaveTextLegacy(journal);
                    break;
                case "0":
                    Console.WriteLine("Goodbye!");
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Enter a number shown in the menu.");
                    break;
             }

            if (running)
            {
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
        }
    }

    static void PrintMenu()
    {
        Console.Clear();
        Console.WriteLine("=== Journal Menu ===");
        Console.WriteLine("1. Write a new entry (random prompt + mood)");
        Console.WriteLine("2. Display the journal");
        Console.WriteLine("3. Save the journal to a JSON file");
        Console.WriteLine("4. Load the journal from a JSON file (replaces current entries)");
        Console.WriteLine("5. Search entries (keyword or date yyyy-MM-dd)");
        Console.WriteLine("6. Show statistics (count, avg mood, most recent)");
        Console.WriteLine("7. Export to CSV (Excel-friendly)");
        Console.WriteLine("8. Manage prompts (view/add/load-from-file)");
        Console.WriteLine("9. (Legacy) Save/Load as pipe-separated text");
        Console.WriteLine("0. Quit");
        Console.WriteLine("====================");
    }

    static void WriteNewEntry(Journal journal, PromptGenerator prompts)
    {
        string prompt = prompts.GetRandomPrompt();
        Console.WriteLine($"\nPrompt: {prompt}");
        Console.Write("Your response: ");
        string response = Console.ReadLine() ?? "";

        int mood = ReadIntInRange("Rate your mood 1-10 (or 0 to skip): ", 0, 10);

        string date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        string time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

        var entry = new Entry(date, time, prompt, response, mood);
        journal.AddEntry(entry);
        Console.WriteLine("Entry added.");
    }

    static int ReadIntInRange(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();
            if (int.TryParse(s, out int val) && val >= min && val <= max) return val;
            Console.WriteLine($"Please enter a number between {min} and {max}.");
        }
    }

    static void SaveJson(Journal journal)
    {
        Console.Write("Enter filename to save JSON (e.g., journal.json): ");
        string file = Console.ReadLine()?.Trim() ?? "journal.json";
        try
        {
            journal.SaveToJson(file);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving JSON: " + ex.Message);
        }
    }

    static void LoadJson(Journal journal)
    {
        Console.Write("Enter filename to load JSON (e.g., journal.json): ");
        string file = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(file)) { Console.WriteLine("No filename entered."); return; }
        try
        {
            journal.LoadFromJson(file);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading JSON: " + ex.Message);
        }
    }

    static void SearchEntries(Journal journal)
    {
        Console.Write("Enter search keyword or date (yyyy-MM-dd): ");
        string q = Console.ReadLine() ?? "";
        var results = journal.Search(q);
        Console.WriteLine($"\nFound {results.Count} results for '{q}':\n");
        foreach (var r in results) Console.WriteLine(r + new string('-', 30));
    }

    static void ShowStats(Journal journal)
    {
        int count = journal.EntryCount();
        double avg = journal.AverageMood();
        var recent = journal.MostRecent();
        Console.WriteLine($"Total entries: {count}");
        Console.WriteLine($"Average mood (entries with mood): {(avg > 0 ? avg.ToString("0.00") : "N/A")}");
        if (recent != null)
        {
            Console.WriteLine("\nMost recent entry:");
            Console.WriteLine(recent);
        }
        else
        {
            Console.WriteLine("No entries yet.");
        }
    }

    static void ExportCsv(Journal journal)
    {
        Console.Write("Enter CSV filename (e.g., journal.csv): ");
        string file = Console.ReadLine()?.Trim() ?? "journal.csv";
        try
        {
            journal.ExportToCsv(file);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error exporting CSV: " + ex.Message);
        }
    }

    static void ManagePrompts(PromptGenerator prompts)
    {
        Console.WriteLine("\nPrompt Manager:");
        Console.WriteLine("1. View all prompts");
        Console.WriteLine("2. Add a prompt");
        Console.WriteLine("3. Load prompts from file");
        Console.Write("Choose an option: ");
        var c = Console.ReadLine();
        switch (c)
        {
            case "1":
                Console.WriteLine("\nPrompts:");
                int i = 1;
                foreach (var p in prompts.GetAllPrompts()) Console.WriteLine($"{i++}. {p}");
                break;
            case "2":
                Console.Write("Enter new prompt text: ");
                string np = Console.ReadLine() ?? "";
                prompts.AddPrompt(np);
                Console.WriteLine("Prompt added.");
                break;
            case "3":
                Console.Write("Enter prompt filename (one prompt per line): ");
                string f = Console.ReadLine() ?? "";
                try
                {
                    prompts.LoadPromptsFromFile(f);
                    Console.WriteLine("Prompts loaded from file.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading prompts: " + ex.Message);
                }
                break;
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }

    static void SaveTextLegacy(Journal journal)
    {
        Console.WriteLine("1. Save to text file (pipe-separated)");
        Console.WriteLine("2. Load from text file (pipe-separated)");
        Console.Write("Choose: ");
        string c = Console.ReadLine() ?? "";
        switch (c)
        {
            case "1":
                Console.Write("Enter filename (e.g., journal.txt): ");
                var s = Console.ReadLine() ?? "journal.txt";
                journal.SaveToTextFile(s);
                break;
            case "2":
                Console.Write("Enter filename to load (e.g., journal.txt): ");
                var l = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(l)) journal.LoadFromTextFile(l);
                break;
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
}
