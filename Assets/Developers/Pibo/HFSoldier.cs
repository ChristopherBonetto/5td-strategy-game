using UnityEngine;

namespace HF
{
	public class HFSoldier : MonoBehaviour
	{
		public HFUnit OwnerUnit { get; private set; }
		public float Phase { get; private set; }
		public HFUnitVisuals LoadedVisuals { get; private set; }
		public Collider Target { get; private set; }
		public Transform BulletSpawn { get; private set; }

		public void Init(HFUnit inOwnerUnit, float inPhase)
		{
			OwnerUnit = inOwnerUnit;
			Phase = inPhase;

			LoadedVisuals = GetComponent<HFUnitVisuals>();
			HFHelpers.NullCheck(gameObject, LoadedVisuals, "soldier visuals");

			Target = GetComponentInChildren<Collider>();
			HFHelpers.NullCheck(gameObject, Target, "soldier collider");

			HFBulletSpawn testBulletSpawn = GetComponentInChildren<HFBulletSpawn>();
			HFHelpers.NullCheck(gameObject, testBulletSpawn, "soldier bullet spawn");
			BulletSpawn = testBulletSpawn.transform;
		}

		public void SetVisualsLevel(int level)
		{
			int index = level - 1;
			int levels = LoadedVisuals.LevelVisuals.Length;
			if (index < levels)
			{
				for(int i = 0; i < levels; i++)
				{
					foreach (GameObject mesh in LoadedVisuals.LevelVisuals[i].List)
					{
						mesh.SetActive(index == i);
					}
				}
			}
			else
			{
				Debug.LogWarning("No visuals for level " + level + " in " + gameObject.name);
			}
		}
	}
}
