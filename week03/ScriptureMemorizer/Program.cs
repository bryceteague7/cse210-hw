using System;
using System.Collections.Generic;
using System.Linq;

public class Reference
{
    private string _book;
    private int _chapter;
    private int _verseStart;
    private int _verseEnd;

    public Reference(string book, int chapter, int verse)
    {
        _book = book;
        _chapter = chapter;
        _verseStart = verse;
        _verseEnd = verse;
    }

    public Reference(string book, int chapter, int startVerse, int endVerse)
    {
        _book = book;
        _chapter = chapter;
        _verseStart = startVerse;
        _verseEnd = endVerse;
    }

    public string GetDisplayText()
    {
        if (_verseStart == _verseEnd)
            return $"{_book} {_chapter}:{_verseStart}";
        else
            return $"{_book} {_chapter}:{_verseStart}-{_verseEnd}";
    }
}

public class Word
{
    private string _text;
    private bool _isHidden;

    public Word(string text)
    {
        _text = text;
        _isHidden = false;
    }

    public void Hide()
    {
        _isHidden = true;
    }

    public bool IsHidden()
    {
        return _isHidden;
    }

    public string GetDisplayText()
    {
        return _isHidden ? new string('_', _text.Length) : _text;
    }
}

public class Scripture
{
    private Reference _reference;
    private List<Word> _words;
    private static Random _rand = new Random();

    public Scripture(Reference reference, string text)
    {
        _reference = reference;
        _words = text.Split(" ").Select(w => new Word(w)).ToList();
    }

    public void HideRandomWords(int count)
    {
        var visibleWords = _words.Where(w => !w.IsHidden()).ToList();
        if (visibleWords.Count == 0) return;

        for (int i = 0; i < count; i++)
        {
            if (visibleWords.Count == 0) break;
            int index = _rand.Next(visibleWords.Count);
            visibleWords[index].Hide();
            visibleWords.RemoveAt(index); // remove to avoid hiding same word twice
        }
    }

    public string GetDisplayText()
    {
        string verse = string.Join(" ", _words.Select(w => w.GetDisplayText()));
        return $"{_reference.GetDisplayText()}\n{verse}";
    }

    public bool AllWordsHidden()
    {
        return _words.All(w => w.IsHidden());
    }
}

// New ScriptureLibrary class
public class ScriptureLibrary
{
    private List<Scripture> _scriptures;
    private static Random _rand = new Random();

    public ScriptureLibrary()
    {
        _scriptures = new List<Scripture>();
    }

    public void AddScripture(Scripture scripture)
    {
        _scriptures.Add(scripture);
    }

    public Scripture GetRandomScripture()
    {
        if (_scriptures.Count == 0) return null;
        return _scriptures[_rand.Next(_scriptures.Count)];
    }

    public List<Scripture> GetAllScriptures()
    {
        return _scriptures;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();

        // Create a scripture library
        ScriptureLibrary library = new ScriptureLibrary();

        // Add multiple scriptures to the library
        library.AddScripture(new Scripture(
            new Reference("Proverbs", 3, 5, 6),
            "Trust in the Lord with all thine heart and lean not unto thine own understanding"));

        library.AddScripture(new Scripture(
            new Reference("John", 3, 16),
            "For God so loved the world that he gave his only begotten Son that whosoever believeth in him should not perish but have everlasting life"));

        library.AddScripture(new Scripture(
            new Reference("Philippians", 4, 13),
            "I can do all things through Christ which strengtheneth me"));

        // Pick a random scripture from the library
        Scripture scripture = library.GetRandomScripture();

        if (scripture == null)
        {
            Console.WriteLine("No scriptures available in the library.");
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine(scripture.GetDisplayText());
            Console.WriteLine("\nPress Enter to hide words or type 'quit' to exit.");
            string input = Console.ReadLine();

            if (input.ToLower() == "quit")
                break;

            scripture.HideRandomWords(3);

            if (scripture.AllWordsHidden())
            {
                Console.Clear();
                Console.WriteLine(scripture.GetDisplayText());
                Console.WriteLine("\nAll words hidden. Program ending.");
                break;
            }
        }
    }
}
