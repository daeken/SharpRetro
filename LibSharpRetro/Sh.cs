using System.Diagnostics;

namespace LibSharpRetro;

public static class Sh {
    public static int Run(string fileName, params string[] args) {
        using var proc = StartProcess(fileName, args, false);
        proc.WaitForExit();
        return proc.ExitCode;
    }

    public static async Task<int> RunAsync(
        string fileName,
        CancellationToken cancellationToken = default,
        params string[] args
    ) {
        using var proc = StartProcess(fileName, args, false);
        await proc.WaitForExitAsync(cancellationToken);
        return proc.ExitCode;
    }

    public static async Task<(int ExitCode, string Stdout, string Stderr)> CaptureAsync(
        string fileName,
        CancellationToken cancellationToken = default,
        params string[] args) {
        using var proc = StartProcess(fileName, args, true);

        var stdoutTask = proc.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderrTask = proc.StandardError.ReadToEndAsync(cancellationToken);

        await proc.WaitForExitAsync(cancellationToken);
        await Task.WhenAll(stdoutTask, stderrTask);

        return (proc.ExitCode, stdoutTask.Result, stderrTask.Result);
    }

    static Process StartProcess(string fileName, string[] args, bool captureOutput) {
        var psi = new ProcessStartInfo(fileName) {
            UseShellExecute = false,
            RedirectStandardOutput = captureOutput,
            RedirectStandardError = captureOutput,
            RedirectStandardInput = false,
        };

        // ArgumentList saves you from manual quoting hell
        foreach(var arg in args)
            psi.ArgumentList.Add(arg);

        var proc = Process.Start(psi)
                   ?? throw new InvalidOperationException("Failed to start process.");

        return proc;
    }
}