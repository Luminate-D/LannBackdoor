using System.Text;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace InstallerUtils;

public class WebUtils {
    public static byte[] ReadFully(Stream input) {
        byte[] buffer = new byte[16 * 1024];
        using MemoryStream ms = new();
        int read;
        while ((read = input.Read(buffer, 0, buffer.Length)) > 0) ms.Write(buffer, 0, read);
        return ms.ToArray();
    }

    public static async Task<string> DownloadString(string url) {
        using HttpClient client = new();
        using HttpResponseMessage message = await client.GetAsync(url);
        await using Stream stream = await message.Content.ReadAsStreamAsync();

        return Encoding.UTF8.GetString(ReadFully(stream));
    }

    public static async Task<T> DownloadJson<T>(string url) {
        string data = await DownloadString(url);
        return JsonConvert.DeserializeObject<T>(data)!;
    }
}