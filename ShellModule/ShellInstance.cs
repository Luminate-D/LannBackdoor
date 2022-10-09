using System.Diagnostics;

namespace ShellModule;

public class OutputEventArgs : EventArgs {
    public string Data;

    public OutputEventArgs(string data) {
        Data = data;
    }
}

public class ShellInstance {
    public readonly int Id;
    private readonly Process _process;

    public event EventHandler<OutputEventArgs> StdOut = delegate { };
    public event EventHandler<OutputEventArgs> StdErr = delegate { };
    public event EventHandler<EventArgs> OnClose = delegate { };

    public ShellInstance(int id, string fileName) {
        Id = id;
        _process = new Process {
            StartInfo = {
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                FileName = fileName
            }
        };

        _process.OutputDataReceived += (_, args) => StdOut(null, new OutputEventArgs(args.Data));
        _process.ErrorDataReceived += (_, args) => StdErr(null, new OutputEventArgs(args.Data));
        _process.Exited += (_, args) => OnClose(null, args);
    }

    public async Task Write(string data) {
        await _process.StandardInput.WriteAsync(data);
    }

    public void Close() {
        _process.Close();
    }

    public void Start() {
        bool success = _process.Start();
        if (!success) throw new Exception("Failed to start process");
    }
}