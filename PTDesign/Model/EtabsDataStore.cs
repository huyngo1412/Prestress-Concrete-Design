using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;
using ETABSv1;
using PTDesign.Abtract;
using System.Security.Cryptography.X509Certificates;
using PTDesign.Library;

namespace PTDesign.Model
{
    public class EtabsDataStore
    {
        // Story
        public int NumberStories = 1;
        public string[] StoryNames = Array.Empty<string>();
        public double[] StoryElevations = Array.Empty<double>();
        public double[] StoryHeights = Array.Empty<double>();
        public bool[] IsMasterStory = Array.Empty<bool>();
        public string[] SimilarToStory = Array.Empty<string>();
        public bool[] SpliceAbove = Array.Empty<bool>();
        public double[] SpliceHeight = Array.Empty<double>();
        // Frame
        public int NumberNameFrame = 1;
        public string[] FrameName = Array.Empty<string>();
        public string[] PropName = Array.Empty<string>();
        public string[] FrameStoryName = Array.Empty<string>();
        public string[] PointName1 = Array.Empty<string>();
        public string[] PointName2 = Array.Empty<string>();
        public double[] Point1X = Array.Empty<double>();
        public double[] Point1Y = Array.Empty<double>();
        public double[] Point1Z = Array.Empty<double>();
        public double[] Point2X = Array.Empty<double>();
        public double[] Point2Y = Array.Empty<double>();
        public double[] Point2Z = Array.Empty<double>();
        public double[] Angle = Array.Empty<double>();
        public double[] Offset1X = Array.Empty<double>();
        public double[] Offset2X = Array.Empty<double>();
        public double[] Offset1Y = Array.Empty<double>();
        public double[] Offset2Y = Array.Empty<double>();
        public double[] Offset1Z = Array.Empty<double>();
        public double[] Offset2Z = Array.Empty<double>();
        public int[] CardinalPoint = Array.Empty<int>();
        public string CoordinateSystem = "Global";

        // Area
        public int NumberNameArea = 1;
        public string[] AreaName = Array.Empty<string>();
        public int NumberBoundaryPts = 1;
        public int[] PointDelimiter = Array.Empty<int>();
        public string[] PointNames = Array.Empty<string>();
        public double[] PointX = Array.Empty<double>();
        public double[] PointY = Array.Empty<double>();
        public double[] PointZ = Array.Empty<double>();
        public eAreaDesignOrientation[] TypeAreaArray = null;

        // Shape/Combo
        public int NumberCombo = 1;
        public string[] NameCombo = Array.Empty<string>();
        //LoadPattern
        public int NumberLoadPattern = 0;
        public string[] NameLoadPattern = Array.Empty<string>();

        // Frame Properties
        public string[] NameFrameProperties = Array.Empty<string>();
        public eFramePropType[] PropType = Array.Empty<eFramePropType>();

        public double[] t3 = Array.Empty<double>();

        public double[] t2 = Array.Empty<double>();

        public double[] tf = Array.Empty<double>();

        public double[] tw = Array.Empty<double>();

        public double[] t2b = Array.Empty<double>();

        public double[] tfb = Array.Empty<double>();

        //Tendon  
        public int NumberName = 0;
        public string[] TendonNameObj = Array.Empty<string>();
        public int NumberItems = 0;
        public string[] TendonName = Array.Empty<string>();
        public string[] DrawingPointID = Array.Empty<string>();
        public double[] GX = Array.Empty<double>();
        public double[] GY = Array.Empty<double>();
        public double[] GZ = Array.Empty<double>();
        public bool IsSelectedTendon = false;
    }
    public class EtabsReader
    {
        private static EtabsReader? _instance;
        private static readonly object _lock = new object();

        private EtabsReader()
        {
            SapModel = null!;
            EtabsObject = null!;
        }

