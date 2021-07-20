using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Material.Mesh
{
    public class Mesh
    {
        /// <summary>
        /// Vertices
        /// </summary>
        public List<Vector3> Vertices;
        
        /// <summary>
        /// Vertex indices
        /// </summary>
        public List<int> Indices;

        /// <summary>
        /// Average normal at each vertex
        /// </summary>
        public List<Normal> Normals;

        /// <summary>
        /// The faces shared by each vertex
        /// </summary>
        public List<List<int>> VertexFaces;

        /// <summary>
        /// U texture coordinate at each vertex
        /// </summary>
        public List<float> U;

        /// <summary>
        /// V texture coordinate at each vertex
        /// </summary>
        public List<float> V;

        /// <summary>
        /// Number of vertices
        /// </summary>
        public int NumVertices;

        /// <summary>
        /// Number of triangles
        /// </summary>
        public int NumTriangles;

        public Mesh()
        {
            Vertices = new List<Vector3>();
            Indices = new List<int>();
            Normals = new List<Normal>();
            VertexFaces = new List<List<int>>();
            U = new List<float>();
            V = new List<float>();
            NumVertices = 0;
            NumTriangles = 0;
        }
    }
}
