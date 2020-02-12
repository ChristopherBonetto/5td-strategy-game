using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Human Factor/New Level Container", fileName = "Level Container")]
public class HFLevelContainerSO : ScriptableObject
{
    [SerializeField] private List<HFLevelInfoSO> m_levels = new List<HFLevelInfoSO>();
    public List<HFLevelInfoSO> Levels { get => m_levels; }
}
