using System;
using LibGit2Sharp;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            using(var repo = new Repository(@"/home/wisnu/github/keil-compiler-web-based"))
            {
                Commit commit = repo.Head.Tip;
                Console.WriteLine("Author: {0}", commit.Author.Name);
                Console.WriteLine("Message: {0}", commit.Message);
                Console.WriteLine("ID: {0}", commit.Id);
                Console.WriteLine("Committer: {0}", commit.Committer);
                Console.WriteLine("Tree: {0}", commit.Tree);
            }
        }
    }
}
