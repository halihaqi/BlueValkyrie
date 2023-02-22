﻿using Game.Model;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_btn_sl_item : ControlBase
    {
        private Button _btn;
        
        private Image _imgTitleSave;
        private Image _imgTitleLoad;
        private Image _imgComplete;
        private Image _imgCompleteBk;
        private float _completeMaxWidth;

        private Text _txtTitle;
        private Text _txtTimeTip;
        private Text _txtName;
        private Text _txtTime;
        private Text _txtComplete;

        protected internal override void OnInit()
        {
            base.OnInit();
            _btn = this.GetComponent<Button>();
            
            _imgTitleSave = GetControl<Image>("img_title_save");
            _imgTitleLoad = GetControl<Image>("img_title_load");
            _imgComplete = GetControl<Image>("img_complete");
            _imgCompleteBk = GetControl<Image>("img_complete_bk");
            _completeMaxWidth = _imgCompleteBk.rectTransform.rect.width;
            
            _txtTimeTip = GetControl<Text>("txt_time_tip");
            _txtTitle = GetControl<Text>("txt_title");
            _txtName = GetControl<Text>("txt_name");
            _txtTime = GetControl<Text>("txt_time");
            _txtComplete = GetControl<Text>("txt_complete");
        }


        public void SetData(bool isSave, int dataId, PlayerInfo info, string userName = null)
        {
            _btn.onClick.RemoveAllListeners();
            if (isSave)
            {
                _imgTitleSave.gameObject.SetActive(true);
                _imgTitleLoad.gameObject.SetActive(false);
                _btn.onClick.RemoveAllListeners();
                _btn.onClick.AddListener(() =>
                {
                    var playerData = BinaryDataMgr.Instance.Load<PlayerData>(GameConst.DATA_PART_PLAYER, "PlayerData");
                    var newInfo = new PlayerInfo
                    {
                        id = dataId,
                        name = userName,
                        time = 0,
                        complete = 0,
                    };
                    playerData.dataDic[dataId] = newInfo;
                    BinaryDataMgr.Instance.Save(GameConst.DATA_PART_PLAYER, "PlayerData", playerData);
                    UpdateView(newInfo);
                });
            }
            else
            {                          
                _imgTitleSave.gameObject.SetActive(false);
                _imgTitleLoad.gameObject.SetActive(true);
                _btn.onClick.RemoveAllListeners();
                _btn.onClick.AddListener(() =>
                {
                     
                });
            }

            _txtTitle.text = $"存档{dataId + 1}";
            UpdateView(info);
        }

        private void UpdateView(PlayerInfo info)
        {
            if (info == null)
                SetControlsActive(false);
            else
            {
                SetControlsActive(true);
                _txtName.text = info.name;
                _txtTime.text = info.time.ToTime();
                _txtComplete.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right,
                    _completeMaxWidth * info.complete,
                    _txtComplete.rectTransform.rect.width);
            }
        }

        private void SetControlsActive(bool isActive)
        {
            _txtTimeTip.gameObject.SetActive(isActive);
            _txtTime.gameObject.SetActive(isActive);
            _txtName.gameObject.SetActive(isActive);
            _txtComplete.gameObject.SetActive(isActive);
            _imgCompleteBk.gameObject.SetActive(isActive);
            _imgComplete.gameObject.SetActive(isActive);
        }
    }
}