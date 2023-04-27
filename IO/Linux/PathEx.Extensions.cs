namespace BxNiom.Linux;

public static class PathEx {
    public static string ToLinuxPath(this string path) {
        return path.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
    }
}