using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using RayTracerLib.Util;

namespace RayTracerLib.GeometricObject
{
    public class GeometricGrid : GeometricCompound
    {
        /// <summary>
        /// Cells are stored in a 1D array
        /// </summary>
        public List<AbstractGeometricObject> Cells;

        /// <summary>
        /// Bounding box
        /// </summary>
        public BBox bbox;

        /// <summary>
        /// Number of cells in x direction
        /// </summary>
        public int nx;

        /// <summary>
        /// Number of cells in y direction
        /// </summary>
        public int ny;

        /// <summary>
        /// Number of cells in z direction
        /// </summary>
        public int nz;

        private static readonly float _kEpsilon = 0.001f;

        public GeometricGrid()
        {
            Cells = new List<AbstractGeometricObject>();
            bbox = new BBox();
        }

        
        public void SetupCells()
        {
            // Find the minimum and maximum coordinates of the grid
            Vector3 p0 = MinCoordinates();
            Vector3 p1 = MaxCoordinates();

            bbox.X0 = p0.X;
            bbox.Y0 = p0.Y;
            bbox.Z0 = p0.Z;

            bbox.X1 = p1.X;
            bbox.Y1 = p1.Y;
            bbox.Z1 = p1.Z;

            // Compute the number of grid cells in the x, y and z directions
            int numObjects = Objects.Count;

            // Dimensions of the grid in the x, y and z directions
            double wx = p1.X - p0.X;
            double wy = p1.Y - p0.Y;
            double wz = p1.Z - p0.Z;

            // Multiplyer scales the number of grid cells relative to the number of objects
            // Literature suggest to start with value between 8 and 10, and then experiment
            // once number of objects is final
            double multiplier = 10.0;

            double s = Math.Pow(wx * wy * wz / numObjects, 0.3333333);
            nx = (int)(multiplier * wx / s + 1);
            ny = (int)(multiplier * wy / s + 1);
            nz = (int)(multiplier * wz / s + 1);

            // Set up the array of grid cells with null pointers
            int numCells = nx * ny * nz;
            Cells = new List<AbstractGeometricObject>(numCells);

            for(int j = 0; j < numCells; j++)
            {
                Cells.Add(null);
            }

            // Set up a temporary array to hold the number of objects stored in each cell
            List<int> counts = new List<int>(numCells);

            for(int j = 0; j < numCells; j++)
            {
                counts.Add(0);
            }

            // Put the objects in the cell
            BBox objBox; // objects bounding box
            int index; // cell's array index
        
            for(int j = 0; j < numObjects; j++)
            {
                objBox = Objects[j].GetBoundingBox();

                // Compute the cell indices at the corners of the bounding box of the object

                int ixmin = (int)Clamp(((float)objBox.X0 - p0.X) * nx / (p1.X - p0.X), 0, nx -1);
                int iymin = (int)Clamp(((float)objBox.Y0 - p0.Y) * ny / (p1.Y - p0.Y), 0, ny - 1);
                int izmin = (int)Clamp(((float)objBox.Z0 - p0.Z) * nz / (p1.Z - p0.Z), 0, nz - 1);

                int ixmax = (int)Clamp(((float)objBox.X1 - p0.X) * nx / (p1.X - p0.X), 0, nx - 1);
                int iymax = (int)Clamp(((float)objBox.Y1 - p0.Y) * ny / (p1.Y - p0.Y), 0, ny - 1);
                int izmax = (int)Clamp(((float)objBox.Z1 - p0.Z) * nz / (p1.Z - p0.Z), 0, nz - 1);

                // Add the objects to the cells
                
                for(int iz = izmin; iz <= izmax; iz++) // cells in z direction
                {
                    for (int iy = iymin; iy <= iymax; iy++) // cells in y direction
                    {
                        for (int ix = ixmin; ix <= ixmax; ix++) // cells in x direction
                        {
                            index = ix + nx * iy + nx * ny * iz;
                            
                            if(counts[index] == 0)
                            {
                                Cells[index] = Objects[j];
                                counts[index] += 1; // now = 1
                            }
                            else
                            {
                                if(counts[index] == 1)
                                {
                                    GeometricCompound compound = new GeometricCompound(); // Construct a compound object
                                    compound.Objects.Add(Cells[index]); // Add object already in the cell
                                    compound.Objects.Add(Objects[j]); // Add the new object
                                    Cells[index] = compound; // Store compound in current cell
                                    counts[index] += 1; // now = 2
                                }
                                else // Counts[index] > 1
                                {
                                    // ToDo: Make sure this is correct
                                    Cells[index].Objects.Add(Objects[j]);
                                    counts[index] += 1;
                                }

                            }

                        }
                    }
                }
                
            }

            // display some statistics on counts
            // this is useful for finding out how many cells have no objects, one object, etc
            // comment this out if you don't want to use it

            int num_zeroes = 0;
            int num_ones = 0;
            int num_twos = 0;
            int num_threes = 0;
            int num_greater = 0;

            for (int j = 0; j < numCells; j++)
            {
                if (counts[j] == 0)
                    num_zeroes += 1;
                if (counts[j] == 1)
                    num_ones += 1;
                if (counts[j] == 2)
                    num_twos += 1;
                if (counts[j] == 3)
                    num_threes += 1;
                if (counts[j] > 3)
                    num_greater += 1;
            }

            Console.WriteLine("num_cells =" + numCells);
            Console.WriteLine("numZeroes = " + num_zeroes + "  numOnes = " + num_ones + "  numTwos = " + num_twos);
            Console.WriteLine("numThrees = " + num_threes + "  numGreater = " + num_greater);

            // erase the temporary counts vector

            //counts.erase(counts.begin(), counts.end());

        }

