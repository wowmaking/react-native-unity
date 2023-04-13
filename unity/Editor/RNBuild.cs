using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Wowmaking.RNU.Editor
{
    public class RNBuild
    {
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
