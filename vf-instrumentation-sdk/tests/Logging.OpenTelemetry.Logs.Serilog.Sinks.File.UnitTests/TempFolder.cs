using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace VF.Logging.OpenTelemetry.Logs.Serilog.Sinks.File.UnitTests
{
    internal class TempFolder : IDisposable
    {
        private static readonly Guid Session = Guid.NewGuid();

        private TempFolder(string? name = null)
        {
            Path = System.IO.Path.Combine(
                Environment.GetEnvironmentVariable("TMP") ?? Environment.GetEnvironmentVariable("TMPDIR") ?? "/tmp",
                "VF.Logging.OpenTelemetry.Logs.Serilog.Sinks.File.UnitTests",
                Session.ToString("n"),
                name ?? Guid.NewGuid().ToString("n"));

            Directory.CreateDirectory(Path);
        }

        private string Path { get; }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(Path))
                    Directory.Delete(Path, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static TempFolder ForCaller([CallerMemberName] string? caller = null,
            [CallerFilePath] string sourceFileName = "")
        {
            if (caller is null) throw new ArgumentNullException(nameof(caller));
            if (sourceFileName is null) throw new ArgumentNullException(nameof(sourceFileName));

            var folderName = System.IO.Path.GetFileNameWithoutExtension(sourceFileName) + "_" + caller;

            return new TempFolder(folderName);
        }

        public string AllocateFilename(string? ext = null) => System.IO.Path.Combine(Path, Guid.NewGuid().ToString("n") + "." + (ext ?? "tmp"));
    }
}