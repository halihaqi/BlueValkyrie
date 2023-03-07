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
        
        //Battle
        BATTLE_ROLE_MOVE,
        BATTLE_ROLE_CHANGE,
    }
}