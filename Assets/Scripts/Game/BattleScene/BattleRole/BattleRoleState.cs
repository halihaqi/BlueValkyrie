namespace Game.BattleScene.BattleRole
{
    /// <summary>
    /// 战斗人物状态，主要描述可以buff或debuff的状态
    /// </summary>
    public struct BattleRoleState
    {
        public int maxHp;
        public int maxAp;
        public int maxAmmo;
        
        public int atkRange;
        public int atk;
        public int def;
        public int critRate;//0 - 100
        public int critDmg;//100 - 9999
        public int agi;//敏捷，闪避率 0 - 90
    }
}