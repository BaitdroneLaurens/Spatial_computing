private void RunScript(Mesh BVol, Plane BPlane, double x, double y, double z, ref object A, ref object Voxize, ref object Voxenters, ref object Boxels)
  {
    Vector3d VoxelSize = new Vector3d(x, y, z);
    Voxize = VoxelSize;

    if(!BVol.IsClosed){throw new Exception("the bounding volume is not a closed/solid mesh; therefore this method cannot work!");}

    double epsilon = 10E-3;
    if((x < epsilon) || (y < epsilon) || (z < epsilon)){throw new Exception("either of the sizes is too tiny to work with!");}
    double tolerance = 0.1 * Math.Min(x, y);
    List<Tuple<int,int,int>> voxels = new List<Tuple<int,int,int>>();//could have been called the raster2D as a sparse matrix

    BoundingBox BBox = BVol.GetBoundingBox(BPlane);
    //B = BBox;

    int iCount = (int) Math.Round(BBox.Diagonal.X / x);
    int jCount = (int) Math.Round(BBox.Diagonal.Y / y);
    int kCount = (int) Math.Round(BBox.Diagonal.Z / z);



    double halfX = 0.5 * x;
    double halfY = 0.5 * y;
    double halfZ = 0.5 * z;
    Vector3d e_x = BPlane.XAxis;
    Vector3d e_y = BPlane.YAxis;
    Vector3d e_z = BPlane.ZAxis;

    Point3d Min = BBox.Min;
    Point3d Origin = BPlane.Origin;

    Origin = Origin + Min.X * e_x + Min.Y * e_y + Min.Z * e_z;//BPlane.PointAt(MinCorner.X, MinCorner.Y, MinCorner.Z);

    Origin.X = x * Math.Round(Origin.X / x);
    Origin.Y = y * Math.Round(Origin.Y / y);
    Origin.Z = z * Math.Round(Origin.Z / z);;
    //A = Origin;
    List<Mesh> BoxelMeshes = new List<Mesh>();
    List<Point3d> voxelPoints = new List<Point3d>();

    for(int i = 0;i < iCount;i++){
      for(int j = 0;j < jCount;j++){
        for(int k = 0;k < kCount;k++){
          Point3d voxelPoint = Origin + i * x * e_x + j * y * e_y + k * z * e_z;
          if(BVol.IsPointInside(voxelPoint, tolerance, false))
          {
            Tuple<int,int,int> voxel = Tuple.Create(i, j, k);
            voxels.Add(voxel);
            //voxelPoints.Add(voxelPoint);
            Mesh BoxelMesh = new Mesh();
            Point3d v000 = voxelPoint - halfX * e_x - halfY * e_y - halfZ * e_z;
            voxelPoints.Add(voxelPoint);
            Point3d v001 = v000 + x * e_x;
            Point3d v010 = v000 + y * e_y;
            Point3d v011 = v000 + x * e_x + y * e_y;
            Point3d v100 = v000 + z * e_z;
            Point3d v101 = v000 + x * e_x + z * e_z;
            Point3d v110 = v000 + y * e_y + z * e_z;
            Point3d v111 = v000 + x * e_x + y * e_y + z * e_z;
            Point3d[] vertices = new Point3d[]{v000,v001,v010,v011,v100,v101,v110,v111};
            BoxelMesh.Vertices.AddVertices(vertices);
            MeshFace fLeft = new MeshFace(0, 4, 6, 2);
            MeshFace fRight = new MeshFace(1, 3, 7, 5);
            MeshFace fBack = new MeshFace(0, 1, 5, 4);
            MeshFace fFront = new MeshFace(2, 6, 7, 3);
            MeshFace fBottom = new MeshFace(0, 2, 3, 1);
            MeshFace fTop = new MeshFace(4, 5, 7, 6);
            MeshFace[] faces = new MeshFace[]{fLeft,fRight,fBack,fFront,fBottom,fTop};
            BoxelMesh.Faces.AddFaces(faces);
            BoxelMeshes.Add(BoxelMesh);
          }
        }
      }
    }
    Boxels = BoxelMeshes;
    Voxenters = voxelPoints;

    //    string Description = "pixels as (i,j) tuples, array size as tuple of (int,int), pixel size x,pixelsize y, Plane, origin point";
    //    Tuple<string,List<Tuple<int,int>>,Tuple<int,int>,double, double, Point3d,Plane> Raster2D;
    //    Raster2D = Tuple.Create(Description, pixels, Tuple.Create(iCount, jCount), x, y, Origin, BPlane);
    //    Raster2DTuple = Raster2D;
    //    PixelTuples = pixels;

  }