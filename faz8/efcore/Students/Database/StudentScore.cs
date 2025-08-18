using System.ComponentModel.DataAnnotations.Schema;

namespace Students.Database;

[Table("studentscores")]
public class StudentScore
{
    [Column("studentnumber")]
    public int StudentNumber { get; set; }

    [Column("lesson")]
    public string Lesson { get; set; } = string.Empty;

    [Column("score")]
    public double Score { get; set; }
}