using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemGenerator : MonoBehaviour, IClickable
{
    [SerializeField] private Transform m_destinationPoint;
    [SerializeField] private float m_xOffSet = 1;
    [SerializeField] private float m_zOffSet = 1;

    private void OnEnable()
    {
        HFEventManager.SubscribeTo(HFEventID.OnWaveBeginned, StartTimer);
        HFEventManager.SubscribeTo(HFEventID.OnWaveCleared, StopSpawn);
    }
    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom(HFEventID.OnWaveBeginned, StartTimer);
        HFEventManager.UnsubscribeFrom(HFEventID.OnWaveCleared, StopSpawn);
    }

    public void StartTimer()
    {
        StartCoroutine(SpawnGemCoroutine());
    }

    IEnumerator SpawnGemCoroutine()
    {
        yield return new WaitForSeconds(3f);

        SpawnGem();

        StartCoroutine(SpawnGemCoroutine());
    }

    public void SpawnGem()
    {
        GameObject gem = ObjectPooler.Instance.GetPooledObject("Gem");
        gem.transform.position = this.transform.position;
        gem.SetActive(true);

        float x = Random.Range(-m_xOffSet, m_xOffSet);
        float z = Random.Range(-m_zOffSet, m_zOffSet);

        Vector3 destination = m_destinationPoint.transform.position + new Vector3(x, 0, z);

        gem.transform.DOJump(destination, 5, 2, 1f);
    }

    public void StopSpawn()
    {
        StopAllCoroutines();
    }

    public void Click()
    {
        SpawnGem();
    }

    public void Deselected()
    {
        
    }
}
