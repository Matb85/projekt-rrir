# Projekt - Równania różniczkowe i różnicowe

Mateusz Bis

Projekt został zrealizowany w ramach przedmiotu Równania różniczkowe i różnicowe na II roku studiów na kierunku Informatyka na Wydziale Informatyki Akademii Górniczo-Hutniczej.

## Wstęp  
Otrzymałem do wykonania problem obliczeniowy nr 4.2 - **„Wibracje akustyczne warstwy materiału”**, o równaniu:

<img width="818" alt="Screenshot 2025-02-12 at 11 09 36" src="https://github.com/user-attachments/assets/b0936ec4-980e-405f-8906-cb57fb5269e8" />

Aby rozwiązać zadanie:
1. Wyprowadziłem sformułowanie wariacyjne równania.
2. Napisałem procedurę, która generuje i rozwiązuje układ równań liniowych, a także rysuje wykres rozwiązania.

## 1. Sformułowanie wariacyjne

Wyprowadzenie jest dostępne w pliku [pdf](https://github.com/Matb85/projekt-rrir/blob/main/Projekt%20RRIR%20Zadanie%204.2.pdf).

## 2. Procedura  

### 2.1 Wymagania funkcjonalne  
Procedura powinna:
- przyjmować argument \(n\) - liczbę elementów skończonych większą lub równą 3,
- rysować wykres wyliczonego przybliżenia funkcji,
- liczyć całki numerycznie.

### 2.2 Użyte technologie  
Do stworzenia procedury wykorzystałem język **C#** na platformie [**.NET 8**](https://learn.microsoft.com/pl-pl/dotnet/core/whats-new/dotnet-8/overview), oraz dwie biblioteki:
1. [**ScottPlot.NET**](https://scottplot.net) do tworzenia wykresów i zapisywania ich jako obrazki,
2. [**Math.NET**](https://numerics.mathdotnet.com) do obliczania numerycznie całek metodą Gauss-Legendre.

### 2.3 Struktura  
Rozwiązanie składa się z dwóch plików:
- **Solver.cs** - klasa `Solver` zawierająca metodę `Solve()`, czyli właściwą procedurę,
- **Program.cs** - program główny, który po skompilowaniu można wywołać z terminalu.

#### Argumenty procedury `Solve()` oraz programu głównego:
1. `n` - liczba elementów skończonych większa lub równa 3,
2. `saveDirectory` - folder do zapisania wykresów w formacie obrazka. Jeśli argument nie zostanie podany, wykresy zostaną otwarte w przeglądarce.

#### Przykłady wywołania `Solve()` w kodzie C#:
```csharp
Solve(6,  "/home/ubuntu") // wykonaj dla 6 elementów i zapisz wykresy do folderu
Solve(15, "/home/ubuntu") // wykonaj dla 15 elementów i zapisz wykresy do folderu
```

#### Przykłady wywołania programu głównego w terminalu:
```sh
./ProjektRRIR 8 /home/ubuntu  # wykonaj dla 8 elementów i zapisz wykresy do folderu
./ProjektRRIR 80 /home/ubuntu # wykonaj dla 80 elementów i zapisz wykresy do folderu
```

## 3. Wykres

Dla n = 16:

![wykres](https://github.com/user-attachments/assets/2f62711b-0cc9-47b5-815b-6e059c6bf077)

![elementy](https://github.com/user-attachments/assets/67b8baea-09b8-4dc7-8989-bdcca447149d)

### Przydatne linki:
- [Microsoft .NET 8 Overview](https://learn.microsoft.com/pl-pl/dotnet/core/whats-new/dotnet-8/overview)
- [ScottPlot.NET](https://scottplot.net)
- [Math.NET Numerics](https://numerics.mathdotnet.com)
- [Gauss-Legendre Rule](https://numerics.mathdotnet.com/Integration#Gauss-Legendre-Rule)
