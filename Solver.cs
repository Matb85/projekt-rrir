using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Plotly.NET.CSharp;

namespace Projekt_RRIR;

public class Solver
{
    private const int IntegrationPointCount = 64; // > 10

    private double h; // długość przedziału
    private int k; // wielkość macierzy

    private int n; // liczba elementów

    public void Solve(int n)
    {
        this.n = n;
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
        var resultVector = new DenseVector(k);

        resultVector[0] = 0; // Warunek brzegowy Dirichleta

        for (var i = 1; i < k; i++) resultVector[i] = L(i);

        return resultVector;
    }

    private Matrix<double> CreateEquationMatrix()
    {
        var equationMatrix = new DenseMatrix(k, k);

        for (var i = 1; i < k; i++)
        for (var j = i; j < k; j++)
            equationMatrix[i, j] = equationMatrix[j, i] = B(i, j);

        for (var i = 1; i < k; i++)
        {
            equationMatrix[0, i] = 0;
            equationMatrix[i, 0] = 0;
        }

        equationMatrix[0, 0] = 1;

        return equationMatrix;
    }

    private double Integrate(Func<double, double> func, double a, double b)
    {
        return GaussLegendreRule.Integrate(func, a, b, IntegrationPointCount);
    }

    private double B(int i, int j)
    {
        var rest = -e(j, 2) * (e(i, 2) + 0);
        if (Math.Abs(i - j) >= 2)
            return rest;

        var a = Math.Max(0, 2.0 / n * (Math.Max(i, j) - 1));
        var b = Math.Min(2, 2.0 / n * (Math.Min(i, j) + 1));

        var integralResult1 = Integrate(x => ePrim(i, x) * ePrim(j, x), a, b);
        var integralResult2 = Integrate(x => -e(i, x) * e(j, x), a, b);
        return integralResult1 + integralResult2 + rest;
    }

    private double L(int i)
    {
        var a = Math.Max(0, 2.0 / n * (i - 1));
        var b = Math.Min(2, 2.0 / n * (i + 1));

        var integralResult = Integrate(x => e(i, x) * Math.Sin(x), a, b);

        return integralResult;
    }

    // Metoda Galerkina
    // Ideą metody jest aproksymacja rozwiązania liniową kombinacją funkcji bazowych e_i = e_i(x)
    private double e(int i, double x)
    {
        var cur = 2.0 / n * i;
        return Math.Max(0, 1 - Math.Abs(x - cur) * n / 2.0);
    }

    private double ePrim(int i, double x)
    {
        var prev = 2.0 / n * (i - 1);
        var cur = 2.0 / n * i;
        var next = 2.0 / n * (i + 1);
        if (x <= prev || x >= next)
            return 0;
        if (x < cur)
            return n / 2.0;
        return -n / 2.0;
    }

    private void PlotElements()
    {
        var xs = new List<List<double>>();
        var ys = new List<List<double>>();

        for (var j = 0; j < n; j++)
        {
            var x = new List<double>();
            var y = new List<double>();

            var n = 100;

            for (double i = 0; i < 2.0; i += h / n)
            {
                x.Add(i);
                y.Add(e(j, i));
            }

            xs.Add(x);
            ys.Add(y);
        }

        var charts = Enumerable.Range(0, n)
            .Select(x => Chart.Line<double, double, string>(xs[x], ys[x], Name: $"Element {x}"))
            .ToList();

        Chart.Combine(charts).Show();
    }
}