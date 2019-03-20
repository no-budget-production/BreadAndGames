using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DigitalOpus.MB.Core;

public class MB3_BatchPrefabBaker : MonoBehaviour {
    public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

	[System.Serializable]
	public class MB3_PrefabBakerRow{
		public GameObject sourcePrefab;
		public GameObject resultPrefab;
	}

	public MB3_PrefabBakerRow[] prefabRows;

    public string outputPrefabFolder;
}
