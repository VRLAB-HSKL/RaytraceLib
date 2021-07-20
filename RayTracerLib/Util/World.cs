using System;
using System.Collections.Generic;
using System.Numerics;


using RayTracerLib.Light;
using RayTracerLib.Tracer;
using RayTracerLib.GeometricObject;
using RayTracerLib.Sampler;
using RayTracerLib.Camera;
using RayTracerLib.Material;
using RayTracerLib.Material.Mesh;

namespace RayTracerLib.Util
{
    public class World
    {
        public ViewPlane ViewPlane { get; set; } = new ViewPlane();

        public RGBColor BackgroundColor { get; set; } // = new RGBColor();

        public AbstractTracer Tracer { get; set; }

        public AbstractCamera Camera { get; set; }


        public AbstractLight AmbientLight { get; set; }

        public List<AbstractGeometricObject> GeometricObjects { get; set; }
        public List<AbstractLight> Lights { get; set; }

        public GeometricSphere Sphere = new GeometricSphere(Vector3.Zero, 20f, RGBColor.Red);

        public void Build()
        {
            int numSamples = 25;

            ViewPlane.HRes = 1024;
            ViewPlane.VRes = 768;
            ViewPlane.PixelSize = 1f;
            ViewPlane.Gamma = 1f;
            ViewPlane.Sampler = new JitteredSampler(numSamples, 50, ViewPlane.PixelSize, ViewPlane.PixelSize);
            ViewPlane.MaxDepth = 10;

            BackgroundColor = RGBColor.White;
            Tracer = new WhittedTracer(this, ViewPlane.MaxDepth); //new AreaLightingTracer(this);  //MultipleObjectsTracer(this);
            GeometricObjects = new List<AbstractGeometricObject>();

            BuildLights();
            BuildGeometryStatic();
            BuildGeometryFromFile();


            GeometricGrid grid = new GeometricGrid();            
            grid.Objects = GeometricObjects;
            grid.SetupCells();
            //GeometricObjects = new List<AbstractGeometricObject>();
            //GeometricObjects.Clear();
            //GeometricObjects.Add(grid);
            ////GeometricObjects.AddRange(tmpList);

            // Environment light
            EmmisiveMaterial envEmMat = new EmmisiveMaterial();
            envEmMat.Color = new RGBColor(1f, 1f, 0.5f);
            envEmMat.RadianceScalingFactor = 20f;

            EnvironmentalLight envLight = new EnvironmentalLight(
                new MultiJitteredSampler(25, 50, ViewPlane.PixelSize, ViewPlane.PixelSize),
                envEmMat);
            envLight.CastShadows = true;

            Lights.Add(envLight);

            var envSphere = new GeometricSphere
                (new Vector3(0f, 0f, 0f), 10000.0f);
            envSphere.Material = envEmMat;
            envSphere.CastShadows = false;
            //GeometricObjects.Add(envSphere);


            PinholeCamera pinCam = new PinholeCamera();
            pinCam.Eye = new Vector3(200f, 100f, 500f); //(300f, 400f, 500f);
            pinCam.LookAt = new Vector3(200f, 100f, 0f);
            pinCam.DistanceToViewPlane = 300f;//850f;
            pinCam.ComputeUVW();
            Camera = pinCam;

            //ThinLensCamera thinLensCam = new ThinLensCamera();
            //thinLensCam.Sampler = new MultiJitteredSampler(numSamples, 50, ViewPlane.PixelSize, ViewPlane.PixelSize);
            //thinLensCam.Eye = new Vector3(0f, 0f, 500f); //(300f, 400f, 500f);
            //thinLensCam.LookAt = new Vector3(0f, 0f, 0f);
            //thinLensCam.ViewPlaneDistance = 500f;
            //thinLensCam.FocalPlaneDistance = 9f;
            //thinLensCam.LensRadius = 0f;
            //thinLensCam.ComputeUVW();
            //Camera = thinLensCam;



        }

