using UnityEditor;
using UnityEditor.Callbacks;

namespace Wowmaking.RNU.Editor
{
    public class RNBuild
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target == BuildTarget.iOS) 
            {
#if UNITY_IOS
                var projectPath = UnityEditor.iOS.Xcode.PBXProject.GetPBXProjectPath(pathToBuiltProject);
                var project = new UnityEditor.iOS.Xcode.PBXProject();
            
                project.ReadFromFile(projectPath);
                var targetId = project.GetUnityFrameworkTargetGuid();

                project.AddFileToBuildSection(
                    targetId,
                    project.GetResourcesBuildPhaseByTarget(targetId),
                    project.FindFileGuidByRealPath("Data")
                );
            
                project.WriteToFile(projectPath);
#endif
            }
        }
    }

}