using LannConstants;

namespace LannUtils; 

public class Utils {
    public static string CreateURL(int serverId) {
        return Constants.DOMAIN.Replace("{0}", serverId.ToString());
    }
}