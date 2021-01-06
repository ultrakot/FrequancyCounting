using System.IO;

namespace DataProcessingExam
{
    public class FileConfiguration
    {
        public int TestFileSize { get; set; }
        public string TestFileName { get; set; }
        public int FullFileSize { get; set; }
        public string FullFileName { get; set; }
        public bool UseTestFile { get; set; }

        public string GetFileForProcessing() => Path.Combine(Directory.GetCurrentDirectory(), UseTestFile ? TestFileName : FullFileName);
    }
}
