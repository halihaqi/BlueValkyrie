using Hali_Framework;

namespace Game.BattleScene
{
    public class BattleOverState : FsmState<BattleMaster>
    {

        protected internal override void OnEnter(IFsm<BattleMaster> fsm)
        {
            base.OnEnter(fsm);
            // UIMgr.Instance.HideAllLoadedPanels();
            //
            // var bm = fsm.Owner;
            // bm.cam.gameObject.SetActive(false);
            // foreach (var student in bm.students)
            // {
            //     student.Fsm.ChangeState<StudentRestState>();
            //     student.transform.SetParent(bm.overMapEntity.showPos[student.RoleIndex]);
            //     student.transform.localPosition = Vector3.zero;
            //     student.transform.localRotation = Quaternion.identity;
            //     student.anim.SetLayerWeight(4, 1);
            //     DelayUtils.Instance.Delay(0.3f, 1, obj =>
            //     {
            //         student.anim.SetTrigger(Victory);
            //     });
            // }
        }
    }
}