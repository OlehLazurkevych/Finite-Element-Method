using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using finiteElementMethod.Externals;

namespace finiteElementMethod.Models
{
    public class FEM
    {
        /*   Private members   */
        private ExObject mExperementalObject;

        public double[,,] DFIABG = External.GenDFIABG();
        public double[,] MG;
        public double[] Fources;
        public double[,,] DPSITE = External.GenDPSITE();
        public double[,] PSIET;

        private double[] LinEquationResult;
        public List<Node> DefformatedObject = new List<Node>();

        /*   Private functionality   */
        /*
         *  Generates one time for all elements
         */
        private void GenMGMatrix()
        {
            double[,,] DXYZABG = new double[3, 3, 27];
            double[] DJ = new double[27];
            double[,,] DFIXYZ = new double[27, 20, 3];

            for (int element = 0; element < mExperementalObject.nel; element++)
            {
                List<int> glblCoords = mExperementalObject.NT[element];

                // Generatting DXYZABG
                double glbCoordinate = 0;
                double deFee = 0;
                double sum = 0;
                
                for (int gl = 0; gl < 3; gl++) // Global coordinates
                {
                    for (int loc = 0; loc < 3; loc++) // Local coordinates
                    {
                        for (int n = 0; n < 27; n++) // Gausse nodes
                        {
                            sum = 0;
                            for (int f = 0; f < 20; f++) // functions
                            {
                                switch(gl)
                                {
                                    case 0: glbCoordinate = mExperementalObject.AKT[glblCoords[f]].X; break;
                                    case 1: glbCoordinate = mExperementalObject.AKT[glblCoords[f]].Y; break;
                                    case 2: glbCoordinate = mExperementalObject.AKT[glblCoords[f]].Z; break;
                                }
                                deFee = DFIABG[n, loc, f];
                                sum += glbCoordinate * deFee;
                            }
                            DXYZABG[gl, loc, n] = sum;
                        }
                    }
                }

                // Generatting DJ
                double[,] jak;
                for (int i = 0; i < 27; i++)
                {
                    jak = new double[3, 3] {
                        { DXYZABG[0,0,i], DXYZABG[1,0,i], DXYZABG[2,0,i] },
                        { DXYZABG[0,1,i], DXYZABG[1,1,i], DXYZABG[2,1,i] },
                        { DXYZABG[0,2,i], DXYZABG[1,2,i], DXYZABG[2,2,i] }
                    };
                    DJ[i] = (
                                jak[0, 0] * jak[1, 1] * jak[2, 2] +
                                jak[0, 1] * jak[1, 2] * jak[2, 0] +
                                jak[0, 2] * jak[1, 0] * jak[2, 1]
                            ) -
                            (
                                jak[0, 2] * jak[1, 1] * jak[2, 0] +
                                jak[0, 1] * jak[1, 0] * jak[2, 2] +
                                jak[0, 0] * jak[1, 2] * jak[2, 1]
                            );
                }

                // Generatting DFIXYZ
                double[] col = new double[3];
                for (int n = 0; n < 27; n++) // Gaussian nodes
                {
                    for (int fee = 0; fee < 20; fee++) // Functions
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            col[k] = DFIABG[n, k, fee];
                        }
                        double[,] matrix = new double[3, 3] {
                            { DXYZABG[0,0,n], DXYZABG[1,0,n], DXYZABG[2,0,n] },
                            { DXYZABG[0,1,n], DXYZABG[1,1,n], DXYZABG[2,1,n] },
                            { DXYZABG[0,2,n], DXYZABG[1,2,n], DXYZABG[2,2,n] }
                        };
                        double[] gaussianResults = GaussianCalculator.Calculate(matrix, col);

                        for (int k = 0; k < 3; k++)
                        {
                            DFIXYZ[n, fee, k] = gaussianResults[k];
                        }
                    }
                }

                double[,][,] lilMG = new double[3, 3][,];

                lilMG[0, 0] = x11(DFIXYZ, DJ);
                lilMG[1, 1] = x22(DFIXYZ, DJ);
                lilMG[2, 2] = x33(DFIXYZ, DJ);
                lilMG[0, 1] = x12(DFIXYZ, DJ);
                lilMG[0, 2] = x13(DFIXYZ, DJ);
                lilMG[1, 2] = x23(DFIXYZ, DJ);
                lilMG[1, 0] = matrixRot(lilMG[0, 1]);
                lilMG[2, 0] = matrixRot(lilMG[0, 2]);
                lilMG[2, 1] = matrixRot(lilMG[1, 2]);

                int x, y, localX, localY, globalX, globalY;
                for (int i = 0; i < 60; i++)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        x = i / 20;
                        y = j / 20;
                        localX = i % 20;
                        localY = j % 20;

