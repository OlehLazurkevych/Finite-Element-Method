using System.Collections.Generic;

namespace finiteElementMethod.Models
{
    class ExObject
        // Experemental Object dividet by slices
    {
        /*  Private members  */
        private double mWidth;  // Section on X axis
        private double mHeight;  // Section on Y axis
        private double mDepth; // Section on Z axis
        private List<Node> mNodes = new List<Node>();

        private uint mXSlices = 0;
        private uint mYSlices = 0;
        private uint mZSlices = 0;

        /*  Private functionality  */
        private List<Node> GenNodes()
        {
            // Generating nodes on slice crossings of a object,
            // also creates intermediate nodes between generated points.

            List<Node> result = new List<Node>();

            uint zIters = (mZSlices != 0) ? (2 + (mZSlices * 2) - 1) : 3;
            uint yIters = (mYSlices != 0) ? (2 + (mYSlices * 2) - 1) : 3;
            uint xIters = (mXSlices != 0) ? (2 + (mXSlices * 2) - 1) : 3;

            double xSpacing = (mXSlices != 0) ? ((mWidth / mXSlices) / 2) : 1;
            double ySpacing = (mYSlices != 0) ? ((mHeight / mYSlices) / 2) : 1;
            double zSpacing = (mZSlices != 0) ? ((mDepth / mZSlices) / 2) : 1;
            
            for (uint y = 0; y < yIters; y++)
            {
                for (uint z = 0; z < zIters; z++)
                {
                    for (uint x = 0; x < xIters; x++)
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
                            result.Add(new Node(x * xSpacing, z * zSpacing, y * ySpacing, (checker == 1)));
                        }
                    }
                }
            }

            return result;
        }

        /*  Constructors  */
        public ExObject()
        {
            // Empty
        }

        public ExObject(double width, double height, double depth)
        {
            mWidth = width;
            mHeight = height;
            mDepth = depth;

            mNodes = GenNodes();
        }

        public ExObject(double width, double height, double depth, uint xSclices, uint ySclices, uint zSclices)
        {
            mWidth = width;
            mHeight = depth;
            mDepth = height;

            mXSlices = xSclices;
            mYSlices = ySclices;
            mZSlices = zSclices;

            mNodes = GenNodes();
        }

        /*  Properties  */
        public double Width
            // Section on X axis
        {
            get
            {
                return mWidth;
            }
            set
            {
                mWidth = value;
                mNodes = GenNodes();
            }
        }
        public double Height
            // Section on Y axis
        {
            get
            {
                return mHeight;
            }
            set
            {
                mHeight = value;
                mNodes = GenNodes();
            }
        }
        public double Depth
            // Section on Z axis
        {
            get
            {
                return mDepth;
            }
            set
            {
                mDepth = value;
                mNodes = GenNodes();
            }
        }
        public List<Node> Nodes
        {
            get
            {
                return mNodes;
            }
            set
            {
                mNodes = value;
            }
        }

        public uint WidthSlices
            // Quantity of slices on section on X axis
        {
            get
            {
                return mXSlices;
            }
            set
            {
                mXSlices = value;
                mNodes = GenNodes();
            }
        }
        public uint HeightSlices
            // Quantity of slices on section on Y axis
        {
            get
            {
                return mYSlices;
            }
            set
            {
                mYSlices = value;
                mNodes = GenNodes();
            }
        }
        public uint DepthSlices
            // Quantity of slices on section on Z axis
        {
            get
            {
                return mZSlices;
            }
            set
            {
                mZSlices = value;
                mNodes = GenNodes();
            }
        }
    }
}
