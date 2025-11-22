/*

using UnityEditor;

namespace GamePackages.Core
{
public class MyPostProcessor : AssetPostprocessor
{
   private static void OnPostprocessAllAssets(
      string[] importedAssets,
      string[] deletedAssets,
      string[] movedAssets,
      string[] movedFromAssetPaths)
   {
       FixBug(movedAssets);
       FixBug(importedAssets);
       FixBug(movedFromAssetPaths);
   }

   static void FixBug(string[] assets)
   {
       foreach (string asset in assets)
       {

           if (!asset.EndsWith(".cs"))
               continue;

           if (asset.EndsWith($"{nameof(MyPostProcessor)}.cs"))
               continue;

           Debug.Log(asset);
           string code = File.ReadAllText(asset);

           Regex r = new Regex(@"\s*[SerializeField] bool \w*;");
           foreach (Match m in r.Matches(code))
           {
               Debug.Log(m.Value);
           }


           //string key = "#region generated";
           //code = code.Replace(key, key + Environment.NewLine + "// Привет");
           //File.WriteAllText(asset, code);
       }
   }
}
}
   */