        private void BuildLights()
        {
            var ambLight = new AmbientLight(1f, RGBColor.White);

            var ambOcc = new AmbientOccluder(
                new MultiJitteredSampler(64, 50, ViewPlane.PixelSize, ViewPlane.PixelSize),
                RGBColor.Black);


            ambOcc.RadianceFactor = 0.25f; //0.5f;
            ambOcc.LightColor = RGBColor.White;
            ambOcc.CastShadows = true;

            AmbientLight = ambOcc; // ambLight; // ambOcc; //new AmbientLight(1f, RGBColor.White);
            Lights = new List<AbstractLight>();


            PointLight pointLight01 =
                new PointLight(10f, RGBColor.White, new Vector3(0f, 150f, 100f)); //new Vector3(100f, 50f, 150f));

            pointLight01.CastShadows = true;
            //Lights.Add(pointLight01);

            PointLight pointLight02 =
                new PointLight(10f, RGBColor.White, new Vector3(400f, 150f, 100f)); //new Vector3(100f, 50f, 150f));

            pointLight02.CastShadows = true;
            //Lights.Add(pointLight02);


            DirectionalLight pl2 = new DirectionalLight
            (
                direction: new Vector3(0f, 0f, -1f),
                location: new Vector3(0f, 100f, 0f)
            );

            pl2.RadianceFactor = 30f;
            pl2.CastShadows = true;
            Lights.Add(pl2);

            DirectionalLight dl2 = new DirectionalLight
            (
                direction: new Vector3(0f, 0f, -1f),
                location: new Vector3(-175f, 100f, 0f)
            );

            dl2.RadianceFactor = 20f;
            dl2.CastShadows = false;
            //Lights.Add(dl2);




            
        }

