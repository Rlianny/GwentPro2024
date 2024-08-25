﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Dynamic;
using System.IO;
using System.Linq;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public static class CardsCreator
{
    /// <summary>
    /// Este método toma un contenedor de archivos de tipo txt y lee su información distribuyéndola en una lista.
    /// </summary>
    /// <param name="path">Ruta de la carpeta que contiene los archivos de tipo txt.</param>
    /// <returns>Devuelve una lista que contine toda la información de cada carta en un array.</returns>
    public static List<string[]> GetCardInfoList(string path)
    {
        TextAsset[] textFiles = Resources.LoadAll<TextAsset>(path);

        List<string[]> CardInfoList = new List<string[]>();

        foreach (TextAsset text in textFiles)
        {
            Debug.Log(text.text);
            CardInfoList.Add(GetCardInfoArray(text));
        }

        return CardInfoList;
    }

    public static List<CompiledObject> LoadAll(string path)
    {
        List<CompiledObject> objects = new();

        foreach (string file in Directory.GetFiles(path, "*.bin"))
        {
            CompiledObject obj = FileFormatter.Load(file);
            if (obj is CompiledCard) objects.Add(obj);

            else if (obj is CompiledEffect compiledEffect)
                Effect.RegisterEffect(compiledEffect);
        }

        return objects;
    }

    private static string[] GetCardInfoArray(TextAsset text)
    {
        string[] cardInfo = text.text.Split('\n');
        return cardInfo;
    }
}
