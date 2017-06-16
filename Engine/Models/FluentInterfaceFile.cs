namespace Engine.Models
{
    public class FluentInterfaceFile
    {
        public string FileName { get; set; }
        public string Contents { get; set; }

        public FluentInterfaceFile(string fileName, string contents)
        {
            FileName = fileName;
            Contents = contents;
        }
    }
}