using OxyPlot;

namespace MethodGrid
{
    public class Grid
    {
        public double H { get; private set; }
        public double Tau { get; private set; }
        public double LengthX { get; private set; }
        public double LengthY { get; private set; }
        public DataPoint[,] grid { get; private set; }

        public Grid(double deltaX, double deltaY, double lengthX = 1, double lengthY = 1)
        {
            H = deltaX;
            Tau = deltaY;
            LengthX = lengthX;
            LengthY = lengthY;
            grid = new DataPoint[(int)(lengthX / deltaX) + 1, (int)(lengthY / deltaY) + 1];

            for (int i = 0; i < grid.GetLength(0); i++)
                for (int j = 0; j < grid.GetLength(1); j++)
                    grid[i, j] = new DataPoint( i * deltaX,  j * deltaY );
        }

        public double GetX(double i) { return i * H; }
        public double GetY(double j) { return j * Tau; }

        public int GetLength(int i) { return grid.GetLength(i % 2); }
    }
}
