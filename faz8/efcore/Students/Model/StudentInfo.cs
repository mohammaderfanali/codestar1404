namespace Students.Model;

internal sealed class StudentInfo
{
    public required int StudentNumber { get; init; }
    public required string FullName { get; init; }
    public required double AverageScore { get; init; }
    public required List<string> Subjects { get; init; }
}