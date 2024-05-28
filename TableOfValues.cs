
namespace MethodGrid
{
    public delegate double MathFunction(double numb);
    public delegate double MathFunction2(double otherElem, double t);
    public class TableOfValues
    {
        public Grid Grid { get; private set; }
        public double[,] Values { get; private set; }

        public string Name { get; private set; }

        public TableOfValues(Grid grid)
        {
            Grid = grid;
            Values = new double[grid.GetLength(0), grid.GetLength(1)];
        }

        public TableOfValues(Grid grid, string name) : this(grid)
        {
            Name = name;  
        }


        public void FillTable(MathFunction InitialCond,
            MathFunction2 LeftBoundaryCond, MathFunction2 RightBoundaryCond, 
            MathFunction g, MathFunction f)
        {
            for (int i = 0; i < Grid.GetLength(0); i++)
                Values[i, 0] = InitialCond(Grid.grid[i, 0].X);

            for (int k = 1; k < Values.GetLength(1); k++)
            {
                for (int i = 1; i < Values.GetLength(0) - 1; i++)
                {
                    Values[i, k] =
                        Values[i, k - 1] + Grid.Tau * f(i) +
                        (g(i - 0.5) * Values[i - 1, k - 1] -
                        (g(i - 0.5) + g(i + 0.5)) * Values[i, k - 1] +
                        g(i + 0.5) * Values[i + 1, k - 1]) * Grid.Tau
                        / (Grid.H * Grid.H);
                }
                Values[0, k] = LeftBoundaryCond(Values[1, k],k);
                Values[Values.GetLength(0) - 1,k] = RightBoundaryCond(Values[Values.GetLength(0) - 2, k],k);
            }
        }

        public void FillTable2(MathFunction InitialCond,
            MathFunction LeftBoundaryCond,
            double a, MathFunction2 f)
        {
            double v = a * Grid.Tau / Grid.H;

            for (int i = 0; i < Grid.GetLength(0); i++)
                Values[i, 0] = InitialCond(Grid.GetX(i));

            for (int k = 1; k < Values.GetLength(1); k++)
            {
                Values[0, k] = LeftBoundaryCond(Grid.GetY(k));

                for (int i = 1; i < Values.GetLength(0); i++)
                    Values[i, k] = Values[i, k-1]* (1 - v) 
                            + v * Values[i-1, k-1] 
                            + f(Grid.GetX(i), Grid.GetY(k-1)) * Grid.Tau;                
            }
        }

        public void SpecifyingBoundaryAndInitialValues(MathFunction InitialCond,
            MathFunction LeftBoundaryCond, MathFunction RightBoundaryCond)
        {
            var b = Grid.GetLength(0) - 1;
            for (int i = 0; i < Grid.GetLength(0); i++)
                Values[i, 0] = InitialCond(Grid.grid[i, 0].X);

            for (int j = 0; j < Grid.GetLength(1); j++)
            {
                Values[0, j] = LeftBoundaryCond(Grid.grid[0, j].Y);
                Values[b, j] = RightBoundaryCond(Grid.grid[b, j].Y);
            }
        }

        public void FillWithExplicitFormula(MathFunction g, MathFunction f)
        {
            Name = "За Явною Схемою";
            for (int k = 1; k < Values.GetLength(1); k++)
            {
                for (int i = 1; i < Values.GetLength(0) - 1; i++)
                {
                    Values[i, k] =
                        Values[i, k - 1] + Grid.Tau * f(i) +
                        (g(i - 0.5) * Values[i - 1, k - 1] -
                        (g(i - 0.5) + g(i + 0.5)) * Values[i, k - 1] +
                        g(i + 0.5) * Values[i + 1, k - 1]) * Grid.Tau
                        / (Grid.H * Grid.H);
                }
            }
        }

        public void FillWithImplicitFormula(MathFunction g, MathFunction f)
        {
            Name = "За Неявною Схемою";
            int n = Values.GetLength(0) - 2;
            double tau = Grid.Tau;
            double hPow2 = Grid.H * Grid.H;
            var tdm = new TridiagonalMatrix(n,
                i => -1 / hPow2,
                i => 1 / tau + 2 / hPow2,
                i => -1 / hPow2);

            var alpha = tdm.GetAlpha();

            for (int k = 1; k < Values.GetLength(1); k++)
            {
                double[] F = new double[n];
                F[0] = Values[1, k - 1] / tau + Values[0, k] / hPow2;
                F[n - 1] = Values[n, k - 1] / tau + Values[n + 1, k] / hPow2;

                for (int j = 1; j < n - 1; j++)
                    F[j] = Values[j + 1, k - 1] / tau;

                var beta = tdm.GetBeta(F, alpha);

                Values[n, k] = (F[n - 1] - tdm.A() * beta[n - 1]) / (tdm.B() + tdm.A() * alpha[n - 1]);

                for (int i = n - 1; i >= 1; i--)
                    Values[i, k] = alpha[i] * Values[i + 1, k] + beta[i];
            }
        }
    }
}
