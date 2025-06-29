using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms; // Add this namespace for MessageBox
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.IO;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Windows.Media.Media3D;
using Autodesk.AutoCAD.DatabaseServices.Filters;

namespace PTDesignDWG
{
    
    public class CreateTendonProfile
    {
        
        string FilePath = string.Empty;
        private const string LayerTendon = "0.TendonProfile";
        private const string LayerTendonNote = "0.TendonNote";
        private double OffsetY = 0;
        [CommandMethod("Tendon")]
        public void DrawSplineFromFileWithPickStart()
        {
            List<Tendon> tendons = new List<Tendon>();
            tendons = ReadFile();

            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            // Bậc đa thức spline
            var degree = new PromptIntegerOptions("\nNhập số bậc đa thức của spline: ")
            {
                DefaultValue = 3,
                AllowNone = true
            };
            int d = ed.GetInteger(degree).Value;
            var tendonX = tendons.Where(t => Math.Abs(t.Point[0].Y - t.Point[t.Point.Count - 1].Y) < 1e-6).ToList();
            var tendonY = tendons.Where(t => Math.Abs(t.Point[0].X - t.Point[t.Point.Count - 1].X) < 1e-6).ToList();
            if(tendonX.Count != 0)
            {
                OffsetY = 0;
                PromptPointOptions ppoX = new PromptPointOptions("\nChọn gốc tọa độ cho cáp phương X : ");
                PromptPointResult pprX = ed.GetPoint(ppoX);
                foreach (var item in tendonX)
                {
                    List<Point3d> cadPc = new List<Point3d>();
                    List<Point3d> cadSc = new List<Point3d>();

                    for (int i = 0; i < item.Point.Count; i++)
                    {
                        cadPc.Add(new Point3d(item.Point[i].X, item.Point[i].Y, 0));
                        cadSc.Add(new Point3d(item.Point[i].X, item.GlobalZ[i], 0));
                    }
                    DrawTendonProfile1(item.Name, cadPc, item.GlobalZ, pprX);
                    DrawTendonProfile2(item.Name,cadSc, item.GlobalZ, pprX,d);

                }
            }
            if (tendonY.Count != 0)
            {
                PromptPointOptions ppoY = new PromptPointOptions("\nChọn gốc tọa độ cho cáp phương Y : ");
                PromptPointResult pprY = ed.GetPoint(ppoY);

                OffsetY = 0;
                foreach (var item in tendonY)
                {
                    List<Point3d> cadPc = new List<Point3d>();
                    List<Point3d> cadSc = new List<Point3d>();
                    for (int i = 0; i < item.Point.Count; i++)
                    {
                        cadPc.Add(new Point3d(item.Point[i].X, item.Point[i].Y, 0));
                        cadSc.Add(new Point3d(item.Point[i].Y, item.GlobalZ[i], 0));

                    }
                    DrawTendonProfile1(item.Name, cadPc, item.GlobalZ, pprY);
                    DrawTendonProfile2(item.Name, cadSc, item.GlobalZ, pprY, d);
                }
            }
        }

        public void DrawTendonProfile1(string name,List<Point3d> point3s,List<double> globalZ, PromptPointResult ppr)
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            // Prompt for start point
           
            if (ppr.Status != PromptStatus.OK) return;
            Point3d pickPt = ppr.Value;

            // Read file points
            if (!File.Exists(FilePath))
            {
                ed.WriteMessage($"\nFile không tìm thấy: {FilePath}");
                return;
            }
            List<Point3d> filePts = point3s;
            if (filePts.Count < 2)
            {
                ed.WriteMessage("\nFile không đủ điểm.");
                return;
            }

