namespace finiteElementMethod.Externals
{
    using System.Collections.Generic;

    class StandartElement
    {
        private double[] mVals;
        private double[,,][] mNodes;
        private Dictionary<int, int[]> mRelPositions = new Dictionary<int, int[]> {
            {0, new int[]{0,0,0} },
            {1, new int[]{2,0,0} },
            {2, new int[]{2,2,0} },
            {3, new int[]{0,2,0} },

            {4, new int[]{0,0,2} },
            {5, new int[]{2,0,2} },
            {6, new int[]{2,2,2} },
            {7, new int[]{0,2,2} },

            {8,  new int[]{1,0,0} },
            {9,  new int[]{2,1,0} },
            {10, new int[]{1,2,0} },
            {11, new int[]{0,1,0} },

            {12, new int[]{0,0,1} },
            {13, new int[]{2,0,1} },
            {14, new int[]{2,2,1} },
            {15, new int[]{0,2,1} },

            {16, new int[]{1,0,2} },
            {17, new int[]{2,1,2} },
            {18, new int[]{1,2,2} },
            {19, new int[]{0,1,2} },
        };
        
        public StandartElement(double bottom, double middle, double top)
        {
            mVals = new double[] { bottom, middle, top };
        }

        public double[,,][] GenNodes()
        {
            mNodes = new double[3, 3, 3][];
            for (int z = 0; z < 3; z++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        mNodes[x, y, z] = new double[] { mVals[x], mVals[2 - y], mVals[z] };
                    }
                }
            }

            return mNodes;
        }

        public double[][] InlineArr()
        {
            GenNodes();
            double[][] result = new double[20][];
            int[] nodes;
            for (int i = 0; i < 20; i++)
            {
                nodes = mRelPositions[i];
                result[i] = mNodes[nodes[0], nodes[1], nodes[2]];
            }
            return result;
        }
    }
}