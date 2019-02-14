using ThrowAwayDataBackground;
namespace ThrowAwayData
{
    public class FileObject : BaseObject
    {
        public int Category { get; set; }
        public string Name { get; set; }
        public string Extention { get; set; }
        public byte[] Bytes { get; set; }

        public FileObject() : base()
        {
            Category = 0;
            Name = null;
            Extention = null;
            Bytes = null;
        }
    }
}