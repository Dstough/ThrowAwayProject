using ThrowAwayDataBackground;
namespace ThrowAwayData
{
    public class FileCategory : BaseObject
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public FileCategory() : base()
        {
            Name = null;
            Description = null;
        }
    }
}