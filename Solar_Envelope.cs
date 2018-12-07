  private void RunScript(List<Vector3d> SunVecs, List<Mesh> Boxels, List<Point3d> Points, List<Vector3d> Normals, double Percent, ref object Blockage, ref object NonBlocking, ref object FBoxels)
  {
/*the algorithm has been designed based on 
    [1]	P. Nourian, R. Gonçalves, S. Zlatanova, K. A. Ohori, and A. Vu Vo, “Voxelization algorithms for geospatial applications: Computational methods for voxelating spatial datasets of 3D city models containing 3D surface, curve and point data models,” MethodsX, vol. 3, pp. 69–86, 2016.
    [2]	F. De Luca, “Solar Form-finding . Subtractive Solar Envelope and Integrated Solar Collection Computational Method for High-rise Bui ....,” Proc. 37th Annu. Conf. Assoc. Comput. Aided Des. Archit. (ACADIA 2017), no. November, pp. 212–221, 2017.
    The code has been written by Pirouz Nourian and Sama Rezvani:
    BSD 3-Clause License
    Copyright (c) 2018, Pirouz Nourian
    All rights reserved.
    A full description of the license is privided in the git repository: https://gitlab.com/Pirouz-Nourian/spatial_computing/tree/master  
    
    Modified BSD 3-Clause License

Copyright (c) 2018, Pirouz Nourian & Sama Rezvani
All rights reserved.

Commerical or Academic use is permitted provided the work is adequately cited and attributed to the author. This work can be cited as follows:

Nourian, Pirouz, and Rezvani ,Sama, Voxelated Solar Envelope, 2018, avaiable at https://gitlab.com/Pirouz-Nourian/spatial_computing


Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/  
    int n = SunVecs.Count;
    int PointCount = Points.Count;
    int NormalCount = Normals.Count;
    int VoxelCount = Boxels.Count;
    if((Percent < 0) || (Percent > 1)){throw new Exception("the percentage tolerance provided must be a number between 0 and 1!");}
    if((PointCount < 1) || (n < 1) || (VoxelCount < 1)){throw new Exception("The number of items in some inputs is not sufficient!");}
    if(PointCount != NormalCount){throw new Exception("The number of normal vectors and points must be the same!");}

    double[] BlockedFluxes = new double[VoxelCount];
    bool[] IsNotBlocking = new bool[VoxelCount];

    int Sum = PointCount * n;//in order to relativize the blockage numbers, we divide them all by the total number of rays which could have been blocked
    List<Mesh> FilteredBoxels = new List<Mesh>();
    for(int i = 0; i < VoxelCount; i++){//foreach(Mesh Boxel in Boxels){
      Mesh Boxel = Boxels[i];
      double BlockedFlux = 0;
      foreach(Vector3d SunVec in SunVecs){
        Vector3d NSunVec = -SunVec;
        for(int k = 0;k < PointCount;k++){//foreach(Point3d Point in Points){
          Point3d Point = Points[k];
          Vector3d Normal = Normals[k];
          double PotentialFlux = Vector3d.Multiply(NSunVec, Normal);
          if(PotentialFlux > 1){throw new Exception("some sunvectors or normal vectors have a norm greater than 1!");}
          if(PotentialFlux > 0){
            Ray3d Ray = new Ray3d(Point, NSunVec);
            double RayParameter = Rhino.Geometry.Intersect.Intersection.MeshRay(Boxel, Ray);
            bool Hit = (RayParameter > 0);
            if(Hit){
              BlockedFlux = BlockedFlux + PotentialFlux;
            }
          }
        }
      }
      BlockedFlux = BlockedFlux / Sum;
      BlockedFluxes[i] = BlockedFlux;
      if(BlockedFlux < Percent){
        FilteredBoxels.Add(Boxel);
        IsNotBlocking[i] = true;
      }else{
        IsNotBlocking[i] = false;
      }
    }
    Blockage = BlockedFluxes;
    NonBlocking = IsNotBlocking;
    FBoxels = FilteredBoxels;
  }