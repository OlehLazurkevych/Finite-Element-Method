namespace finiteElementMethod.Models
{
    class Node
        // Node is simply a point in space with a lable
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
}
