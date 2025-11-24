using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        List<Video> videos = new List<Video>();

        // ----- Video 1 -----
        Video v1 = new Video
        {
            Title = "How to Cook Pasta",
            Author = "Chef Tony",
            LengthSeconds = 320
        };
        v1.AddComment(new Comment("Alex", "This helped a lot!"));
        v1.AddComment(new Comment("Maria", "I love pasta!"));
        v1.AddComment(new Comment("Sam", "Great explanation."));
        videos.Add(v1);

        // ----- Video 2 -----
        Video v2 = new Video
        {
            Title = "Learn C# in 10 Minutes",
            Author = "CodeMaster",
            LengthSeconds = 600
        };
        v2.AddComment(new Comment("Jake", "Very helpful tutorial."));
        v2.AddComment(new Comment("Lily", "Thanks! Now I understand classes."));
        v2.AddComment(new Comment("Owen", "Super clear examples."));
        videos.Add(v2);

        // ----- Video 3 -----
        Video v3 = new Video
        {
            Title = "Minecraft Building Tips",
            Author = "BlockBuilder",
            LengthSeconds = 450
        };
        v3.AddComment(new Comment("Noah", "This design is amazing"));
        v3.AddComment(new Comment("Zoe", "I built this on my world!"));
        v3.AddComment(new Comment("Ella", "Awesome tutorial!"));
        videos.Add(v3);

        // ----- Display All Videos -----
        foreach (Video video in videos)
        {
            Console.WriteLine($"Title: {video.Title}");
            Console.WriteLine($"Author: {video.Author}");
            Console.WriteLine($"Length: {video.LengthSeconds} seconds");
            Console.WriteLine($"Number of Comments: {video.GetCommentCount()}");
            Console.WriteLine("Comments:");

            foreach (Comment c in video.GetComments())
            {
                Console.WriteLine($"  {c.Name}: {c.Text}");
            }

            Console.WriteLine();
        }
    }
}
