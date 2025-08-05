using calculator;

using faz3;
using Xunit;
namespace MyProject.Tests;
using System.Collections.Generic;

public class UnitTest1
{
    private readonly Calculator _cal;

    public UnitTest1()
    {
        _cal = new ();
    }
    [Fact]
    public void TestReminder()
    {
        int answer = _cal.Getreminder(13, 7);
        Assert.Equal(6, answer);

    }

    [Theory]
    [ClassData(typeof(Data))]
    public void TestGetreminder(int expected,params int[] par)
    {
        int answer = _cal.Getreminder(par);
        Assert.Equal(expected, answer);
    }
}

public class Data : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]{0,14,7};
        yield return new object[]{0,15,7};;
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

}
