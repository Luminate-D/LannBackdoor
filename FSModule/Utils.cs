using Ionic.Zip;

namespace FSModule;

public class Utils {
    public static byte[] CreateZipFile(string file) {
        ZipFile zip = new("archive.zip");

        FileAttributes attributes = File.GetAttributes(file);
        if (attributes.HasFlag(FileAttributes.Directory))
            zip.AddDirectory(file);
        else zip.AddFile(file);

        MemoryStream stream = new();
        zip.Save(stream);
        zip.Dispose();

        return stream.ToArray();
    }
}