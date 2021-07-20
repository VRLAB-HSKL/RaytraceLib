using System;
using System.Collections.Generic;
using System.Numerics;

using RayTracerLib.Sampler;
using RayTracerLib.Light;
using RayTracerLib.Tracer;
using RayTracerLib.Material;
using RayTracerLib.Util;

public static class RayTraceUtility
{
    /// <summary>
    /// Global world object the unity scene is parsed into
    /// </summary>
    //public static WorldInformation GlobalWorld;

    /// <summary>
    /// Static instance of solid color material
    /// </summary>
    public static PhongMaterial SolidColorMaterial = new PhongMaterial();        


    private static ReflectiveMaterial _metalMat;

    /// <summary>
    /// Static instance of metal material
    /// </summary>
    public static ReflectiveMaterial MetalMaterial
    { 
        get
        {
            if(_metalMat is null)
            {
                _metalMat = new ReflectiveMaterial();
                _metalMat.SetKA(Metal_KA);
                _metalMat.SetKD(Metal_KD);
                _metalMat.SetKS(Metal_KS);
                _metalMat.SetExp(Metal_EXP);
                _metalMat.SetKR(Metal_KR);
                _metalMat.SetCR(RGBColor.White);
            }

            return _metalMat;
        }    
    }

    // Metal parameters
    public static float Metal_KA = 0.25f;
    public static float Metal_KD = 0.5f;
    public static float Metal_KS = 0.15f;
    public static int Metal_EXP = 100;
    public static float Metal_KR = 0.75f;


    private static DielectricMaterial _glassMat;

    /// <summary>
    /// Static instance of dielectric material
    /// </summary>
    public static DielectricMaterial GlassMaterial
    {
        get
        {
            if(_glassMat is null)
            {
                _glassMat = new DielectricMaterial(RGBColor.White, RGBColor.White);
                _glassMat.SetKS(Dielectric_KS);
                _glassMat.SetExp((int)Dielectric_EXP);
                _glassMat.SetEtaIn(Dielectric_EtaIN);
                _glassMat.SetEtaOut(Dielectric_EtaOUT);
            }

            return _glassMat;
        }
    }

    // Dielectric parameters
    public static float Dielectric_KS = 0.2f;
    public static float Dielectric_EXP = 100f;
    public static float Dielectric_EtaIN = 1.5f;
    public static float Dielectric_EtaOUT = 1f;

    /// <summary>
    /// Maximum distance that rays are cast
    /// </summary>
    public static float Raycast_Distance = 30f;

    /// <summary>
    /// Bitfield mask to control which objects can be hit by a ray.
    /// Currently, all objects that should be ignored by the raytracer were moved to a
    /// custom layer (IgnoreRayCast). This layer corresponds to the value 9, so we set
    /// all bits in the Int32 to 1, except for the ninth bit. This means all layers except for layer 9
    /// can be hit by rays.
    /// </summary>
    public static int LayerMask = ~(1 << 9);

    /// <summary>
    /// Horizontal width between centers of individual pixels on viewport
    /// </summary>
    public static float H_STEP;

    /// <summary>
    /// Vertical height between centers of individual pixels on viewport
    /// </summary>
    public static float V_STEP;


    /// <summary>
    /// Enum type containing all material types that the raytracer can differentiate between
    /// </summary>
    public enum MaterialType { SolidColor = 1, Metal = 2, Dielectric = 3 };


    /// <summary>
    /// Static metal fuzz factor
    /// </summary>
    private static float metalFuzz = 0.3f;


    /// <summary>
    /// Create a color for the case that a ray does not hit an object.
    /// This function creates a color between white and RGB(0.5, 0.7, 1.0)
    /// based on the direction vector.
    /// Based on "Raytracing in a weekend" C++ function
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static RGBColor CreateNonHitColor(Vector3 direction)
    {
        Vector3 unit_direction = Vector3.Normalize(direction);
        float t = 0.5f * (unit_direction.Y + 1f);
        var colVec = (1f - t) * new Vector3(1f, 1f, 1f) + t * new Vector3(.5f, .7f, 1f);
        RGBColor col = new RGBColor();
        col.R = (byte)colVec.X;
        col.G = (byte)colVec.Y;
        col.B = (byte)colVec.Z;
        return col;

        //return RayTraceUtility.GlobalWorld.BackgroundColor;
    }

    /// <summary>
    /// Static function to translate an ingame material into a <see cref="MaterialType"/> enum value
    /// based on ingame material identifier.
    /// </summary>
    /// <param name="mat"></param>
    /// <returns></returns>
    //public static MaterialType DetermineMaterialType(Material mat)
    //{
    //    string matName = mat.name;

    //    // Remove ' (Instance)' postfix
    //    matName = matName.Split(' ')[0];

