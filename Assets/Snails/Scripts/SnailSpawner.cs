using System;
using System.Collections.Generic;
using Resources;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Snails
{
    public class SnailSpawner : MonoBehaviour
    {
        [SerializeField] private Snail snailPrefab;
        
        private List<string> unUsedNames = new List<string>();

        private const string NAME_FILE_NAME = "snail_names.txt";
        private string nameFilePath => Application.streamingAssetsPath + "/" + NAME_FILE_NAME;

        private void Start()
        {
            LoadNames();
        }

        public Snail SpawnSnailAtPosition(Vector3 position)
        {
            Snail snail = Instantiate(snailPrefab,position, Quaternion.identity);
            snail.GetComponent<Snail>().Initialize(GetRandomName());
            
            return snail;
        }

        private string GetRandomName()
        {

            if (unUsedNames.Count <= 0)
            {
                LoadNames();

                if (unUsedNames.Count <= 0)
                {
                    throw new ArgumentException("No names found in " + nameFilePath);
                }
            }
            
            int randomIndex = Random.Range(0, unUsedNames.Count);
            
            string name = unUsedNames[randomIndex];
            unUsedNames.RemoveAt(randomIndex);
            
            return name;
        }
        
        private void LoadNames()
        {
            string[] lines = System.IO.File.ReadAllLines(nameFilePath);
            
            foreach (var line in lines)
            {
                string[] names = line.Split(';');
                
                foreach (var name in names)
                {
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        unUsedNames.Add(name);
                    }
                }
            }
        }
    }
}