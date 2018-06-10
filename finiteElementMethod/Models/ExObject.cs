namespace finiteElementMethod.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /*
     *  Experimental Object represented by nodes in space
     */
    public class ExObject
    {
        /*  Private members  */
        private double mWidth;  // Section on X axis
        private double mLength; // Section on Y axis
        private double mHeight; // Section on Z axis

        private int mXSlices = 0;
        private int mYSlices = 0;
        private int mZSlices = 0;

        private List<Node> mAKT = new List<Node>(); // Represents the Experimental Object by nodes which are stored in global indexation
        private List<List<int>> mNT = new List<List<int>>(); // Matrix which represents a local indexation of nodes

        private List<int> mZU = new List<int>(); // List of fixed nodes
        private List<ZPValue> mZP = new List<ZPValue>(); // List of finite elements which have some pressure on their side

        private const double FORCE = 0.3;

        /*  Private functionality  */
        /*
         *  Generating nodes on slice crossings of an object,
         *  also creates intermediate nodes between generated nodes.
         */
        private List<Node> GenAKT(double width, double length, double height, int xSlices, int ySlices, int zSlices)
        {
            List<Node> result = new List<Node>();

            int zIters = (zSlices != 0) ? (2 * (zSlices + 2) - 1) : 3;
            int yIters = (ySlices != 0) ? (2 * (ySlices + 2) - 1) : 3;
            int xIters = (xSlices != 0) ? (2 * (xSlices + 2) - 1) : 3;

            double zSpacing = height / (zIters - 1);
            double ySpacing = length / (yIters - 1);
            double xSpacing = width / (xIters - 1);

            for (int z = 0; z < zIters; z++)
            {
                for (int y = 0; y < yIters; y++)
                {
                    for (int x = 0; x < xIters; x++)
                    {
                        int checker = 0;
                        if (x % 2 != 0)
                        {
                            ++checker;
                        }
                        if (y % 2 != 0)
                        {
                            ++checker;
                        }
                        if (z % 2 != 0)
                        {
                            ++checker;
                        }

                        if (checker < 2)
                        {
                            result.Add(new Node(x * xSpacing, y * ySpacing, z * zSpacing, (checker == 1)));
                        }
                    }
                }
            }

            return result;
        }

        /*
         *  Calculates how many elements are from beginning of the node list to node on (x,y,z) position.
         */
        private int DistanceTo(int x, int y, int z, int xSlices, int ySlices)
        {
            int zPointsBigLey = (2 * (2 + xSlices) - 1) * (2 + ySlices) + (2 + xSlices) * (1 + ySlices);
            int zPointsSmallLey = (2 + xSlices) * (2 + ySlices);

            int zPoints = (int)Math.Ceiling(z / 2.0) * zPointsBigLey + (int)Math.Floor(z / 2.0) * zPointsSmallLey;

            int yPointsBigVec = 2 * (2 + xSlices) - 1;
            int yPointsSmallVec = 2 + xSlices;

            int yPoints;
            if ((z % 2 != 0) && (z != 0))
            {
                yPoints = (int)Math.Floor(y / 2.0) * yPointsSmallVec;
            }
            else
            {
                yPoints = (int)Math.Ceiling(y / 2.0) * yPointsBigVec + (int)Math.Floor(y / 2.0) * yPointsSmallVec;
            }

            int xPoints;
            if (((z % 2 != 0) && (z != 0)) || ((y % 2 != 0) && (y != 0)))
            {
                xPoints = x / 2;
            }
            else
            {
                xPoints = x;
            }

            return zPoints + yPoints + xPoints;
        }

        /*
         *  Generating NT matrix.
         */
        private List<List<int>> GenNT(int xSlices, int ySlices, int zSlices)
        {
            List<List<int>> result = new List<List<int>>();

            for (int z = 0; z < zSlices + 1; z++)
            {
                for (int y = 0; y < ySlices + 1; y++)
                {
                    for (int x = 0; x < xSlices + 1; x++)
                    {
                        result.Add(new List<int>(20) { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

                        result[result.Count - 1][0] = DistanceTo((x * 2) + 0, (y * 2) + 0, (z * 2) + 0, xSlices, ySlices);
                        result[result.Count - 1][1] = DistanceTo((x * 2) + 2, (y * 2) + 0, (z * 2) + 0, xSlices, ySlices);
                        result[result.Count - 1][2] = DistanceTo((x * 2) + 2, (y * 2) + 2, (z * 2) + 0, xSlices, ySlices);
                        result[result.Count - 1][3] = DistanceTo((x * 2) + 0, (y * 2) + 2, (z * 2) + 0, xSlices, ySlices);
                        result[result.Count - 1][4] = DistanceTo((x * 2) + 0, (y * 2) + 0, (z * 2) + 2, xSlices, ySlices);
                        result[result.Count - 1][5] = DistanceTo((x * 2) + 2, (y * 2) + 0, (z * 2) + 2, xSlices, ySlices);
                        result[result.Count - 1][6] = DistanceTo((x * 2) + 2, (y * 2) + 2, (z * 2) + 2, xSlices, ySlices);
                        result[result.Count - 1][7] = DistanceTo((x * 2) + 0, (y * 2) + 2, (z * 2) + 2, xSlices, ySlices);
                        result[result.Count - 1][8] = DistanceTo((x * 2) + 1, (y * 2) + 0, (z * 2) + 0, xSlices, ySlices);
                        result[result.Count - 1][9] = DistanceTo((x * 2) + 2, (y * 2) + 1, (z * 2) + 0, xSlices, ySlices);
                        result[result.Count - 1][10] = DistanceTo((x * 2) + 1, (y * 2) + 2, (z * 2) + 0, xSlices, ySlices);
                        result[result.Count - 1][11] = DistanceTo((x * 2) + 0, (y * 2) + 1, (z * 2) + 0, xSlices, ySlices);
                        result[result.Count - 1][12] = DistanceTo((x * 2) + 0, (y * 2) + 0, (z * 2) + 1, xSlices, ySlices);
                        result[result.Count - 1][13] = DistanceTo((x * 2) + 2, (y * 2) + 0, (z * 2) + 1, xSlices, ySlices);
                        result[result.Count - 1][14] = DistanceTo((x * 2) + 2, (y * 2) + 2, (z * 2) + 1, xSlices, ySlices);
                        result[result.Count - 1][15] = DistanceTo((x * 2) + 0, (y * 2) + 2, (z * 2) + 1, xSlices, ySlices);
                        result[result.Count - 1][16] = DistanceTo((x * 2) + 1, (y * 2) + 0, (z * 2) + 2, xSlices, ySlices);
                        result[result.Count - 1][17] = DistanceTo((x * 2) + 2, (y * 2) + 1, (z * 2) + 2, xSlices, ySlices);
                        result[result.Count - 1][18] = DistanceTo((x * 2) + 1, (y * 2) + 2, (z * 2) + 2, xSlices, ySlices);
                        result[result.Count - 1][19] = DistanceTo((x * 2) + 0, (y * 2) + 1, (z * 2) + 2, xSlices, ySlices);
                    }
                }
            }

            return result;
        }

        /*
         *  Generating ZU list.
         */
        private List<int> GenZU()
        {
            List<int> result = new List<int>();
            
            int elemFootNodesQuantity = (2 * (2 + mXSlices) - 1) * (2 + mYSlices) + (2 + mXSlices) * (1 + mYSlices);
            for (int i = 0; i < elemFootNodesQuantity; i++) 
            {
                result.Add(i);
            }

            return result;
        }

        /*
         *  Generating ZP list.
         */
        private List<ZPValue> GenZP()
        {
            List<ZPValue> result = new List<ZPValue>();

            int xyEms = (mXSlices + 1) * (mYSlices + 1);
            int start = mZSlices * xyEms;
            
            for(int i = start; i < start + xyEms; i++)
            {
                result.Add(new ZPValue(i, 6, FORCE));
            }

            return result;
        }

        /*  Constructors  */
        public ExObject()
        {
            // Empty
        }

        public ExObject(double width, double length, double height)
        {
            mWidth = width;
            mLength = length;
            mHeight = height;

            mAKT = GenAKT(mWidth, mLength, mHeight, 0, 0, 0);
            mNT = GenNT(0, 0, 0);
            mZU = GenZU();
            mZP = GenZP();
        }

        public ExObject(double width, double length, double height, int xSclices, int ySclices, int zSclices)
        {
            mWidth = width;
            mLength = length;
            mHeight = height;

            mXSlices = xSclices;
            mYSlices = ySclices;
            mZSlices = zSclices;

            mAKT = GenAKT(mWidth, mLength, mHeight, mXSlices, mYSlices, mZSlices);
            mNT = GenNT(mXSlices, mYSlices, mZSlices);
            mZU = GenZU();
            mZP = GenZP();
        }

        /*  Properties (readonly) */
        public double Width // Section on X axis
        {
            get
            {
                return mWidth;
            }
            private set
            {
                // Empty
            }
        }
        public double Length // Section on Y axis
        {
            get
            {
                return mLength;
            }
            private set
            {
                // Empty
            }
        }
        public double Height // Section on Z axis
        {
            get
            {
                return mHeight;
            }
            private set
            {
                // Empty
            }
        }
        public List<Node> AKT // Nodes which represent the Experimental Object
        {
            get
            {
                return mAKT;
            }
            set
            {
                mAKT = value;
            }
        }
        public List<List<int>> NT // Matrix  of connectivity
        {
            get
            {
                return mNT;
            }
            private set
            {
                // Empty
            }
        }

        public int WidthSlices // Quantity of slices on section on X axis
        {
            get
            {
                return mXSlices;
            }
            private set
            {
                // Empty
            }
        }
        public int LengthSlices // Quantity of slices on section on Y axis
        {
            get
            {
                return mYSlices;
            }
            private set
            {
                // Empty
            }
        }
        public int HeightSlices // Quantity of slices on section on Z axis
        {
            get
            {
                return mZSlices;
            }
            private set
            {
                // Empty
            }
        }
        public List<int> ZU // List of fixed nodes
        {
            get
            {
                return mZU;
            }
            private set
            {
                // Empty
            }
        }
        public List<ZPValue> ZP // List of finite elements which have some pressure on their side
        {
            get
            {
                return mZP;
            }
            private set
            {
                // Empty
            }
        }
    }
}