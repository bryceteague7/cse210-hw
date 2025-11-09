using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

public class Journal
{
    public List<Entry> Entries { get; set; } = new List<Entry>();

    public void AddEntry(Entry entry) => Entries.Add(entry);

    public void DisplayAll()
    {
        if (Entries.Count == 0)
        {
            Console.WriteLine("No entries yet.");
            return;
        }

        foreach (var e in Entries)
        {
            Console.WriteLine(e);
            Console.WriteLine(new string('-', 40));
        }
    }

    // JSON storage (recommended)
    public void SaveToJson(string filename)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(Entries, options);
        File.WriteAllText(filename, json, Encoding.UTF8);
        Console.WriteLine($"Journal saved as JSON to '{filename}'.");
    }

    public void LoadFromJson(string filename)
    {
        if (!File.Exists(filename))
        {
            Console.WriteLine($"File '{filename}' not found.");
            return;
        }

        string json = File.ReadAllText(filename, Encoding.UTF8);
        try
        {
            var loaded = JsonSerializer.Deserialize<List<Entry>>(json) ?? new List<Entry>();
            Entries = loaded;
            Console.WriteLine($"Loaded {Entries.Count} entries from '{filename}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reading JSON: " + ex.Message);
        }
    }

    // Simple pipe-separated text save/load (legacy / safe)
    public void SaveToTextFile(string filename)
    {
        using var writer = new StreamWriter(filename, false, Encoding.UTF8);
        foreach (var e in Entries)
            writer.WriteLine(e.ToFileString());
        Console.WriteLine($"Journal saved to text file '{filename}'.");
    }

    public void LoadFromTextFile(string filename)
    {
        if (!File.Exists(filename))
        {
            Console.WriteLine($"File '{filename}' not found.");
            return;
        }
        var lines = File.ReadAllLines(filename, Encoding.UTF8);
        var list = new List<Entry>();
        foreach (var line in lines)
            list.Add(Entry.FromFileString(line));
        Entries = list;
        Console.WriteLine($"Loaded {Entries.Count} entries from '{filename}'.");
    }

    // CSV export for Excel (wrap fields in quotes, double internal quotes)
    public void ExportToCsv(string filename)
    {
        using var writer = new StreamWriter(filename, false, Encoding.UTF8);
        writer.WriteLine("Date,Time,Mood,Prompt,Response");
        foreach (var e in Entries)
        {
            string q(string field) => $"\"{field?.Replace("\"", "\"\"")}\"";
            writer.WriteLine($"{q(e.Date)},{q(e.Time)},{e.Mood},{q(e.Prompt)},{q(e.Response)}");
        }
        Console.WriteLine($"Exported {Entries.Count} entries to CSV '{filename}'.");
    }

    // Search by keyword (in prompt or response) or exact date (yyyy-MM-dd)
    public List<Entry> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return new List<Entry>();
        query = query.Trim();
        bool isDate = DateTime.TryParse(query, out var date);
        if (isDate)
        {
            string dateStr = date.ToString("yyyy-MM-dd");
            return Entries.Where(e => e.Date == dateStr).ToList();
        }
        string qLower = query.ToLowerInvariant();
        return Entries.Where(e => (e.Prompt ?? "").ToLowerInvariant().Contains(qLower)
                                || (e.Response ?? "").ToLowerInvariant().Contains(qLower)).ToList();
    }

    // Simple stats
    public int EntryCount() => Entries.Count;

    public double AverageMood()
    {
        var moods = Entries.Where(e => e.Mood > 0).Select(e => e.Mood).ToList();
        if (!moods.Any()) return 0.0;
        return moods.Average();
    }

    public Entry MostRecent()
    {
        // try to parse Time fields for ordering, fall back to list order
        var parsed = Entries.Select(e =>
        {
            if (DateTime.TryParse($"{e.Date}T{e.Time}", out var dt)) return (entry: e, when: dt);
            if (DateTime.TryParse(e.Time, out var t)) return (entry: e, when: t);
            return (entry: e, when: DateTime.MinValue);
        });

        var most = parsed.OrderByDescending(p => p.when).FirstOrDefault();
        return most.entry;
    }
}
