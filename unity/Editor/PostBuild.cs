#if UNITY_EDITOR && UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Wowmaking.RNU.Editor
{

    public class PostBuild
    {

        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            var projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            var project = new PBXProject();

            project.ReadFromFile(projectPath);

            var targetId = project.GetUnityFrameworkTargetGuid();

            project.AddFileToBuildSection(
                targetId,
                project.GetResourcesBuildPhaseByTarget(targetId),
                project.FindFileGuidByRealPath("Data")
            );

            project.AddFrameworkToProject(targetId, "RNUProxy.framework", false);

            project.WriteToFile(projectPath);
        }

    }

}
#endif
