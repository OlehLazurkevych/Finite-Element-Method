using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using finiteElementMethod.Models;
using System.Windows.Media.Animation;

namespace finiteElementMethod.Views
{
    /// <summary>
    /// Interaction logic for ExObjectView.xaml
    /// </summary>
    public partial class ExObjectView : UserControl
    {
        public ExObjectView()
        {
            InitializeComponent();

            ExObject obj = new ExObject(4, 8, 4, 5, 5, 5);
            
            objGroup.Children.Add(GetCubeMode((obj.Nodes[0].X - (obj.Width / 2)) * (-1), obj.Nodes[0].Y - (obj.Depth / 2), obj.Nodes[0].Z - (obj.Height / 2), 0.1, Color.FromArgb(255, 50, 50, 50)));
            objGroup.Children.Add(GetCubeMode((obj.Nodes[obj.Nodes.Count - 1].X - (obj.Width / 2)) * (-1), obj.Nodes[obj.Nodes.Count - 1].Y - (obj.Depth / 2), obj.Nodes[obj.Nodes.Count - 1].Z - (obj.Height / 2), 0.1, Color.FromArgb(255, 50, 50, 50)));

            for (int i=1; i<obj.Nodes.Count - 1; i++)
            {
                double width = (obj.Nodes[i].IsIntermediate) ? 0.05 : 0.1;
                Model3DGroup cube = GetCubeMode((obj.Nodes[i].X - (obj.Width / 2)) * (-1), obj.Nodes[i].Y - (obj.Depth / 2), obj.Nodes[i].Z - (obj.Height / 2), width, Color.FromArgb(255, 200, 50, 50));
                objGroup.Children.Add(cube);
            }

            // ReadY
            //foreach (var node in obj.Nodes)
            //{
            //    double width = (node.IsIntermediate) ? 0.05: 0.1;
            //    Model3DGroup cube = GetCubeMode((node.X - (obj.Width / 2)) * (-1), node.Y - (obj.Depth / 2), node.Z - (obj.Height / 2), width, Color.FromArgb(255, 200, 50, 50));
            //    objGroup.Children.Add(cube);
            //}

            //Model3DGroup cubeC = GetCubeMode(0, 0, 0, 0.5, Color.FromArgb(255, 50, 50, 50));
            //objGroup.Children.Add(cubeC);

            

            RotateTransform3D myRotateTransform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 1), 180));
            
            Vector3DAnimation myVectorAnimation = new Vector3DAnimation(new Vector3D(0, 0, 0.5), new Duration(TimeSpan.FromSeconds(40)));
            myVectorAnimation.AccelerationRatio = 0.1;
            myVectorAnimation.DecelerationRatio = 0.1;
            myVectorAnimation.RepeatBehavior = RepeatBehavior.Forever;

            myRotateTransform.Rotation.BeginAnimation(AxisAngleRotation3D.AxisProperty, myVectorAnimation);
            
            objTransformGroup.Children.Add(myRotateTransform);
        }

        public Model3DGroup GetCubeMode(double x, double y, double z, double width, Color color)
        {
            Model3DGroup cube = new Model3DGroup();
            GeometryModel3D side1 = new GeometryModel3D();
            GeometryModel3D side2 = new GeometryModel3D();
            GeometryModel3D side3 = new GeometryModel3D();
            GeometryModel3D side4 = new GeometryModel3D();
            GeometryModel3D side5 = new GeometryModel3D();
            GeometryModel3D side6 = new GeometryModel3D();
            MeshGeometry3D side1Plane = new MeshGeometry3D();
            MeshGeometry3D side2Plane = new MeshGeometry3D();
            MeshGeometry3D side3Plane = new MeshGeometry3D();
            MeshGeometry3D side4Plane = new MeshGeometry3D();
            MeshGeometry3D side5Plane = new MeshGeometry3D();
            MeshGeometry3D side6Plane = new MeshGeometry3D();

            //side1-------------------------------------------------
            side1Plane.Positions.Add(new Point3D(x-(width / 2), y-(width / 2), z-(width / 2)));
            side1Plane.Positions.Add(new Point3D(x-(width / 2), y+(width / 2), z-(width / 2)));
            side1Plane.Positions.Add(new Point3D(x+(width / 2), y+(width / 2), z-(width / 2)));
            side1Plane.Positions.Add(new Point3D(x+(width / 2), y+(width / 2), z-(width / 2)));
            side1Plane.Positions.Add(new Point3D(x+(width / 2), y-(width / 2), z-(width / 2)));
            side1Plane.Positions.Add(new Point3D(x-(width / 2), y-(width / 2), z-(width / 2)));

            side1Plane.TriangleIndices.Add(0);
            side1Plane.TriangleIndices.Add(1);
            side1Plane.TriangleIndices.Add(2);
            side1Plane.TriangleIndices.Add(3);
            side1Plane.TriangleIndices.Add(4);
            side1Plane.TriangleIndices.Add(5);

            side1Plane.Normals.Add(new Vector3D(0, 0, -1));
            side1Plane.Normals.Add(new Vector3D(0, 0, -1));
            side1Plane.Normals.Add(new Vector3D(0, 0, -1));
            side1Plane.Normals.Add(new Vector3D(0, 0, -1));
            side1Plane.Normals.Add(new Vector3D(0, 0, -1));
            side1Plane.Normals.Add(new Vector3D(0, 0, -1));

            side1Plane.TextureCoordinates.Add(new Point(1, 0));
            side1Plane.TextureCoordinates.Add(new Point(1, 1));
            side1Plane.TextureCoordinates.Add(new Point(0, 1));
            side1Plane.TextureCoordinates.Add(new Point(0, 1));
            side1Plane.TextureCoordinates.Add(new Point(0, 0));
            side1Plane.TextureCoordinates.Add(new Point(1, 0));

            //side2-------------------------------------------------
            side2Plane.Positions.Add(new Point3D(x-(width / 2), y-(width / 2), z+(width / 2)));
            side2Plane.Positions.Add(new Point3D(x+(width / 2), y-(width / 2), z+(width / 2)));
            side2Plane.Positions.Add(new Point3D(x+(width / 2), y+(width / 2), z+(width / 2)));
            side2Plane.Positions.Add(new Point3D(x+(width / 2), y+(width / 2), z+(width / 2)));
            side2Plane.Positions.Add(new Point3D(x-(width / 2), y+(width / 2), z+(width / 2)));
            side2Plane.Positions.Add(new Point3D(x-(width / 2), y-(width / 2), z+(width / 2)));

            side2Plane.TriangleIndices.Add(0);
            side2Plane.TriangleIndices.Add(1);
            side2Plane.TriangleIndices.Add(2);
            side2Plane.TriangleIndices.Add(3);
            side2Plane.TriangleIndices.Add(4);
            side2Plane.TriangleIndices.Add(5);

            side2Plane.Normals.Add(new Vector3D(0, 0, 1));
            side2Plane.Normals.Add(new Vector3D(0, 0, 1));
            side2Plane.Normals.Add(new Vector3D(0, 0, 1));
            side2Plane.Normals.Add(new Vector3D(0, 0, 1));
            side2Plane.Normals.Add(new Vector3D(0, 0, 1));
            side2Plane.Normals.Add(new Vector3D(0, 0, 1));

            side2Plane.TextureCoordinates.Add(new Point(0, 0));
            side2Plane.TextureCoordinates.Add(new Point(1, 0));
            side2Plane.TextureCoordinates.Add(new Point(1, 1));
            side2Plane.TextureCoordinates.Add(new Point(1, 1));
            side2Plane.TextureCoordinates.Add(new Point(0, 1));
            side2Plane.TextureCoordinates.Add(new Point(0, 0));

            //side3-------------------------------------------------
            side3Plane.Positions.Add(new Point3D(x-(width / 2), y-(width / 2), z-(width / 2)));
            side3Plane.Positions.Add(new Point3D(x+(width / 2), y-(width / 2), z-(width / 2)));
            side3Plane.Positions.Add(new Point3D(x+(width / 2), y-(width / 2), z+(width / 2)));
            side3Plane.Positions.Add(new Point3D(x+(width / 2), y-(width / 2), z+(width / 2)));
            side3Plane.Positions.Add(new Point3D(x-(width / 2), y-(width / 2), z+(width / 2)));
            side3Plane.Positions.Add(new Point3D(x-(width / 2), y-(width / 2), z-(width / 2)));

            side3Plane.TriangleIndices.Add(0);
            side3Plane.TriangleIndices.Add(1);
            side3Plane.TriangleIndices.Add(2);
            side3Plane.TriangleIndices.Add(3);
            side3Plane.TriangleIndices.Add(4);
            side3Plane.TriangleIndices.Add(5);

            side3Plane.Normals.Add(new Vector3D(0, -1, 0));
            side3Plane.Normals.Add(new Vector3D(0, -1, 0));
            side3Plane.Normals.Add(new Vector3D(0, -1, 0));
            side3Plane.Normals.Add(new Vector3D(0, -1, 0));
            side3Plane.Normals.Add(new Vector3D(0, -1, 0));
            side3Plane.Normals.Add(new Vector3D(0, -1, 0));

            side3Plane.TextureCoordinates.Add(new Point(0, 0));
            side3Plane.TextureCoordinates.Add(new Point(1, 0));
            side3Plane.TextureCoordinates.Add(new Point(1, 1));
            side3Plane.TextureCoordinates.Add(new Point(1, 1));
            side3Plane.TextureCoordinates.Add(new Point(0, 1));
            side3Plane.TextureCoordinates.Add(new Point(0, 0));

            //side4-------------------------------------------------
            side4Plane.Positions.Add(new Point3D(x+(width / 2), y-(width / 2), z-(width / 2)));
            side4Plane.Positions.Add(new Point3D(x+(width / 2), y+(width / 2), z-(width / 2)));
            side4Plane.Positions.Add(new Point3D(x+(width / 2), y+(width / 2), z+(width / 2)));
            side4Plane.Positions.Add(new Point3D(x+(width / 2), y+(width / 2), z+(width / 2)));
            side4Plane.Positions.Add(new Point3D(x+(width / 2), y-(width / 2), z+(width / 2)));
            side4Plane.Positions.Add(new Point3D(x+(width / 2), y-(width / 2), z-(width / 2)));

            side4Plane.TriangleIndices.Add(0);
            side4Plane.TriangleIndices.Add(1);
            side4Plane.TriangleIndices.Add(2);
            side4Plane.TriangleIndices.Add(3);
            side4Plane.TriangleIndices.Add(4);
            side4Plane.TriangleIndices.Add(5);

            side4Plane.Normals.Add(new Vector3D(1, 0, 0));
            side4Plane.Normals.Add(new Vector3D(1, 0, 0));
            side4Plane.Normals.Add(new Vector3D(1, 0, 0));
            side4Plane.Normals.Add(new Vector3D(1, 0, 0));
            side4Plane.Normals.Add(new Vector3D(1, 0, 0));
            side4Plane.Normals.Add(new Vector3D(1, 0, 0));

            side4Plane.TextureCoordinates.Add(new Point(1, 0));
            side4Plane.TextureCoordinates.Add(new Point(1, 1));
            side4Plane.TextureCoordinates.Add(new Point(0, 1));
            side4Plane.TextureCoordinates.Add(new Point(0, 1));
            side4Plane.TextureCoordinates.Add(new Point(0, 0));
            side4Plane.TextureCoordinates.Add(new Point(1, 0));

            //side5-------------------------------------------------
            side5Plane.Positions.Add(new Point3D(x+(width / 2), y+(width / 2), z-(width / 2)));
            side5Plane.Positions.Add(new Point3D(x-(width / 2), y+(width / 2), z-(width / 2)));
            side5Plane.Positions.Add(new Point3D(x-(width / 2), y+(width / 2), z+(width / 2)));
            side5Plane.Positions.Add(new Point3D(x-(width / 2), y+(width / 2), z+(width / 2)));
            side5Plane.Positions.Add(new Point3D(x+(width / 2), y+(width / 2), z+(width / 2)));
            side5Plane.Positions.Add(new Point3D(x+(width / 2), y+(width / 2), z-(width / 2)));

            side5Plane.TriangleIndices.Add(0);
            side5Plane.TriangleIndices.Add(1);
            side5Plane.TriangleIndices.Add(2);
            side5Plane.TriangleIndices.Add(3);
            side5Plane.TriangleIndices.Add(4);
            side5Plane.TriangleIndices.Add(5);

            side5Plane.Normals.Add(new Vector3D(0, 1, 0));
            side5Plane.Normals.Add(new Vector3D(0, 1, 0));
            side5Plane.Normals.Add(new Vector3D(0, 1, 0));
            side5Plane.Normals.Add(new Vector3D(0, 1, 0));
            side5Plane.Normals.Add(new Vector3D(0, 1, 0));
            side5Plane.Normals.Add(new Vector3D(0, 1, 0));

            side5Plane.TextureCoordinates.Add(new Point(1, 1));
            side5Plane.TextureCoordinates.Add(new Point(0, 1));
            side5Plane.TextureCoordinates.Add(new Point(0, 0));
            side5Plane.TextureCoordinates.Add(new Point(0, 0));
            side5Plane.TextureCoordinates.Add(new Point(1, 0));
            side5Plane.TextureCoordinates.Add(new Point(1, 1));

            //side6-------------------------------------------------
            side6Plane.Positions.Add(new Point3D(x-(width / 2), y+(width / 2), z-(width / 2)));
            side6Plane.Positions.Add(new Point3D(x-(width / 2), y-(width / 2), z-(width / 2)));
            side6Plane.Positions.Add(new Point3D(x-(width / 2), y-(width / 2), z+(width / 2)));
            side6Plane.Positions.Add(new Point3D(x-(width / 2), y-(width / 2), z+(width / 2)));
            side6Plane.Positions.Add(new Point3D(x-(width / 2), y+(width / 2), z+(width / 2)));
            side6Plane.Positions.Add(new Point3D(x-(width / 2), y+(width / 2), z-(width / 2)));

            side6Plane.TriangleIndices.Add(0);
            side6Plane.TriangleIndices.Add(1);
            side6Plane.TriangleIndices.Add(2);
            side6Plane.TriangleIndices.Add(3);
            side6Plane.TriangleIndices.Add(4);
            side6Plane.TriangleIndices.Add(5);

            side6Plane.Normals.Add(new Vector3D(-1, 0, 0));
            side6Plane.Normals.Add(new Vector3D(-1, 0, 0));
            side6Plane.Normals.Add(new Vector3D(-1, 0, 0));
            side6Plane.Normals.Add(new Vector3D(-1, 0, 0));
            side6Plane.Normals.Add(new Vector3D(-1, 0, 0));
            side6Plane.Normals.Add(new Vector3D(-1, 0, 0));

            side6Plane.TextureCoordinates.Add(new Point(0, 1));
            side6Plane.TextureCoordinates.Add(new Point(0, 0));
            side6Plane.TextureCoordinates.Add(new Point(1, 0));
            side6Plane.TextureCoordinates.Add(new Point(1, 0));
            side6Plane.TextureCoordinates.Add(new Point(1, 1));
            side6Plane.TextureCoordinates.Add(new Point(0, 1));

            //Set Brush property for the Material applied to each face
            SolidColorBrush brush = new SolidColorBrush(color);

            DiffuseMaterial side1Material = new DiffuseMaterial(brush);
            DiffuseMaterial side2Material = new DiffuseMaterial(brush);
            DiffuseMaterial side3Material = new DiffuseMaterial(brush);
            DiffuseMaterial side4Material = new DiffuseMaterial(brush);
            DiffuseMaterial side5Material = new DiffuseMaterial(brush);
            DiffuseMaterial side6Material = new DiffuseMaterial(brush);

            side1.Material = side1Material;
            side2.Material = side2Material;
            side3.Material = side3Material;
            side4.Material = side4Material;
            side5.Material = side5Material;
            side6.Material = side6Material;

            //set Geometry property of MeshGeometry3D's
            side1.Geometry = side1Plane;
            side2.Geometry = side2Plane;
            side3.Geometry = side3Plane;
            side4.Geometry = side4Plane;
            side5.Geometry = side5Plane;
            side6.Geometry = side6Plane;

            cube.Children.Add(side1);
            cube.Children.Add(side2);
            cube.Children.Add(side3);
            cube.Children.Add(side4);
            cube.Children.Add(side5);
            cube.Children.Add(side6);

            return cube;
        }
    }
}
