using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Riddle : MonoBehaviour
{
    [System.Serializable]
    public class RiddleType
    {
        [TextArea]
        public string FranksText;
        [TextArea]
        public string FloranceText;
        [TextArea]
        public string JanusText;
        public int goodChestId;
    }
    [SerializeField] RiddleType[] riddles;
    [SerializeField] Transform[] chests;
    [SerializeField] GameObject[] rewards;
    [SerializeField] TMP_Text[] texts;

    [SerializeField] MeshRenderer[] eyeRenderes;
    [SerializeField] Material badEyes;
    [SerializeField] Light[] lights;
    [SerializeField] Color badLightColor;

    int selectedRiddleId;
    [HideInInspector] public bool locked;

    private void Start()
    {
        selectedRiddleId = Random.Range(0, riddles.Length);
        texts[0].text = riddles[selectedRiddleId].FranksText;
        texts[1].text = riddles[selectedRiddleId].FloranceText;
        texts[2].text = riddles[selectedRiddleId].JanusText;
    }
    public void OpenChest(Transform chest)
    {
        locked = true;

        if(chests[selectedRiddleId] == chest)
        {
            //Good job
            if(DungeonManager.Instance != null)
                DungeonManager.Instance.PuzzleFinished();
            rewards[selectedRiddleId].SetActive(true);
        }
        else
        {
            //No bad shit
            Debug.Log("Bad");
            foreach(MeshRenderer renderer in eyeRenderes)
            {
                renderer.material = badEyes;
            }
            foreach(Light light in lights)
            {
                light.color = badLightColor;
            }
        }
    }
}
