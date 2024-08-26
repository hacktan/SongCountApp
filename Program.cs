using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;


class Program
{
    static void Main()
    {
        // Original CSV Path
        string filePath = "C:/Users/haktan/Downloads/CSharp_Project/TestConsoleApp/exhibitA-input.csv";
        
        // Cleaned CSV Path
        string cleanedFilePath = "C:/Users/haktan/Downloads/CSharp_Project/TestConsoleApp/cleaned_data.csv";

        // List to hold cleaned rows of data
        List<string[]> cleanedData = new List<string[]>();


        // Read, clean the CSV data and write to a new CSV file
        using (var reader = new StreamReader(filePath))
        {
            // Read the header
            string header = reader.ReadLine();
            cleanedData.Add(header.Split('\t'));

            // Read the rest of the data other then header
            while (!reader.EndOfStream)
            {
                // Read a line
                var line = reader.ReadLine();
                var values = line.Split('\t');

                // Extract the date and time string
                string playTsString = values[3];
                DateTime playTs;

                // Try to parse the date and time using multiple formats
                string[] formats = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy" };
                bool isValidDate = DateTime.TryParseExact(playTsString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out playTs);

                // If the date is valid and only contains the date, add the time component
                if (isValidDate && playTsString.Length == 10) // "dd/MM/yyyy"
                {
                    playTs = playTs.Date; // This automatically sets time to 00:00:00
                }
                else if (!isValidDate)
                {
                    Console.WriteLine($"Invalid date format in row: {line}");
                    continue; // Skip this record
                }

                // Update the date to the standardized format
                values[3] = playTs.ToString("dd/MM/yyyy HH:mm:ss");

                // Add the cleaned row to the list
                cleanedData.Add(values);
            }
        }

        // Write the cleaned data to a new CSV file
        using (var writer = new StreamWriter(cleanedFilePath))
        {
            foreach (var row in cleanedData)
            {
                writer.WriteLine(string.Join("\t", row));
            }
        }

        // Now perform the calculation on the cleaned data
        Dictionary<int, HashSet<int>> userSongMap = new Dictionary<int, HashSet<int>>();

        foreach (var values in cleanedData.Skip(1)) // Skip the header
        {
            int songId = int.Parse(values[1]);
            int clientId = int.Parse(values[2]);
            DateTime playTs = DateTime.ParseExact(values[3], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            // Check if the date is August 10, 2016
            if (playTs.Date == new DateTime(2016, 8, 10))
            {
                if (!userSongMap.ContainsKey(clientId))
                {
                    userSongMap[clientId] = new HashSet<int>();
                }
                userSongMap[clientId].Add(songId);
            }
        }

        // Calculate the distinct play count for each client
        Dictionary<int, int> distinctPlayCountMap = new Dictionary<int, int>();

        foreach (var entry in userSongMap)
        {
            int distinctSongCount = entry.Value.Count;

            if (!distinctPlayCountMap.ContainsKey(distinctSongCount))
            {
                distinctPlayCountMap[distinctSongCount] = 0;
            }

            distinctPlayCountMap[distinctSongCount]++;
        }

        Console.WriteLine("DISTINCT_PLAY_COUNT\tCLIENT_COUNT");
        foreach (var entry in distinctPlayCountMap.OrderBy(x => x.Key))
        {
            Console.WriteLine($"{entry.Key}\t{entry.Value}");
        }

    }
}