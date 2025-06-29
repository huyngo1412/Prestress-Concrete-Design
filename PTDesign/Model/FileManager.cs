using ETABSv1;
using PTDesign.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.IO;

namespace PTDesign.Model
{
    public class FileManager
    {
        private static FileManager? _instance;
        public static FileManager Instance => _instance ??= new FileManager();

        public string FilePath { get; private set; } = string.Empty;
        public string FilePathEtabs { get; private set; } = string.Empty;
        public bool IsDirty { get; private set; }

        private FileManager()
        {

        }

        public void SetFilePath(string path)
        {
            FilePath = path;
            IsDirty = false;
        }
        public void SetFilePathEtabs(string path)
        {
            FilePathEtabs = path;
            IsDirty = false;
        }
        public void MarkDirty()
        {
            IsDirty = true;
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new InvalidOperationException("Bạn chưa chọn đường dẫn lưu file.");
            var tendonList = new ObservableCollection<Tendon>();
            EtabsReader.Instance.GetSelectedObjTendon(tendonList);
            if(tendonList.Count == 0)
            {
                MessageBox.Show($"Bạn chưa chọn đối tượng cáp dự ứng lực ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }    
            var xmlSerializer = new XmlSerializer(typeof(ObservableCollection<Tendon>), new XmlRootAttribute("Tendons"));
            using (var write = new StreamWriter(FilePath))
            {
                xmlSerializer.Serialize(write, tendonList);
            }
        }

        public void SaveAs(string newPath)
        {
            FilePath = newPath;
            Save();
        }
        public  List<Tendon> LoadTendons(string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<Tendon>), new XmlRootAttribute("Tendons"));
            using (var stream = File.OpenRead(filePath))
            {
                return (List<Tendon>)serializer.Deserialize(stream);
            }
        }


    }

    [XmlRoot("DataContainer")]
    public class DataContainer
    {
        private static DataContainer? _instance;
        private static readonly object _lock = new();

        [XmlArray("Beams")]
        [XmlArrayItem("Beam")]
        public ObservableCollection<Beam> Beams { get; set; } = new();

        [XmlArray("Tendons")]
        [XmlArrayItem("Tendon")]
        public ObservableCollection<Tendon> Tendons { get; set; } = new();
        [XmlArray("Storys")]
        public ObservableCollection<Story> Stories { get; set; } = new();
        [XmlArray("Floor")]
        [XmlArrayItem("Floor")]
        public ObservableCollection<Floor> Floors { get; set; } = new();
        private DataContainer() { }

        public static DataContainer Instance
        {
            get
            {
                lock (_lock)
                {
                    _instance ??= new DataContainer();
                    return _instance;
                }
            }
        }

        public void AddBeam(Beam beam) => Beams.Add(beam);
        public void AddTendon(Tendon tendon) => Tendons.Add(tendon);
        public void AddFloor(Floor floor) => Floors.Add(floor);
        public void Clear()
        {
            Beams.Clear();
            Tendons.Clear();
        }
    }

}
