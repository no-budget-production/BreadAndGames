using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CheatHelp : MonoBehaviour
{
    public Text[] Cheats;
    public string CheatsName = "Cheats";
    public Cheats Cheat;

    public List<CheatConfig> CheatsArray;
    public List<CheatConfig> SortedList;

    void Start()
    {
        Cheat = GameObject.Find(CheatsName).GetComponent<Cheats>();

        CheatsArray = new List<CheatConfig>(Cheat.UsedCheats.Length);

        for (int i = 0; i < Cheat.UsedCheats.Length; i++)
        {
            CheatsArray.Add(Cheat.UsedCheats[i]);
        }

        //SortedList = new List<CheatConfig>(CheatsArray.Count);
        //SortedList = CheatsArray.OrderBy(o => o.ButtonStringBC).ToList();

        for (int i = 0; i < CheatsArray.Count; i++)
        {
            Cheats[0].text += CheatsArray[i].ButtonStringBC.name + ": " + CheatsArray[i].Cheat.GetComponent<Cheat>().name + "\n";
        }
    }

}
