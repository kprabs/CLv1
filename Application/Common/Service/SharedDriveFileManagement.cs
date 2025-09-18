using CoreLib.Application.Common.Interfaces;

namespace CoreLib.Application.Common.Service
{
    public class SharedDriveFileManagement : IFileManagement
    {
        public byte[] GetFileData(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        public string ReadFile(string filePath)
        {
            using StreamReader reader = new(filePath);
            return reader.ReadToEnd();
        }
    }
}