                        globalX = (mExperementalObject.NT[element][localX]) * 3 + x;
                        globalY = (mExperementalObject.NT[element][localY]) * 3 + y;
                        MG[globalX, globalY] += lilMG[x, y][localX, localY];
                    }
                }
            }
        }

        private double[,] x11(double[,,] dfixyz, double[] dj)
        {
            double[,] res = new double[20, 20];
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (i > j)
                    {
                        res[i, j] = res[j, i];
                    }
                    else
                    {
                        double sum = 0;
                        int counter = 0;
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                for (int m = 0; m < 3; m++)
                                {
                                    sum += (
                                            (mExperementalObject.Lamda * (1 - mExperementalObject.v) * (dfixyz[counter, i, 0] * dfixyz[counter, j, 0]))
                                            +
                                            (mExperementalObject.Mu * (dfixyz[counter, i, 1] * dfixyz[counter, j, 1] + dfixyz[counter, i, 2] * dfixyz[counter, j, 2]))
                                        ) * Math.Abs(dj[counter]) * c[m] * c[l] * c[k];
                                    ++counter;
                                }
                            }
                        }
                        res[i, j] = sum;
                    }
                }
            }
            return res;
        }
        private double[,] x22(double[,,] dfixyz, double[] dj)
        {
            double[,] res = new double[20, 20];
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (i > j)
                    {
                        res[i, j] = res[j, i];
                    }
                    else
                    {
                        double sum = 0;
                        int counter = 0;
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                for (int m = 0; m < 3; m++)
                                {
                                    sum += (
                                            (mExperementalObject.Lamda * (1 - mExperementalObject.v) * (dfixyz[counter, i, 1] * dfixyz[counter, j, 1]))
                                            +
                                            (mExperementalObject.Mu * (dfixyz[counter, i, 0] * dfixyz[counter, j, 0] + dfixyz[counter, i, 2] * dfixyz[counter, j, 2]))
                                        ) * Math.Abs(dj[counter]) * c[m] * c[l] * c[k];
                                    ++counter;

                                }
                            }
                        }
                        res[i, j] = sum;
                    }
                }
            }
            return res;
        }
        private double[,] x33(double[,,] dfixyz, double[] dj)
        {
            double[,] res = new double[20, 20];
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (i > j)
                    {
                        res[i, j] = res[j, i];
                    }
                    else
                    {
                        double sum = 0;
                        int counter = 0;
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                for (int m = 0; m < 3; m++)
                                {
                                    sum += (
                                            (mExperementalObject.Lamda * (1 - mExperementalObject.v) * (dfixyz[counter, i, 2] * dfixyz[counter, j, 2]))
                                            +
                                            (mExperementalObject.Mu * (dfixyz[counter, i, 0] * dfixyz[counter, j, 0] + dfixyz[counter, i, 1] * dfixyz[counter, j, 1]))
                                        ) * Math.Abs(dj[counter]) * c[m] * c[l] * c[k];
                                    ++counter;
                                }
                            }
                        }
                        res[i, j] = sum;
                    }
                }
            }
            return res;
        }

        private double[,] x12(double[,,] dfixyz, double[] dj)
        {
            double[,] res = new double[20, 20];
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    double sum = 0;
                    int counter = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            for (int m = 0; m < 3; m++)
                            {
                                sum += (
                                    (mExperementalObject.Lamda * mExperementalObject.v * (dfixyz[counter, i, 0] * dfixyz[counter, j, 1]))
                                      +
                                    (mExperementalObject.Mu * (dfixyz[counter, i, 1] * dfixyz[counter, j, 0]))
                                    ) * Math.Abs(dj[counter]) * c[m] * c[l] * c[k];
                                ++counter;
                            }
                        }
                    }
                    res[i, j] = sum;

                }
            }
            return res;
        }
        private double[,] x13(double[,,] dfixyz, double[] dj)
        {
            double[,] res = new double[20, 20];
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    double sum = 0;
                    int counter = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            for (int m = 0; m < 3; m++)
                            {
                                sum += (
                                    (mExperementalObject.Lamda * mExperementalObject.v * (dfixyz[counter, i, 0] * dfixyz[counter, j, 2]))
                                      +
                                    (mExperementalObject.Mu * (dfixyz[counter, i, 2] * dfixyz[counter, j, 0]))
                                    ) * Math.Abs(dj[counter]) * c[m] * c[l] * c[k];
                                ++counter;
                            }
                        }
                    }
                    res[i, j] = sum;

                }
            }
            return res;
        }
        private double[,] x23(double[,,] dfixyz, double[] dj)
        {
            double[,] res = new double[20, 20];
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    double sum = 0;
                    int counter = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            for (int m = 0; m < 3; m++)
                            {
                                sum += (
                                    (mExperementalObject.Lamda * mExperementalObject.v * (dfixyz[counter, i, 1] * dfixyz[counter, j, 2]))
                                      +
                                    (mExperementalObject.Mu * (dfixyz[counter, i, 2] * dfixyz[counter, j, 1]))
                                    ) * Math.Abs(dj[counter]) * c[m] * c[l] * c[k];
                                ++counter;
                            }
                        }
                    }
                    res[i, j] = sum;

                }
            }
            return res;
        }

        private double[,] matrixRot(double[,] toRotate)
        {
            double[,] res = new double[20, 20];
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    res[i, j] = toRotate[j, i];
                }
            }
            return res;
        }

        private void updateMGMatrix()
        {
            int index;
            for (int i = 0; i < mExperementalObject.ZU.Capacity; i++)
            {
                index = mExperementalObject.ZU[i] * 3;
                for (int j = 0; j < 3; j++)
                {
                    MG[index + j, index + j] = Math.Pow(10, 20);
                }
            }
        }

        private void createPSI()
        {
            PSIET = new double[8, 9];

            double[] values;
            double[][] nodes = External.lilGaussNodes;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    values = nodes[j];
                    PSIET[i, j] = FunctionsForPSI.CalculatePSI(i, values[0], values[1]);
                }
            }
        }

        private void createF()
        {
            double[,,] DXYZET;
            int cubeSide = 5;

            int loadElementsCount = mExperementalObject.WidthSlices * mExperementalObject.LengthSlices;
            int start = mExperementalObject.nel - loadElementsCount;
            for (int number = start; number < mExperementalObject.nel; number++)
            {
                DXYZET = new double[3, 2, 9];

                List<int> coordinates = mExperementalObject.NT[number];

                // Generatinng DXYZET
                double glbCoordinate = 0;
                double dPSI = 0;
                double sum = 0;
                
                for (int gl = 0; gl < 3; gl++) // Global coords
                {
                    for (int loc = 0; loc < 2; loc++) // Local coords
                    {
                        for (int n = 0; n < 9; n++) // Gaussian nodes
                        {
                            sum = 0;
                            for (int f = 0; f < 8; f++) // Functions
                            {
                                switch (gl)
                                {
                                    case 0: glbCoordinate = mExperementalObject.AKT[coordinates[relPoss[cubeSide][f]]].X; break;
                                    case 1: glbCoordinate = mExperementalObject.AKT[coordinates[relPoss[cubeSide][f]]].Y; break;
                                    case 2: glbCoordinate = mExperementalObject.AKT[coordinates[relPoss[cubeSide][f]]].Z; break;
                                }
                                dPSI = DPSITE[n, loc, f];
                                sum += glbCoordinate * dPSI;
                            }
                            DXYZET[gl, loc, n] = sum;
                        }
                    }
                }
                
                double presure = -0.3;
                double[] f2 = new double[8];

                for (int i = 0; i < 8; i++)
                {
                    sum = 0;
                    int counter = 0;
                    for (int m = 0; m < 3; m++)
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            sum += presure *
                                (DXYZET[0, 0, counter] * DXYZET[1, 1, counter] - DXYZET[1, 0, counter] * DXYZET[0, 1, counter]) *
                                PSIET[i, counter]
                                * c[n] * c[m];
                            ++counter;
                        }
                    }
                    f2[i] = sum;
                }

                for (int i = 0; i < 8; i++)
                {
                    Fources[coordinates[relPoss[cubeSide][i]] * 3 + 2] += f2[i];
                }
            }
        }

        private void getResult()
        {
            LinEquationResult = GaussianCalculator.Calculate(MG, Fources);
            
            for (int i = 0; i < mExperementalObject.AKT.Count; i++)
            {
                Node prev = mExperementalObject.AKT[i];
                double[] point = LinEquationResult.Skip(i * 3).Take(3).ToArray();
                DefformatedObject.Add(new Node(Math.Round(prev.X + point[0], 4), Math.Round(prev.Y + point[1], 4), Math.Round(prev.Z + point[2], 4)));
            }
        }

        /*   Constructors   */
        public FEM(ExObject obj)
        {
            mExperementalObject = obj;

            MG = new double[3 * mExperementalObject.AKT.Count, 3 * mExperementalObject.AKT.Count];
            Fources = new double[3 * mExperementalObject.AKT.Count];
        }

        /*   Public functionality   */
        public void RunSimulation()
        {
            GenMGMatrix();
            updateMGMatrix();
            createPSI();
            createF();
            getResult();
            //createPressureVector();
        }

        /*   Properties   */
        public ExObject ExperementalObject
        {
            get
            {
                return mExperementalObject;
            }
            private set
            {
                // Empty
            }
        }

        public double[] c = new double[3] { 5.0 / 9.0, 8.0 / 9.0, 5.0 / 9.0 };
        public int[][] relPoss = new int[6][] {
            new int[8] { 0, 1, 5, 4, 8, 13, 16, 12},
            new int[8] { 1, 2, 6, 5, 9, 14, 17, 13},
            new int[8] { 2, 3, 7, 6, 10, 15, 18, 14 },
            new int[8] { 3, 0, 4, 7, 11, 12, 19, 15},
            new int[8] { 0, 1, 2, 3, 8, 9, 10, 11},
            new int[8] { 4, 5, 6, 7, 16, 17, 18, 19}
        };
    }
}