        private void BuildGeometryStatic()
        {
            // Materials
            var phongRed = new PhongMaterial();
            phongRed.SetKA(0.25f);
            phongRed.SetKD(0.6f);
            phongRed.SetCD(RGBColor.Red);
            phongRed.SetKS(0.2f);
            phongRed.SetExp(1000);

            var phongYellow = new PhongMaterial();
            phongYellow.SetKA(0.25f);
            phongYellow.SetKD(0.65f);
            phongYellow.SetCD(RGBColor.Yellow);
            phongYellow.SetKS(0.5f);
            phongYellow.SetExp(100);

            var phongBlue = new PhongMaterial();
            phongBlue.SetKA(0.25f);
            phongBlue.SetKD(0.65f);
            phongBlue.SetCD(RGBColor.Blue);
            phongBlue.SetKS(0.5f);
            phongBlue.SetExp(10);

            var phongGreen = new PhongMaterial();
            phongGreen.SetKA(0.25f);
            phongGreen.SetKD(0.65f);
            phongGreen.SetCD(RGBColor.Green);
            phongGreen.SetKS(0.5f);
            phongGreen.SetExp(10);

            var phongWhite = new PhongMaterial();
            phongWhite.SetKA(0.25f);
            phongWhite.SetKD(0.65f);
            phongWhite.SetCD(RGBColor.White);
            phongWhite.SetKS(0.5f);
            phongWhite.SetExp(10);

            var lightBrownMat = new PhongMaterial();
            lightBrownMat.SetKA(0.25f);
            lightBrownMat.SetKD(0.6f);
            lightBrownMat.SetCD(new RGBColor(0.8207547f, 0.6273146f, 0.2671324f));
            lightBrownMat.SetKS(0.2f);
            lightBrownMat.SetExp(1000);

            var phongBrown = new PhongMaterial();
            phongBrown.SetKA(0.25f);
            phongBrown.SetKD(0.6f);
            phongBrown.SetCD(new RGBColor(37, 14, 3));
            phongBrown.SetKS(0.2f);
            phongBrown.SetKS(1000);


            ReflectiveMaterial refMat = new ReflectiveMaterial();
            refMat.SetKA(0.25f);
            refMat.SetKD(0.6f);
            refMat.SetCD(RGBColor.Blue); //new RGBColor(0.75f, .075f, 0f));
            refMat.SetKS(0.2f);
            refMat.SetExp(10);
            refMat.SetKR(0.25f);
            refMat.SetCR(RGBColor.Blue);

            float exp = 1000f;
            GlossyReflectorMaterial glossyRefMat = new GlossyReflectorMaterial();
            glossyRefMat.SetSamples(50, exp);
            glossyRefMat.SetKA(0f);
            glossyRefMat.SetKD(0f);
            glossyRefMat.SetKS(0f);
            glossyRefMat.SetExp((int)exp);
            glossyRefMat.SetCD(RGBColor.White);
            glossyRefMat.SetKR(0.5f); //0.9f);
            glossyRefMat.SetExponent(exp);
            //glossyRefMat.

            TransparentMaterial transpMat = new TransparentMaterial(0.5f, 2000f, 1.5f, 0.1f, 0.9f);

            DielectricMaterial dielMat = new DielectricMaterial(RGBColor.White, RGBColor.White);
            dielMat.SetKS(0.2f);            
            dielMat.SetExp(2000);
            dielMat.SetEtaIn(0.9f); //1.5f);
            dielMat.SetEtaOut(0.1f); // 1f);
            dielMat.SetIOR(1.33f);
            dielMat.SetKT(0.25f);
            

            EmmisiveMaterial emMat = new EmmisiveMaterial();
            emMat.RadianceScalingFactor = 40f;
            emMat.Color = RGBColor.White;


            //GlossySpecularMaterial gloss = new Glossy

            // Geometric objects
            var sphere01 = new GeometricSphere(new Vector3(0f, 100f, 0f), 100f, RGBColor.White);
            sphere01.Material = phongRed;
            GeometricObjects.Add(sphere01);

            var sphere02 = new GeometricSphere(new Vector3(200f, 100f, 0f), 100f, RGBColor.Yellow);
            sphere02.Material = phongRed;
            GeometricObjects.Add(sphere02);

            var sphere03 = new GeometricSphere(new Vector3(400f, 100f, 0f), 100f, RGBColor.Yellow);
            sphere03.Material = transpMat; //phongYellow;
            GeometricObjects.Add(sphere03);

            var sphere04 = new GeometricSphere(new Vector3(600f, 100f, 0f), 100f, RGBColor.Yellow);
            sphere04.Material = phongGreen;
            GeometricObjects.Add(sphere04);

            var sphere05 = new GeometricSphere(new Vector3(800f, 100f, 0f), 100f, RGBColor.Yellow);
            sphere05.Material = phongBlue;
            GeometricObjects.Add(sphere05);

            var sphereBack = new GeometricSphere(new Vector3(100f, 325f, -250f), 300f, RGBColor.Red);
            sphereBack.Material = glossyRefMat; //dielMat; //glossyRefMat; //refMat;
            GeometricObjects.Add(sphereBack);

            // Planes
            var floorPlane = new GeometricPlane(new Vector3(0f, 0f, 0f), new Normal(0f, 1f, 0.125f), RGBColor.Green);
            floorPlane.Material = lightBrownMat;
            GeometricObjects.Add(floorPlane);

            GeometricPlane backPlane = new GeometricPlane(
                position: new Vector3(0f, 0f, -500f), 
                normal: new Normal(0f, 0f, 1f), 
                RGBColor.White);
            backPlane.Material = phongWhite;
            GeometricObjects.Add(backPlane);

            var leftPlane = new GeometricPlane(
                position: new Vector3(-200f, 0f, 0f),
                normal: new Normal(1f, 0f, 0f), 
                RGBColor.Red);
            
            leftPlane.Material = phongWhite;
            GeometricObjects.Add(leftPlane);


            Vector3 p0 = new Vector3(50f, 200f, 150f);
            Vector3 a = new Vector3(-100f, 0f, 0f);
            Vector3 b = new Vector3(0f, 100f, 0f);

            var rect = new GeometricRectangle(p0, a, b);
            rect.Material = emMat;
            rect.sampler = new MultiJitteredSampler(50, 50, 1f, 1f);
            //GeometricObjects.Add(rect);

            var box = new GeometricBox(
                new Vector3(0f, 0f, 0f),//-100f, 0f, 500f), 
                new Vector3(0f, 100f, -100f));

            box.Material = phongBrown;
            GeometricObjects.Add(box);

            AreaLight al = new AreaLight();
            al.Object = rect;
            al.CastShadows = false;
            //Lights.Add(al);

        }

