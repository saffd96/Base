using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Support;
using UniRx;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

namespace Editor
{
    public class GameBuilder
    {
        private static readonly CompositeDisposable BuildDisposable = new();
        private static Dictionary<string, string> _commandLineArguments = new();

        [MenuItem("Build/Build Android")]
        public static void BuildAndroid()
        {
            var buildPlayerOptions = new BuildPlayerOptions();

            var commandLineArgs = Environment.GetCommandLineArgs();
            _commandLineArguments = ParseCommandLineArguments(commandLineArgs);

            var scenesArgument = GetArgumentValue(_commandLineArguments, "-scenes");
            var scenes = scenesArgument.Split(',');

            ExecuteShell()
                .ContinueWith(BuildApk(buildPlayerOptions, scenes))
                .EmptySubscribe()
                .AddTo(BuildDisposable);

            BuildDisposable.Clear();
        }

        private static IObservable<Unit> BuildApk(BuildPlayerOptions buildPlayerOptions, string[] args)
        {
            buildPlayerOptions.scenes = args;

            buildPlayerOptions.locationPathName = "Builds/Android/Box.apk";
            buildPlayerOptions.target = BuildTarget.Android;

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            else if (summary.result == BuildResult.Failed) Debug.Log("Build failed");

            return Observable.ReturnUnit();
        }

        private static IObservable<Unit> ExecuteShell()
        {
            var projectFolderPath = Directory.GetCurrentDirectory();
            var bashFilePath = $"{projectFolderPath}/build_android.sh";

            var process = new Process();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = bashFilePath;

            process.Start();
            process.WaitForExit();

            if (process.ExitCode == 0)
                Debug.Log("Bash file executed successfully");
            else
                Debug.LogError("Failed to execute bash file");

            return Observable.ReturnUnit();
        }

        private static Dictionary<string, string> ParseCommandLineArguments(string[] commandLineArgs)
        {
            var argumentsDict = new Dictionary<string, string>();

            for (var i = 0; i < commandLineArgs.Length; i++)
            {
                var arg = commandLineArgs[i];

                if (arg.StartsWith("-"))
                {
                    if (i < commandLineArgs.Length - 1 && !commandLineArgs[i + 1].StartsWith("-"))
                    {
                        var value = commandLineArgs[i + 1];
                        argumentsDict[arg] = value;
                        i++;
                    }
                    else
                    {
                        argumentsDict[arg] = null;
                    }
                }
            }

            return argumentsDict;
        }

        private static string GetArgumentValue(Dictionary<string, string> argumentsDict, string flag)
        {
            if (argumentsDict.ContainsKey(flag)) return argumentsDict[flag];

            return null;
        }
    }
}