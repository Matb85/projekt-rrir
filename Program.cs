namespace Projekt_RRIR;

internal class Program
{
    public static void Main(string[] args)
    {
        var solver = new Solver();

        if (args.Length == 0)
            // Default for 3 elements
            solver.Solve(6);
        else if (int.Parse(args[0]) < 3)
            throw new Exception("Liczba elementów musi być >= 3");
        else
            // User provided the number of elements
            solver.Solve(int.Parse(args[0]));
    }
}