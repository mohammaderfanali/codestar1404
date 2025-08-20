using Students.Database;
using Students.Model;

namespace Students.Abstraction;

internal interface ITopStudentsGetter
{
    Task<List<StudentInfo>> GetTopStudents(StudentDbContext context);
}