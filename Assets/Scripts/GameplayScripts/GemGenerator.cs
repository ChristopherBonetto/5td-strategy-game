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



    private void OnEnable()
    {
        HFEventManager.SubscribeTo(HFEventID.OnWaveBeginned, ActivateGemSpawn);
        HFEventManager.SubscribeTo(HFEventID.OnWaveCleared, StopGemSpawn);
    }
    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom(HFEventID.OnWaveBeginned, ActivateGemSpawn);
        HFEventManager.UnsubscribeFrom(HFEventID.OnWaveCleared, StopGemSpawn);
    }

    private void Update()
    {
        SpawnGem();
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



    public void ActivateGemSpawn()
    {
        canSpawnGem = true;
    }
    public void StopGemSpawn()
    {
        canSpawnGem = false;
    }




}
