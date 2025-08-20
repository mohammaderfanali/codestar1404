using Students.Business;
using Students.Database;

await using var context = new StudentDbContext();

var topStudentInfos = await new TopStudentsGetter().GetTopStudents(context);

Console.WriteLine("Top 3 Students with Names:");
Console.WriteLine("==========================");
            
var rank = 1;
foreach (var student in topStudentInfos)
{
    Console.WriteLine($"{rank++}. {student.FullName} (#{student.StudentNumber})");
    Console.WriteLine($"   Average Score: {student.AverageScore:F2}");
    Console.WriteLine();
}
