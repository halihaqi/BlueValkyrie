using System;
using System.Collections.Generic;
using Game.Model;
using Game.UI.Controls;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Begin
{
    public class SaveLoadPop : PopBase
    {
        private bool _isSave;
        private string _nowUserName;
        private Transform _content;
        private const string ITEM_PATH = "UI/Controls/btn_sl_item";

        protected internal override void OnInit(object userData)
        {
            isModal = true;
            base.OnInit(userData);
            
            if (userData is SaveLoadParam p)
            {
                _isSave = p.isSave;
                _nowUserName = p.userName;
            }
            else
                throw new Exception($"{Name} param is invalid.");
            
            _content = GetControl<ScrollRect>("sv_sl_data").content;
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);

            //获取存档
            var playerData = BinaryDataMgr.Instance.Load<PlayerData>(GameConst.DATA_PART_PLAYER, "PlayerData");
            if (playerData == null)
            {
                playerData = new PlayerData();
                BinaryDataMgr.Instance.Save(GameConst.DATA_PART_PLAYER, "PlayerData", playerData);
            }

            //添加存档item
            foreach (var kv in playerData.dataDic)
            {
                AddCustomControl(ITEM_PATH, go =>
                {
                    go.transform.SetParent(_content, false);
                    var item = go.GetComponent<UI_btn_sl_item>();
                    item.SetData(_isSave, kv.Key, kv.Value, _nowUserName);
                });
            }
        }
    }

    public class SaveLoadParam
    {
        public bool isSave;
        public string userName;

        public SaveLoadParam(bool isSave, string userName)
            => (this.isSave, this.userName) = (isSave, userName);
    }
}