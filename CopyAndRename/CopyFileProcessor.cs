using System;
using System.IO;
public class CopyFileProcessor
{
    // 複製並重新命名檔案
    public void CopyAndRenameFiles(
        string[] sourcePaths,
        string[] destinationPaths,
        string[] searchPatterns,
        // 可選：重新命名規則，接受檔案名稱和索引，返回新的檔案名稱
        Func<string, int, string>? renameRule = null)
    {
        foreach (var sourcePath in sourcePaths)
        {
            foreach (var pattern in searchPatterns)
            {
                // 檢查來源目錄是否有符合模式的檔案
                if (!HasFiles(sourcePath, pattern))
                    continue;
                // 檢查目的地目錄是否存在，若不存在則建立
                foreach (var destinationPath in destinationPaths)
                {
                    string[] sourceFilePaths = Directory.GetFiles(sourcePath, pattern);
                    // 檢查來源目錄是否有符合模式的檔案
                    if (sourceFilePaths.Length == 0)
                    {
                        Console.WriteLine($"找不到符合 {pattern} 的檔案於 {sourcePath}");
                        continue;
                    }
                    CopyFilesWithPattern(sourcePath, pattern, destinationPath);
                    VerifyCopiedFiles(sourceFilePaths, destinationPath);
                }
            }
        }
        // 取得符合模式的檔案並產生新檔名
        string[] matchedFilePaths = GetMatchedFiles(destinationPaths[0], searchPatterns);
        // 檢查是否有符合模式的檔案
        string[] newFileNames = matchedFilePaths
            .Select((f, idx) => renameRule != null
                ? renameRule(f, idx)
                : $"自訂名稱_{Path.GetFileNameWithoutExtension(f)}{Path.GetExtension(f)}")
            .ToArray();
        // 檢查目的地是否有全部新檔名的檔案
        foreach (var destinationPath in destinationPaths)
        {
            // 檢查目的地目錄是否存在
            if (!HasAllFiles(destinationPath, newFileNames))
            {
                Console.WriteLine($"目的地 {destinationPath} 沒有全部新檔名的檔案");
            }
            else
            {
                Console.WriteLine($"目的地 {destinationPath} 有全部新檔名的檔案");
            }
        }
    }
    // 取得符合模式的檔案
    public string[] GetMatchedFiles(string directory, string[] patterns)
    {
        // 檢查目錄是否存在
        if (!Directory.Exists(directory))
        {
            Console.WriteLine($"來源目錄不存在：{directory}");
            return Array.Empty<string>();
        }
        // 確保模式不為空
        var files = patterns
            .SelectMany(pattern => Directory.GetFiles(directory, pattern))
            .Distinct()
            .ToArray();

        return files;
    }
    // 複製符合模式的檔案到目的地
    public void CopyFilesWithPattern(string sourcePath, string pattern, string destinationPath)
    {
        // 檢查來源目錄是否存在
        string[] sourceFilePaths = Directory.GetFiles(sourcePath, pattern);
        //  檢查目標目錄是否有建立
        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }
        // 檢查來源目錄是否有符合模式的檔案
        for (int i = 0; i < sourceFilePaths.Length; i++)
        {
            // 取得來源檔案名稱並建立新的目的地檔案名稱
            string sourceFileName = Path.GetFileNameWithoutExtension(sourceFilePaths[i]);
            // 產生新的目的地檔案名稱，格式為 "原檔名_YYYYMMDD.csv"
            string destinationFileName = $"{sourceFileName}_{GetTodayString()}.csv";
            // 目的地檔案完整路徑
            string destinationFilePath = Path.Combine(destinationPath, destinationFileName);
            // 檢查目的地是否已存在同名檔案
            try
            {
                File.Copy(sourceFilePaths[i], destinationFilePath, overwrite: true);
                Console.WriteLine($"檔案已成功複製到：{destinationFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"複製失敗：{ex.Message}");
            }
        }
    }
    // 檢查目錄是否有符合模式的檔案
    public bool HasFiles(string path, string pattern)
    {
        // 檢查來源目錄是否存在
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"來源目錄不存在：{path}");
            return false;
        }
        // 確保模式不為空
        string[] files = Directory.GetFiles(path, pattern);
        if (files.Length == 0)
        {
            Console.WriteLine($"找不到符合 {pattern} 的檔案於 {path}");
            return false;
        }
        return true;
    }
    // 檢查目錄是否包含所有指定的檔案
    public bool HasAllFiles(string directory, string[] fileNames)
    {
        // 檢查目錄是否存在
        foreach (var fileName in fileNames)
        {
            // 檢查檔案名稱是否為空
            string filePath = Path.Combine(directory, fileName);
            if (!File.Exists(filePath))
            {
                return false;
            }
        }
        return true;
    }
    //  驗證複製的檔案是否存在且大小一致
    public void VerifyCopiedFiles(string[] sourceFilePaths, string destinationPath)
    {
        // 檢查目的地目錄是否存在
        foreach (var sourceFilePath in sourceFilePaths)
        {
            // 取得來源檔案名稱並建立新的目的地檔案名稱
            string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            // 產生新的目的地檔案名稱，格式為 "原檔名_YYYYMMDD.csv"
            string destinationFileName = $"{fileName}_{GetTodayString()}.csv";
            // 目的地檔案完整路徑
            string destinationFilePath = Path.Combine(destinationPath, destinationFileName);
            // 檢查目的地是否存在同名檔案
            if (!File.Exists(destinationFilePath))
            {
                Console.WriteLine($"檔案不存在於目的地：{destinationFilePath}");
            }
            else
            {
                // 檢查檔案大小是否一致
                long sourceLen = new FileInfo(sourceFilePath).Length;
                // 取得目的地檔案大小
                long destLen = new FileInfo(destinationFilePath).Length;
                // 比較來源檔案和目的地檔案的大小
                if (sourceLen != destLen)
                {
                    Console.WriteLine($"檔案大小不一致：{destinationFilePath}");
                }
                else
                {
                    Console.WriteLine($"檔案驗證成功：{destinationFilePath}");
                }
            }
        }
    }
    // 取得今天的日期字串
    public string GetTodayString()
    {
        return DateTime.Now.ToString("yyyyMMdd");
    }
}
