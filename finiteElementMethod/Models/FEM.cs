namespace finiteElementMethod.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using finiteElementMethod.Externals;

    public class FEM
    {
        /*   Private members   */
        private ExObject mExperementalObject;

        public double[,,] DFIABG = External.GenDFIABG();
        public double[,] MG;
        public double[] Fources;
        public double[,,] DPSITE = External.GenDPSITE();
        public double[,] PSIET;
        public double[,,] DFIABG_P = External.GenDFIABG_P();

        private double[] LinEquationResult;
        public List<KeyValuePair<Node, double>> DefformatedObject = new List<KeyValuePair<Node, double>>();

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
            for (int i = 0; i < mExperementalObject.ZU.Count; i++)
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
            }

            for (int i = 0; i < mExperementalObject.ZP.Count; i++)  
            {
                Fources[i * 3 + 0] = mExperementalObject.ZP[i].X;
                Fources[i * 3 + 1] = mExperementalObject.ZP[i].Y;
                Fources[i * 3 + 2] = mExperementalObject.ZP[i].Z;
            }
        }

        private void getResult()
        {
            LinEquationResult = GaussianCalculator.Calculate(MG, Fources);
            
            for (int i = 0; i < mExperementalObject.AKT.Count; i++)
            {
                Node prev = mExperementalObject.AKT[i];
                double[] point = LinEquationResult.Skip(i * 3).Take(3).ToArray();
                DefformatedObject.Add(new KeyValuePair<Node, double>(new Node(Math.Round(prev.X + point[0], 4), Math.Round(prev.Y + point[1], 4), Math.Round(prev.Z + point[2], 4), prev.IsIntermediate), 0));
            }
        }

        private void TENSORCalculation()
        {
            // One for each finite element
            double[,,] dxyzabg = new double[3, 3, 20];
            double[,,] dfixyz = new double[20, 20, 3];
            double[,,] duxyz = new double[20, 3, 3];

            double[][,] SUM = new double[mExperementalObject.AKT.Count][,];
            for (int i = 0; i < mExperementalObject.AKT.Count; i++)
            {
                SUM[i] = new double[3, 3];
            }

            double[][] sigma = new double[mExperementalObject.AKT.Count][];
            
            double[] amount = new double[mExperementalObject.AKT.Count];
            List<int> coordinates = new List<int>();

            // Get number of entries for each node
            for (int number = 0; number < mExperementalObject.nel; number++)
            {
                coordinates = mExperementalObject.NT[number];
                for (int j = 0; j < 20; j++)
                {
                    amount[coordinates[j]]++;
                }
            }

            // Fill sum matrix
            for (int number = 0; number < mExperementalObject.nel; number++)
            {
                coordinates = mExperementalObject.NT[number];

                // Generating dxyzabg
                double globalCoordinate = 0;
                double diFi = 0;
                double sum = 0;
                
                for (int i = 0; i < 3; i++) // Global coords
                {
                    for (int j = 0; j < 3; j++) // Local goords
                    {
                        for (int n = 0; n < 20; n++) // Gaussian Nodes
                        {
                            sum = 0;
                            for (int f = 0; f < 20; f++) // Functions
                            {
                                switch(i)
                                {
                                    case 0: globalCoordinate = mExperementalObject.AKT[coordinates[f]].X; break;
                                    case 1: globalCoordinate = mExperementalObject.AKT[coordinates[f]].Y; break;
                                    case 2: globalCoordinate = mExperementalObject.AKT[coordinates[f]].Z; break;
                                }
                                diFi = DFIABG_P[n, j, f];
                                sum += globalCoordinate * diFi;
                            }
                            dxyzabg[i, j, n] = sum;
                        }
                    }
                }

                // Get dfixyz
                // col is free column
                double[] col = new double[3];
                for (int i = 0; i < 20; i++) // Gaussian Nodes
                {
                    for (int phi = 0; phi < 20; phi++) // Functions
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            col[k] = DFIABG_P[i, k, phi];
                        }
                        double[,] matrix = new double[3, 3] {
                            { dxyzabg[0,0,i], dxyzabg[1,0,i], dxyzabg[2,0,i] },
                            { dxyzabg[0,1,i], dxyzabg[1,1,i], dxyzabg[2,1,i] },
                            { dxyzabg[0,2,i], dxyzabg[1,2,i], dxyzabg[2,2,i] }
                        };
                        double[] gaussianSolve = GaussianCalculator.Calculate(matrix, col);

                        for (int k = 0; k < 3; k++)
                        {
                            dfixyz[i, phi, k] = gaussianSolve[k];
                        }
                    }
                }

                // Get duxyz
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            sum = 0;
                            for (int l = 0; l < 20; l++)
                            {
                                sum += LinEquationResult[coordinates[l] * 3 + j] * dfixyz[i, l, k];
                            }
                            duxyz[i, j, k] = sum;
                        }
                    }
                }

                // Get all sums: in each global point add all 9 values
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            SUM[coordinates[i]][j, k] += duxyz[i, j, k];
                        }
                    }
                }
            }

            // Get the avarage for each point
            for (int i = 0; i < mExperementalObject.AKT.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        SUM[i][j, k] /= amount[i];
                    }
                }
            }

            for (int i = 0; i < mExperementalObject.AKT.Count; i++)
            {
                sigma[i] = CalculateSigma(SUM[i]);
            }

            for (int i = 0; i < mExperementalObject.AKT.Count; i++)
            {
                DefformatedObject[i] = new KeyValuePair<Node, double>(DefformatedObject[i].Key, GetNodePreasure(sigma[i]).Sum());
            }
        }

        private double[] CalculateSigma(double[,] u)
        {
            double[] result = new double[6];

            result[0] = mExperementalObject.Lamda * ((1 - mExperementalObject.v) * u[0, 0] + mExperementalObject.v * (u[1, 1] + u[2, 2]));
            result[1] = mExperementalObject.Lamda * ((1 - mExperementalObject.v) * u[1, 1] + mExperementalObject.v * (u[0, 0] + u[2, 2]));
            result[2] = mExperementalObject.Lamda * ((1 - mExperementalObject.v) * u[2, 2] + mExperementalObject.v * (u[0, 0] + u[1, 1]));
            result[3] = mExperementalObject.Mu * (u[0, 1] + u[1, 0]);
            result[4] = mExperementalObject.Mu * (u[1, 2] + u[2, 1]);
            result[5] = mExperementalObject.Mu * (u[0, 2] + u[2, 0]);

            return result;
        }

        private double[] GetNodePreasure(double[] sigma)
        {
            double[] result = new double[3];

            result[0] = sigma[0] + sigma[1] + sigma[2];
            result[1] = sigma[0] * sigma[1] + sigma[0] * sigma[2] + sigma[1] * sigma[2] - (Math.Pow(sigma[3], 2) + Math.Pow(sigma[4], 2) + Math.Pow(sigma[5], 2));
            result[2] = sigma[0] * sigma[1] * sigma[2] + 2 * sigma[3] * sigma[4] * sigma[5] - (sigma[0] * Math.Pow(sigma[4], 2) + sigma[1] * Math.Pow(sigma[5], 2) + sigma[2] * Math.Pow(sigma[3], 2));

            return result;
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
            TENSORCalculation();
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