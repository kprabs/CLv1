namespace CoreLib.Application.Common.Interfaces
{
    public interface IFileManagement
    {
        public byte[] GetFileData(string filePath);

        public string ReadFile(string filePath);
    }
}
