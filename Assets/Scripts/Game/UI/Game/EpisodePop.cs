using Game.BattleScene;
using Game.Managers;
using Game.UI.Controls;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Game
{
    public class EpisodePop : PopBase
    {
        private UI_btn_big _btnFight;
        private Button _btnBack;
        private Button _btnLeft;
        private Button _btnRight;
     
        private Image _imgStar1;
        private Image _imgStar2;
        private Image _imgStar3;
        private Image _imgComplete;

        private RawImage _rawMap;
        private Text _txtId;
        private Text _txtName;
        private Text _txtRound;
        private Text _txtChallenge1;
        private Text _txtChallenge2;
        private Text _txtChallenge3;
        private HList _svReward;

        private int _episodeId = 1;
        private EpisodeInfo _info;

        protected internal override void OnInit(object userData)
        {
            IsFullScreen = true;
            base.OnInit(userData);
            _rawMap = GetControl<RawImage>("raw_map");
            _imgStar1 = GetControl<Image>("img_star1");
            _imgStar2 = GetControl<Image>("img_star2");
            _imgStar3 = GetControl<Image>("img_star3");
            _imgComplete = GetControl<Image>("img_complete");
            _svReward = GetControl<HList>("sv_reward");
            _btnFight = GetControl<UI_btn_big>("btn_fight");
            _btnBack = GetControl<Button>("btn_back");
            _btnLeft = GetControl<Button>("btn_left");
            _btnRight = GetControl<Button>("btn_right");
            _txtId = GetControl<Text>("txt_id");
            _txtName = GetControl<Text>("txt_name");
            _txtRound = GetControl<Text>("txt_round");
            _txtChallenge1 = GetControl<Text>("txt_challenge1");
            _txtChallenge2 = GetControl<Text>("txt_challenge2");
            _txtChallenge3 = GetControl<Text>("txt_challenge3");

            _btnBack.onClick.AddListener(HideMe);
            _btnLeft.onClick.AddListener(OnBtnLeft);
            _btnRight.onClick.AddListener(OnBtnRight);
            _svReward.itemRenderer = OnItemRenderer;
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            //默认打开时显示第一关
            _episodeId = 1;
            _info = EpisodeMgr.Instance.GetEpisodeInfo(_episodeId);
            _btnFight.SetData("任务开始", OnFight);
            UpdateView();
        }

        private void UpdateView()
        {
            //根据info
            _txtId.text = $"{_episodeId / 10 + 1}-{_episodeId % 10}";
            _txtName.text = _info.name;
            _txtRound.text = $"<color=green>{_info.round}</color> 回合";
            _txtChallenge1.text = $"{_info.perfectRound}回合内通关";
            _txtChallenge2.text = $"{_info.perfectTime}秒内通关";
            _txtChallenge3.text = _info.perfectDieCount <= 0 ? "全员存活" : $"伤亡不超过{_info.perfectDieCount}人";
            _svReward.numItems = _info.rewardItems.Length + _info.rareItems.Length;
            ResMgr.Instance.LoadAsync<Texture>(GameConst.RES_GROUP_UI, ResPath.GetMiniMap(_episodeId), img =>
            {
                _rawMap.texture = img;
            });
            UpdateLrBtn();
            
            //根据人物data
            var episodeItem = PlayerMgr.Instance.CurPlayer.Episodes[_episodeId];
            _imgStar1.gameObject.SetActive(episodeItem.minPassRound <= _info.perfectRound);
            _imgStar2.gameObject.SetActive(episodeItem.minPassTime <= _info.perfectTime);
            _imgStar3.gameObject.SetActive(episodeItem.minPassDeadCount <= _info.perfectDieCount);
            bool isComplete = episodeItem.minPassRound <= _info.perfectRound &&
                              episodeItem.minPassTime <= _info.perfectTime &&
                              episodeItem.minPassDeadCount <= _info.perfectDieCount;
            _imgComplete.gameObject.SetActive(isComplete);
        }

        private void OnBtnLeft()
        {
            --_episodeId;
            _info = EpisodeMgr.Instance.GetEpisodeInfo(_episodeId);
            UpdateView();
        }

        private void OnBtnRight()
        {
            ++_episodeId;
            _info = EpisodeMgr.Instance.GetEpisodeInfo(_episodeId);
            UpdateView();
        }

        private void UpdateLrBtn()
        {
            if (EpisodeMgr.Instance.EpisodeCount <= 1)
            {
                _btnLeft.gameObject.SetActive(false);
                _btnRight.gameObject.SetActive(false);
                return;
            }
            
            _btnLeft.gameObject.SetActive(_episodeId > 1);
            _btnRight.gameObject.SetActive(_episodeId < EpisodeMgr.Instance.EpisodeCount);
        }

        private void OnItemRenderer(int index, GameObject obj)
        {
            var item = obj.GetComponent<UI_reward_item>();
            int len = _info.rewardItems.Length;
            if(index <= len - 1)
                item.SetData(_info.rewardItems[index], _info.rewardNum[index], false);
            else
                item.SetData(_info.rareItems[index - len], _info.rareItemNum[index - len], true);
        }

        private void OnFight()
        {
            ProcedureMgr.Instance.SetData(BattleConst.MAP_KEY, _info);
            UIMgr.Instance.ShowPanel<FormationPanel>();
            HideMe();
            //ProcedureMgr.Instance.ChangeState<BattleProcedure>();
        }
    }
}