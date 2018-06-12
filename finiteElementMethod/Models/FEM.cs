﻿using System;
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
        public double[] F;

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

        /*   Constructors   */
        public FEM(ExObject obj)
        {
            mExperementalObject = obj;
        }

        /*   Public functionality   */


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
    }
}