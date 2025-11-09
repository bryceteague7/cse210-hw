using System;
using System.Text.Json.Serialization;

public class Entry
{
    // parameterless constructor required for JSON deserialization
    public Entry() { }

    public string Date { get; set; }           // ISO date e.g. "2025-11-09"
    public string Time { get; set; }           // ISO time e.g. "2025-11-09T19:42:00"
    public string Prompt { get; set; }
    public string Response { get; set; }
    public int Mood { get; set; }              // 1..10 rating, optional (0 if not set)

    // Convenience constructor
    public Entry(string date, string time, string prompt, string response, int mood)
    {
        Date = date;
        Time = time;
        Prompt = prompt;
        Response = response;
        Mood = mood;
    }

    // Human-friendly display
    public override string ToString()
    {
        string moodText = Mood > 0 ? $"Mood: {Mood}/10\n" : "";
        return $"Date: {Date} {Time}\n{moodText}Prompt: {Prompt}\nResponse: {Response}\n";
    }

    // For legacy text file saving (pipe-separated) if needed
    public string ToFileString()
    {
        return $"{Date}|{Time}|{EscapeForPipe(Prompt)}|{EscapeForPipe(Response)}|{Mood}";
    }

    public static Entry FromFileString(string line)
    {
        string[] parts = line.Split('|');
        if (parts.Length < 5)
        {
            // corrupted line → return a simple entry
            return new Entry(DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"),
                             "CORRUPTED", line, 0);
        }
        return new Entry(parts[0], parts[1], UnescapeFromPipe(parts[2]), UnescapeFromPipe(parts[3]), int.TryParse(parts[4], out var m) ? m : 0);
    }

    private static string EscapeForPipe(string s) => s?.Replace("|", "¦") ?? "";
    private static string UnescapeFromPipe(string s) => s?.Replace("¦", "|") ?? "";
}
