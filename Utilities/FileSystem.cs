namespace Utilities; 

public class FileSystem {
    public static void RecursiveDelete(string path) {
        FileAttributes attributes = File.GetAttributes(path);
        if (attributes.HasFlag(FileAttributes.Directory)) {
            DirectoryInfo info = new(path);
            info.Delete(true);
        } else File.Delete(path);
    }
}