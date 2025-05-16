using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyOrderInLayerManager : MonoBehaviour
{
    public static EnemyOrderInLayerManager instance;

    List<SpriteRenderer> enemyesSpR = new List<SpriteRenderer>();

    public void Add (SpriteRenderer spp) { enemyesSpR.Add(spp); }
    public void Dell (SpriteRenderer spp) { enemyesSpR.Remove(spp); }


    float[] posYs;
    SpriteRenderer[] spitesRends;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(nameof(Check));
    }


    IEnumerator Check()
    {
        yield return new WaitForSeconds(1);

        int n = enemyesSpR.Count;


        posYs = new float[n];
        spitesRends = new SpriteRenderer[n];

        for (int i = 0; i < n; i++)
        {
            posYs[i] = enemyesSpR[i].transform.position.y;
            spitesRends[i] = enemyesSpR[i];
        }

        Array.Sort(posYs, spitesRends);

        for (int i = 0; i < spitesRends.Length; i++)
        {
            spitesRends[i].sortingOrder = -i;
        }

        StartCoroutine(nameof(Check));
    }
}
