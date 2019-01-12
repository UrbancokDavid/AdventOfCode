﻿using System;
using System.Collections.Generic;
using System.IO;

namespace December3
{   
    /*
    --- Day 3: No Matter How You Slice It ---
    The Elves managed to locate the chimney-squeeze prototype fabric for Santa's suit (thanks to someone who helpfully wrote its box IDs on the wall of the warehouse in the middle of the night). Unfortunately, anomalies are still affecting them - nobody can even agree on how to cut the fabric.

    The whole piece of fabric they're working on is a very large square - at least 1000 inches on each side.

    Each Elf has made a claim about which area of fabric would be ideal for Santa's suit. All claims have an ID and consist of a single rectangle with edges parallel to the edges of the fabric. Each claim's rectangle is defined as follows:

    The number of inches between the left edge of the fabric and the left edge of the rectangle.
    The number of inches between the top edge of the fabric and the top edge of the rectangle.
    The width of the rectangle in inches.
    The height of the rectangle in inches.
    A claim like #123 @ 3,2: 5x4 means that claim ID 123 specifies a rectangle 3 inches from the left edge, 2 inches from the top edge, 5 inches wide, and 4 inches tall. Visually, it claims the square inches of fabric represented by # (and ignores the square inches of fabric represented by .) in the diagram below:

    ...........
    ...........
    ...#####...
    ...#####...
    ...#####...
    ...#####...
    ...........
    ...........
    ...........
    The problem is that many of the claims overlap, causing two or more claims to cover part of the same areas. For example, consider the following claims:

    #1 @ 1,3: 4x4
    #2 @ 3,1: 4x4
    #3 @ 5,5: 2x2
    Visually, these claim the following areas:

    ........
    ...2222.
    ...2222.
    .11XX22.
    .11XX22.
    .111133.
    .111133.
    ........
    The four square inches marked with X are claimed by both 1 and 2. (Claim 3, while adjacent to the others, does not overlap either of them.)

    If the Elves all proceed with their own plans, none of them will have enough fabric. How many square inches of fabric are within two or more claims?
    
     --- Part Two ---
    Amidst the chaos, you notice that exactly one claim doesn't overlap by even a single square inch of fabric with any other claim. If you can somehow draw attention to it, maybe the Elves will be able to make Santa's suit after all!

    For example, in the claims above, only claim 3 is intact after all claims are made.

    What is the ID of the only claim that doesn't overlap?
     */
    public class Program
    {
        private const string FILENAME = @"..\..\..\input.txt";

        public static void Main(string[] args)
        {
            Part1();

            Part2();

            Console.ReadKey();
        }

        private static void Part1()
        {
            var field = new int[1000, 1000];

            try
            {
                using (var reader = new StreamReader(FILENAME))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        ParseLine(line, out var x, out var y, out var horizontal, out var vertical);

                        for (int i = x; i < x + horizontal; i++)
                        {
                            for (int j = y; j < y + vertical; j++)
                            {
                                if (field[i, j] == -1)
                                {
                                    continue;
                                }

                                if (field[i, j] == 0)
                                {
                                    field[i, j]++;

                                    continue;
                                }

                                if (field[i, j] == 1)
                                {
                                    field[i, j] = -1;
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("The file could not be read.");
                Console.WriteLine(ex.Message);
            }

            var fields = 0;
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    if (field[i, j] == -1)
                    {
                        fields++;
                    }
                }
            }

            Console.WriteLine("PART 1");
            Console.WriteLine($"Overlapping fields: {fields}");
        }

        private static void Part2()
        {
            var field = new int[1000, 1000];
            var lines = new List<string>();

            try
            {
                using (var reader = new StreamReader(FILENAME))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        lines.Add(line);

                        ParseLine(line, out var x, out var y, out var horizontal, out var vertical);

                        for (int i = x; i < x + horizontal; i++)
                        {
                            for (int j = y; j < y + vertical; j++)
                            {
                                field[i, j]++;
                            }
                        }
                    }

                    foreach (var line in lines)
                    {
                        var idFound = CheckLine(field, line);

                        if (idFound)
                        {
                            var id = GetValue(line, line.IndexOf('@') - 2, '#');

                            Console.WriteLine("PART 2");
                            Console.WriteLine($"Not overlapping claim ID: {id}");

                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("The file could not be read.");
                Console.WriteLine(ex.Message);
            }
        }

        private static bool CheckLine(int[,] field, string line)
        {
            ParseLine(line, out var x, out var y, out var horizontal, out var vertical);
            
            for (int i = x; i < x + horizontal; i++)
            {
                for (int j = y; j < y + vertical; j++)
                {
                    if (field[i, j] != 1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void ParseLine(string line, out int x, out int y, out int horizontal, out int vertical)
        {
            x = GetValue(line, line.IndexOf(',') - 1, ' ');
            y = GetValue(line, line.IndexOf(':') - 1, ',');
            horizontal = GetValue(line, line.IndexOf('x') - 1, ' ');
            vertical = GetValue(line, line.Length - 1, 'x');
        }

        private static int GetValue(string line, int beginningPosition, char endingCharacter)
        {
            var multiplier = 1;
            var number = 0;

            while (line[beginningPosition] != endingCharacter)
            {
                var num = (int)char.GetNumericValue(line[beginningPosition]);
                beginningPosition--;

                number += num * multiplier;
                multiplier *= 10;
            }

            return number;
        }
    }
}
