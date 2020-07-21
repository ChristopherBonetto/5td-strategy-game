using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemGenerator : MonoBehaviour
{
    [SerializeField] private Transform m_destinationPoint;
    [SerializeField] private float m_xOffSet = 1;
    [SerializeField] private float m_zOffSet = 1;
    [SerializeField] private float timer = 10f;
    [SerializeField] private float currentTimer = 0;
    bool canSpawnGem = false;

    private bool inWave = false;

    private void OnEnable()
    {
        HFEventManager.SubscribeTo(HFEventID.OnWaveBeginned, StartWave);
        HFEventManager.SubscribeTo(HFEventID.OnWaveCleared, WaveCleared);
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, GameStateChanged);
        HFEventManager.SubscribeTo<bool>(HFEventID.OnPauseMode, FreezeMode);
    }
    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom(HFEventID.OnWaveBeginned, StartWave);
        HFEventManager.UnsubscribeFrom(HFEventID.OnWaveCleared, WaveCleared);
        HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, GameStateChanged);
        HFEventManager.UnsubscribeFrom<bool>(HFEventID.OnPauseMode, FreezeMode);
    }

    private void Update()
    {
        SpawnGem();
        Debug.LogError(canSpawnGem);
    }



    public void SpawnGem()
    {
        
      if(canSpawnGem==true)
        {
            if (currentTimer >= timer)
            {

                currentTimer = 0;
                GameObject gem = ObjectPooler.Instance.GetPooledObject("Gem");
                gem.transform.position = this.transform.position;
                gem.SetActive(true);

                float x = Random.Range(-m_xOffSet, m_xOffSet);
                float z = Random.Range(-m_zOffSet, m_zOffSet);

                Vector3 destination = m_destinationPoint.transform.position + new Vector3(x, 0, z);

                gem.transform.DOJump(destination, 5, 2, 1f);
            }
            else currentTimer += Time.deltaTime;
        }
    }

    public void GameStateChanged(GameStates inState)
    {
        if(inState == GameStates.Pause)
        {
            canSpawnGem = false;
        }
        else if(inState == GameStates.PlayingLevel)
        {
            if(inWave)
            {
                canSpawnGem = true;
            }
        }
    }

    public void FreezeMode(bool inValue)
    {
        if(inValue)
        {
            canSpawnGem = false;
        }
        else
        {
            if(inWave)
            {
                canSpawnGem = true;
            }
        }
    }




    public void StartWave()
    {
        inWave = true;
        canSpawnGem = true;
    }
    public void WaveCleared()
    {
        inWave = false;
        canSpawnGem = false;
    }




}
