using System.Configuration;

namespace TextFileSplitter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string fileLocation = ConfigurationManager.AppSettings["FileLocation"]?.ToString();
                string splitByLines = ConfigurationManager.AppSettings["NumberOfLines"]?.ToString();
                if (!string.IsNullOrWhiteSpace(fileLocation) && File.Exists(fileLocation))
                {
                    int splitSize = Convert.ToInt32(splitByLines);
                    using (var lineIterator = File.ReadLines(fileLocation).GetEnumerator())
                    {
                        string fileName = GetFileName(fileLocation, false);
                        fileLocation = GetDirectory(fileLocation) + fileName;
                        CreateDirectory(fileLocation);
                        bool stillGoing = true;
                        for (int chunk = 0; stillGoing; chunk++)
                            stillGoing = WriteChunk(lineIterator, splitSize, chunk, fileLocation, fileName);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }

        private static bool WriteChunk(IEnumerator<string> lineIterator, int splitSize, int chunk, string fileLocation, string fileName)
        {
            using (var writer = File.CreateText($"{fileLocation}\\{fileName}{chunk}.txt"))
            {
                for (int i = 0; i < splitSize; i++)
                {
                    if (!lineIterator.MoveNext())
                        return false;
                    writer.WriteLine(lineIterator.Current);
                }
            }
            return true;
        }

        private static void CreateDirectory(string path)
        {
            if (!string.IsNullOrWhiteSpace(Path.GetExtension(path)))
                path = path.Substring(0, path.LastIndexOf("\\") + 1);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        private static string GetDirectory(string path)
        {
            if (!string.IsNullOrWhiteSpace(Path.GetExtension(path)))
                return path.Substring(0, path.LastIndexOf("\\") + 1);
            return path;
        }
        private static string GetFileName(string path, bool withExtension = true)
        {
            if (!string.IsNullOrWhiteSpace(Path.GetExtension(path)))
            {
                path = Path.GetFileName(path);
                if (withExtension)
                    return path;
                else
                    return path.Substring(0, path.LastIndexOf("."));
            }
            return string.Empty;
        }

    }
}