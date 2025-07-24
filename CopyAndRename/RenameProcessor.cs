using System;
using System.IO;

public class RenameProcessor
{
    // 檢查檔案是否存在
    public bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    // 重新命名單一檔案
    public void RenameFile(string filePath, string newFileName)
    {
        if (!FileExists(filePath))
        {
            Console.WriteLine($"檔案不存在：{filePath}");
            return;
        }
        string directory = Path.GetDirectoryName(filePath)!;
        string newFilePath = Path.Combine(directory, newFileName);
        try
        {
            File.Move(filePath, newFilePath, overwrite: true);
            Console.WriteLine($"檔案已重新命名：{newFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"重新命名失敗：{ex.Message}");
        }
    }

    // 確認檔案更名是否成功
    public bool IsRenameSuccessful(string oldFilePath, string newFileName)
    {
        string directory = Path.GetDirectoryName(oldFilePath)!;
        string newFilePath = Path.Combine(directory, newFileName);
        // 舊檔案不存在且新檔案存在，視為更名成功
        return !File.Exists(oldFilePath) && File.Exists(newFilePath);
    }

    // 批次重新命名檔案
    public void RenameFilesByList(string[] oldFilePaths, string[] newFileNames)
    {
        if (oldFilePaths.Length != newFileNames.Length)
        {
            Console.WriteLine("檔案數量與新檔名數量不一致，無法執行批次重新命名。");
            return;
        }
        for (int i = 0; i < oldFilePaths.Length; i++)
        {
            RenameFile(oldFilePaths[i], newFileNames[i]);
            if (IsRenameSuccessful(oldFilePaths[i], newFileNames[i]))
            {
                Console.WriteLine($"更名成功：{newFileNames[i]}");
            }
            else
            {
                Console.WriteLine($"更名失敗：{oldFilePaths[i]} -> {newFileNames[i]}");
            }
        }
    }

    // 取得今天的日期字串
    public string GetTodayString()
    {
        return DateTime.Now.ToString("yyyyMMdd");
    }
}
