using Students.Model;
using Microsoft.EntityFrameworkCore;
using Students.Abstraction;
using Students.Database;

namespace Students.Business;

internal sealed class TopStudentsGetter : ITopStudentsGetter
{
    public async Task<List<StudentInfo>> GetTopStudents(StudentDbContext context)
    {
        return await context.StudentScore
            .GroupBy(s => s.StudentNumber)
            .Select(scoreGroup => new {
                StudentNumber = scoreGroup.Key,
                AverageScore = scoreGroup.Average(s => s.Score),
                Subjects = scoreGroup.Select(s => s.Lesson).ToList()
            })
            .Join(context.Students,
                score => score.StudentNumber,
                student => student.StudentNumber,
                (score, student) => new StudentInfo {
                    StudentNumber = student.StudentNumber,
                    FullName = $"{student.FirstName} {student.LastName}",
                    AverageScore = score.AverageScore,
                    Subjects = score.Subjects
                })
            .OrderByDescending(s => s.AverageScore)
            .Take(3)
            .ToListAsync();
    }
}