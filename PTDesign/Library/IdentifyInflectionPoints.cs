using PTDesign.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;

namespace PTDesign.Library
{
    public class IdentifyInflectionPoints
    {
        private static Dictionary<Point3D, (bool, int)> GetInflectionPoints(List<Point3D> points)
        {
            Dictionary<Point3D, (bool, int)> inflectionPoints = new Dictionary<Point3D, (bool, int)>();

            if (points == null || points.Count < 3)
                return inflectionPoints; // Cần ít nhất 3 điểm để xác định điểm uốn

            bool? isIncreasing = false; // Trạng thái ban đầu chưa xác định
            inflectionPoints.Add(new Point3D(points[0].X, points[0].Y, points[0].Z), (true, 0));
            for (int i = 1; i < points.Count; i++)
            {
                double prevZ = points[i - 1].Z;
                double currentZ = points[i].Z;
                if (currentZ > prevZ)
                {
                    if (isIncreasing == false) // Trước đó đang giảm, bây giờ tăng -> Điểm uốn
                        inflectionPoints.Add(new Point3D(points[i - 1].X, points[i - 1].Y, points[i - 1].Z), (false, i));
                    isIncreasing = true;
                }
                else if (currentZ < prevZ)
                {
                    if (isIncreasing == true) // Trước đó đang tăng, bây giờ giảm -> Điểm uốn
                        inflectionPoints.Add(new Point3D(points[i - 1].X, points[i - 1].Y, points[i - 1].Z), (true, i));
                    isIncreasing = false;
                }
            }
            inflectionPoints.Add(new Point3D(points[points.Count - 1].X, points[points.Count - 1].Y, points[points.Count - 1].Z), (true, points.Count - 1));
            return inflectionPoints;
        }
        public static List<TendonProfile> FindInflectionPoints(Tendon tendon)
        {
            List<TendonProfile> ListInflectionPoints = new List<TendonProfile>();
            var inflectionPoints = GetInflectionPoints(tendon.Point);

            var IncreasingPoints = inflectionPoints
                .Where(kv => kv.Value.Item1 == true)
                .Select(kv => (kv.Key.X, kv.Key.Y, kv.Key.Z))
                .ToList();

            var DecreasingPoints = inflectionPoints
                .Where(kv => kv.Value.Item1 == false)
                .Select(kv => (kv.Key.X, kv.Key.Y, kv.Key.Z))
                .ToList();
            int NumberSpan = 1;
            for (int i = 1; i < IncreasingPoints.Count; i++)
            {
                int index1 = tendon.Point.FindIndex(p =>
                    Math.Abs(p.X - IncreasingPoints[i - 1].X) < 1e-6 &&
                    Math.Abs(p.Y - IncreasingPoints[i - 1].Y) < 1e-6 &&
                    Math.Abs(p.Z - IncreasingPoints[i - 1].Z) < 1e-6);

                int index2 = tendon.Point.FindIndex(p =>
                    Math.Abs(p.X - IncreasingPoints[i].X) < 1e-6 &&
                    Math.Abs(p.Y - IncreasingPoints[i].Y) < 1e-6 &&
                    Math.Abs(p.Z - IncreasingPoints[i].Z) < 1e-6);

                if (index1 >= 0 && index2 >= 0 && index1 < index2)
                {
                    var ip = new TendonProfile
                    {
                        TendonName = "Nhịp " + NumberSpan,
                        FirstIncresingPoint = new Point3D(tendon.Point[index1].X, tendon.Point[index1].Y, tendon.Point[index1].Z),
                        LastIncresingPoint = new Point3D(tendon.Point[index2].X, tendon.Point[index2].Y, tendon.Point[index2].Z),
                        Point = tendon.Point.GetRange(index1, index2 - index1 + 1)
                            .Select(p => new Point3D(p.X, p.Y, p.Z)).ToList(),

                    };
                    var matchingPoint = ip.Point
                        .FirstOrDefault(p => DecreasingPoints
                            .Any(dp => Math.Abs(dp.X - p.X) < 0.2 &&
                                       Math.Abs(dp.Y - p.Y) < 0.2 &&
                                       Math.Abs(dp.Z - p.Z) < 0.2));

                    if (matchingPoint == default(Point3D))
                    {
                        MessageBox.Show("Đã xảy ra lỗi không thể xác định điểm uốn");
                    }

                    ip.DecreasingPoints = matchingPoint;
                    ListInflectionPoints.Add(ip);
                    NumberSpan++;
                }
            }
            return ListInflectionPoints;
        }
    }
}
