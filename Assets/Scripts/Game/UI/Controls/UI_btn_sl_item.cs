using Game.Global;
using Game.Managers;
using Game.Model;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_btn_sl_item : ControlBase
    {
        private Button _btn;

        private Image _imgBadge;
        private Image _imgTitleSave;
        private Image _imgTitleLoad;
        private Slider _sldComplete;

        private Text _txtTitle;
        private Text _txtTimeTip;
        private Text _txtName;
        private Text _txtTime;
        private Text _txtComplete;

        private int _userId;
        private string _userName;
        private int _secretaryId;

        private PlayerInfo _info;

        protected internal override void OnInit()
        {
            base.OnInit();
            _btn = this.GetComponent<Button>();
            
            _imgTitleSave = GetControl<Image>("img_title_save");
            _imgTitleLoad = GetControl<Image>("img_title_load");
            _imgBadge = GetControl<Image>("img_badge");
            _sldComplete = GetControl<Slider>("sld_complete");
            
            _txtTimeTip = GetControl<Text>("txt_time_tip");
            _txtTitle = GetControl<Text>("txt_title");
            _txtName = GetControl<Text>("txt_name");
            _txtTime = GetControl<Text>("txt_time");
            _txtComplete = GetControl<Text>("txt_complete");
        }


        public void SetData(bool isSave, int userId, PlayerInfo info, string userName = null, int secretaryId = 0)
        {
            _userId = userId;
            _userName = userName;
            _secretaryId = secretaryId;
            _info = info;
            
            //添加按钮事件
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
                if (info != null)
                {
                    _imgTitleSave.gameObject.SetActive(false);
                    _imgTitleLoad.gameObject.SetActive(true);
                    _btn.onClick.AddListener(() =>
                    {
                        TipMgr.Instance.ShowConfirm("是否进入该存档？", OnLoadClick);
                    });
                }
            }

            //更新面板
            _txtTitle.text = $"存档{_userId + 1}";
            UpdateView(info);
        }

        private void OnSaveClick()
        {
            var playerData = BinaryDataMgr.Instance.Load<PlayerData>(GameConst.DATA_PART_PLAYER, "PlayerData");
            var playerInfo = new PlayerInfo(_userId, _userName, _secretaryId);
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
                _sldComplete.value = info.complete;
                ResMgr.Instance.LoadAsync<Sprite>(ResPath.GetSchoolBadgeIcon(info.secretaryId), img =>
                {
                    _imgBadge.sprite = img;
                });
            }
        }

        private void SetControlsActive(bool isActive)
        {
            _txtTimeTip.gameObject.SetActive(isActive);
            _txtTime.gameObject.SetActive(isActive);
            _txtName.gameObject.SetActive(isActive);
            _txtComplete.gameObject.SetActive(isActive);
            _sldComplete.gameObject.SetActive(isActive);
            _imgBadge.gameObject.SetActive(isActive);
        }
    }
}