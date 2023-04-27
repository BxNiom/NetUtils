namespace BxNiom.Linux;

public class EventFile : IDisposable {
    private readonly FileStream _stream;

    private EventFile(string filename, FileAccess access) {
        Filename = filename;
        _stream  = new FileStream(Filename, FileMode.Open, access, FileShare.ReadWrite);
    }

    public string Filename { get; }

    public void Dispose() {
        _stream?.Dispose();
    }

    public void Write(int type, int code, int value) {
        var sec    = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
        var millis = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();

        _stream.Write(BitConverter.GetBytes(sec));
        _stream.Write(BitConverter.GetBytes(millis));
        _stream.Write(BitConverter.GetBytes(Convert.ToUInt16(type)));
        _stream.Write(BitConverter.GetBytes(Convert.ToUInt16(code)));
        _stream.Write(BitConverter.GetBytes(value));
        _stream.Flush();
    }

    public static EventFile Open(string filename, FileAccess access = FileAccess.Write) {
        return new EventFile(filename, access);
    }
}