    //    switch (matName)
    //    {
    //        case "Metal": return MaterialType.Metal;
    //        case "Dielectric": return MaterialType.Dielectric;
    //        default: return MaterialType.SolidColor;
    //    };
    //}


    public static List<Vector3> RT_points = new List<Vector3>();
    public static List<Vector3> RT_rec_points = new List<Vector3>();


    /// <summary>
    /// Global tone mapping operator to map out-of-gamut colors into displayable range
    /// </summary>
    /// <param name="c">Color in initial range</param>
    /// <returns>Range corrected color</returns>
    public static RGBColor MaxToOne(RGBColor c)
    {
        // Determine max value in color
        float maxValue = (float)Math.Max(c.R, Math.Max(c.G, c.B));



        if (maxValue > 1f)
        {
            RGBColor retColor = c / maxValue;
            return retColor;
        }
        else
        {
            return c;
        }
    }



    /// <summary>
    /// Corrects color values if any value is above 1
    /// </summary>
    /// <param name="rawColor">Color to be checked</param>
    /// <returns>Corrected color</returns>
    public static RGBColor ClampToColor(RGBColor rawColor)
    {
        RGBColor c = rawColor; //VectorToColor(rawColor); // new Color(rawColor.r, rawColor.g, rawColor.g);

        // If any value is out of range, map color to red
        /*
        if (rawColor.R > 1f || rawColor.g > 1f || rawColor.b > 1f)
        {
            c.r = 1f; c.g = 0f; c.b = 0f;
        }
        */


        return c;
    }


    

    /// <summary>
    /// World encapsulation to parse unity scene into a simple construct that can be used in raytracing
    /// </summary>
    public class WorldInformation
    {
        /// <summary>
        /// Information about the viewport in the scene
        /// </summary>
        //public ViewPortPlaneInformation VP { get; set; }

        /// <summary>
        /// Tracer instance to be used to shoot rays during raytracing
        /// </summary>
        public AbstractTracer Tracer { get; set; }

        /// <summary>
        /// Maximum amount of reflections that are calculated into final color
        /// </summary>
        public int MaxDepth { get; set; } = 10;
        
        /// <summary>
        /// Background color of the raytraced scene
        /// </summary>
        public RGBColor BackgroundColor { get; set; } = RGBColor.Black;

        /// <summary>
        /// Global ambient light instance that illuminates the whole scene with a constant light amount
        /// </summary>
        public AbstractLight GlobalAmbientLight { get; set; }

        /// <summary>
        /// Lights in unity scene that influence the ray traced scene
        /// </summary>
        public List<AbstractLight> GlobalLights { get; set; } = new List<AbstractLight>();
                
        public WorldInformation()
        {
            // Create sampler for ambient occluder
            MultiJitteredSampler ambOccSampler = new MultiJitteredSampler(50, 50, H_STEP, V_STEP);

            // Use ambient occluder as global ambient light
            RGBColor ambientMinColor = new RGBColor(0.05f, 0.05f, 0.05f); //, 1); <- used to be Vec4
            AmbientOccluder occluder = new AmbientOccluder(ambOccSampler, ambientMinColor)
            {
                RadianceFactor = 1f,
                LightColor = RGBColor.White
            };
            GlobalAmbientLight = occluder;

            // Parse scene lights
            //foreach (Light l in Resources.FindObjectsOfTypeAll(typeof(Light)) as Light[])
            //{
            //    switch (l.type)
            //    {
            //        case LightType.Directional:
                        
            //                // Direction vector of default direction light path
            //                Vector3 dirVector = new Vector3(0, 0, 1);
            //                Vector3 lightRotationVec = l.transform.rotation.eulerAngles;
            //                //Debug.Log("GlobalLights - DirectionalLight: EulerAngles: " + lightRotationVec);

            //                // Parse light object rotation 
            //                if (lightRotationVec.x != 0f)
            //                    dirVector = Quaternion.AngleAxis(lightRotationVec.x, Vector3.right) * dirVector;

            //                if (lightRotationVec.y != 0f)
            //                    dirVector = Quaternion.AngleAxis(lightRotationVec.y, Vector3.up) * dirVector;

            //                if (lightRotationVec.z != 0f)
            //                    dirVector = Quaternion.AngleAxis(lightRotationVec.z, Vector3.forward) * dirVector;

            //                DirectionalLight tmpLight = new DirectionalLight(dirVector, l.transform.position);
            //                tmpLight.CastShadows = false;
            //                tmpLight.RadianceFactor = l.intensity;

            //                GlobalLights.Add(tmpLight);
            //                //Debug.Log("GlobalLights - DirectionalLight Added: " + dirVector);
                        
            //            break;

            //        case LightType.Point:
            //            GlobalLights.Add(new PointLight(l.intensity, l.color, l.transform.position));
            //            break;
            //    }
            //}
        
        
        }

    }

}

