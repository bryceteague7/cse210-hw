using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class PromptGenerator
{
    private List<string> _prompts;
    private Random _rand = new Random();

    public PromptGenerator()
    {
        // default prompt set (≥ 5 prompts)
        _prompts = new List<string>
        {
            "Who was the most interesting person I interacted with today?",
            "What was the best part of my day?",
            "How did I see the hand of the Lord in my life today?",
            "What was the strongest emotion I felt today?",
            "If I had one thing I could do over today, what would it be?",
            "What did I learn today that surprised me?",
            "What am I most grateful for right now?"
        };
    }

    // Optionally load prompts from a text file (one prompt per line)
    public void LoadPromptsFromFile(string filename)
    {
        if (!File.Exists(filename))
            throw new FileNotFoundException($"Prompt file '{filename}' not found.");

        var lines = File.ReadAllLines(filename).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
        if (lines.Count > 0) _prompts = lines;
    }

    public string GetRandomPrompt()
    {
        if (_prompts == null || _prompts.Count == 0) return "Write something about your day.";
        int idx = _rand.Next(_prompts.Count);
        return _prompts[idx];
    }

    // Add a prompt at runtime
    public void AddPrompt(string prompt)
    {
        if (!string.IsNullOrWhiteSpace(prompt)) _prompts.Add(prompt.Trim());
    }

    // List prompts (for review)
    public IEnumerable<string> GetAllPrompts() => _prompts.AsReadOnly();
}
