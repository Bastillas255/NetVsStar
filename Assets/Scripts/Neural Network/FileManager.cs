using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager
{
    private string path = Application.dataPath + "/";
    public void WriteToFile(string fileName, string data)
    {
        //path = Application.dataPath + "/" + fileName;
        int i = 1;
        string fileNumberedName = fileName + i + ".txt";
        while(File.Exists(Application.dataPath + "/" + fileNumberedName))
        {
            i++;
            fileNumberedName = fileName + i + ".txt";
        }
        path = Application.dataPath + "/" + fileNumberedName;
        File.WriteAllText(path, data);
    }

    public void AddToFile(string fileName, string data)
    {
        path = Application.dataPath + "/" + fileName;
        if(!File.Exists(path))
        {
            File.Create(path).Dispose();
        }
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(data);
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

    public List<string> GetListOfLines(string fileName)
    {
        string filePath = Application.dataPath + "/" + fileName;
        if(File.Exists(filePath))
        {
            List<string> lines = new List<string>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string linea;
                while((linea = reader.ReadLine()) != null)
                {
                    lines.Add(linea);
                }
            }
            return lines;
        }
        else
        {
            List<string> empty = new List<string>();
            return empty;
        }
    }
}