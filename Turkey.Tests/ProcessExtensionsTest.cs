using Xunit;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Turkey.Tests
{
    public class ProcessExtensiosnTests
    {
        [Fact]
        public async Task WaitForExitAsync_DoesNotHangForOrphanedGrandChildren()
        {
            const int WaitTimeoutSeconds = 3;
            const int GrandChildAgeSeconds = 3 * WaitTimeoutSeconds;

            string filename = Path.GetTempFileName();
            File.Copy(filename, "test.tmp");
            string path = "test.tmp";
            try
            {
                // This script creates a 'sleep' grandchild that outlives its parent.
                File.WriteAllText(path,
$@"#!/bin/bash

sleep {GrandChildAgeSeconds} &
");
                Process chmodProcess = Process.Start("chmod", $"+x {path}");
                chmodProcess.WaitForExit();
                Assert.Equal(0, chmodProcess.ExitCode);

                var psi = new ProcessStartInfo()
                {
                    FileName = path,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                using Process process = Process.Start(psi);

                // Use a shorter timeout for WaitForExitAsync than the grandchild lives.
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(WaitTimeoutSeconds));

                // The WaitForExit completes by cancellation.
                await Assert.ThrowsAsync<TaskCanceledException>(() => process.WaitForExitAsync(logger: msg => { }, cts.Token));
            }
            finally
            {
                File.Delete(path);
                File.Delete(filename);
            }
        }
    }
}
