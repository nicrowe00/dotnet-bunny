using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Turkey
{
    public class DotNet
    {
        public List<Version> RuntimeVersions
        {
            get
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = "dotnet",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = "--list-runtimes",
                };
                Process p = Process.Start(startInfo);
                p.WaitForExit();
                string output = p.StandardOutput.ReadToEnd();
                var list = output
                    .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Split(" ")[1])
                    .Select(versionString => Version.Parse(versionString))
                    .OrderBy(x => x)
                    .ToList();
                return list;
            }
        }

        public Version LatestRuntimeVersion
        {
            get
            {
                return RuntimeVersions.Last();
            }
        }

        public List<Version> SdkVersions
        {
            get
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = "dotnet",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        Arguments = "--list-sdks",
                    };
                Process p = Process.Start(startInfo);
                p.WaitForExit();
                string output = p.StandardOutput.ReadToEnd();
                var list = output
                    .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Split(" ")[0])
                    .Select(versionString => Version.Parse(versionString))
                    .OrderBy(x => x)
                    .ToList();
                return list;
            }
        }

        public Version LatestSdkVersion
        {
            get
            {
                return SdkVersions.Last();
            }
        }

        public static Process Command(DirectoryInfo workingDirectory, params string[] commands)
        {
            var arguments = string.Join(" ", commands);
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = arguments,
                WorkingDirectory = workingDirectory.FullName,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            return Process.Start(startInfo);
        }
    }
}
