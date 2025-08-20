using Students.Model;

namespace Students.Abstraction;

internal interface IStudentReporter
{
    void ReportStudents(List<StudentInfo> topStudentInfos);
}