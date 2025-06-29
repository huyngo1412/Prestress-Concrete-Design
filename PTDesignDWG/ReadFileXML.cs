
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;

namespace PTDesignDWG
{
    [XmlRoot("Tendon")]
    public class Tendon
    {
        public Tendon()
        {
            // Khởi tạo danh sách để tránh null và đảm bảo XML được serialize
            _point = new List<Point3D>();
        }
        private string _name;
        [XmlElement("Name")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        // Các property không cần serialize sẽ được đánh dấu XmlIgnore
        [XmlIgnore]
        private string _storyName;
        [XmlIgnore]
        public string StoryName
        {
            get { return _storyName; }
            set
            {
                _storyName = value;
            }
        }

        private List<Point3D> _point;
        [XmlArray("Points")]
        [XmlArrayItem("Point")]
        public List<Point3D> Point
        {
            get { return _point; }
            set { _point = value; }
        }

        private List<double> _globalZ;
        [XmlArray("GlobalZValues")]
        [XmlArrayItem("Z")]
        public List<double> GlobalZ
        {
            get { return _globalZ; }
            set { _globalZ = value; }
        }

        // Loại bỏ các property không cần in ra
        [XmlIgnore]
        private double _a;
        [XmlIgnore]
        public double A
        {
            get { return _a; }
            set
            {
                _a = value;
            }
        }

        [XmlIgnore]
        private double _b;
        [XmlIgnore]
        public double B
        {
            get { return _b; }
            set
            {
                _b = value;
            }
        }

        [XmlIgnore]
        private string _loadPattern;
        [XmlIgnore]
        public string LoadPattern
        {
            get { return _loadPattern; }
            set
            {
                _loadPattern = value;
            }
        }

        [XmlIgnore]
        private double _loadTranfer;
        [XmlIgnore]
        public double LoadTranfer
        {
            get { return _loadTranfer; }
            set
            {
                _loadTranfer = value;
            }
        }


    }
    public class ReadFileXML
    {
        public static List<Tendon> LoadTendons(string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<Tendon>), new XmlRootAttribute("Tendons"));
            using (var stream = File.OpenRead(filePath))
            {
                return (List<Tendon>)serializer.Deserialize(stream);
            }
        }
    }
}
