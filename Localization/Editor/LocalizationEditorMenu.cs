using System;
using System.Diagnostics;
using System.IO;
using GamePackages.Localization.LocalizationWorkflow;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace GamePackages.Localization
{
    public static class EmptyStringPrinter
    {
        [MenuItem("SiberianWellness/Localization/Test2")]
        public static void Test2()
        {
            var      a    = Resources.Load<TextAsset>("Localization/Files/Locations");
            var      path = AssetDatabase.GetAssetPath(a);
            FileInfo f    = new FileInfo(path);
            Debug.Log(f.Extension);                 //".txt"
            Debug.Log(path);                        //"Assets\folder\file.txt";
            Debug.Log(Path.GetDirectoryName(path)); // "Assets\folder"

        }

        [MenuItem("SiberianWellness/Localization/Test")]
        public static void Test()
        {
            var allFiles = Workflow.LoadAllSelectionLocalizedFiles();

            foreach (var f in allFiles)
            {
                StringWriter w= new StringWriter();
                f.Write(w);
                Debug.Log(w.ToString());
            }
        }

        [MenuItem("SiberianWellness/Localization/RewriteAllLocalizedFiles")]
        public static void RewriteAllLocalizedFiles()
        { 
            
            Workflow.RewriteAllLocalizedFiles();
        } 
        
        [MenuItem("SiberianWellness/Localization/MergeSelectedFilesAndOriginFiles")]
        public static void MergeSelectedFilesAndOriginFiles()
        {    
            Workflow.MergeSelectedFilesAndOriginFiles();
        }

        [MenuItem("SiberianWellness/Localization/Save empty text blocks")]
        public static void SaveEmptyTextBlocks()
        { 
            var allFiles = Workflow.LoadAllLocalizedFilesInProject();
            Workflow.ExtractVoidTextBlocksAndSaveToFiles(allFiles);
        }
}

    public class LocalizationEditorMenu
    {
        [MenuItem("SiberianWellness/Localization/Reload ")]
        public static void Reload()
        {  
            Debug.Log("Reload"); 
            LocalizationFacade.Reload();

            Workflow.LoadAllLocalizedFilesInProject();//Вызывает проверку допустимых символов
        }
 
        [MenuItem("SiberianWellness/Localization/Ru ")]
        public static void Ru()
        {
            LocalizationSettings.SetLanguage(LocalizationSettings.Ru);
            Reload();


            ReDrawCanwas();
        }

        [MenuItem("SiberianWellness/Localization/En ")]
        public static void En()
        {
            LocalizationSettings.SetLanguage(LocalizationSettings.En);
            Reload();

            ReDrawCanwas();
        }

        [MenuItem("SiberianWellness/Localization/Cn ")]
        public static void Cn()
        {
            LocalizationSettings.SetLanguage(LocalizationSettings.Cn);
            Reload();
            
            ReDrawCanwas();
        }
        
        [MenuItem("SiberianWellness/Localization/GetFilesFromServer ")]
        public static void GetFilesFromServer()
        {
//            WebClient myWebClient = new WebClient();
//            string path = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Resources", "Localization", "Files");
//            Debug.Log(path);
//            myWebClient.DownloadFile("http://192.168.3.11/gameplay.csv", Path.Combine(path, "gameplay.csv"));

            string localizationFolder = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Resources", "Localization", "Files");
            string appPath = Path.Combine(Directory.GetCurrentDirectory(), "GetLocalizationApp", "GoogleSheetsQuickstart", "bin", "Debug");
            Debug.Log(" <color=yellow> Скачивание файлов </color> ");
            
            Process process = new Process();
            process.StartInfo.FileName               = Path.Combine(appPath, "GoogleSheetsQuickstart.exe");
            process.StartInfo.UseShellExecute        = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError  = true;
            process.StartInfo.WorkingDirectory = appPath;
            
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            Debug.Log("Консольный вывод приложения:"+ Environment.NewLine + output);
            string err = process.StandardError.ReadToEnd();
            Debug.Log("Ошибки: " + err);
            process.WaitForExit();

            string[] pathsFiles = File.ReadAllLines(Path.Combine(appPath, "Localization", "ListPaths.txt"));

            foreach (string pathInApp in pathsFiles)
            {
                char[] charArr = pathInApp.Trim().ToCharArray();
                string folderName = "";
                string fileName = "";
                for (int i = charArr.Length - 1; i > 0; i--)
                {
                    if (charArr[i] == Path.DirectorySeparatorChar)
                    {
                        for (int j = i + 1; j < charArr.Length; j++)
                        {
                            fileName += charArr[j];
                        }

                        i--;
                        while (charArr[i] != Path.DirectorySeparatorChar)
                        {
                            folderName = folderName.Insert(0, charArr[i].ToString());
                            i--;
                        }
                        break;
                    }
                }
                Debug.Log("folder: " + folderName + " Name: " + fileName);
                
                string fullPathInUnity = Path.Combine(localizationFolder, folderName);
                if (!File.Exists(fullPathInUnity))
                {
                    Directory.CreateDirectory(fullPathInUnity);
                }
                fullPathInUnity = Path.Combine(fullPathInUnity, fileName);
                if (File.Exists(fullPathInUnity))
                {
                    File.Delete(fullPathInUnity);
                }
                File.Move(pathInApp, fullPathInUnity);
            }
            
            //AssetStoreToolUtils.UpdatePreloadingInternal();
            LocalizationFacade.Reload(); 
            Debug.Log(" <color=green> Скачивание файлов завершено </color> ");
        }

        static void ReDrawCanwas()
        {
            var canvasRoots = GameObject.FindObjectsOfType<Canvas>();
            foreach (var c in canvasRoots)
            {
                c.gameObject.SetActive(false);
                c.gameObject.SetActive(true);
            }
        }
    }
}