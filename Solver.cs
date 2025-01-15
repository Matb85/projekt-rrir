using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Plotly.NET.CSharp;

namespace Projekt_RRIR;

public class Solver
{
    private const int IntegrationPointCount = 128; // > 10

    private double h; // długość przedziału

    private int k;

    private int n; // liczba elementów

    public void Solve(int elements)
    {
        n = elements;
        k = n + 1;
        h = 2.0 / n;

        var equationMatrix = CreateEquationMatrix();
        var resultVector = CreateResultVector();
        var coefficients = equationMatrix.Solve(resultVector);

        var y = coefficients.ToList();
        y.Add(0.0);

        var x = CreateXList();

        Console.WriteLine("----------Macierz A----------");
        Console.WriteLine(equationMatrix.ToString());
        Console.WriteLine("\n----------Macierz B----------");
        Console.WriteLine(resultVector.ToString());
        Console.WriteLine("\n----------x----------");
        Console.WriteLine(string.Join(", ", x));
        Console.WriteLine("\n----------y----------");
        Console.WriteLine(string.Join(", ", y));

        Chart.Line<double, double, string>(x, y, Name: "Wibracje akustyczne warstwy materiału").Show();

        //PlotElements();
    }

    private List<double> CreateXList()
    {
        return Enumerable.Range(0, k).Select(i => h * i).ToList();
    }

    private Vector<double> CreateResultVector()
    {
        var v = DenseVector.Build.Dense(k, i => L(i));

        v[0] = 1;

        return v;
    }

    private Matrix<double> CreateEquationMatrix()
    {
        var equationMatrix = new DenseMatrix(k, k);

        for (var i = 1; i < k; i++)
        {
            if (i > 0)
            {
                // nad przekątną
                var a2 = Math.Max(0.0, 2.0 * (i - 1) / n);
                var b2 = Math.Min(2.0, 2.0 * i / n);

                equationMatrix[i - 1, i] = B(i - 1, i, a2, b2);
            }

            // po przekątnej
            var a1 = Math.Max(0.0, h * (i - 1));
            var b1 = Math.Min(2.0, h * (i + 1));

            equationMatrix[i, i] = B(i, i, a1, b1);

            if (i < k - 1)
            {
                // pod przekątną
                var a3 = Math.Max(0.0, 2.0 * i / n);
                var b3 = Math.Min(2.0, 2.0 * (i + 1) / n);

                equationMatrix[i + 1, i] = B(i + 1, i, a3, b3);
            }
        }

        equationMatrix[0, 0] = 1;
        equationMatrix[0, 1] = 0;

        return equationMatrix;
    }

    private double Integrate(Func<double, double> func, double a, double b)
    {
        return GaussLegendreRule.Integrate(func, a, b, IntegrationPointCount);
    }

    private double B(int i, int j, double a, double b)
    {
        Console.WriteLine($"B, a: {a}, b: {b}");

        return Integrate(x => ePrim(i, x) * ePrim(j, x), a, b)
               - Integrate(x => e(i, x) * e(j, x), a, b)
               - e(i, 2) * e(j, 2);
    }

    private double L(int i)
    {
        var a = Math.Max(0.0, h * (i - 1));
        var b = Math.Min(2.0, h * (i + 1));
        Console.WriteLine($"L, a: {a}, b: {b}");

        return Integrate(x => e(i, x) * Math.Sin(x), a, b) // L(v_i)
               + 5 * e(i, 2) // L(v_i)
               + Integrate(x => uTilde(x) * e(i, x), a, b) // B(uTilde, v_i)
               - Integrate(x => uTildePrim(x) * ePrim(i, x), a, b) // B(uTilde, v_i)
               + uTilde(2) * e(i, 2); // B(uTilde, v_i)
    }

    // Funkcje e_i tworzą bazę przestrzeni V, tzw. baza daszkowa
    // e_i zwraca liczbę z przedziału [0,1]
    private double e(int i, double x)
    {
        return Math.Max(0.0, 1 - Math.Abs(x - h * i) * n / 2.0);
    }

    private double ePrim(int i, double x)
    {
        var prev = h * (i - 1);
        var cur = h * i;
        var next = h * (i + 1);

        return x switch
        {
            _ when x <= prev || x >= next => 0,
            _ when x < cur => n / 2.0,
            _ => -n / 2.0
        };
    }

    // ~u(0) = 1
    private double uTilde(double x)
    {
        return e(0, x);
        //return 1.0 - x / 2.0;
    }

    private double uTildePrim(double x)
    {
        return ePrim(0, x);
        //return -1.0 / 2.0;
    }

    private void PlotElements()
    {
        var xs = new List<List<double>>();
        var ys = new List<List<double>>();

        for (var j = 0; j < n; j++)
        {
            var x = new List<double>();
            var y = new List<double>();

            for (double i = 0; i < 2.0; i += h / 100)
            {
                x.Add(i);
                y.Add(e(j, i));
            }

            xs.Add(x);
            ys.Add(y);
        }

        var charts = Enumerable.Range(0, n)
            .Select(i => Chart.Line<double, double, string>(xs[i], ys[i], Name: $"Element {i}"));

        Chart.Combine(charts).Show();
    }
}