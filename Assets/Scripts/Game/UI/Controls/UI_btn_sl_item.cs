using Game.Global;
using Game.Managers;
using Game.Model;
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

        private int _userId;
        private string _userName;

        private PlayerInfo _info;

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


        public void SetData(bool isSave, int userId, PlayerInfo info, string userName = null)
        {
            _userId = userId;
            _userName = userName;
            _info = info;
            
            _btn.onClick.RemoveAllListeners();
            if (isSave)
            {
                _imgTitleSave.gameObject.SetActive(true);
                _imgTitleLoad.gameObject.SetActive(false);
                _btn.onClick.AddListener(() =>
                {
                    string str = _info != null ? "是否覆盖存档？" : "是否创建新存档？";
                    TipMgr.Instance.ShowConfirm(str, OnSaveClick);
                });
            }
            else
            {                          
                _imgTitleSave.gameObject.SetActive(false);
                _imgTitleLoad.gameObject.SetActive(true);
                _btn.onClick.AddListener(() =>
                {
                     
                });
            }

            _txtTitle.text = $"存档{_userId + 1}";
            UpdateView(info);
        }

        private void OnSaveClick()
        {
            var playerData = BinaryDataMgr.Instance.Load<PlayerData>(GameConst.DATA_PART_PLAYER, "PlayerData");
            var playerInfo = new PlayerInfo
            {
                id = _userId,
                name = _userName,
                time = 0,
                complete = 0,
            };
            playerData.dataDic[_userId] = playerInfo;
            BinaryDataMgr.Instance.Save(GameConst.DATA_PART_PLAYER, "PlayerData", playerData);
            UpdateView(playerInfo);
            
            //进入游戏流程
            ProcedureMgr.Instance.SetData(PlayerMgr.PLAYER_DATA_KEY, playerInfo);
            ProcedureMgr.Instance.ChangeState<GameProcedure>();
        }

        private void OnLoadClick()
        {
            var dic = BinaryDataMgr.Instance.Load<PlayerData>(GameConst.DATA_PART_PLAYER, "PlayerData").dataDic;
            var playerInfo = dic[_userId];
            
            //进入游戏流程
            ProcedureMgr.Instance.SetData(PlayerMgr.PLAYER_DATA_KEY, playerInfo);
            ProcedureMgr.Instance.ChangeState<GameProcedure>();
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