        /// <summary>
        /// Read geometry from collada file
        /// </summary>
        private void BuildGeometryFromFile()
        {
            string filepath = @"C:\Users\user\Desktop\Collada\sphere.dae";
            
            // Create context to import
            Assimp.AssimpContext importer = new Assimp.AssimpContext();

            //importer.SetConfig()

            Assimp.Scene sc = importer.ImportFile(filepath);
            if (sc is null) return;

            // Todo: Implement mesh objetcs in raytracer 
            foreach(Assimp.Mesh m in sc.Meshes)
            {
                switch(m.PrimitiveType)
                {
                    case Assimp.PrimitiveType.Triangle:
                        Mesh ms = new Mesh();
                        //MeshTriangle trMesh = new MeshTriangle()
                        if(m.HasNormals)
                        {
                            foreach (var n in m.Normals)
                            {
                                ms.Normals.Add(new Normal(n.X, n.Y, n.Z));
                            }
                        }
                        else
                        {
                            Normal[] tmpArr = new Normal[m.Vertices.Count];
                            // iterate over all vertices
                            for(int i = 0; i < m.VertexCount; i++)
                            {
                                Assimp.Vector3D vertex = m.Vertices[i];
                                Normal n = new Normal();
                                for (int j = 0; j < m.FaceCount; j++)
                                {   
                                    // If triangle contains vertex
                                    var triangle = m.Faces[j];
                                    if (triangle.Indices.Contains(i))
                                    {
                                        for(int k = 0; k < triangle.IndexCount; k++)
                                        {
                                            if(tmpArr[i + k] is null)
                                            {
                                                // Calculate normal and add normal at index
                                                int idx01 = k;
                                                int idx02 = (k + 1) % (triangle.IndexCount - 1);
                                                int idx03 = (k + 2) % (triangle.IndexCount - 1);

                                                Assimp.Vector3D vertexA = m.Vertices[triangle.Indices[idx01]];
                                                Assimp.Vector3D vertexB = m.Vertices[triangle.Indices[idx02]];
                                                Assimp.Vector3D vertexC = m.Vertices[triangle.Indices[idx03]];

                                                Assimp.Vector3D ab = vertexB - vertexA;
                                                Assimp.Vector3D ac = vertexC - vertexA;

                                                Assimp.Vector3D trNorm = Assimp.Vector3D.Cross(ab, ac);
                                                n = new Normal(trNorm.X, trNorm.Y, trNorm.Z);
                                                tmpArr[i] = n;
                                            }
                                        }

                                        
                                    }


                                    //if(m.Faces[j].Indices.Contains())
                                }
                            }

                            ms.Normals.AddRange(tmpArr);
                        }
                            

                        if(m.HasVertices)
                            foreach(var v in m.Vertices)
                            {
                                ms.Vertices.Add(new Vector3(v.X, v.Y, v.Z));
                            }

                        ms.Indices = new List<int>(m.GetIndices());
                        //ms.U = m.

                        ms.NumVertices = ms.Vertices.Count;
                        //ms.VertexFaces = m.vert

                        ms.NumTriangles = m.FaceCount;

                        ms.VertexFaces = new List<List<int>>(ms.NumVertices);
                        // For each point
                        for (int i = 0; i < m.VertexCount; i++)
                        {
                            ms.VertexFaces.Add(new List<int>());
                            // For each face
                            for(int j = 0; j < m.FaceCount; j++)
                            {                                
                                var face = m.Faces[j];
                                
                                // If current face includes current vertex
                                if(face.Indices.Contains(i))
                                {
                                    // Add face to list to shared faces of vertex
                                    ms.VertexFaces[i].Add(j);
                                }
                            }
                            
                        }

                        for(int i = 0; i < m.FaceCount; i++)
                        {
                            var face = m.Faces[i];//ms.VertexFaces[i];
                            FlatMeshTriangle fltr = new FlatMeshTriangle();
                            fltr.mesh = ms;
                            fltr.index0 = face.Indices[0];
                            fltr.index1 = face.Indices[1];
                            fltr.index2 = face.Indices[2];
                            fltr.ComputeNormal(false);

                            GeometricObjects.Add(fltr);
                        }

                        break;

                    default:
                        break;
                }
            }
            
            //var rootNode = sc.RootNode;
            //for(int i = 0; i < rootNode.ChildCount; i++)
            //{
            //    var node = rootNode.Children[i];
            //    if(node.HasMeshes)
            //    {
            //        sc.mesh
            //    }
            //}

            // ToDo: parse mesh nodes into GeometrictMesh objects
            //GeometricGrid grid = new GeometricGrid();

        }

        public void InitViewPlane()
        {

        }


        public static RGBColor[][] PixelPuffer;
        public System.Drawing.Bitmap bitmapFile;

