using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Students.Database;

[Table("students")]
public class Student
{
    [Key]
    [Column("studentnumber")]
    public int StudentNumber { get; set; }

    [Column("firstname")]
    public string FirstName { get; set; } = string.Empty;

    [Column("lastname")]
    public string LastName { get; set; } = string.Empty;
}