using Game.UI.Controls;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Game
{
    public class EpisodePop : PopBase
    {
        private RawImage _rawMap;
        private Image _imgStar;
        private Image _imgComplete;
        private HList _svReward;
        private UI_btn_big _btnFight;
        private Button _btnBack;
        private Text _txtId;
        private Text _txtName;
        private Text _txtRound;
        private Text _txtChallenge;

        protected internal override void OnInit(object userData)
        {
            IsFullScreen = true;
            base.OnInit(userData);
            _rawMap = GetControl<RawImage>("raw_map");
            _imgStar = GetControl<Image>("img_star");
            _imgComplete = GetControl<Image>("img_complete");
            _svReward = GetControl<HList>("sv_reward");
            _btnFight = GetControl<UI_btn_big>("btn_fight");
            _btnBack = GetControl<Button>("btn_back");
            _txtId = GetControl<Text>("txt_id");
            _txtName = GetControl<Text>("txt_name");
            _txtRound = GetControl<Text>("txt_round");
            _txtChallenge = GetControl<Text>("txt_challenge");
        }
    }
}