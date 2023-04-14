using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Wowmaking.RNU.Editor
{
    static class RNBuild
    {
        [MenuItem("ReactNative/Export Android", false, 1)]
        public static void PerformAndroidBuild()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

            string buildPath = "./Builds/android";

            BuildPipeline.BuildPlayer(
                GetEnabledScenes(),
                buildPath,
                BuildTarget.Android,
                BuildOptions.None);

            Debug.Log("Build Success (Android)");
        }

        [MenuItem("ReactNative/Export iOS (Device)", false, 2)]
        public static void PerformIOSBuildForDevice()
        {
            PerformIOSBuild(iOSSdkVersion.DeviceSDK);
            Debug.Log("Build Success (iOS Device)");
        }

        [MenuItem("ReactNative/Export iOS (Simulator)", false, 3)]
        public static void PerformIOSBuildForSimulator()
        {
            PerformIOSBuild(iOSSdkVersion.SimulatorSDK);
            Debug.Log("Build Success (iOS Simulator)");
        }

        /* Builds iOS project */
        static void PerformIOSBuild(iOSSdkVersion sdkVersion = iOSSdkVersion.DeviceSDK)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            PlayerSettings.iOS.sdkVersion = sdkVersion;

            string buildPath = "./Builds/ios";

            switch (sdkVersion)
            {
                case iOSSdkVersion.DeviceSDK:
                    buildPath = "./Builds/ios_device";
                    break;
                case iOSSdkVersion.SimulatorSDK:
                    buildPath = "./Builds/ios_simulator";
                    break;
            }

            BuildPipeline.BuildPlayer(
                GetEnabledScenes(),
                buildPath,
                BuildTarget.iOS,
                BuildOptions.CompressWithLz4HC);

        }

        /* Returns project scenes list */
        static string[] GetEnabledScenes()
        {
            return (
                from scene in EditorBuildSettings.scenes
                where scene.enabled
                where !string.IsNullOrEmpty(scene.path)
                select scene.path
            ).ToArray();
        }

        /* Modifies exported projects */
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            /* iOS project modification */
            if (target == BuildTarget.iOS)
            {
                string projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
                PBXProject project = new PBXProject();
                project.ReadFromFile(projectPath);

                string mainTargetGuid = project.GetUnityMainTargetGuid();
                string unityFrameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
                string dataFolderGuid = project.FindFileGuidByRealPath("Data");

                /* Detach Data folder from Unity-iPhone target */
                project.RemoveFileFromBuild(
                    mainTargetGuid,
                    dataFolderGuid
                );

                /* Attach Data folder to UnityFramework target */
                project.AddFileToBuildSection(
                    unityFrameworkTargetGuid,
                    project.GetResourcesBuildPhaseByTarget(unityFrameworkTargetGuid),
                    dataFolderGuid
                );

                project.WriteToFile(projectPath);
            }

            /* Android project modification */
            if (target == BuildTarget.Android)
            {
                /* Fix ndk path in build.gradle */
                string gradleFile = Path.Combine(pathToBuiltProject, "unityLibrary/build.gradle");
                string gradleText = File.ReadAllText(gradleFile);
                gradleText = gradleText.Replace("android.ndkDirectory", "\"" + AndroidExternalToolsSettings.ndkRootPath + "\"");
                File.WriteAllText(gradleFile, gradleText);

                /* Delete intent from AndroidManifest.xml */
                string manifestFile = Path.Combine(pathToBuiltProject, "unityLibrary/src/main/AndroidManifest.xml");
                string manifestText = File.ReadAllText(manifestFile);
                manifestText = Regex.Replace(manifestText, @"<application .*>", "<application>");
                Regex regex = new Regex(@"<activity.*>(\s|\S)+?</activity>", RegexOptions.Multiline);
                manifestText = regex.Replace(manifestText, "");
                File.WriteAllText(manifestFile, manifestText);
            }
        }
    }
}
