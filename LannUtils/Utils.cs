using Constants = LannConstants.Constants;

namespace LannUtils;

public static class Utils {
    public static string CreateUrl(int serverId) {
        return Constants.Debug ? "127.0.0.1" : Constants.Domain.Replace("{0}", serverId.ToString());
    }
}