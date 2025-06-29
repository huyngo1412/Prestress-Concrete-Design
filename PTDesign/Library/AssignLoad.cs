using PTDesign.Model;
using System.Windows;
using System.Windows.Media.Media3D;

namespace PTDesign.Library
{
    public static class AssignLoad
    {
        private static int Length = 0;
        private static Tendon tendon1;

        public static void AssignLoads(Tendon tendon, string framePrefix, string loadPattern, double prestressForce)
        {
            tendon1 = tendon;
            Length = tendon.ListTendonProfile.Count;

            var story = DataContainer.Instance.Stories
                .FirstOrDefault(s => s.StoryName == tendon.StoryName)
                ?? throw new InvalidOperationException($"Story '{tendon.StoryName}' not found.");

            double baseElevation = story.Elevation;

            for (int i = 0; i < Length; i++)
            {
                var profile = tendon.ListTendonProfile[i];

                Point3D startPlanar = ToPlanar(profile.FirstIncresingPoint, baseElevation);
                Point3D endPlanar = ToPlanar(profile.LastIncresingPoint, baseElevation);

                FindOffsetPoints(startPlanar, endPlanar,
                                 profile.A, profile.B,
                                 out Point3D x1, out Point3D x2);

                if (x1 != startPlanar || x2 != endPlanar)
                {
                    CreateFrameForce_1(
                        startPlanar, endPlanar,
                        x1, x2,
                        profile, i,
                        baseElevation,
                        framePrefix,
                        loadPattern,
                        prestressForce);
                }
                if(x1 == startPlanar && x2 == endPlanar)
                {
                    CreateFrameForce_2(
                       startPlanar, endPlanar,
                       
                       profile, i,
                       baseElevation,
                       framePrefix,
                       loadPattern,
                       prestressForce);
                }    
            }
            AssignMoment(tendon, loadPattern, prestressForce*1000);
        }