            // Compute offset to align first file point with pickPt
            // Adjust all points
            Point3dCollection fitPts = new Point3dCollection();
            foreach (var pt in filePts)
            {
                fitPts.Add(new Point3d(pt.X + pickPt.X, pt.Y + pickPt.Y, pt.Z + pickPt.Z));
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                CreateLayerIfNotExists(tr, db);
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                Spline spline = new Spline(fitPts, 2, 0.0)
                {
                    Layer = LayerTendon,
                    
                };
                ms.AppendEntity(spline);
                tr.AddNewlyCreatedDBObject(spline, true);

                double radius = 15;
                foreach (Point3d pt in fitPts)
                {
                    Circle cir = new Circle(pt, Vector3d.ZAxis, radius) { Layer = LayerTendon };
                    ms.AppendEntity(cir);
                    tr.AddNewlyCreatedDBObject(cir, true);
                    int index = fitPts.IndexOf(pt);
                    DBText txt = new DBText
                    {
                        Position = new Point3d(pt.X + radius + 0.5, pt.Y, 0),
                        Height = 50,
                        TextString = globalZ[index].ToString("F0"),
                        Layer = LayerTendonNote
                    };
                    ms.AppendEntity(txt);
                    tr.AddNewlyCreatedDBObject(txt, true);
                }
                DBText txt2 = new DBText
                {
                    Position = new Point3d(fitPts[0].X - 500, fitPts[0].Y, 0),
                    Height = 500,
                    TextString = name,
                    Layer = LayerTendonNote
                };
                ms.AppendEntity(txt2);
                tr.AddNewlyCreatedDBObject(txt2, true);
                tr.Commit();
            }

            ed.WriteMessage("\nĐã vẽ spline bắt đầu từ điểm click và theo dữ liệu file.");
        }

        public void DrawTendonProfile2(string name,List<Point3d> point3s, List<double> globalZ, PromptPointResult ppr,int degree)
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;


            if (ppr.Status != PromptStatus.OK) return;
            Point3d pickPt = ppr.Value;

            double offsetX = point3s.Select(pt => pt.X).Max() + 15000;
            // Read file points
            if (!File.Exists(FilePath))
            {
                ed.WriteMessage($"\nFile không tìm thấy: {FilePath}");
                return;
            }
            List<Point3d> filePts = point3s;
            if (filePts.Count < 2)
            {
                ed.WriteMessage("\nFile không đủ điểm.");
                return;
            }
           
