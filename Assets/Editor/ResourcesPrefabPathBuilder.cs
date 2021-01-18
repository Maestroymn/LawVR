#if UNITY_EDITOR
using Managers;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Editor
{
    public class ResourcesPrefabPathBuilder : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0;} }
        
        public void OnPreprocessBuild(BuildReport report)
        {
            GameManager.PopulateNetworkedPrefabs();
        }
    }
}
#endif
