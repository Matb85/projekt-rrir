namespace Projekt_RRIR;

internal class Program {
    public static void Main(string[] args) {
        var solver = new Solver();

        var elements = args.Length > 0 ? int.Parse(args[0]) : 0;
        if (elements < 3)
            throw new Exception("Liczba elementów musi być >= 3");

        var saveDir = args.Length > 1 ? args[1] : "";

        solver.Solve(elements, saveDir);
    }
}