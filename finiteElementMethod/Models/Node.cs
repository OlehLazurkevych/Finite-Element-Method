namespace finiteElementMethod.Models
{
    /*
     *  Node is simply a point in space with a lable
     */
    public class Node
    {
        /*  Private members  */
        private double mX;
        private double mY;
        private double mZ;
        private bool mIsIntermediate;

        /*  Constructors  */
        public Node()
        {
            mX = 0;
            mY = 0;
            mZ = 0;
            mIsIntermediate = false;
        }

        public Node(double x, double y, double z)
        {
            mX = x;
            mY = y;
            mZ = z;
            mIsIntermediate = false;
        }

        public Node(double x, double y, double z, bool isIntermediate)
        {
            mX = x;
            mY = y;
            mZ = z;
            mIsIntermediate = isIntermediate;
        }

        /*  Properties  */
        public double X
        {
            get { return mX; }
            set { mX = value; }
        }
        public double Y
        {
            get { return mY; }
            set { mY = value; }
        }
        public double Z
        {
            get { return mZ; }
            set { mZ = value; }
        }
        public bool IsIntermediate
        {
            get { return mIsIntermediate; }
            set { mIsIntermediate = value; }
        }
    }

    /*
     *  Data model for pressure definition on certain side of a finite element
     */
    public class ZPValue
    {
        /*  Constructors  */
        public ZPValue()
        {
            // Empty
        }
        public ZPValue(double x, double y, double z)
        {
            Z = z;
            Y = y;
            Z = z;
        }

        /*  Properties  */
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}