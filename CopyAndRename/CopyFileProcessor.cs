using System;
using System.IO;
public class CopyFileProcessor
{
    // �ƻs�í��s�R�W�ɮ�
    public void CopyAndRenameFiles(
        string[] sourcePaths,
        string[] destinationPaths,
        string[] searchPatterns,
        // �i��G���s�R�W�W�h�A�����ɮצW�٩M���ޡA��^�s���ɮצW��
        Func<string, int, string>? renameRule = null)
    {
        foreach (var sourcePath in sourcePaths)
        {
            foreach (var pattern in searchPatterns)
            {
                // �ˬd�ӷ��ؿ��O�_���ŦX�Ҧ����ɮ�
                if (!HasFiles(sourcePath, pattern))
                    continue;
                // �ˬd�ت��a�ؿ��O�_�s�b�A�Y���s�b�h�إ�
                foreach (var destinationPath in destinationPaths)
                {
                    string[] sourceFilePaths = Directory.GetFiles(sourcePath, pattern);
                    // �ˬd�ӷ��ؿ��O�_���ŦX�Ҧ����ɮ�
                    if (sourceFilePaths.Length == 0)
                    {
                        Console.WriteLine($"�䤣��ŦX {pattern} ���ɮש� {sourcePath}");
                        continue;
                    }
                    CopyFilesWithPattern(sourcePath, pattern, destinationPath);
                    VerifyCopiedFiles(sourceFilePaths, destinationPath);
                }
            }
        }
        // ���o�ŦX�Ҧ����ɮרò��ͷs�ɦW
        string[] matchedFilePaths = GetMatchedFiles(destinationPaths[0], searchPatterns);
        // �ˬd�O�_���ŦX�Ҧ����ɮ�
        string[] newFileNames = matchedFilePaths
            .Select((f, idx) => renameRule != null
                ? renameRule(f, idx)
                : $"�ۭq�W��_{Path.GetFileNameWithoutExtension(f)}{Path.GetExtension(f)}")
            .ToArray();
        // �ˬd�ت��a�O�_�������s�ɦW���ɮ�
        foreach (var destinationPath in destinationPaths)
        {
            // �ˬd�ت��a�ؿ��O�_�s�b
            if (!HasAllFiles(destinationPath, newFileNames))
            {
                Console.WriteLine($"�ت��a {destinationPath} �S�������s�ɦW���ɮ�");
            }
            else
            {
                Console.WriteLine($"�ت��a {destinationPath} �������s�ɦW���ɮ�");
            }
        }
    }
    // ���o�ŦX�Ҧ����ɮ�
    public string[] GetMatchedFiles(string directory, string[] patterns)
    {
        // �ˬd�ؿ��O�_�s�b
        if (!Directory.Exists(directory))
        {
            Console.WriteLine($"�ӷ��ؿ����s�b�G{directory}");
            return Array.Empty<string>();
        }
        // �T�O�Ҧ�������
        var files = patterns
            .SelectMany(pattern => Directory.GetFiles(directory, pattern))
            .Distinct()
            .ToArray();

        return files;
    }
    // �ƻs�ŦX�Ҧ����ɮר�ت��a
    public void CopyFilesWithPattern(string sourcePath, string pattern, string destinationPath)
    {
        // �ˬd�ӷ��ؿ��O�_�s�b
        string[] sourceFilePaths = Directory.GetFiles(sourcePath, pattern);
        //  �ˬd�ؼХؿ��O�_���إ�
        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }
        // �ˬd�ӷ��ؿ��O�_���ŦX�Ҧ����ɮ�
        for (int i = 0; i < sourceFilePaths.Length; i++)
        {
            // ���o�ӷ��ɮצW�٨ëإ߷s���ت��a�ɮצW��
            string sourceFileName = Path.GetFileNameWithoutExtension(sourceFilePaths[i]);
            // ���ͷs���ت��a�ɮצW�١A�榡�� "���ɦW_YYYYMMDD.csv"
            string destinationFileName = $"{sourceFileName}_{GetTodayString()}.csv";
            // �ت��a�ɮק�����|
            string destinationFilePath = Path.Combine(destinationPath, destinationFileName);
            // �ˬd�ت��a�O�_�w�s�b�P�W�ɮ�
            try
            {
                File.Copy(sourceFilePaths[i], destinationFilePath, overwrite: true);
                Console.WriteLine($"�ɮפw���\�ƻs��G{destinationFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"�ƻs���ѡG{ex.Message}");
            }
        }
    }
    // �ˬd�ؿ��O�_���ŦX�Ҧ����ɮ�
    public bool HasFiles(string path, string pattern)
    {
        // �ˬd�ӷ��ؿ��O�_�s�b
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"�ӷ��ؿ����s�b�G{path}");
            return false;
        }
        // �T�O�Ҧ�������
        string[] files = Directory.GetFiles(path, pattern);
        if (files.Length == 0)
        {
            Console.WriteLine($"�䤣��ŦX {pattern} ���ɮש� {path}");
            return false;
        }
        return true;
    }
    // �ˬd�ؿ��O�_�]�t�Ҧ����w���ɮ�
    public bool HasAllFiles(string directory, string[] fileNames)
    {
        // �ˬd�ؿ��O�_�s�b
        foreach (var fileName in fileNames)
        {
            // �ˬd�ɮצW�٬O�_����
            string filePath = Path.Combine(directory, fileName);
            if (!File.Exists(filePath))
            {
                return false;
            }
        }
        return true;
    }
    //  ���ҽƻs���ɮ׬O�_�s�b�B�j�p�@�P
    public void VerifyCopiedFiles(string[] sourceFilePaths, string destinationPath)
    {
        // �ˬd�ت��a�ؿ��O�_�s�b
        foreach (var sourceFilePath in sourceFilePaths)
        {
            // ���o�ӷ��ɮצW�٨ëإ߷s���ت��a�ɮצW��
            string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            // ���ͷs���ت��a�ɮצW�١A�榡�� "���ɦW_YYYYMMDD.csv"
            string destinationFileName = $"{fileName}_{GetTodayString()}.csv";
            // �ت��a�ɮק�����|
            string destinationFilePath = Path.Combine(destinationPath, destinationFileName);
            // �ˬd�ت��a�O�_�s�b�P�W�ɮ�
            if (!File.Exists(destinationFilePath))
            {
                Console.WriteLine($"�ɮפ��s�b��ت��a�G{destinationFilePath}");
            }
            else
            {
                // �ˬd�ɮפj�p�O�_�@�P
                long sourceLen = new FileInfo(sourceFilePath).Length;
                // ���o�ت��a�ɮפj�p
                long destLen = new FileInfo(destinationFilePath).Length;
                // ����ӷ��ɮשM�ت��a�ɮת��j�p
                if (sourceLen != destLen)
                {
                    Console.WriteLine($"�ɮפj�p���@�P�G{destinationFilePath}");
                }
                else
                {
                    Console.WriteLine($"�ɮ����Ҧ��\�G{destinationFilePath}");
                }
            }
        }
    }
    // ���o���Ѫ�����r��
    public string GetTodayString()
    {
        return DateTime.Now.ToString("yyyyMMdd");
    }
}
