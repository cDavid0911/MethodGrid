// documentation: https://ru.wikipedia.org/wiki/Метод_прогонки

namespace MethodGrid
{
    public delegate double TridiagonalMatrixComponent(int i = 1);
    public class TridiagonalMatrix
    {
        public int Length { get; private set; }
        public TridiagonalMatrixComponent A { get; private set; }
        public TridiagonalMatrixComponent B { get; private set; }
        public TridiagonalMatrixComponent C { get; private set; }

        public TridiagonalMatrix(int length,
            TridiagonalMatrixComponent A,
            TridiagonalMatrixComponent B,
            TridiagonalMatrixComponent C)
        {
            Length = length;
            this.A = i => (i == 0) ? 0 : A(i);
            this.B = B;
            this.C = i => (i == Length - 1) ? 0 : C(i);
        }

        public double[] GetAlpha()
        {
            var alpha = new double[Length];
            alpha[0] = -C(0) / B(0);
            for (int i = 0; i < alpha.Length - 1; i++)
                alpha[i + 1] = -C(i) / (A(i) * alpha[i] + B(i));
            return alpha;
        }

        public double[] GetBeta(double[] F, double[] alpha)
        {
            var beta = new double[Length];
            beta[0] = F[0] / B(0);
            for (int i = 0; i < beta.Length - 1; i++)
                beta[i + 1] = (F[i] - A(i) * beta[i]) / (A(i) * alpha[i] + B(i));
            return beta;
        }
    }
}
