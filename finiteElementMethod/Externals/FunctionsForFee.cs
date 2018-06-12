using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finiteElementMethod.Externals
{
    class FunctionsForFee
    {
        public static double[][] Fee = new StandartElement(-1, 0, 1).InlineArr();

        public static double ONE_EIGHT = 0.125;
        public static double ONE_FOURTH = 0.25;

        // Main Calculation
        public static double CalculateDFee(int var, int i, double alpha, double beta, double gamma)
        {
            switch (var)
            {
                case 0: return dAlpha(i, alpha, beta, gamma);
                case 1: return dBeta(i, alpha, beta, gamma);
                case 2: return dGamma(i, alpha, beta, gamma);
                default: return 0.0;
            }
        }

        private static double firstFee(int i, double alpha, double beta, double gamma)
        {
            double result;
            double[] coord = Fee[i];
            result = ONE_EIGHT *
                (1 + alpha * coord[0]) *
                (1 + beta * coord[1]) *
                (1 + gamma * coord[2]) *
                (alpha * coord[0] + beta * coord[1] + gamma * coord[2] - 2);

            return result;
        }

        private static double secondFee(int i, double alpha, double beta, double gamma)
        {
            double result;
            double[] coord = Fee[i];
            result = ONE_FOURTH *
                (1 + alpha * coord[0]) *
                (1 + beta * coord[1]) *
                (1 + gamma * coord[2]) *
                (1 - Math.Pow((alpha * coord[1] * coord[2]), 2) - Math.Pow((beta * coord[0] * coord[2]), 2) - Math.Pow((gamma * coord[0] * coord[1]), 2));

            return result;
        }

        // Calculates Feei-th(A,B,G)
        public static double getFee(int i, double alpha, double beta, double gamma)
        {
            return i < 8 ? firstFee(i, alpha, beta, gamma) : secondFee(i, alpha, beta, gamma);
        }

        // Deriviates 
        public static double dAlphaFirst(int i, double alpha, double beta, double gamma)
        {
            double result;
            double[] coord = Fee[i];
            result = ONE_EIGHT *
                coord[0] *
                (1 + beta * coord[1]) *
                (1 + gamma * coord[2]) *
                (2 * alpha * coord[0] + beta * coord[1] + gamma * coord[2] - 1);

            return result;
        }

        public static double dBetaFirst(int i, double alpha, double beta, double gamma)
        {
            double result;
            double[] coord = Fee[i];
            result = ONE_EIGHT *
                (1 + alpha * coord[0]) *
                coord[1] *
                (1 + gamma * coord[2]) *
                (alpha * coord[0] + 2 * beta * coord[1] + gamma * coord[2] - 1);

            return result;
        }

        public static double dGammaFirst(int i, double alpha, double beta, double gamma)
        {
            double result;
            double[] coord = Fee[i];
            result = ONE_EIGHT *
                (1 + alpha * coord[0]) *
                (1 + beta * coord[1]) *
                coord[2] *
                (alpha * coord[0] + beta * coord[1] + 2 * gamma * coord[2] - 1);

            return result;
        }

        public static double dAlphaSecond(int i, double alpha, double beta, double gamma)
        {
            double result;
            double[] coord = Fee[i];
            result = ONE_FOURTH *
                (1 + beta * coord[1]) *
                (1 + gamma * coord[2]) *
                (coord[0] * (1 - Math.Pow((alpha * coord[1] * coord[2]), 2) - Math.Pow((beta * coord[0] * coord[2]), 2) - Math.Pow((gamma * coord[0] * coord[1]), 2)) -
                  2 * alpha * Math.Pow((coord[1] * coord[2]), 2) * (1 + alpha * coord[0]));

            return result;
        }

        public static double dBetaSecond(int i, double alpha, double beta, double gamma)
        {
            double result;
            double[] coord = Fee[i];
            result = ONE_FOURTH *
                (1 + alpha * coord[0]) *
                (1 + gamma * coord[2]) *
                (coord[1] * (1 - Math.Pow((alpha * coord[1] * coord[2]), 2) - Math.Pow((beta * coord[0] * coord[2]), 2) - Math.Pow((gamma * coord[0] * coord[1]), 2)) -
                  2 * beta * Math.Pow((coord[0] * coord[2]), 2) * (1 + beta * coord[1]));

            return result;
        }

        public static double dGammaSecond(int i, double alpha, double beta, double gamma)
        {
            double result;
            double[] coord = Fee[i];
            result = ONE_FOURTH *
                (1 + alpha * coord[0]) *
                (1 + beta * coord[1]) *
                (coord[2] * (1 - Math.Pow((alpha * coord[1] * coord[2]), 2) - Math.Pow((beta * coord[0] * coord[2]), 2) - Math.Pow((gamma * coord[0] * coord[1]), 2)) -
                  2 * gamma * Math.Pow((coord[0] * coord[1]), 2) * (1 + gamma * coord[2]));

            return result;
        }

        // Fun-calls specifivations 
        private static double dAlpha(int i, double alpha, double beta, double gamma)
        {
            return i < 8 ? dAlphaFirst(i, alpha, beta, gamma) : dAlphaSecond(i, alpha, beta, gamma);
        }
        private static double dBeta(int i, double alpha, double beta, double gamma)
        {
            return i < 8 ? dBetaFirst(i, alpha, beta, gamma) : dBetaSecond(i, alpha, beta, gamma);
        }
        private static double dGamma(int i, double alpha, double beta, double gamma)
        {
            return i < 8 ? dGammaFirst(i, alpha, beta, gamma) : dGammaSecond(i, alpha, beta, gamma);
        }
    }
}