        public void RenderScene()
        {
            PixelPuffer = new RGBColor[ViewPlane.VRes][];
            for (int i = 0; i < PixelPuffer.Length; i++)
            {
                PixelPuffer[i] = new RGBColor[ViewPlane.HRes];
            }

            bitmapFile = new System.Drawing.Bitmap(ViewPlane.HRes, ViewPlane.VRes, System.Drawing.Imaging.PixelFormat.Format32bppArgb);


            Camera.RenderScene(this);
            
        }

        

        public void DisplayPixel(int x, int y, RGBColor pixelColor)
        {
            //RGBColor mappedColor = new RGBColor();

            if(ViewPlane.ShowOutOfGamut)
            {
                pixelColor = RayTraceUtility.ClampToColor(pixelColor);
            }
            else
            {
                pixelColor = RayTraceUtility.MaxToOne(pixelColor);
            }

            if(ViewPlane.Gamma != 1f)
            {
                pixelColor = pixelColor.PowC(ViewPlane.INV_Gamma);
            }

            //if(pixelColor.R < 1f)
            //{
            //    var D = 1;
            //}

            


            PixelPuffer[y][x] = pixelColor;

            //byte red = (byte)finalColor.R;//((pixelColor.R * 255));
            //byte green = (byte)finalColor.G; //((pixelColor.G * 255));
            //byte blue = (byte)finalColor.B; //((pixelColor.B * 255));

            //var col = System.Drawing.Color.FromArgb(red, green, blue);

            //bitmapFile.SetPixel(x, y, col);
        }

        //public static void DisplayPixel(int x, int y, RGBColor pixelColor, ViewPlane vp)
        //{
        //    //RGBColor mappedColor = new RGBColor();

        //    if (vp.ShowOutOfGamut)
        //    {
        //        pixelColor = RayTraceUtility.ClampToColor(pixelColor);
        //    }
        //    else
        //    {
        //        pixelColor = RayTraceUtility.MaxToOne(pixelColor);
        //    }

        //    if (vp.Gamma != 1f)
        //    {
        //        pixelColor = pixelColor.PowC(vp.INV_Gamma);
        //    }

        //    //if(pixelColor.R < 1f)
        //    //{
        //    //    var D = 1;
        //    //}




        //    PixelPuffer[y][x] = pixelColor;

        //    //byte red = (byte)finalColor.R;//((pixelColor.R * 255));
        //    //byte green = (byte)finalColor.G; //((pixelColor.G * 255));
        //    //byte blue = (byte)finalColor.B; //((pixelColor.B * 255));

        //    //var col = System.Drawing.Color.FromArgb(red, green, blue);

        //    //bitmapFile.SetPixel(x, y, col);
        //}

        public ShadeRec HitObjects(Ray ray) //, ref float tmin)
        {
            ShadeRec shr = new ShadeRec(this);
            double t = 0f;
            Normal normal = new Normal();
            Vector3 localHitPoint = Vector3.Zero;
            double tMin = 10000; //double.MaxValue; //kHugeValue;
            int numObjects = GeometricObjects.Count;

            

            for(int j = 0; j < numObjects; j++)
            {
                if(GeometricObjects[j].Hit(ray, ref t, ref shr) && t < tMin)
                {
                    shr.HitAnObject = true;
                    tMin = t;
                    shr.Material = GeometricObjects[j].Material;
                    shr.HitPoint = ray.Origin + (float)t * ray.Direction;
                    normal = shr.Normal;
                    localHitPoint = shr.LocalHitPoint;

                }

                if(shr.HitAnObject)
                {
                    shr.T = (float)t;
                    shr.Normal = normal;
                    shr.LocalHitPoint = localHitPoint;
                }
            }

            return shr;
        }

        public ShadeRec HitBareBonesObjects(Ray ray)
        {
            ShadeRec shr = new ShadeRec(this);
            double t = 0.0;
            double tmin = double.MaxValue;
            int numObjects = GeometricObjects.Count;

            for(int j = 0; j < numObjects; j++)
            {
                if(GeometricObjects[j].Hit(ray, ref t, ref shr) && (t < tmin))
                {
                    shr.HitAnObject = true;
                    tmin = t;
                    shr.Color = GeometricObjects[j].Color;
                }
            }

            return shr;
        }
    }
}
