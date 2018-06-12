namespace finiteElementMethod.Externals
{
    using System;

    public static class External
    {
        public static double[][] GaussianNodes = GenGaussianNodes();
        public static double[][] lilGaussNodes = GenLilGaussianNodes();

        public static double[][] GenGaussianNodes()
        {
            double[][] result = new double[27][];
            double[] values = new double[] { Math.Sqrt(0.6) * (-1), 0, Math.Sqrt(0.6) };

            int counter = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        result[counter] = new double[] { values[i], values[j], values[k] };
                        ++counter;
                    }
                }
            }

            return result;
        }

        public static double[,,] GenDFIABG()
        {
            double[,,] result = new double[27, 3, 20];
            
            for (int n = 0; n < 27; n++) // Gauss nodes
            {
                for (int loc = 0; loc < 3; loc++) // Local variables
                {
                    for (int f = 0; f < 20; f++) // Functions
                    {
                        result[n, loc, f] = FunctionsForFee.CalculateDFee(loc, f, GaussianNodes[n][0], GaussianNodes[n][1], GaussianNodes[n][2]);
                    }
                }
            }

            return result;
        }

        private static double[][] GenLilGaussianNodes()
        {
            double[][] res = new double[9][];
            double[] val = new double[] { -1 * Math.Sqrt(0.6), 0, Math.Sqrt(0.6) };

            int counter = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    res[counter] = new double[] { val[i], val[j] };
                    ++counter;
                }
            }

            return res;
        }

        public static double[,,] GenDPSITE()
        {
            double[,,] result = new double[9, 2, 8];

            for (int n = 0; n < 9; n++) // Gaussian nodes
            {
                for (int loc = 0; loc < 2; loc++) // Local variables
                {
                    for (int f = 0; f < 8; f++) // Functions
                    {
                        result[n, loc, f] = FunctionsForPSI.CalculateDPSI(loc, f, lilGaussNodes[n][0], lilGaussNodes[n][1]);
                    }
                }
            }

            return result;
        }
    }
}