        public static EtabsReader Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new EtabsReader();
                    }
                    return _instance;
                }
            }
        }
        private cSapModel SapModel { get; set; }
        private cOAPI EtabsObject { get; set; }
        private EtabsDataStore store { get; set; } = new EtabsDataStore();
        private int ret = -1;

        public void InitializeEtabsObject(cOAPI etabsObject)
        {
            EtabsObject = etabsObject;
            SapModel = etabsObject.SapModel;
        }

        public void ReleaseEtabsObject()
        {
            EtabsObject = null!;
            SapModel = null!;
        }
        public void AssignLoadDistributed(string Name, double force, string loadPattern, double start, double end, bool replace)
        {

            ret = SapModel.FrameObj.SetLoadDistributed(Name, loadPattern, 1, 10, start, end, force, force, "Global", true, replace);
        }
        public void AssignLoadPoint(string Name, double force, string loadPattern, double Dis,int Dir, bool replace)
          {
            if (Dis == 0)
            {
                ret = SapModel.FrameObj.SetLoadPoint(Name, loadPattern, 2, 3, .0, force,"Local");
            }
            if (Dis == 1)
            {
                ret = SapModel.FrameObj.SetLoadPoint(Name, loadPattern, 2, 3, 1.0, force, "Local");
            }

        }
        public void AssignJointLoadForce(string Name, double force, string loadPattern, double[] Value, bool replace)
        {

            ret = SapModel.PointObj.SetLoadForce(Name, loadPattern,ref Value,replace,"Global",eItemType.Objects);
        }

        public void GetAll()
        {
            if (_instance?.SapModel?.FrameObj == null)
            {
                throw new InvalidOperationException("SapModel or FrameObj is not initialized.");
            }
            ret = _instance.SapModel.FrameObj.GetAllFrames(
    ref store.NumberNameFrame,
    ref store.FrameName,
    ref store.PropName,
    ref store.FrameStoryName,
    ref store.PointName1,
    ref store.PointName2,
    ref store.Point1X,
    ref store.Point1Y,
    ref store.Point1Z,
    ref store.Point2X,
    ref store.Point2Y,
    ref store.Point2Z,
    ref store.Angle,
    ref store.Offset1X,
    ref store.Offset2X,
    ref store.Offset1Y,
    ref store.Offset2Y,
    ref store.Offset1Z,
    ref store.Offset2Z,
    ref store.CardinalPoint,
    store.CoordinateSystem
        );
            ret = SapModel.LoadPatterns.GetNameList(ref store.NumberLoadPattern, ref store.NameLoadPattern);
            ret = SapModel.Story.GetStories(ref store.NumberStories, ref store.StoryNames, ref store.StoryElevations, ref store.StoryHeights, ref store.IsMasterStory,
              ref store.SimilarToStory, ref store.SpliceAbove, ref store.SpliceHeight);//Lấy dữ liệu tầng
            for (int i = 0; i < store.StoryNames.Length; i++)
            {
                Story story = new Story()
                {
                    StoryName = store.StoryNames[i],
                    Elevation = store.StoryElevations[i],
                };
                DataContainer.Instance.Stories.Add(story);
            }
        }
        public void ReadDataFrame()
        {
            if (_instance?.SapModel?.FrameObj == null)
            {
                throw new InvalidOperationException("SapModel or FrameObj is not initialized.");
            }

            for (int i = 0; i < store.FrameName.Length; i++)
            {
                eFrameDesignOrientation frameType = eFrameDesignOrientation.Beam;
                eFramePropType PropTypeOAPI = eFramePropType.Rectangular;
                ret = _instance.SapModel.FrameObj.GetDesignOrientation(store.FrameName[i], ref frameType);
                if (frameType.ToString() == "Beam")
                {
                    ret = _instance.SapModel.PropFrame.GetTypeOAPI(store.PropName[i], ref PropTypeOAPI);
                    Beam beam = new Beam()
                    {
                        Name = store.FrameName[i],
                        Start = new Point3D(store.Point1X[i], store.Point1Y[i], store.Point1Z[i]),
                        End = new Point3D(store.Point2X[i], store.Point2Y[i], store.Point2Z[i]),
                        StoryName = store.FrameStoryName[i],
                        PointName1 = store.PointName1[i],
                        PointName2 = store.PointName2[i],

                        FrameShapeType = PropTypeOAPI,
                        ShapeType = ShapeInstance.SetShapeInstance(PropTypeOAPI),
                    };
                    beam.GetSection(_instance.SapModel, store.PropName[i]);
                    DataContainer.Instance.AddBeam(beam);

                }
            }
        }
        public void ReadDataFloor()
        {
            if (_instance?.SapModel?.FrameObj == null)
            {
                throw new InvalidOperationException("SapModel or FrameObj is not initialized.");
            }
            ret = _instance.SapModel.AreaObj.GetAllAreas(ref store.NumberNameArea, ref store.AreaName, ref store.TypeAreaArray, ref store.NumberBoundaryPts, ref store.PointDelimiter,
                                ref store.PointNames, ref store.PointX, ref store.PointY, ref store.PointZ);// Lấy toàn bộ thông tin các đối tượng Floor, Brace,...
            if (store.AreaName != null)
            {
                for (int i = 0; i < store.AreaName.Length; i++)
                {
                    if (store.TypeAreaArray[i] == eAreaDesignOrientation.Floor)
                    {
                        eSlabType SlabType = eSlabType.Slab;
                        eShellType ShellType = eShellType.ShellThick;
                        string MatProp = null;
                        double Thickness = -1;
                        int Color = -1;
                        string Notes = null;
                        string GUI = null;
                        string SlabName = store.AreaName[i];
                        int NumberPoint = -1;
                        string[] pointNames = null;
                        string PropName = null;
                        _instance.SapModel.AreaObj.GetProperty(store.AreaName[i], ref PropName);//Lấy tên property area object  
                        _instance.SapModel.AreaObj.GetPoints(SlabName, ref NumberPoint, ref pointNames);
                        ret = _instance.SapModel.PropArea.GetSlab(PropName, ref SlabType, ref ShellType, ref MatProp, ref Thickness, ref Color, ref Notes, ref GUI);
                        List<(double X, double Y, double Z)> point3Ds = new List<(double X, double Y, double Z)>();//Khởi tạo giá trị point của slab
                        double maxEleOfWall = -1e11;
                        string storyName = null;
                        for (int j = 0; j < pointNames.Length; j++)
                        {
                            double x = 0, y = 0, z = 0;
                            _instance.SapModel.PointObj.GetCoordCartesian(pointNames[j], ref x, ref y, ref z);//Lấy giá trị x,y,z với mỗi namepoint
                            maxEleOfWall = Math.Max(maxEleOfWall, z);
                            storyName = DataContainer.Instance.Stories.Where(s => Math.Abs(s.Elevation - maxEleOfWall) < 0.1).Select(s => s.StoryName).FirstOrDefault();
                            point3Ds.Add((x, y, z));
                        }

                        Floor floor = new Floor()
                        {
                            Name = SlabName,
                            Thickness = Thickness,
                            Point = point3Ds,
                            StoryName = storyName
                        };
                        DataContainer.Instance.AddFloor(floor);
                    }
                }
            }
        }
        public void ReadLoadPatternList(ObservableCollection<string> listcombo)
        {
            ret = SapModel.LoadPatterns.GetNameList(ref store.NumberLoadPattern, ref store.NameLoadPattern);
            if (ret != 0 || store.NameLoadPattern.Count() == 0)
            {
                MessageBox.Show("Đã xảy ra lỗi không thể cập nhập Load Pattern trong mô hình.");
                listcombo.Clear();
                return;
            }
            for (int i = 0; i < store.NameLoadPattern.Count(); i++)
            {
                listcombo.Add(store.NameLoadPattern[i]);
            }
        }
        public List<Beam> GetSelectedFrame()
        {
            List<Beam> selectedBeams = new List<Beam>();
            bool selected = false;
            foreach (var item in DataContainer.Instance.Beams)
            {
                ret = SapModel.FrameObj.GetSelected(item.Name, ref selected);
                if (selected)
                {
                    selectedBeams.Add(item);
                }
            }
            return selectedBeams;
        }
        public void GetFrameProperties(ObservableCollection<string> listframeprop)
        {
            listframeprop.Clear();
            ret = SapModel.PropFrame.GetAllFrameProperties(
                ref store.NumberNameFrame,
                ref store.NameFrameProperties,
                ref store.PropType,
                ref store.t3,
                ref store.t2,
                ref store.tf,
                ref store.tw,
                ref store.t2b,
                ref store.tfb
            );

            foreach (var item in store.NameFrameProperties)
            {
                listframeprop.Add(item);
            }

        }
        public void GetTendonObject()
        {
            ret = SapModel.TendonObj.GetNameList(ref store.NumberName, ref store.TendonNameObj);
            if (store.TendonNameObj != null) DataContainer.Instance.Tendons = new ObservableCollection<Tendon>(GetTendonEDB().ToList());
            foreach (var item in DataContainer.Instance.Tendons)
            {
                item.ListTendonProfile = new ObservableCollection<TendonProfile>(IdentifyInflectionPoints.FindInflectionPoints(item));
            }

        }
        public void GetSelectedObjTendon(ObservableCollection<Tendon> listselectedtendon)
        {
            listselectedtendon.Clear();
            foreach (var item in DataContainer.Instance.Tendons)
            {
                ret = SapModel.TendonObj.GetSelected(item.Name, ref store.IsSelectedTendon);
                if (store.IsSelectedTendon)
                {
                    listselectedtendon.Add(item);
                }
            }
        }
        public string GetFileName()
        {
            return SapModel.GetModelFilename();
        }
        public void CreateFrame(Point3D firstpoint,Point3D lastpoint,string propname,ref string name)
        {
            ret = SapModel.FrameObj.AddByCoord(firstpoint.X, firstpoint.Y, firstpoint.Z, lastpoint.X, lastpoint.Y, lastpoint.Z, ref name, propname, "", "Global");
        }
        private IEnumerable<Tendon> GetTendonEDB()
        {
            var tendonStoryMap = new Dictionary<string, string>();

            var storyElevationMap = DataContainer.Instance.Stories.ToDictionary(s => s.StoryName, s => s.Elevation);

            foreach (var story in DataContainer.Instance.Stories)
            {
                string[] nameListOnStory = Array.Empty<string>();
                ret = SapModel.TendonObj.GetNameListOnStory(story.StoryName, ref store.NumberName, ref nameListOnStory);

                if (nameListOnStory != null)
                {
                    foreach (var name in nameListOnStory)
                    {
                        tendonStoryMap[name] = story.StoryName;
                    }
                }
            }
            foreach (var tendonName in store.TendonNameObj)
            {
                

                ret = SapModel.TendonObj.GetDrawingPoint(tendonName, ref store.NumberItems,
                    ref store.TendonName, ref store.DrawingPointID, ref store.GX, ref store.GY, ref store.GZ, eItemType.Objects);

                if (store.GX == null || store.GY == null || store.GZ == null) continue;

                tendonStoryMap.TryGetValue(tendonName, out string? storyName);
                var points = store.GX.Zip(store.GY, (x, y) => (x, y))
     .Zip(store.GZ, (xy, z) => new Point3D(
         Math.Round(xy.x, 1),
         Math.Round(xy.y, 1),
         Math.Round(z, 1)))
     .ToList();
                var globalZ = points
          .Select(pt => pt.Z)  
          .ToList();
                yield return new Tendon()
                {
                    Name = tendonName,
                    Point = points,
                    StoryName = storyName ?? string.Empty,
                    GlobalZ = globalZ

                };
            }
        }
        private IEnumerable<Floor> GetFloorETABS()
        {

            if (store.AreaName != null)
            {
                for (int i = 0; i < store.AreaName.Length; i++)
                {
                    if (store.TypeAreaArray[i] == eAreaDesignOrientation.Floor)
                    {
                        eSlabType SlabType = eSlabType.Slab;
                        eShellType ShellType = eShellType.ShellThick;
                        string MatProp = null;
                        double Thickness = -1;
                        int Color = -1;
                        string Notes = null;
                        string GUI = null;
                        string SlabName = store.AreaName[i];
                        int NumberPoint = -1;
                        string[] pointNames = null;
                        string PropName = null;
                        SapModel.AreaObj.GetProperty(store.AreaName[i], ref PropName);//Lấy tên property area object  
                        SapModel.AreaObj.GetPoints(SlabName, ref NumberPoint, ref pointNames);
                        ret = SapModel.PropArea.GetSlab(PropName, ref SlabType, ref ShellType, ref MatProp, ref Thickness, ref Color, ref Notes, ref GUI);
                        List<(double X, double Y, double Z)> point3Ds = new List<(double X, double Y, double Z)>();//Khởi tạo giá trị point của slab
                        double maxEleOfWall = -1e11;
                        string storyName = null;
                        for (int j = 0; j < pointNames.Length; j++)
                        {
                            double x = 0, y = 0, z = 0;
                            SapModel.PointObj.GetCoordCartesian(pointNames[j], ref x, ref y, ref z);//Lấy giá trị x,y,z với mỗi namepoint
                            maxEleOfWall = Math.Max(maxEleOfWall, z);
                            storyName = DataContainer.Instance.Stories.Where(s => Math.Abs(s.Elevation - maxEleOfWall) < 0.1).Select(s => s.StoryName).FirstOrDefault();
                            point3Ds.Add((x, y, z));
                        }

                        Floor floor = new Floor()
                        {
                            Name = SlabName,
                            Thickness = Thickness,
                            Point = point3Ds,
                            StoryName = storyName
                        };
                        yield return floor;
                    }
                }
            }
        }
    }
}
