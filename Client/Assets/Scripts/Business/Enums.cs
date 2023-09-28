namespace Core.Business
{
    public enum SKILL_ACTION_TYPE
    {
        Attack
    }

    public enum TARGET_FINDER_TYPE
    {
        Nearest,
        LowestHp,
    }

    public enum MOVING_TYPE
    {
        None,
        FirstTarget,
        CenterTargets
    }

    public enum CharacterRole
    {
        Dealer,
        Supporter,
        Healer,
        Tanker
    }

    public enum BulletType
    {
        None,
        Handgun,
        ShortGun,
        Meelee,
        AOE,
        AddBuff,
        MultiplyBuff
    }

    public enum WeaponType
    {
        Auto,
        Touch,
        Meelee,
        Manual,
        AuraBuff,
        Provoke
    }
}