            Point3dCollection fitPts = new Point3dCollection();
            foreach (var pt in filePts)
            {
                fitPts.Add(new Point3d(pt.X + pickPt.X + offsetX, pt.Y + pickPt.Y + OffsetY, pt.Z + pickPt.Z));
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                CreateLayerIfNotExists(tr, db);
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                Spline spline = new Spline(fitPts, degree, 0.0)
                {
                    Layer = LayerTendon,
                };
                ms.AppendEntity(spline);
                tr.AddNewlyCreatedDBObject(spline, true);

                double radius = 15;
                foreach (Point3d pt in fitPts)
                {
                    Circle cir = new Circle(pt, Vector3d.ZAxis, radius) { Layer = LayerTendon };
                    ms.AppendEntity(cir);
                    tr.AddNewlyCreatedDBObject(cir, true);
                    int index = fitPts.IndexOf(pt);
                    DBText txt = new DBText
                    {
                        Position = new Point3d(pt.X + radius + 0.5, pt.Y, 0),
                        Height = 50,
                        TextString = globalZ[index].ToString("F0"),
                        Layer = LayerTendonNote
                    };
                    ms.AppendEntity(txt);
                    tr.AddNewlyCreatedDBObject(txt, true);
                }
                DBText txt2 = new DBText
                {
                    Position = new Point3d(fitPts[0].X - 5000, fitPts[0].Y, 0),
                    Height = 500,
                    TextString = name,
                    Layer = LayerTendonNote
                };
                ms.AppendEntity(txt2);
                tr.AddNewlyCreatedDBObject(txt2, true);
                tr.Commit();
            }
            OffsetY += 10000;
            ed.WriteMessage("\nĐã vẽ spline bắt đầu từ điểm click và theo dữ liệu file.");
        }
        [CommandMethod("EditTendon")]
        public void EditTendonProfile()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            // 1. Chọn Spline hoặc BlockReference chứa spline
            var pe = new PromptEntityOptions("\nChọn Spline hoặc BlockReference chứa spline: ");
            pe.SetRejectMessage("\nPhải chọn Spline hoặc BlockReference.");
            pe.AddAllowedClass(typeof(Spline), false);
            pe.AddAllowedClass(typeof(BlockReference), false);
            var res = ed.GetEntity(pe);
            if (res.Status != PromptStatus.OK) return;

            // 2. Chọn Polyline
            var pe2 = new PromptEntityOptions("\nChọn Polyline: ");
            pe2.SetRejectMessage("\nPhải chọn Polyline.");
            pe2.AddAllowedClass(typeof(Polyline), false);
            var res2 = ed.GetEntity(pe2);
            if (res2.Status != PromptStatus.OK) return;

            // 3. Khoảng cách mẫu
            var pd = new PromptDoubleOptions("\nNhập khoảng cách giữa các điểm: ")
            {
                DefaultValue = 500,
                AllowNone = true
            };

            var prD = ed.GetDouble(pd);
            if (prD.Status != PromptStatus.OK) return;
            double interval = prD.Value;

            using (var tr = db.TransactionManager.StartTransaction())
            {
                // --- Lấy spline gốc và block transform nếu có ---
                Spline baseSpline = null;
                Matrix3d blockXform = Matrix3d.Identity;
                var selEnt = (Entity)tr.GetObject(res.ObjectId, OpenMode.ForRead);
                if (selEnt is Spline sp) baseSpline = sp;
                else if (selEnt is BlockReference br)
                {
                    var btr = (BlockTableRecord)tr.GetObject(br.BlockTableRecord, OpenMode.ForRead);
                    foreach (ObjectId id in btr)
                    {
                        var child = tr.GetObject(id, OpenMode.ForRead) as Entity;
                        if (child is Spline childSp)
                        {
                            baseSpline = childSp;
                            blockXform = br.BlockTransform;
                            break;
                        }
                    }
                }
                if (baseSpline == null)
                {
                    ed.WriteMessage("\nKhông tìm thấy spline.");
                    return;
                }
                Spline splineWorld = blockXform == Matrix3d.Identity
                    ? baseSpline
                    : baseSpline.GetTransformedCopy(blockXform) as Spline;

                // --- Lấy polyline, tạo layer và modelspace ---
                var pl = (Polyline)tr.GetObject(res2.ObjectId, OpenMode.ForRead);
                double totalLen = pl.Length;
                CreateLayerIfNotExists(tr, db);

                var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                var ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                double curDist = 0.0;
                double markerR = 15.0;

                while (curDist <= totalLen)
                {
                    // 1. Tính điểm trên polyline
                    Point3d ptPoly = pl.GetPointAtDist(curDist);

                    // 2. Tạo đường thẳng dọc (song song Y) đi qua ptPoly
                    var infiniteLine = new Line(
                        new Point3d(ptPoly.X, ptPoly.Y - 1e5, 0),
                        new Point3d(ptPoly.X, ptPoly.Y + 1e5, 0)
                    );

                    // 3. Tính giao điểm giữa infiniteLine và splineWorld
                    var interPts = new Point3dCollection();
                    infiniteLine.IntersectWith(splineWorld, Intersect.OnBothOperands, interPts, IntPtr.Zero, IntPtr.Zero);

                    // 4. Chọn giao điểm (nếu có), ngược lại fallback về GetClosestPointTo
                    Point3d ptSpl;
                    if (interPts.Count > 0)
                    {
                        // Nếu nhiều giao điểm, chọn gần polyline nhất (về Y)
                        ptSpl = interPts[0];
                        double minDY = Math.Abs(ptPoly.Y - ptSpl.Y);
                        foreach (Point3d ip in interPts)
                        {
                            double dy = Math.Abs(ptPoly.Y - ip.Y);
                            if (dy < minDY)
                            {
                                minDY = dy;
                                ptSpl = ip;
                            }
                        }
                    }
                    else
                    {
                        // fallback
                        ptSpl = splineWorld.GetClosestPointTo(ptPoly, false);
                    }

                    // 5. Tính khoảng cách và làm tròn
                    double minD = Math.Round(ptPoly.DistanceTo(ptSpl) / 5.0) * 5.0;
                    string txt = minD.ToString("F0");

                    // 6. Vẽ circle + text trên polyline
                    var cirPoly = new Circle(ptPoly, Vector3d.ZAxis, markerR) { Layer = LayerTendon };
                    ms.AppendEntity(cirPoly); tr.AddNewlyCreatedDBObject(cirPoly, true);
                    var txtPoly = new DBText
                    {
                        Position = new Point3d(ptPoly.X + markerR + 1, ptPoly.Y, 0),
                        Height = 25,
                        TextString = txt,
                        Layer = LayerTendonNote,
                        Rotation = 0
                    };
                    ms.AppendEntity(txtPoly); tr.AddNewlyCreatedDBObject(txtPoly, true);

                    // 7. Vẽ circle + text trên spline
                    var cirSpl = new Circle(ptSpl, Vector3d.ZAxis, markerR) { Layer = LayerTendon };
                    ms.AppendEntity(cirSpl); tr.AddNewlyCreatedDBObject(cirSpl, true);
                    var txtSpl = new DBText
                    {
                        Position = new Point3d(ptSpl.X + markerR + 1, ptSpl.Y, 0),
                        Height = 25,
                        TextString = txt,
                        Layer = LayerTendonNote,
                        Rotation = 0
                    };
                    ms.AppendEntity(txtSpl); tr.AddNewlyCreatedDBObject(txtSpl, true);

                    curDist += interval;
                }

                tr.Commit();
            }

            ed.WriteMessage("\nĐã ghi text và hình tròn vuông góc theo phương X trên cả polyline và spline.");
        }


        public List<Tendon> ReadFile()
        {
            OpenFileDialog OpenEDB = new OpenFileDialog();
            OpenEDB.InitialDirectory = "C:\\";
            OpenEDB.Filter = " (*.xml)|*.xml|All Files (*.*)|*.*";
            OpenEDB.Multiselect = true;
            List<Tendon> tendons = new List<Tendon>();
            if (OpenEDB.ShowDialog() == DialogResult.OK)
            {
                FilePath = OpenEDB.FileName;
                tendons = ReadFileXML.LoadTendons(FilePath);
                return tendons;
            }
            return null;
        }
        private  void CreateLayerIfNotExists(Transaction tr, Database db)
        {
            LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
            if (!lt.Has(LayerTendon))
            {
                lt.UpgradeOpen();
                LayerTableRecord ltr = new LayerTableRecord
                {
                    Name = LayerTendon,
                    Color = Color.FromColorIndex(ColorMethod.ByAci, 1),
                    LinetypeObjectId = db.ByLayerLinetype,
                    IsPlottable = true
                };
                lt.Add(ltr);
                tr.AddNewlyCreatedDBObject(ltr, true);
            }
            if (!lt.Has(LayerTendonNote))
            {
                lt.UpgradeOpen();
                LayerTableRecord ltr = new LayerTableRecord
                {
                    Name = LayerTendonNote,
                    Color = Color.FromColorIndex(ColorMethod.ByAci, 2),
                    LinetypeObjectId = db.ByLayerLinetype,
                    IsPlottable = true
                };
                lt.Add(ltr);
                tr.AddNewlyCreatedDBObject(ltr, true);
            }
        }
    }
}
