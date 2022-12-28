using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager
{
    private string path = Application.dataPath + "/";
    public void WriteToFile(string fileName, string data)
    {
        path = Application.dataPath + "/" + fileName;
        File.WriteAllText(path, data);
    }

    public void AddToFile(string fileName, string data)
    {
        path = Application.dataPath + "/" + fileName;
        if(File.Exists(path))
        {
            File.AppendAllText(path, data);
        }
        else
        {
            WriteToFile(fileName, data);
        }
    }

    //Crear función que lea línea por línea
    public string ReadFile(string fileName)
    {
        string filePath = Application.dataPath + "/" + fileName;
        if(File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string contenido = reader.ReadToEnd();
                return contenido;
            }
        }
        else
        {
            Debug.Log("File not found");
            return "File not found";
        }
    }

    public string ReadFileByLine(string fileName)
    {
        string filePath = Application.dataPath + "/" + fileName;
        if(File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string contenido = reader.ReadLine();
                return contenido;
            }
        }
        else
        {
            Debug.Log("File not found");
            return "File not found";
        }
    }
}