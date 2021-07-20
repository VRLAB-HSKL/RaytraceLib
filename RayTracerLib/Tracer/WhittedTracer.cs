using System;
using System.Numerics;

using RayTracerLib.Util;

namespace RayTracerLib.Tracer
{
    public class WhittedTracer : AbstractTracer
    {
        //private float _maxDist;
        //private int _layerMask;
        //private RGBColor _bgColor;

        //private int _maxDepth;


        public WhittedTracer(World w, int maxDepth) : base(w)
        {
            //_maxDist = RayTraceUtility.Raycast_Distance;
            //_layerMask = RayTraceUtility.LayerMask;
            //_bgColor = RayTraceUtility.GlobalWorld.BackgroundColor;
        }

        public WhittedTracer(World w, int maxDepth, float maxDist, int layerMask, RGBColor bgRGBColor) : base(w)
        {
            //_maxDist = maxDist;
            //_layerMask = layerMask;
            //_bgColor = bgRGBColor;
        }

        public override RGBColor TraceRay(Ray ray)
        {
            return TraceRay(ray, World.ViewPlane.MaxDepth);
        }

        public override RGBColor TraceRay(Ray ray, int depth)
        {
            if(depth > World.ViewPlane.MaxDepth)
            {
                return RGBColor.Black;
            }
            else
            {
                ShadeRec hit = World.HitObjects(ray);

                if(hit.HitAnObject)
                {
                    hit.Depth = depth;
                    hit.SpecRay = ray;

                    //return hit.Material.Shade(ref hit, depth);
                    return hit.Material.AreaLightShade(ref hit);
                }
                else
                {
                    return World.BackgroundColor;
                }
            }


            //Debug.Log("WhittedTracer - TraceRay - Depth: " + depth);

            //if(depth > RayTraceUtility.GlobalWorld.MaxDepth)
            //{
            //    return RGBColor.Black;
            //}
            //else
            //{
            //    //if(Physics.Raycast(ray, out ShadeRec hit, _maxDist, _layerMask))
            //    //{
            //    //    // Check if the ray hit anything
            //    //    if (!(hit.collider is null))
            //    //    {
            //    //        // Material of the object that was hit
            //    //        Material mat = hit.transform.gameObject.GetComponent<MeshRenderer>().material;

            //    //        // On empty material, return error color
            //    //        if (mat is null) return RGBColor.red; // new RGBColor(1, 0, 0);

            //    //        // If material contains a texture, use that texture
            //    //        if (!(mat.mainTexture is null))
            //    //        {
            //    //            // Determine u,v coordinates on texture and return texture pixel color
            //    //            var texture = mat.mainTexture as Texture2D;
            //    //            Vector2 pixelUVCoords = hit.textureCoord;
            //    //            pixelUVCoords.x *= texture.width;
            //    //            pixelUVCoords.y *= texture.height;
            //    //            return texture.GetPixel(Mathf.FloorToInt(pixelUVCoords.x), Mathf.FloorToInt(pixelUVCoords.y));
            //    //        }
            //    //        else
            //    //        {
            //    //            // On raw material hit, check for type
            //    //            switch (RayTraceUtility.DetermineMaterialType(mat))
            //    //            {
            //    //                case RayTraceUtility.MaterialType.Metal:
            //    //                    //Debug.Log("Metal Hit!");
            //    //                    RayTraceUtility.MetalMaterial.SetDir(ray.direction);
            //    //                    RayTraceUtility.MetalMaterial.SetCD(mat.color);
            //    //                    return RayTraceUtility.MetalMaterial.Shade(hit, depth);

            //    //                case RayTraceUtility.MaterialType.Dielectric:
            //    //                    //Debug.Log("Dielectric Hit!");
            //    //                    RayTraceUtility.GlassMaterial.SetDir(ray.direction);
            //    //                    RayTraceUtility.GlassMaterial.SetCD(mat.color);
            //    //                    return RayTraceUtility.GlassMaterial.Shade(hit, depth);

            //    //                default:
            //    //                case RayTraceUtility.MaterialType.SolidRGBColor:
            //    //                    //Debug.Log("SolidRGBColor - InitMatRGBColor: " + mat.color);
            //    //                    RayTraceUtility.SolidRGBColorMaterial.SetDir(ray.direction);                                
            //    //                    RayTraceUtility.SolidRGBColorMaterial.SetCD(mat.color);
            //    //                    return RayTraceUtility.SolidRGBColorMaterial.Shade(hit, depth);

            //    //            }
            //    //        }

            //    //    }
            //    //    else
            //    //    {
            //    //        // On non-hit, return non hit color
            //    //        return RayTraceUtility.CreateNonHitRGBColor(ray.direction);
            //    //    }

            //    //}
            //    //else
            //    //{
            //    //    return RayTraceUtility.CreateNonHitRGBColor(ray.direction);
            //    //}

            //    return RGBColor.Black;
            //}
        }

        public override RGBColor TraceRay(Ray ray, float tmin, int depth)
        {
            if (depth > World.ViewPlane.MaxDepth)
            {
                return RGBColor.Black;
            }
            else
            {
                ShadeRec hit = World.HitObjects(ray);

                if (hit.HitAnObject)
                {
                    hit.Depth = depth;
                    hit.SpecRay = ray;
                    tmin = hit.T;

                    //return hit.Material.Shade(ref hit, depth);
                    return hit.Material.AreaLightShade(ref hit);
                }
                else
                {
                    tmin = float.MaxValue;
                    return World.BackgroundColor;
                }
            }
        }
    }


}