public struct DamageInfo
{
    public float baseDamage;
    public float bonusDamage;
    public bool isCritical;

    public float GetTotalDamage()
    {
        return isCritical ? baseDamage + bonusDamage : baseDamage;
    }

    public DamageInfo(float baseDmg, float bonusDmg = 0f, bool isCrit = false)
    {
        baseDamage = baseDmg;
        bonusDamage = bonusDmg;
        isCritical = isCrit;
    }
}
