namespace LannConstants {
    public static class Constants {
        public const bool Debug = true;

//        public const string Domain = "lann-{0}.ml";
        public const string Domain = "localhost:2022";

        public const int Port = 1337;

        public const string ServiceName = "LannBackdoorServiceName";
        public const string ServiceDisplayName = "LannBackdoorService";
        public const string ServiceFileName = "LBService.exe";
        public const string BackdoorFileName = "LannBackdoor.exe";
        public const string InstallPath = "C:/LannBackdoor";

        public const string RsaPubKey =
            @"-----BEGIN PUBLIC KEY-----MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAtg4CJmyc2wYNscVDhaxrcDcXLN+8w1BSK38HbhrxFg3eBwHZdgDBblehGNCa14x1hHeTUPy/09Hdebd0a3sdxCkPJLFdS1uh6WvoGZUt3CiJGR5Y+yQi2ookCg5p35XaRtkSOWxnsaTpLCJOjEmJahmH2FJKhRPdLRoZtKlY1pLYeNnEoNj6LZGdPKRq1bDE9inAEP/cee1P+g6u0z9YuVMdRBZI++bzclMw4lapcdoM++pfnimbgz5k9d9agKk1bqlSqqSKz8Vs3f6F5IF61jfjFpLoPbqk7rGhOCSRxK0LviWZrI5h8I39+uwBDMxbNCmm7zF+sPi+dzTuRP6rVuvfS2+z+VwLv8CnrmCPwO+CWmDMqC7vX1F5BAgBc5+MmNx6TeBC3nwwsmpzPTeMDG43NKTnzc9wYICFwRAd6j9pd4QO0900oUwQE7tZK2rzpKahgv3ALqT1obv5347T6rYWkExzUw/VU7DkiqZ+9jVRDbFy8cAO9h97/wZe/x0HwFVdbl0axmm25i6YngmqzEE3azNJhYpMV4M8wDQlLy0JmVEnxMVHfkTrvPTqPJzlP0tfMhXF1/1s8XscAL+nxHNgzZOI4Zy9mjQtCg6xxw3BgXACiZtj4SF/xTFNnJH0b3jJV4SBFC/U7+atXfPNPjmOpM9YtCbpMBSgC/LWqJECAwEAAQ==-----END PUBLIC KEY-----";
    }
}