        /// <summary>
        /// TH có cả A và B
        /// </summary>
        private static void CreateFrameForce_1(
            Point3D start, Point3D end,
            Point3D x1, Point3D x2,
            TendonProfile tendonprofiles,
            int index,
            double elevation,
            string proname,
            string loadpattern,
            double force)
        {
            Point3D p1 = tendonprofiles.Point.Single(x => x.X == x1.X && x.Y == x1.Y);
            Point3D p2 = tendonprofiles.Point.Single(x => x.X == x2.X && x.Y == x2.Y);
            string name1 = string.Empty, name2 = string.Empty, name3 = string.Empty;

            if (index == 0)
            {
                double e1 = (tendonprofiles.FirstIncresingPoint.Z - p1.Z) / 1000;
                double e2 = (((p1.Z + p2.Z) / 2) - tendonprofiles.DecreasingPoints.Z) / 1000;

                FindOffsetPoints(
                    new Point3D(tendon1.ListTendonProfile[index + 1].FirstIncresingPoint.X,
                                tendon1.ListTendonProfile[index + 1].FirstIncresingPoint.Y,
                                elevation),
                    new Point3D(tendon1.ListTendonProfile[index + 1].LastIncresingPoint.X,
                                tendon1.ListTendonProfile[index + 1].LastIncresingPoint.Y,
                                elevation),
                    tendon1.ListTendonProfile[index + 1].A,
                    tendon1.ListTendonProfile[index + 1].B,
                    out Point3D xa, out Point3D xb);

                Point3D p3 = tendon1.ListTendonProfile[index + 1]
                    .Point.FirstOrDefault(x => x.X == xa.X && x.Y == xa.Y);

                double e3 = (tendonprofiles.LastIncresingPoint.Z - ((p2.Z + p3.Z) / 2)) / 1000;

                double L1 = (tendonprofiles.A * 2) / 1000;
                double L2 = Distance2D(p1, p2) / 1000;
                double L3 = Distance2D(p2, p3) / 1000;

                double Load1 = 8 * force * e1 / Math.Pow(L1, 2);
                double Load2 = 8 * force * e2 / Math.Pow(L2, 2);
                double Load3 = 8 * force * e3 / Math.Pow(L3, 2);

                EtabsReader.Instance.CreateFrame(
                    new Point3D(start.X, start.Y, elevation),
                    new Point3D(p1.X, p1.Y, elevation),
                    proname, ref name1);
                EtabsReader.Instance.CreateFrame(
                    new Point3D(p1.X, p1.Y, elevation),
                    new Point3D(p2.X, p2.Y, elevation),
                    proname, ref name2);
                EtabsReader.Instance.CreateFrame(
                    new Point3D(p2.X, p2.Y, elevation),
                    new Point3D(p3.X, p3.Y, elevation),
                    proname, ref name3);

                EtabsReader.Instance.AssignLoadDistributed(name1, Load1, loadpattern, 0, 1, true);
                EtabsReader.Instance.AssignLoadDistributed(name2, -Load2, loadpattern, 0, 1, true);
                EtabsReader.Instance.AssignLoadDistributed(name3, Load3, loadpattern, 0, 1, true);
            }
            else if (index == Length - 1)
            {
                double e2 = (((p1.Z + p2.Z) / 2) - tendonprofiles.DecreasingPoints.Z) / 1000;
                double e3 = (tendonprofiles.LastIncresingPoint.Z - p2.Z) / 1000;

                double L2 = Distance2D(p1, p2) / 1000;
                double L3 = (tendonprofiles.B * 2) / 1000;

                double Load2 = 8 * force * e2 / Math.Pow(L2, 2);
                double Load3 = 8 * force * e3 / Math.Pow(L3, 2);

                EtabsReader.Instance.CreateFrame(
                    new Point3D(p1.X, p1.Y, elevation),
                    new Point3D(p2.X, p2.Y, elevation),
                    proname, ref name2);
                EtabsReader.Instance.CreateFrame(
                    new Point3D(p2.X, p2.Y, elevation),
                    new Point3D(end.X, end.Y, elevation),
                    proname, ref name3);

                EtabsReader.Instance.AssignLoadDistributed(name2, -Load2, loadpattern, 0, 1, true);
                EtabsReader.Instance.AssignLoadDistributed(name3, Load3, loadpattern, 0, 1, true);
            }
            else
            {
                double e2 = (((p1.Z + p2.Z) / 2) - tendonprofiles.DecreasingPoints.Z) / 1000;

                FindOffsetPoints(
                    new Point3D(tendon1.ListTendonProfile[index + 1].FirstIncresingPoint.X,
                                tendon1.ListTendonProfile[index + 1].FirstIncresingPoint.Y,
                                elevation),
                    new Point3D(tendon1.ListTendonProfile[index + 1].LastIncresingPoint.X,
                                tendon1.ListTendonProfile[index + 1].LastIncresingPoint.Y,
                                elevation),
                    tendon1.ListTendonProfile[index + 1].A,
                    tendon1.ListTendonProfile[index + 1].B,
                    out Point3D xa, out Point3D xb);

                Point3D p3 = tendon1.ListTendonProfile[index + 1]
                    .Point.FirstOrDefault(x => x.X == xa.X && x.Y == xa.Y);

                double e3 = (tendonprofiles.LastIncresingPoint.Z - ((p2.Z + p3.Z) / 2)) / 1000;

                double L2 = Distance2D(p1, p2) / 1000;
                double L3 = Distance2D(p2, p3) / 1000;

                double Load2 = 8 * force * e2 / Math.Pow(L2, 2);
                double Load3 = 8 * force * e3 / Math.Pow(L3, 2);

                EtabsReader.Instance.CreateFrame(
                    new Point3D(p1.X, p1.Y, elevation),
                    new Point3D(p2.X, p2.Y, elevation),
                    proname, ref name2);
                EtabsReader.Instance.CreateFrame(
                    new Point3D(p2.X, p2.Y, elevation),
                    new Point3D(p3.X, p3.Y, elevation),
                    proname, ref name3);

                EtabsReader.Instance.AssignLoadDistributed(name2, -Load2, loadpattern, 0, 1, true);
                EtabsReader.Instance.AssignLoadDistributed(name3, Load3, loadpattern, 0, 1, true);
            }
        }
        private static void CreateFrameForce_2(
            Point3D start, Point3D end,
            TendonProfile tendonprofiles,
            int index,
            double elevation,
            string proname,
            string loadpattern,
            double force)
        {
            string name = string.Empty;
            double e2 = (((start.Z + end.Z) / 2) - tendonprofiles.DecreasingPoints.Z) / 1000;
            double L2 = Distance2D(start, end) / 1000;
            double Load2 = 8 * force * e2 / Math.Pow(L2, 2);
            var beam = DataContainer.Instance.Beams.Where(x => x.Start == start && x.End == end).FirstOrDefault();
            if(beam == null)
            {
                EtabsReader.Instance.CreateFrame(
               new Point3D(start.X, start.Y, elevation),
               new Point3D(end.X, end.Y, elevation),
               proname, ref name);
            }    
            else
            {
                name = beam.Name;
            }    

            EtabsReader.Instance.AssignLoadDistributed(name, Load2, loadpattern, 0, 1, true);
        }
        private static void AssignMoment(Tendon tendon, string loadpattern, double force)
        {
            var data = DataContainer.Instance;
            var beamX = data.Beams.Where(t => Math.Abs(t.Start.Y - t.End.Y) < 500).ToList();
            var beamY = data.Beams.Where(t => Math.Abs(t.Start.X - t.End.X) < 500).ToList();
            var firstpoint = tendon.Point[0];
            var lastpoint = tendon.Point[tendon.Point.Count - 1];
            bool isX = (Math.Abs(firstpoint.Y - lastpoint.Y) < 1e-2);
            bool allOnBeams = tendon.Point.All(pt => Segment.FindBeamForPoint(data.Beams, pt.X, pt.Y, tendon) != null);
            Beam beam1 =  null, beam2 = null;
            if(allOnBeams)
            {
                if (isX)
                {
                    beam1 = Segment.FindBeamForPoint(beamX, firstpoint.X, firstpoint.Y, tendon);
                    beam2 = Segment.FindBeamForPoint(beamX, lastpoint.X, lastpoint.Y, tendon);
                    if (beam1 != null && beam1.ShapeType is Rectangular rect1 && tendon.StoryName == beam1.StoryName)
                    {
                        if (firstpoint.Z == beam1.Start.Z - (rect1.Height / 2))
                        {
                            double[] value = new double[] { force, 0, 0, 0, 0, 0 };
                            EtabsReader.Instance.AssignJointLoadForce(beam1.PointName1, force, loadpattern, value, true);
                        }
                        else
                        {
                            double momen = (Math.Abs(firstpoint.Z - (beam1.Start.Z - (rect1.Height / 2)))) * force;
                            double[] Value = new double[] { force, 0, 0, 0, momen, 0 };
                            EtabsReader.Instance.AssignJointLoadForce(beam1.PointName1, force, loadpattern, Value, true);
                        }

                    }
                    if (beam2 != null && beam2.ShapeType is Rectangular rect2 && tendon.StoryName == beam2.StoryName)
                    {
                        if (lastpoint.Z == beam2.End.Z - (rect2.Height / 2))
                        {
                            double[] value = new double[] { -force, 0, 0, 0, 0, 0 };
                            EtabsReader.Instance.AssignJointLoadForce(beam2.PointName2, force, loadpattern, value, true);
                            

                        }
                        else
                        {
                            double momen = (Math.Abs(lastpoint.Z - (beam2.End.Z - (rect2.Height / 2)))) * force;
                            double[] Value = new double[] { -force, 0, 0, 0, -momen, 0 };
                            EtabsReader.Instance.AssignJointLoadForce(beam2.PointName2, force, loadpattern, Value, true);
                        }
                    }
                }
                else
                {
                    beam1 = Segment.FindBeamForPoint(beamY, firstpoint.X, firstpoint.Y, tendon);
                    beam2 = Segment.FindBeamForPoint(beamY, lastpoint.X, lastpoint.Y, tendon);
                    if (beam1 != null && beam1.ShapeType is Rectangular rect1 && tendon.StoryName == beam1.StoryName)
                    {
                        if (firstpoint.Z == beam1.Start.Z - (rect1.Height / 2))
                        {
                            double[] Value = new double[] { 0, force, 0, 0, 0, 0 };
                            EtabsReader.Instance.AssignJointLoadForce(beam1.PointName1, force, loadpattern, Value, true);
                        }
                        else
                        {
                            double momen = (Math.Abs(firstpoint.Z - (beam1.Start.Z - (rect1.Height / 2)))) * force;
                            double[] Value = new double[] { force, 0, 0, momen, 0, 0 };
                            EtabsReader.Instance.AssignJointLoadForce(beam1.PointName1, force, loadpattern, Value, true);
                        }    

                    }
                    if (beam2 != null && beam2.ShapeType is Rectangular rect2 && tendon.StoryName == beam2.StoryName)
                    {
                        if (lastpoint.Z == beam2.End.Z - (rect2.Height / 2))
                        {
                            double[] Value = new double[] { 0, -force, 0, 0, 0, 0 };
                            EtabsReader.Instance.AssignJointLoadForce(beam2.PointName2, force, loadpattern, Value, true);

                        }
                        else
                        {
                            double momen = (Math.Abs(lastpoint.Z - (beam2.End.Z - (rect2.Height / 2)))) * force;
                            double[] Value = new double[] { -force, 0, 0, -momen, 0, 0 };
                            EtabsReader.Instance.AssignJointLoadForce(beam2.PointName2, force, loadpattern, Value, true);
                        }    
                    }
                }
            }    
        }
        private static void FindOffsetPoints(
            Point3D start, Point3D end,
            double offsetFromStart, double offsetFromEnd,
            out Point3D pointA, out Point3D pointB)
        {
            var delta = end - start;
            double length = delta.Length;
            if (length <= 0) throw new ArgumentException("Start and End must be distinct points.");

            var dir = delta / length;
            pointA = offsetFromStart > 0 ? start + dir * offsetFromStart : start;
            pointB = offsetFromEnd > 0 ? end - dir * offsetFromEnd : end;
        }

        private static Point3D ToPlanar(Point3D pt, double elevation)
            => new Point3D(pt.X, pt.Y, elevation);

        private static double Distance2D(Point3D p1, Point3D p2)
            => Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
    }
}