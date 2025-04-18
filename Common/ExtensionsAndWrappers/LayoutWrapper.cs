using UnityEngine;
using UnityEngine.UI;

namespace GamePackages.Core
{
    public static class LayoutWrapper
    {
        public static void RefreshLayoutGroups(Transform root)
        {
            foreach (var layoutGroup in root.GetComponentsInChildren<LayoutGroup>())
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            }
        }
    }
}
