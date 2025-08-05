namespace calculator;

public class Calculator
{
    public int Getreminder(params int[] s)
    {
        int result = 0;
        foreach (var i in s)
        {
            result += i;
        }
        return result%7;
    }
}