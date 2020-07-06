using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HFCastleHealthContainer : MonoBehaviour
{

    private List<CastleStarter> m_CastlesCollection;
    /// <summary>
    /// Get Reference to Castles in scene level.
    /// </summary>
    public List<CastleStarter> CastleCollection
    {
        get
        {
            if (m_CastlesCollection == null)
                m_CastlesCollection = new List<CastleStarter>();
            return m_CastlesCollection;
        }
    }

    [SerializeField]
    private Slider[] HealthBars;
    [SerializeField]
    private Button[] Buttons;
    private Dictionary<CastleStarter, Slider> CastleSliderKeyValuePair = new Dictionary<CastleStarter, Slider>();
    private int m_indexReferenceToCastle = 0;


    #region Collection management

    public void AddCastle(CastleStarter castle)
    {
        if (!CastleCollection.Contains(castle))
        {
            CastleCollection.Add(castle);
            CastleSliderKeyValuePair.Add(castle, HealthBars[m_indexReferenceToCastle]);
            HealthBars[m_indexReferenceToCastle].value = 1;
            Buttons[m_indexReferenceToCastle].onClick.AddListener(() => HFCameraController.Instance.SetTarget(castle.transform));
            HealthBars[m_indexReferenceToCastle].gameObject.SetActive(true);
            m_indexReferenceToCastle += 1;
        }
    }

    public void RemoveCastle(CastleStarter castle)
    {
        if (CastleCollection.Contains(castle))
        {
            CastleCollection.Remove(castle);
            CastleSliderKeyValuePair[castle].gameObject.SetActive(false);
            CastleSliderKeyValuePair.Remove(castle);
        }
    }

    public void ClearCollection()
    {
        CastleCollection.Clear();
        CastleSliderKeyValuePair.Clear();
        m_indexReferenceToCastle = 0;

        ResetButtons();
        ResetHealtBars();
    }

    #endregion

    #region Health bars management

    public void CastleTakeDamageFeedback(CastleStarter castle)
    {
        if (!CastleSliderKeyValuePair.ContainsKey(castle))
            AddCastle(castle);

        CastleSliderKeyValuePair[castle].value = ((float)castle.CurrentHp / (float)castle.m_buildingStats.MaxHp);
    }

    public void ResetHealtBars()
    {
        foreach (var item in HealthBars)
            item.value = 1;
    }

    public void ResetButtons()
    {
        foreach (var item in Buttons)
            item.onClick.RemoveAllListeners();
    }

    #endregion
}
