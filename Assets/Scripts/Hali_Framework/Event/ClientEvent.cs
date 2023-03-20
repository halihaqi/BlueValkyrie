namespace Hali_Framework
{
    /// <summary>
    /// 事件名枚举类
    /// </summary>
    public enum ClientEvent
    {
        //Input
        GET_KEY_DOWN,
        GET_KEY_UP,
        GET_KEY,
        
        //Scene
        LOADING,
        LOAD_COMPLETE,
        
        //Pool
        POOL_CHANGED,
        
        //Role
        ROLE_CHANGE_BEGIN,
        ROLE_SHOW_INFO,
        ROLE_CHANGE_COMPLETE,
        
        //UI
        SHOW_PANEL_SUCCESS,
        SHOW_PANEL_FAIL,
        SHOW_PANEL_COMPLETE,
        HIDE_PANEL_COMPLETE,
        
        //Bag
        BAG_ITEM_CLICK,
        BUY_COMPLETE,
        EXP_ADD,
        EXP_UP,
        LEVEL_PRE_UP,
        STAR_PRE_UP,
        WEAR_EQUIP,
        STAR_UP,

        //Battle
        CHESS_CLICK,
        CHESS_AUTO_CLICK,
        BATTLE_ROLE_CHANGE,
        BATTLE_ROLE_AIM,
        BATTLE_ROLE_ACTION,
        BATTLE_ROLE_REST,
        BATTLE_ROLE_RELOAD,
        BATTLE_BULLET_SHOOT,
        BATTLE_STEP_OVER,
        BATTLE_ROUND_RUN,
        BATTLE_HALF_ROUND_OVER,
        BATTLE_ROUND_OVER,
    }
}