using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Human Factor/New Level Container", fileName = "Level Container")]
public class HFLevelContainerSO : ScriptableObject
{
    [SerializeField] private List<HFLevelInfoSO> m_levels = new List<HFLevelInfoSO>();
    public List<HFLevelInfoSO> Levels
    {
        get
        {
            return m_levels;
        }
        private set
        {
            m_levels = value;
        }
    }

    public void SorLevelByIndex()
    {
        Levels = Levels.OrderBy(x => x.LevelSceneIndex).ToList();
    }
}
