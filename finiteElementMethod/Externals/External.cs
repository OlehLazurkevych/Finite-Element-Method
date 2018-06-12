using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finiteElementMethod.Externals
{
    public static class External
    {
        public static double[][] GaussianNodes = GenGaussianNodes();

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
    }
}
