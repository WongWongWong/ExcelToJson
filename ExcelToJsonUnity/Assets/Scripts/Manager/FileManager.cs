using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{
    static FileManager _ins = new FileManager();
    public static FileManager Ins { get { return _ins; } }
    private FileManager() { }

    /// <summary>
    /// 生成文件
    /// </summary>
    /// <param name="json"></param>
    /// <param name="exprotPath"></param>
    public void ExportFile(string context, string exprotPath)
    {
        if (File.Exists(exprotPath))
        {
            File.Delete(exprotPath);
        }
        FileStream fileStream = new FileStream(exprotPath, FileMode.CreateNew);
        StreamWriter streamWriter = new StreamWriter(fileStream);
        streamWriter.Write(context);
        streamWriter.Close(); //此处有坑。。。。要注意安全
        fileStream.Close();
    }

    /// <summary>
    /// 创建路径
    /// </summary>
    /// <param name="dirPath"></param>
    /// <returns></returns>
    public void CreateDirPath(string dirPath)
    {
        DirectoryInfo mydir = new DirectoryInfo(dirPath);
        if (mydir.Exists)
        {
            return;
        }

        mydir.Create();
    }

    /// <summary>
    /// 获得文件流数据
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public FileStream GetFileStream(string path)
    {
        FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return fileStream;
    }

    /// <summary>
    /// 获得文件夹下的所有.xlsx文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public List<string> GetExcelFilePathList(string path)
    {
        List<string> ret = new List<string>();

        DirectoryInfo direction = new DirectoryInfo(path);
        //文件夹下一层的所有子文件
        //SearchOption.TopDirectoryOnly：这个选项只取下一层的子文件
        //SearchOption.AllDirectories：这个选项会取其下所有的子文件
        FileInfo[] files = direction.GetFiles("*", SearchOption.TopDirectoryOnly);
        //文件夹下一层的所有文件夹
        DirectoryInfo[] folders = direction.GetDirectories("*", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < folders.Length; i++)
        {
            //folders[i].FullName：硬盘上的完整路径名称
            //folders[i].Name：文件夹名称
            var folder = folders[i];
            var list = GetExcelFilePathList(folder.FullName);
            ret.AddRange(list);
        }
        for (int i = 0; i < files.Length; i++)
        {
            //files[i].FullName：硬盘上的完整路径名称，包括文件名(D:\Project\Test\Assets\Scripts\Font\Test.cs)
            //files[i].Name：文件名称 Test.cs"
            //files[i].DirectoryName：文件的存放路径(D:\UnityProject\Test\Assets\Scripts\Font\)
            var file = files[i];
            if (file.Name[0] != '~' && file.Name.EndsWith(".xlsx"))//取脚本文件
            {
                ret.Add(file.FullName);
            }
        }

        return ret;
    }
}
