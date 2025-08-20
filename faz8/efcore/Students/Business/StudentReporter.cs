using Students.Abstraction;
using Students.Model;

namespace Students.Business;

internal sealed class StudentReporter : IStudentReporter
{
    public void ReportStudents(List<StudentInfo> topStudentInfos)
    {
        Console.WriteLine($"Top {topStudentInfos.Count} Students with Names:");
        Console.WriteLine("==========================");
            
        var rank = 1;
        foreach (var student in topStudentInfos)
        {
            Console.WriteLine($"{rank++}. {student.FullName} (#{student.StudentNumber})");
            Console.WriteLine($"   Average Score: {student.AverageScore:F2}");
            Console.WriteLine();
        }
    }
}