using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class StudentScore
{
    public int StudentNumber { get; set; }
    public string Lesson { get; set; } = string.Empty;
    public double Score { get; set; }
}

public class StudentInfo
{
    public int StudentNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // 1. Load student scores
            string scoresJson = File.ReadAllText("scores.json");
            List<StudentScore>? scores = JsonSerializer.Deserialize<List<StudentScore>>(scoresJson);
            
            if (scores is null)
            {
                Console.WriteLine("Error: No scores data found");
                return;
            }

            // 2. Load student information
            string infoJson = File.ReadAllText("students.json");
            List<StudentInfo>? studentInfos = JsonSerializer.Deserialize<List<StudentInfo>>(infoJson);
            
            if (studentInfos is null)
            {
                Console.WriteLine("Error: No student info found");
                return;
            }

            // 3. Join the data and get top 3 students
            var topStudents = scores
                .GroupBy(s => s.StudentNumber)
                .Join(studentInfos,
                    scoreGroup => scoreGroup.Key,
                    info => info.StudentNumber,
                    (scoreGroup, info) => new {
                        info.StudentNumber,
                        FullName = $"{info.FirstName} {info.LastName}",
                        AverageScore = scoreGroup.Average(s => s.Score),
                        Subjects = scoreGroup.Select(s => s.Lesson).ToList()
                    })
                .OrderByDescending(s => s.AverageScore)
                .Take(3);

            // 4. Print results
            Console.WriteLine("Top 3 Students with Names:");
            Console.WriteLine("==========================");
            
            int rank = 1;
            foreach (var student in topStudents)
            {
                Console.WriteLine($"{rank}. {student.FullName} (#{student.StudentNumber})");
                Console.WriteLine($"   Average Score: {student.AverageScore:F2}");
                Console.WriteLine();
                rank++;
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Error: JSON file not found");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}