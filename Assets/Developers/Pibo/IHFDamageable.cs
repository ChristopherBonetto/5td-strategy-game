/// <summary>
/// Struct containing damage event information
/// </summary>
public struct DamageInfo
{
	public float Amount;

	public DamageInfo(float amount = 0f)
	{
		Amount = amount;
	}
}

/// <summary>
/// Struct containing heal event information
/// </summary>
public struct HealInfo
{
	public float Amount;

	public HealInfo(float amount = 0f)
	{
		Amount = amount;
	}
}

/// <summary>
/// Interface for health system
/// </summary>
public interface IHFDamageable
{
	/// <summary>
	/// Max health
	/// </summary>
	float MaxHealth { get; }

	/// <summary>
	/// Current health
	/// </summary>
	float CurrentHealth { get; }

	/// <summary>
	/// Invincibility flag
	/// </summary>
	bool CanSufferDamage { get; }

	/// <summary>
	/// Team ID number
	/// </summary>
	int Team { get; }

	/// <summary>
	/// Killed flag
	/// </summary>
	bool IsKilled { get; }

	/// <summary>
	/// Should be used to receive damage
	/// </summary>
	/// <param name="info">Struct containing damage event information</param>
	/// <returns>Actual health value decrease amount</returns>
	float TakeDamage(DamageInfo info);

	/// <summary>
	/// Should be used to receive heal
	/// </summary>
	/// <param name="info">Struct containing heal event information</param>
	/// <returns>Actual health value increase amount</returns>
	float Heal(HealInfo info);
}