        public override bool Hit(Ray ray, ref double tmin, ref ShadeRec shr)
        {
            // if the ray misses the grids bounding box
            //  return false
            //if (!BoundingBox.Hit(ray))
            //    return false;

            double ox = ray.Origin.X;
            double oy = ray.Origin.Y;
            double oz = ray.Origin.Z;
            double dx = ray.Direction.X;
            double dy = ray.Direction.Y;
            double dz = ray.Direction.Z;

            double x0 = bbox.X0;
            double y0 = bbox.Y0;
            double z0 = bbox.Z0;
            double x1 = bbox.X1;
            double y1 = bbox.Y1;
            double z1 = bbox.Z1;

            double tx_min, ty_min, tz_min;
            double tx_max, ty_max, tz_max;

            // the following code includes modifications from Shirley and Morley (2003)

            double a = 1.0 / dx;
            if (a >= 0)
            {
                tx_min = (x0 - ox) * a;
                tx_max = (x1 - ox) * a;
            }
            else
            {
                tx_min = (x1 - ox) * a;
                tx_max = (x0 - ox) * a;
            }

            double b = 1.0 / dy;
            if (b >= 0)
            {
                ty_min = (y0 - oy) * b;
                ty_max = (y1 - oy) * b;
            }
            else
            {
                ty_min = (y1 - oy) * b;
                ty_max = (y0 - oy) * b;
            }

            double c = 1.0 / dz;
            if (c >= 0)
            {
                tz_min = (z0 - oz) * c;
                tz_max = (z1 - oz) * c;
            }
            else
            {
                tz_min = (z1 - oz) * c;
                tz_max = (z0 - oz) * c;
            }

            double t0 = 0.0, t1 = 0.0;

            if (tx_min > ty_min)
                t0 = tx_min;
            else
                t0 = ty_min;

            if (tz_min > t0)
                t0 = tz_min;

            if (tx_max < ty_max)
                t1 = tx_max;
            else
                t1 = ty_max;

            if (tz_max < t1)
                t1 = tz_max;

            if (t0 > t1)
                return (false);




            // traverse the grid

            //return base.Hit(ray, ref tmin, ref shr);

            // Initial cell coordinates
            int ix = 0;
            int iy = 0;
            int iz = 0;

            // if the ray starts inside the grid
            //  find the cell that contains the ray origin
            if (bbox.Inside(ray.Origin))
            {
                ix = (int)Clamp((float)(ox - x0) * nx / (float)(x1 - x0), 0, nx - 1);
                iy = (int)Clamp((float)(oy - y0) * ny / (float)(y1 - y0), 0, ny - 1);
                iz = (int)Clamp((float)(oz - z0) * nz / (float)(z1 - z0), 0, nz - 1);
            }
            // else
            // find the cell where the ray hits the grid from the outside
            else
            {
                // ToDo: Calculate t0
                //float t0 = shr.T;
                Vector3 p = ray.Origin + (float)t0 * ray.Direction;

                ix = (int)Clamp((float)(p.X - x0) * nx / (float)(x1 - x0), 0, nx - 1);
                iy = (int)Clamp((float)(p.Y - y0) * ny / (float)(y1 - y0), 0, ny - 1);
                iz = (int)Clamp((float)(p.Z - z0) * nz / (float)(z1 - z0), 0, nz - 1);

            }

            double dtx = (tx_max - tx_min) / nx;
            double dty = (ty_max - ty_min) / ny;
            double dtz = (tz_max - tz_min) / nz;

            double tx_next, ty_next, tz_next;
            int ix_step, iy_step, iz_step;
            int ix_stop, iy_stop, iz_stop;

            double kHugeValue = 10000;

            if (dx > 0)
            {
                tx_next = tx_min + (ix + 1) * dtx;
                ix_step = +1;
                ix_stop = nx;
            }
            else
            {
                tx_next = tx_min + (nx - ix) * dtx;
                ix_step = -1;
                ix_stop = -1;
            }

            if (dx == 0.0)
            {
                tx_next = kHugeValue;
                ix_step = -1;
                ix_stop = -1;
            }


            if (dy > 0)
            {
                ty_next = ty_min + (iy + 1) * dty;
                iy_step = +1;
                iy_stop = ny;
            }
            else
            {
                ty_next = ty_min + (ny - iy) * dty;
                iy_step = -1;
                iy_stop = -1;
            }

            if (dy == 0.0)
            {
                ty_next = kHugeValue;
                iy_step = -1;
                iy_stop = -1;
            }

            if (dz > 0)
            {
                tz_next = tz_min + (iz + 1) * dtz;
                iz_step = +1;
                iz_stop = nz;
            }
            else
            {
                tz_next = tz_min + (nz - iz) * dtz;
                iz_step = -1;
                iz_stop = -1;
            }

            if (dz == 0.0)
            {
                tz_next = kHugeValue;
                iz_step = -1;
                iz_stop = -1;
            }


            // traverse the grid

            while (true)
            {
                int index = ix + nx * iy + nx * ny * iz;
                AbstractGeometricObject object_ptr = Cells[index];

                if (tx_next < ty_next && tx_next < tz_next)
                {
                    if (object_ptr != null && object_ptr.Hit(ray, ref tmin, ref shr) && tmin < tx_next)
                    {
                        Material = object_ptr.Material;
                        return (true);
                    }

                    tx_next += dtx;
                    ix += ix_step;

                    if (ix == ix_stop)
                        return (false);
                }
                else
                {
                    if (ty_next < tz_next)
                    {
                        if (object_ptr != null && object_ptr.Hit(ray, ref tmin, ref shr) && tmin < ty_next)
                        {
                            Material = object_ptr.Material;
                            return (true);
                        }

                        ty_next += dty;
                        iy += iy_step;

                        if (iy == iy_stop)
                            return (false);
                    }
                    else
                    {
                        if (object_ptr != null && object_ptr.Hit(ray, ref tmin, ref shr) && tmin < tz_next)
                        {
                            Material = object_ptr.Material;
                            return (true);
                        }

                        tz_next += dtz;
                        iz += iz_step;

                        if (iz == iz_stop)
                            return (false);
                    }
                }
            }


        }

