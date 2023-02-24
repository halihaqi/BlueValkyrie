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
        private int _secretaryId;
        private Transform _content;
        private const string ITEM_PATH = "UI/Controls/btn_sl_item";

        protected internal override void OnInit(object userData)
        {
            isModal = true;
            base.OnInit(userData);

            _content = GetControl<ScrollRect>("sv_sl_data").content;
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            if (userData is SaveLoadParam p)
            {
                _isSave = p.isSave;
                _nowUserName = p.userName;
                _secretaryId = p.secretaryId;
            }
            else
                throw new Exception($"{Name} param is invalid.");

            //获取存档
            var playerData = BinaryDataMgr.Instance.Load<PlayerData>(GameConst.DATA_PART_PLAYER, "PlayerData");
            if (playerData == null)
            {
                playerData = new PlayerData();
                BinaryDataMgr.Instance.Save(GameConst.DATA_PART_PLAYER, "PlayerData", playerData);
            }

            //添加存档item
            var saveDic = playerData.dataDic;
            for (int i = 0; i < GameConst.FILE_NUM; i++)
            {
                int index = i;
                if(saveDic.ContainsKey(index))
                    AddSaveItem(index, playerData.dataDic[index]);
                else
                    AddSaveItem(index, null);
            }
        }

        private void AddSaveItem(int saveId, PlayerInfo saveInfo)
        {
            AddCustomControl(ITEM_PATH, go =>
            {
                go.transform.SetParent(_content, false);
                var item = go.GetComponent<UI_btn_sl_item>();
                item.SetData(_isSave, saveId, saveInfo, _nowUserName, _secretaryId);
            });
        }
    }

    public class SaveLoadParam
    {
        public bool isSave;
        public string userName;
        public int secretaryId;

        public SaveLoadParam(bool isSave, string userName, int secretaryId)
            => (this.isSave, this.userName, this.secretaryId) = (isSave, userName, secretaryId);

        public SaveLoadParam(bool isSave) => this.isSave = isSave;
    }
}