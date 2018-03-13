using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finiteElementMethod.Models
{
    class ExObject
    {
        /*  Private members  */
        private double mWidth;  // Section on X axis
        private double mDepth;  // Section on Y axis
        private double mHeight; // Section on Z axis
        private List<Point> mPoints = new List<Point>();

        private uint mXSlices = 0;
        private uint mYSlices = 0;
        private uint mZSlices = 0;

        /*  Private functionality  */
        private List<Point> GeneratePoints()
        {
            // Generating points on slice crossings of a rectangle,
            // also creates intermediate points between generated points.

            List<Point> result = new List<Point>();

            uint zIters = (mZSlices != 0) ? (2 + (mZSlices * 2) - 1) : 3;
            uint yIters = (mYSlices != 0) ? (2 + (mYSlices * 2) - 1) : 3;
            uint xIters = (mXSlices != 0) ? (2 + (mXSlices * 2) - 1) : 3;

            double xSpacing = (mXSlices != 0) ? ((mWidth / mXSlices) / 2) : 1;
            double ySpacing = (mYSlices != 0) ? ((mDepth / mYSlices) / 2) : 1;
            double zSpacing = (mZSlices != 0) ? ((mHeight / mZSlices) / 2) : 1;
            
            for (uint z = 0; z < zIters; z++)
            {
                for (uint y = 0; y < yIters; y++)
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
                            result.Add(new Point(x * xSpacing, y * ySpacing, z * zSpacing, (checker == 1)));
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

        public ExObject(double width, double depth, double height)
        {
            mWidth = width;
            mDepth = depth;
            mHeight = height;
        }

        public ExObject(double width, double depth, double height, uint xSclices, uint ySclices, uint zSclices)
        {
            mWidth = width;
            mDepth = depth;
            mHeight = height;

            mXSlices = xSclices;
            mYSlices = ySclices;
            mZSlices = zSclices;
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
                GeneratePoints();
            }
        }
        public double Depth
            // Section on Y axis
        {
            get
            {
                return mDepth;
            }
            set
            {
                mDepth = value;
                GeneratePoints();
            }
        }
        public double Height
            // Section on Z axis
        {
            get
            {
                return mHeight;
            }
            set
            {
                mHeight = value;
                GeneratePoints();
            }
        }
        public List<Point> Points
        {
            get
            {
                return mPoints;
            }
            set
            {
                mPoints = value;
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
                GeneratePoints();
            }
        }
        public uint DepthSlices
            // Quantity of slices on section on Y axis
        {
            get
            {
                return mYSlices;
            }
            set
            {
                mYSlices = value;
                GeneratePoints();
            }
        }
        public uint HeightSlices
            // Quantity of slices on section on Z axis
        {
            get
            {
                return mZSlices;
            }
            set
            {
                mZSlices = value;
                GeneratePoints();
            }
        }
    }
}
