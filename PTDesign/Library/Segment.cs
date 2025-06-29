using PTDesign.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PTDesign.Library
{
    public enum TendonDirection
    {
        X,
        Y,
        Other
    }

    public static class Segment
    {
        private const double BeamTolerance = 1e-2;

        /// <summary>
        /// Adjusts the GlobalZ values of each tendon point based on whether
        /// it lies on a beam or within a floor. If all points lie on some beam (possibly different ones),
        /// beam logic is applied for all points; otherwise, beam logic for beam-located points and floor logic for others.
        /// </summary>
        public static void AdjustTendonZ()
        {
            var data = DataContainer.Instance;

            foreach (var tendon in data.Tendons)
            {

                // Check if every point is on some beam (could be different beams)
                bool allOnBeams = tendon.Point.All(pt => FindBeamForPoint(data.Beams, pt.X, pt.Y,tendon) != null);

                for (int i = 0; i < tendon.Point.Count; i++)
                {
                    var pt = tendon.Point[i];
                    Beam beam = null;

                    if (allOnBeams)
                    {
                        // All points lie on beams: apply beam logic for each
                        beam = FindBeamForPoint(data.Beams, pt.X, pt.Y, tendon);
                        if (beam != null && beam.ShapeType is Rectangular rect && tendon.StoryName == beam.StoryName)
                        {
                            var story = data.Stories
                                .FirstOrDefault(s => string.Equals(s.StoryName, beam.StoryName, StringComparison.OrdinalIgnoreCase));
                            if (story != null)
                            {
                                tendon.GlobalZ[i] = pt.Z - (story.Elevation - rect.Height);
                                continue;
                            }
                        }
                    }
                    else
                    {

                        // If not on beam, or story not found, try floor
                        var floor = FindFloorForPoint(data.Floors, pt.X, pt.Y, tendon);
                        if (floor != null && tendon.StoryName == floor.StoryName)
                        {
                            var storyF = data.Stories
                                .FirstOrDefault(s => string.Equals(s.StoryName, floor.StoryName, StringComparison.OrdinalIgnoreCase));
                            if (storyF != null)
                            {
                                tendon.GlobalZ[i] = pt.Z - (storyF.Elevation - floor.Thickness);
                            }
                        }
                    }
                }
            }
        }

        #region Helper Methods

        public static Beam FindBeamForPoint(IEnumerable<Beam> beams, double x, double y,Tendon tendon)
        {
            return beams.FirstOrDefault(b => IsPointOnBeam(b, x, y, BeamTolerance, tendon));
        }

        public static bool IsPointOnBeam(Beam beam, double x, double y, double tolerance,Tendon tendon)
        {
            if (beam.StoryName != tendon.StoryName)
            {
                return false;
            }
            //double ax = beam.Start.X, ay = beam.Start.Y;
            //double bx = beam.End.X, by = beam.End.Y;
            //double abx = bx - ax, aby = by - ay;
            //double apx = x - ax, apy = y - ay;
            //double ab2 = abx * abx + aby * aby;
            //if (ab2 == 0) return false;

            //double t = (apx * abx + apy * aby) / ab2;
            //if (t < 0 || t > 1) return false;

            //double projX = ax + t * abx;
            //double projY = ay + t * aby;
            //double dx = projX - x, dy = projY - y;
            //return Math.Sqrt(dx * dx + dy * dy) <= tolerance;

            var startXY = (beam.Start.X, beam.Start.Y);
            var endXY = (beam.End.X, beam.End.Y);
            return (x >= Math.Min(startXY.X, endXY.X) && x <= Math.Max(startXY.X, endXY.X) &&
                    y >= Math.Min(startXY.Y, endXY.Y) && y <= Math.Max(startXY.Y, endXY.Y));
        }

        public static Floor FindFloorForPoint(IEnumerable<Floor> floors, double x, double y,Tendon tendon)
        {
            return floors.FirstOrDefault(f => IsPointInFloor(f, x, y,tendon));
        }

        public static bool IsPointInFloor(Floor floor, double x, double y,Tendon tendon)
        {
            if(floor.StoryName != tendon.StoryName)
            {
                return false;
            }
            var poly = floor.Point.Select(p => (p.X, p.Y)).ToList();
            int n = poly.Count;
            const double tol = 1e-6;

            // Check boundary first
            for (int i = 0; i < n; i++)
            {
                var (x1, y1) = poly[i];
                var (x2, y2) = poly[(i + 1) % n];
                if (IsPointOnSegment(x1, y1, x2, y2, x, y, tol))
                    return true;
            }

            // Ray-casting
            bool inside = false;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                var (xi, yi) = poly[i];
                var (xj, yj) = poly[j];
                bool intersect = ((yi > y) != (yj > y)) &&
                                 (x < (xj - xi) * (y - yi) / (yj - yi) + xi);
                if (intersect) inside = !inside;
            }
            return inside;
        }

        public static bool IsPointOnSegment(double x1, double y1, double x2, double y2,
                                             double x, double y, double tol)
        {
            double cross = (x - x1) * (y2 - y1) - (y - y1) * (x2 - x1);
            if (Math.Abs(cross) > tol) return false;
            double dot = (x - x1) * (x - x2) + (y - y1) * (y - y2);
            return dot <= tol;
        }

        #endregion
    }
}
