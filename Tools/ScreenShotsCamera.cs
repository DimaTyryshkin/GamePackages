using System.IO;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePackages.Tools
{
    public class ScreenShotsCamera : MonoBehaviour
    {
#if UNITY_EDITOR
        public enum Format
        {
            PNG = 0,
            JPG = 1,
        }

        [SerializeField] int width = 1920;
        [SerializeField] int height = 1080;
        [SerializeField] Format format;

        string lastFile
        {
            get => EditorPrefs.GetString("ScreenShotsCamera.lastFile", null);
            set => EditorPrefs.SetString("ScreenShotsCamera.lastFile", value);
        }

        FileInfo LastFile => string.IsNullOrWhiteSpace(lastFile) ? null : new FileInfo(lastFile);

        [Button]
        public void TakeScreenShoot()
        {
            TakeScreenShoot(GetComponent<Camera>(), width, height);
        }

        [Button]
        public void ShowInExplorer()
        {
            if (LastFile == null)
            {
                Debug.LogError("Путь не найден");
            }
            else
            {
                EditorUtility.RevealInFinder(LastFile.FullName);
                //System.Diagnostics.Process.Start(LastFile.FullName);
            }
        }

        [Button]
        public void SetFullHd()
        {
            Undo.RecordObject(this, "change");
            width = 1920;
            height = 1080;
        }

        [Button]
        public void Set4K()
        {
            Undo.RecordObject(this, "change");
            width = 3840;
            height = 2160;
        }

        public void TakeScreenShoot(Camera camera, int resWidth, int resHeight)
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            camera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            DestroyImmediate(rt);

            byte[] bytes = format == Format.PNG ? screenShot.EncodeToPNG() : screenShot.EncodeToJPG();

            string path = Application.dataPath;
            if (LastFile != null)
                path = LastFile.Directory.FullName;

            string fileName = System.DateTime.Now.Ticks.ToString();
            if (LastFile != null)
            {
                string lastFileName = Path.GetFileNameWithoutExtension(LastFile.Name);
                if (int.TryParse(lastFileName, out int oldValue))
                {
                    fileName = (oldValue + 1).ToString();
                }
            }

            string filename = EditorUtility.SaveFilePanel("Save screen shoot", path, fileName, format == Format.PNG ? "png" : "jpg");
            if (!string.IsNullOrWhiteSpace(filename))
            {
                Undo.RecordObject(this, "TakeScreenShoot");
                lastFile = filename;
                File.WriteAllBytes(filename, bytes);
            }
        }
#endif
    }
}