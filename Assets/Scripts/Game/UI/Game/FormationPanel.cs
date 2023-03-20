using System.Collections;
using System.Collections.Generic;
using Game.Entity;
using Game.Global;
using Game.Managers;
using Game.Model;
using Game.UI.Controls;
using Game.UI.Controls.Formation;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Game
{
    public class FormationPanel : PanelBase
    {
        private static readonly Vector3[] _rolePos =
        {
            new Vector3(1.88f, 0, 0), new Vector3(0.53f, 0, 0), 
            new Vector3(-0.82f, 0, 0), new Vector3(-2.17f, 0, 0)
        };

        private UI_hub_form _hubForm;
        private HList _svFormationList;
        private RawImage _roleContainer;
        
        private FormationRoleEntity[] _formation = new FormationRoleEntity[4];
        private FormationRoleEntity _curDragRole;
        private int _curDragIndex;
        private Vector3 _roleBeginPos;

        private readonly Dictionary<int, UI_formation_role_info> _infoDic = 
            new Dictionary<int, UI_formation_role_info>
        {
            {0, null}, {1,null}, {2, null}, {3, null}
        };
        private Button _btnFight;
        private Button _btnFormation;

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _hubForm = GetControl<UI_hub_form>("hub_form");
            _svFormationList = GetControl<HList>("sv_formation_list");
            _roleContainer = GetControl<RawImage>("role_container");
            _infoDic[0] = GetControl<UI_formation_role_info>("formation_role_info_1");
            _infoDic[1] = GetControl<UI_formation_role_info>("formation_role_info_2");
            _infoDic[2] = GetControl<UI_formation_role_info>("formation_role_info_3");
            _infoDic[3] = GetControl<UI_formation_role_info>("formation_role_info_4");
            _btnFight = GetControl<Button>("btn_fight");
            _btnFormation = GetControl<Button>("btn_formation");
            _svFormationList.itemRenderer = OnItemRenderer;
            _svFormationList.onClickItem = OnClickItem;
            UIMgr.AddCustomEventListener(_roleContainer, EventTriggerType.BeginDrag, OnDragBegin);
            UIMgr.AddCustomEventListener(_roleContainer, EventTriggerType.Drag, OnDrag);
            UIMgr.AddCustomEventListener(_roleContainer, EventTriggerType.EndDrag, OnDragEnd);
            _btnFight.onClick.AddListener(OnFight);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            _hubForm.SetData(HideMe);
            _svFormationList.numItems = PlayerMgr.Instance.CurPlayer.Formation.Count;
            UIMgr.Instance.BindStageRT(_roleContainer);
            UIMgr.Instance.SetStageSize(1.5f);
            UpdateView();
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            UIMgr.Instance.ClearModel();
        }

        private void UpdateView()
        {
            StopAllCoroutines();
            UIMgr.Instance.RecycleAllModel();
            StartCoroutine(LoadModelCoroutine());
        }

        private IEnumerator LoadModelCoroutine()
        {
            var formation = PlayerMgr.Instance.CurFormation;
            int index = 0;
            while (index < formation.students.Length)
            {
                if (formation.students[index] == null)
                {
                    _infoDic[index].SetData(null);
                    ++index;
                    continue;
                }
                int index1 = index;
                LoadModel(formation.students[index], index, () =>
                {
                    _infoDic[index].SetData(formation.students[index]);
                    ++index;
                });
                
                //一个加载完才加载下一个
                while (index1 == index)
                    yield return null;
            }
        }
        private void LoadModel(StudentItem student, int formationIndex, UnityAction callback)
        {
            UIMgr.Instance.ShowModel(ResPath.GetStudentObj(student.roleId), obj =>
            {
                obj.transform.localPosition = _rolePos[formationIndex];
                var entity = obj.GetComponent<FormationRoleEntity>();
                entity ??= obj.AddComponent<FormationRoleEntity>();
                entity.SetStudent(student);
                _formation[formationIndex] = entity;
                callback?.Invoke();
            });
        }

        private void OnFight()
        {
            TipHelper.ShowConfirm("是否以该阵容出战？", () =>
            {
                ProcedureMgr.Instance.ChangeState<BattleProcedure>();
            });
        }

        private void OnItemRenderer(int index, GameObject obj)
        {
            var item = obj.GetComponent<UI_btn_normal>();
            item.SetData(PlayerMgr.Instance.CurPlayer.Formation[index].formationName);
        }

        private void OnClickItem(int index, ControlBase cb)
        {
            PlayerMgr.Instance.CurFormationIndex = index;
            UpdateView();
        }

        private void OnDragBegin(BaseEventData data)
        {
            var point = data as PointerEventData;
            var rawRect = _roleContainer.rectTransform;
            //鼠标在rawImage中的位置
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rawRect,
                point.position, point.pressEventCamera, out var localPos);
            //计算点在raw中的viewport坐标
            Vector2 viewport = new Vector2((localPos.x + rawRect.sizeDelta.x * rawRect.pivot.x) / rawRect.sizeDelta.x,
                (localPos.y + rawRect.sizeDelta.y * rawRect.pivot.y) / rawRect.sizeDelta.y);
            //获取RT中像素对应的世界坐标
            Ray ray = UIMgr.Instance.StageCamera.ViewportPointToRay(viewport);
            if (Physics.Raycast(ray, out var hit, 100))
            {
                for (int i = 0; i < _formation.Length; i++)
                {
                    if (hit.collider.gameObject == _formation[i].gameObject)
                    {
                        _curDragIndex = i;
                        _curDragRole = _formation[i];
                        _roleBeginPos = hit.transform.position;
                        _curDragRole.SetPick(true);
                        break;
                    }
                }
            }
        }

        private void OnDrag(BaseEventData data)
        {
            if(_curDragRole == null) return;
            var point = data as PointerEventData;
            var rawRect = _roleContainer.rectTransform;
            //鼠标在rawImage中的位置
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rawRect,
                point.position, point.pressEventCamera, out var localPos);
            //计算点在raw中的viewport坐标
            Vector2 viewport = new Vector2((localPos.x + rawRect.sizeDelta.x * rawRect.pivot.x) / rawRect.sizeDelta.x,
                (localPos.y + rawRect.sizeDelta.y * rawRect.pivot.y) / rawRect.sizeDelta.y);
            //获取RT中像素对应的世界坐标
            Vector3 worldPos = UIMgr.Instance.StageCamera.ViewportToWorldPoint(viewport);
            
            //因为人物锚点在脚上，要中间对着就减去一半身高
            //将人物往前一点，防止模型穿透，因为是正交所以大小不变
            _curDragRole.SetDragTarget(new Vector3(worldPos.x, worldPos.y - 1, _roleBeginPos.z + 2));
        }
        
        private void OnDragEnd(BaseEventData data)
        {
            if(_curDragRole == null) return;
            
            //检测结束时是否拖到别的位置
            var point = data as PointerEventData;
            var rawRect = _roleContainer.rectTransform;
            //鼠标在rawImage中的位置
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rawRect,
                point.position, point.pressEventCamera, out var localPos);
            //计算点在raw中的viewport坐标
            Vector2 viewport = new Vector2((localPos.x + rawRect.sizeDelta.x * rawRect.pivot.x) / rawRect.sizeDelta.x,
                (localPos.y + rawRect.sizeDelta.y * rawRect.pivot.y) / rawRect.sizeDelta.y);
            //获取RT中像素对应的世界坐标
            Ray ray = UIMgr.Instance.StageCamera.ViewportPointToRay(viewport);
            if (Physics.Raycast(ray, out var hit, 100))
            {
                for (int i = 0; i < _formation.Length; i++)
                {
                    if (hit.collider.gameObject == _formation[i].gameObject)
                    {
                        var exchangeRole = _formation[i];
                        //数据层交换
                        PlayerMgr.Instance.CurFormation.SwitchStudent(_curDragIndex, i);
                        (_formation[_curDragIndex], _formation[i]) = (exchangeRole, _curDragRole);
                        //显示层交换
                        _curDragRole.transform.position = exchangeRole.transform.position;
                        exchangeRole.transform.position = _roleBeginPos;
                        //更新UI
                        for (int j = 0; j < _formation.Length; j++)
                            _infoDic[j].SetData(_formation[j].Student);
                        
                        _curDragRole.SetPick(false);
                        _curDragRole = null;
                        _curDragIndex = -1;
                        return;
                    }
                }
            }
            
            //未交换
            //归位
            _curDragRole.SetPick(false);
            _curDragRole.transform.position = _roleBeginPos;
            _curDragRole = null;
            _curDragIndex = -1;
        }
    }
}