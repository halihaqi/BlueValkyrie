using System;
using Game.Global;
using Game.Managers;
using Game.Model;
using Game.UI.Base;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_btn_sl_item : ControlBase
    {
        private Image _imgBadge;
        private Image _imgTitleSave;
        private Image _imgTitleLoad;
        
        private Slider _sldComplete;

        private Text _txtTitle;
        private Text _txtName;
        private Text _txtTime;

        private ControlGroup _slGroup;

        private int _userId;
        private string _userName;
        private int _secretaryId;

        private PlayerItem _item;

        protected internal override void OnInit()
        {
            base.OnInit();
            
            _imgTitleSave = GetControl<Image>("img_title_save");
            _imgTitleLoad = GetControl<Image>("img_title_load");
            _imgBadge = GetControl<Image>("img_badge");
            _sldComplete = GetControl<Slider>("sld_complete");
            
            _txtTitle = GetControl<Text>("txt_title");
            _txtName = GetControl<Text>("txt_name");
            _txtTime = GetControl<Text>("txt_time");

            _slGroup = GetControl<ControlGroup>("sl_group");
        }

        public void SetSave(int userId, PlayerItem item)
        {
            _userId = userId;
            _item = item;
            
            _imgTitleSave.gameObject.SetActive(true);
            _imgTitleLoad.gameObject.SetActive(false);
            //更新面板
            _txtTitle.text = $"存档{_userId + 1}";
            UpdateView(item);
        }

        public void SetNew(int userId, PlayerItem item, string userName, int secretaryId)
        {
            _userId = userId;
            _item = item;
            _userName = userName;
            _secretaryId = secretaryId;
            
            _imgTitleSave.gameObject.SetActive(true);
            _imgTitleLoad.gameObject.SetActive(false);
            //更新面板
            _txtTitle.text = $"存档{_userId + 1}";
            UpdateView(item);
        }

        public void SetLoad(int userId, PlayerItem item)
        {
            _userId = userId;
            _item = item;
            
            _imgTitleSave.gameObject.SetActive(false);
            _imgTitleLoad.gameObject.SetActive(true);
            //更新面板
            _txtTitle.text = $"存档{_userId + 1}";
            UpdateView(item);
        }

        public void OnSaveClick()
        {
            string str = _item != null ? "是否覆盖存档？" : "是否创建新存档？";
            TipHelper.ShowConfirm(str, () =>
            {
                _item = PlayerMgr.Instance.CurPlayer; 
                PlayerMgr.Instance.SaveUser(_userId, _item);
                UpdateView(_item);
                
                TipHelper.ShowTip("保存成功！");
            });
        }

        public void OnNewClick()
        {
            string str = _item != null ? "是否覆盖存档？" : "是否创建新存档？";
            TipHelper.ShowConfirm(str, () =>
            {
                var newPlayerInfo = new PlayerItem(_userId, _userName, _secretaryId);
                PlayerMgr.Instance.SaveUser(_userId, newPlayerInfo);
                UpdateView(newPlayerInfo);

                //进入游戏流程
                ProcedureMgr.Instance.SetData(PlayerMgr.PLAYER_DATA_KEY, newPlayerInfo);
                ProcedureMgr.Instance.ChangeState<GameProcedure>();
            });
        }

        public void OnLoadClick()
        {
            if (_item == null) return;
            
            TipHelper.ShowConfirm("是否进入该存档？", () =>
            {
                //进入游戏流程
                ProcedureMgr.Instance.SetData(PlayerMgr.PLAYER_DATA_KEY, _item);
                ProcedureMgr.Instance.ChangeState<GameProcedure>();
            });
        }

        private void UpdateView(PlayerItem item)
        {
            if (item == null)
                _slGroup.SetActive(false);
            else
            {
                _slGroup.SetActive(true);
                _txtName.text = item.name;
                _txtTime.text = ((int)item.time).ToTime();
                _sldComplete.value = item.complete;
                ResMgr.Instance.LoadAsync<Sprite>(ResPath.GetSchoolBadgeIcon(item.secretaryId), img =>
                {
                    _imgBadge.sprite = img;
                });
            }
        }
    }
}