using System;
using System.IO;
using System.Linq;

try
{
    AppDomain();
}
catch (Exception e)
{
    Console.WriteLine($"檔案執行失敗：{e.Message}");
}

void AppDomain()
{
    var renameProcessor = new RenameProcessor();
    var processor = new CopyFileProcessor();
    // 複製檔案
    processor.CopyAndRenameFiles(
        new[] { @"d:\temp\FATA\temp01" },
        new[] { @"d:\temp\FATA\Back99", @"d:\temp\FATA\Back98" },
        new[] { "OD*.csv", "Sample*.csv" }
    // 可選：, (f, idx) => $"MyName_{idx + 1}{Path.GetExtension(f)}"
    );
    // 取得要更名的檔案清單
    string[] oldFilePaths = Directory.GetFiles(@"d:\temp\FATA\Back99", "OD*.csv");
    // 產生新檔名（範例：加上 "_Renamed" 前綴，數量需與 oldFilePaths 相同）
    string[] newFileNames = oldFilePaths
        .Select((f, idx) => $"Renamed_{idx + 1}{Path.GetExtension(f)}")
        .ToArray();
    // 執行批次重新命名
    renameProcessor.RenameFilesByList(oldFilePaths, newFileNames);
}