        public override bool ShadowHit(Ray ray, ref double tmin)
        {
            return base.ShadowHit(ray, ref tmin);
        }

        /// <summary>
        /// Compute minimum grid coordinates
        /// </summary>
        /// <returns></returns>
        private Vector3 MinCoordinates()
        {
            BBox box;
            Vector3 p0 = new Vector3(float.MaxValue);

            int numObjects = Objects.Count;

            for(int i = 0; i < numObjects; i++)
            {
                if (Objects[i].GetType() == typeof(GeometricPlane))
                    continue;

                box = Objects[i].GetBoundingBox();


                if (box.X0 < p0.X)
                    p0.X = (float)box.X0;

                if (box.Y0 < p0.Y)
                    p0.Y = (float)box.Y0;

                if (box.Z0 < p0.Z)
                    p0.Z = (float)box.Z0;
            }

            p0.X -= _kEpsilon;
            p0.Y -= _kEpsilon;
            p0.Z -= _kEpsilon;

            return p0;
        }

        /// <summary>
        /// Compute maximum grid coordinates
        /// </summary>
        /// <returns></returns>
        private Vector3 MaxCoordinates()
        {
            BBox box;
            Vector3 p1 = new Vector3(float.MinValue);
            int numObjects = Objects.Count;

            for (int i = 0; i < numObjects; i++)
            {
                box = Objects[i].GetBoundingBox();

                if (box.X0 > p1.X)
                    p1.X = (float)box.X0;

                if (box.Y0 > p1.Y)
                    p1.Y = (float)box.Y0;

                if (box.Z0 > p1.Z)
                    p1.Z = (float)box.Z0;
            }

            p1.X += _kEpsilon;
            p1.Y += _kEpsilon;
            p1.Z += _kEpsilon;

            return p1;
        }

        private float Clamp(float x, float min, float max)
        {
            return (x < min ? min : (x > max ? max : x));
        }

        

    }
}
