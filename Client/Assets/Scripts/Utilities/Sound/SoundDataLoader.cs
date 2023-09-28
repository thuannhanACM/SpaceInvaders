using Core.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SoundDataLoader
{
    private Dictionary<string, string> SoundKey = new Dictionary<string, string>();
    private const string columnKey = "NAME";
    private const string soundRootPath = "Assets/Bundles/Sounds/";
    private const string heroesVoicePath = "Assets/Bundles/Sounds/Interfaces/HeroVoice_Formation/";
    private const string creepSoundPath = "Assets/Bundles/Sounds/Enemies/Creeps/NormalAttacks/";
    private const string heroVoicePathPrefix = "UI_DX_Formation_Select_";

    private static readonly Dictionary<string, string> PrefixDirectoryMap = new Dictionary<string, string>()
    {
        {"UI","Interfaces"}
    };

    private readonly SignalBus _signalBus;

    public SoundDataLoader(SignalBus signalBus)
    {
        _signalBus = signalBus;
        LoadGameCSV();
    }

    public string GetAudioPath(string audioName, bool randomSound = false, int upperBound = 1)
    {
        if (randomSound && TryGetRandomPath(audioName, upperBound, out string randomPath))
            return randomPath;
        else if (SoundKey.TryGetValue(audioName, out string path))
            return path;

        throw new SoundKeyMissing(string.Format("Key {0} missing", audioName));
    }

    private bool TryGetRandomPath(string audioName, int upperBound, out string randomPath)
    {
        int randomIndex = UnityEngine.Random.Range(1, upperBound + 1);
        string finalName = audioName + "_" + randomIndex;
        if (SoundKey.TryGetValue(finalName, out randomPath))
            return true;
        return false;
    }

    public string GetHeroVoicePath(string heroName)
    {
        int randomIndex = UnityEngine.Random.Range(1, 6);
        string randomName = heroName + "_" + randomIndex;
        string finalName = heroVoicePathPrefix + randomName;
        if (SoundKey.TryGetValue(finalName, out string path))
            return path;

        throw new SoundKeyMissing(string.Format("Key {0} missing", heroName));
    }

    public string GetCreepNormalAttackSoundPath(string audioName)
    {
        if (SoundKey.TryGetValue(audioName, out string path))
            return path;

        throw new SoundKeyMissing(string.Format("Key {0} missing", audioName));
    }

    private void LoadGameCSV()
    {
        ReadCSV("csv\\SoundFota", soundRootPath, isCombineWithPrefixDirectory: true);

        ReadCSV("csv\\HeroesVoice", heroesVoicePath, isCombineWithPrefixDirectory: false);

        ReadCSV("csv\\CreepSound", creepSoundPath, isCombineWithPrefixDirectory: false);
    }

    private void ReadCSV(string path, string rootPath, bool isCombineWithPrefixDirectory)
    {
        //List<Dictionary<string, object>> result = CSVReader.Read(path);

        //for (int i = 0; i < result.Count; i++)
        //    if (result[i].ContainsKey(columnKey))
        //        FindSoundThenAddToSoundKey(result, i, rootPath, isCombineWithPrefixDirectory);
        //    else
        //        throw new CSVFileWrongFormat();
    }

    private void FindSoundThenAddToSoundKey(List<Dictionary<string, object>> result, int i, string rootPath, bool isCombineWithPrefixDirectory)
    {
        string keyName = result[i][columnKey].ToString();
        string soundPath;
        if (isCombineWithPrefixDirectory)
            soundPath = GetSoundPathWithPrefixDirectory(keyName, rootPath);
        else
            soundPath = GetSoundPath(keyName, rootPath, "");

        if (!SoundKey.ContainsKey(keyName))
            SoundKey.Add(keyName, soundPath);
    }

    private string GetSoundPath(string keyName, string rootPath, string directory)
    {       
        string fileName = keyName + ".ogg";
        return rootPath + directory + fileName;
    }

    private string GetSoundPathWithPrefixDirectory(string keyName, string rootPath)
    {
        string directoryPrefix = GetUntilOrEmpty(keyName);

        if(PrefixDirectoryMap.TryGetValue(directoryPrefix, out string directoryMapped))
            directoryPrefix = directoryMapped;

        directoryPrefix = string.IsNullOrEmpty(directoryPrefix) ? "" : directoryPrefix + "/";
        return GetSoundPath(keyName, rootPath, directoryPrefix);
    }

    public void FireOneShotNormalyAudioAsRandom(string audioPath)
    {
        _signalBus.Fire(
            new PlayOneShotAudioSignal(
                audioPath, 
                AudioMixerType.Sfx)
            .SetupPosition<PlayOneShotAudioSignal>(Vector3.zero)
            .SetupAudioSetting<PlayOneShotAudioSignal>(1.0f));
    }

    private string GetUntilOrEmpty(string text, string stopAt = "_")
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            int charLocation = text.IndexOf(stopAt);

            if (charLocation > 0)
            {
                return text.Substring(0, charLocation);
            }
        }

        return string.Empty;
    }

    private class SoundKeyMissing : Exception
    {
        public SoundKeyMissing(string mess) : base(mess) { }
    }

    private class CSVFileWrongFormat : Exception
    {
    }
}
