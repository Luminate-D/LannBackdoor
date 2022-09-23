using LannConstants;

namespace LannUtils; 

public static class Utils {
    public static string CreateUrl(int serverId) {
        return Constants.Debug ? "localhost" : Constants.Domain.Replace("{0}", serverId.ToString());
    }
}