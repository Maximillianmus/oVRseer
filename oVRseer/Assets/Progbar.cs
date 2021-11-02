using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Progbar : NetworkBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject stepPrefab;
    [SerializeField] private Text numberRemainingText;

    [SerializeField] private int keysTotal = 5;

    [SerializeField] private int keysRemaining = 5;

    private List<GameObject> steps = new List<GameObject>();



    // Start is called before the first frame update
    void Start()
    {
        if (!hasAuthority)
            return;
        keysTotal = GameObject.FindWithTag("KeySpawnSystem").GetComponent<KeySpawnSystem>().numOfKeysToSpawn;
        if (!hasAuthority)
            return;
        keysRemaining = keysTotal;
        for (int i = 0; i < keysTotal; i++)
        {
            steps.Add(Instantiate(stepPrefab, panel.transform));
        }

        for (int i = 0; i < keysTotal; i++)
        {
            var color = steps[i].GetComponent<Image>().color;
            steps[i].GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0);
        }
    }

    public override void OnStartAuthority()
    {
        NetworkClient.RegisterHandler<KeyCollectedMsg>(OnKeyRetrieved);
    }

    // Update is called once per frame
    void Update()
    {
        numberRemainingText.text = "number of keys remaining : " + keysRemaining;

    }

    public void OnKeyRetrieved(KeyCollectedMsg msg)
    {
        if (!hasAuthority)
        {
            return;
        }
        var i = Math.Min(keysTotal - keysRemaining, keysTotal - 1);
        var color = steps[i].GetComponent<Image>().color;
        steps[i].GetComponent<Image>().color = new Color(color.r, color.g, color.b, 255);
        keysRemaining--;
        if (keysRemaining == 0)
        {
            ChangeColorBar(Color.green);
        }
    }

    private void ChangeColorBar(Color newColor)
    {
        foreach (var step in steps)
        {
            step.GetComponent<Image>().color = newColor;
        }
    }
}
