using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

public class UnzipLargeAssets : EditorWindow
{
    private float progress = 0f;
    private bool isExtracting = false;

    [MenuItem("Zeksters Lab/Unzip Large Assets")]
    public static void UnzipAssets()
    {
        string zipFilePath = Application.dataPath + "/LargeAssets0000.zip";
        string extractPath = Application.dataPath;
        UnzipLargeAssets window = GetWindow<UnzipLargeAssets>();
        window.Show();
        window.isExtracting = true;
        EditorApplication.update += window.UpdateProgress;
        using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
        {
            float totalSize = 0f;
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                totalSize += entry.Length;
            }
            float currentSize = 0f;
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string fullPath = Path.Combine(extractPath, entry.FullName);
                if (entry.Length == 0)
                {
                    Directory.CreateDirectory(fullPath);
                }
                else
                {
                    entry.ExtractToFile(fullPath, true);
                    currentSize += entry.Length;
                    window.progress = Mathf.Clamp01(currentSize / totalSize);
                }
            }
        }
        EditorApplication.update -= window.UpdateProgress;
        window.isExtracting = false;
        window.Close();

        // Refresh the project window to show the extracted files
        AssetDatabase.Refresh();
    }

    private void UpdateProgress()
    {
        if (isExtracting)
        {
            Repaint();
        }
    }

    private void OnGUI()
    {
        if (isExtracting)
        {
            GUILayout.Label("Unzipping Large Assets...");
            EditorGUI.ProgressBar(new Rect(0, 30, position.width, 20), progress, "");
        }
        else
        {
            GUILayout.Label("Large Assets are already unzipped.");
        }
    }

    //[InitializeOnLoadMethod]
    //private static void CheckIfUnzipped()
    //{
    //    string extractPath = Application.dataPath;
    //    string[] files = Directory.GetFiles(extractPath, "*", SearchOption.AllDirectories);
    //    bool isUnzipped = false;
    //    foreach (string file in files)
    //    {
    //        if (file.EndsWith(".zip") && file.Contains("LargeAssets"))
    //        {
    //            isUnzipped = true;
    //            break;
    //        }
    //    }

    //    if (!isUnzipped)
    //    {
    //        UnzipAssets();
    //    }
    //}
}
