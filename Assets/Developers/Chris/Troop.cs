using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using UnityEngine.AI;

public class Troop : Entity
{
    public UnitsStatsSO m_troopStats;
    public override EntityStatsSO EntityStats
    {
        get { return m_troopStats; }
        set { m_troopStats = (UnitsStatsSO)value; }
    }

    public List<Unit> m_units;
    public Unit Captain { get => m_units[0]; }

    public float FormationRadius;
    private Vector3[] m_formationPosition = new Vector3[4];


    private void Awake()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Captain.UnitAgent.SetDestination(Vector3.zero);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Captain.UnitAgent.SetDestination(new Vector3(-10, 0, -10));
        }
    }

    public override void AssignStats(EntityStatsSO inStats)
    {
        base.AssignStats(inStats);
        CreateUnits(m_troopStats.UnitType, m_troopStats.UnitQuantity);
    }

    public void CreateUnits(UnitType inType, int inValue)
    {
        m_units = new List<Unit>();

        for (int i = 0; i < inValue; i++)
        {
            GameObject tempUnit = ObjectPooler.Instance.GetUnityObject(inType);
            tempUnit.transform.parent = this.transform;
            tempUnit.SetActive(true);

            Unit tempUnitScript = tempUnit.GetComponent<Unit>();
            tempUnitScript.AssignStats(m_troopStats);
            tempUnitScript.AssignPlayer(this.EntityPlayerType);

            m_units.Add(tempUnitScript);
        }
        SetFormationPositions(FormationRadius);
    }

    #region Troop Formation

    public void SetFormationPositions(float inRadius = 1)
    {
        if (m_troopStats == null || m_units.Count == 0)
        {
            return;
        }


        // Begin Modification @Panta
        // Here we store each offset position.
        // Note the case 2 and 4 are different cause of angle offset.
        // In case 2 the offset is -90 degree, while in case 4 is -45 degree.

        switch (m_units.Count)
        {
            case 1:
                m_formationPosition[0] = Vector3.zero;
                break;

            case 2:
                // Reassign value to each position.
                for (int i = 0; i < m_units.Count; i++)
                {
                    // Calculate the angle in radian (not degree)
                    float angle = Mathf.PI * 2 / m_units.Count * i - (90 * Mathf.Deg2Rad);
                    angle += transform.eulerAngles.y * Mathf.Deg2Rad;

                    m_formationPosition[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * inRadius;
                }
                break;

            case 4:
                // Reassign value to each position.
                for (int i = 0; i < m_units.Count; i++)
                {
                    // Calculate the angle in radian (not degree)
                    float angle = Mathf.PI * 2 / m_units.Count * i - (45 * Mathf.Deg2Rad);
                    angle += transform.eulerAngles.y * Mathf.Deg2Rad;

                    m_formationPosition[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * inRadius;
                }
                break;

            default:
                // Reassign value to each position.
                for (int i = 0; i < m_units.Count; i++)
                {
                    // Calculate the angle in radian (not degree)
                    float angle = Mathf.PI * 2 / m_units.Count * i;
                    angle += transform.eulerAngles.y * Mathf.Deg2Rad;

                    m_formationPosition[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * inRadius;
                }
                break;
        }
        // End modification @Panta

        AssignFormation(m_formationPosition);
    }

    public void AssignFormation(Vector3[] inPos)
    {
        for (int i = 0; i < m_units.Count; i++)
        {
            Debug.Log(inPos[i]);
            m_units[i].transform.localPosition = inPos[i];
        }
    }

    public void ResetFormation()
    {
        for (int i = 0; i < m_units.Count; i++)
        {
            Vector3 destination = transform.position + m_formationPosition[i];
            m_units[i].UnitAgent.SetDestination(destination);
        }
    }
    #endregion
}
