using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using ScottPlot;

namespace Projekt_RRIR;

public class Solver {
    private const int IntegrationPointCount = 128; // > 10

    private double h; // długość przedziału
    private int k; // rozmiar macierzy
    private int n; // liczba elementów

    private string saveDir;
    private bool saveToFile;

    public void Solve(int elements, string saveDirectory = "") {
        n = elements;
        k = n + 1;
        h = 2.0 / n;
        saveDir = saveDirectory;
        saveToFile = saveDir.Length > 0;

        var equationMatrix = CreateEquationMatrix();
        var resultVector = CreateResultVector();

        // u = w + uTilde
        var shifts = DenseVector.Build.Dense(k, i => uTilde(i * h));
        var w = equationMatrix.Solve(resultVector);
        var u = w.Add(shifts).ToList();

        var x = Enumerable.Range(0, k).Select(i => h * i).ToList();

        Console.WriteLine("----------Macierz B(w,v)----------");
        Console.WriteLine(equationMatrix.ToString());
        Console.WriteLine("\n----------Macierz LTilde(v)----------");
        Console.WriteLine(resultVector.ToString());
        Console.WriteLine("\n----------x----------");
        Console.WriteLine(string.Join(", ", x));
        Console.WriteLine("\n----------y----------");
        Console.WriteLine(string.Join(", ", u));

        Plot chart = new();
        chart.XLabel("x");
        chart.YLabel("u(x)");
        chart.Title("Wibracje akustyczne warstwy materiału");
        chart.Add.ScatterLine(x, u);
        chart.SavePng(saveDir + "/wykres.png", 800, 450);

        PlotElements();
    }

    private Vector<double> CreateResultVector() {
        return DenseVector.Build.Dense(k, i => L(i));
    }

    private Matrix<double> CreateEquationMatrix() {
        var equationMatrix = new DenseMatrix(k, k);

        for (var i = 1; i < k; i++) {
            var prev = i - 1;
            var next = i + 1;
            if (prev >= 0)
                // nad przekątną
                equationMatrix[prev, i] = B(prev, i);

            // po przekątnej
            equationMatrix[i, i] = B(i, i);

            if (next < k)
                // pod przekątną
                equationMatrix[next, i] = B(next, i);
        }

        equationMatrix[0, 0] = 1;
        equationMatrix[0, 1] = 0;

        return equationMatrix;
    }

    private double Integrate(Func<double, double> func, double a, double b) {
        return GaussLegendreRule.Integrate(func, a, b, IntegrationPointCount);
    }

    private double B(int i, int j) {
        var a = Math.Max(0.0, h * (Math.Max(i, j) - 1));
        var b = Math.Min(2.0, h * (Math.Min(i, j) + 1));
        // Console.WriteLine($"B, a: {a}, b: {b}");

        return Integrate(x => ePrim(i, x) * ePrim(j, x), a, b) // ∫ w'(x)*v'(x)
               - Integrate(x => e(i, x) * e(j, x), a, b) // ∫ w(x)*v(x)
               - e(i, 2) * e(j, 2); // w(2)*v(2)
    }

    private double L(int i) {
        if (i == 0) return 0;

        var a = Math.Max(0.0, h * (i - 1));
        var b = Math.Min(2.0, h * (i + 1));
        // Console.WriteLine($"L, a: {a}, b: {b}");

        return Integrate(x => e(i, x) * Math.Sin(x), a, b) // ∫ v(x)*sin(x) dx | L(v_i)
               + 5 * e(i, 2) // 5v(2) | L(v_i)
               + Integrate(x => uTilde(x) * e(i, x), a, b) // ∫ uTilde(x)*v(x) dx | B(uTilde, v_i)
               - Integrate(x => uTildePrim(x) * ePrim(i, x), a, b) // ∫ uTilde'(x)*v'(x) dx | B(uTilde, v_i)
               + uTilde(2) * e(i, 2); // uTilde(x)*v'x) | B(uTilde, v_i)
    }

    // Funkcje e_i tworzą bazę przestrzeni V, tzw. baza daszkowa
    // e_i zwraca liczbę z przedziału [0,1]
    private double e(int i, double x) {
        return Math.Max(0.0, 1 - Math.Abs(x - h * i) * n / 2.0);
    }

    private double ePrim(int i, double x) {
        return x switch {
            _ when x <= h * (i - 1) || x >= h * (i + 1) => 0,
            _ when x < h * i => n / 2.0,
            _ => -n / 2.0
        };
    }

    // shift
    private double uTilde(double x) {
        return e(0, x);
    }

    private double uTildePrim(double x) {
        return ePrim(0, x);
    }

    private void PlotElements() {
        Plot chart = new();

        chart.XLabel("x");
        chart.YLabel("y");
        chart.Title($"Wykres elementów dla n={n}");

        for (var j = 0; j < n; j++) {
            var x = new List<double>();
            var y = new List<double>();

            for (double i = 0; i < 2.0; i += h / 100) {
                x.Add(i);
                y.Add(e(j, i));
            }

            var currentLine = chart.Add.ScatterLine(x, y);
            currentLine.LegendText = $"Element {j}";
        }

        chart.ShowLegend(Edge.Bottom);
        if (saveToFile) chart.SavePng(saveDir + "/elementy.png", 800, 450);
    }
}