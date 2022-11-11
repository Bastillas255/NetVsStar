using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager : MonoBehaviour
{

    public void CreateText()
    {
        //Ruta que tendrá el archivo dentro del proyecto
        string path = Application.dataPath + "/Log.txt";

        //Crea el archivo si es que no existe
        if(!File.Exists(path))
        {
            File.WriteAllText(path, "Log \n\n");
        }

        //Contenido del archivo
        string content = "Login date: " + System.DateTime.Now + "\n";

        File.AppendAllText(path, content);
    }
    // Start is called before the first frame update
    void Start()
    {
        CreateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
