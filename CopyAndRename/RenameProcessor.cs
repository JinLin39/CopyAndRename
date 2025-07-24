using System;
using System.IO;

public class RenameProcessor
{
    // �ˬd�ɮ׬O�_�s�b
    public bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    // ���s�R�W��@�ɮ�
    public void RenameFile(string filePath, string newFileName)
    {
        if (!FileExists(filePath))
        {
            Console.WriteLine($"�ɮפ��s�b�G{filePath}");
            return;
        }
        string directory = Path.GetDirectoryName(filePath)!;
        string newFilePath = Path.Combine(directory, newFileName);
        try
        {
            File.Move(filePath, newFilePath, overwrite: true);
            Console.WriteLine($"�ɮפw���s�R�W�G{newFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"���s�R�W���ѡG{ex.Message}");
        }
    }

    // �T�{�ɮק�W�O�_���\
    public bool IsRenameSuccessful(string oldFilePath, string newFileName)
    {
        string directory = Path.GetDirectoryName(oldFilePath)!;
        string newFilePath = Path.Combine(directory, newFileName);
        // ���ɮפ��s�b�B�s�ɮצs�b�A������W���\
        return !File.Exists(oldFilePath) && File.Exists(newFilePath);
    }

    // �妸���s�R�W�ɮ�
    public void RenameFilesByList(string[] oldFilePaths, string[] newFileNames)
    {
        if (oldFilePaths.Length != newFileNames.Length)
        {
            Console.WriteLine("�ɮ׼ƶq�P�s�ɦW�ƶq���@�P�A�L�k����妸���s�R�W�C");
            return;
        }
        for (int i = 0; i < oldFilePaths.Length; i++)
        {
            RenameFile(oldFilePaths[i], newFileNames[i]);
            if (IsRenameSuccessful(oldFilePaths[i], newFileNames[i]))
            {
                Console.WriteLine($"��W���\�G{newFileNames[i]}");
            }
            else
            {
                Console.WriteLine($"��W���ѡG{oldFilePaths[i]} -> {newFileNames[i]}");
            }
        }
    }

    // ���o���Ѫ�����r��
    public string GetTodayString()
    {
        return DateTime.Now.ToString("yyyyMMdd");
    }
}
