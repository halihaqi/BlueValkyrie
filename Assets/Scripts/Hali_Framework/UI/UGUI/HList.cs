using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hali_Framework
{
    public enum ListLayoutType
    {
        Horizontal,
        Vertical,
        SingleColumn,
        SingleRow,
    }

    public delegate void ListItemRenderer(int index, GameObject item);
    
    [RequireComponent(typeof(ScrollRect))]
    public class HList : UIBehaviour
    {
        private class ItemInfo
        {
            public GameObject obj;
            public uint updateFlag;
            public bool selected;
        }
        
        //Unity设置
        [Header("Base")]
        [SerializeField]
        private GameObject defaultItem = null;
        [SerializeField]
        private ListLayoutType layoutType = ListLayoutType.Horizontal;
        [SerializeField]
        private bool isVirtual = false;
        [SerializeField]
        private Vector2 space = Vector2.zero;

        [Space]
        [Header("Padding")]
        [SerializeField]
        private float left = 0;
        [SerializeField]
        private float top = 0;

        public ListItemRenderer itemRenderer;
        
        private ScrollRect _sv;
        private ObjectPool _pool;
        
        private Vector2 _itemSize;
        private int _columnCount;
        private int _rowCount;
        
        //normal list
        private List<GameObject> _children;

        //virtual list
        private List<ItemInfo> _virtualItems;
        private int _numItems;
        private int _firstIndex;
        private int _lastIndex;
        private uint _itemInfoVer;//用来标志item是否在本次处理中已被重用
        

        public RectTransform Content => _sv.content;
        
        public RectTransform Viewport => _sv.viewport;
        
        public GameObject DefaultItem
        {
            get => defaultItem;
            set
            {
                if(value != null && defaultItem == value) return;
                defaultItem = value;
                _itemSize = defaultItem.GetComponent<RectTransform>().sizeDelta;
                SetColumnRow();
            }
        }

        public bool IsVirtual
        {
            get => isVirtual;
            set
            {
                isVirtual = value;
                SetScrollEvent();
            }
        }

        public int numItems
        {
            get
            {
                if (isVirtual)
                    return _numItems;
                return _children.Count;
            }
            set
            {
                if (isVirtual)
                {
                    if (itemRenderer == null)
                        throw new Exception("Set itemRenderer first!");

                    _numItems = value;
                    int oldCount = _virtualItems.Count;
                    if (_numItems > oldCount)
                    {
                        for (int i = oldCount; i < _numItems; i++)
                        {
                            ItemInfo ii = new ItemInfo();
                            _virtualItems.Add(ii);
                        }
                    }
                    else
                    {
                        for (int i = _numItems; i < oldCount; i++)
                            _virtualItems[i].selected = false;
                    }
                    
                    SetContentSize();
                    RefreshVirtualList(true);
                }
                else
                {
                    int count = _children.Count;
                    if (value > count)
                    {
                        for (int i = count; i < value; i++)
                            _children.Add(AddChildFromPool());
                    }
                    else
                    {
                        for (int i = value; i < count; i++)
                        {
                            PushToPool(_children[i]);
                            _children.RemoveAt(i);
                        }
                    }
                    SetContentSize();
                    RefreshNormalList();
                }
            }
        }

        public Vector2 Padding
        {
            get => new(left, top);
            set
            {
                if (Math.Abs(value.x - left) < 0.001f && Math.Abs(value.y - top) < 0.001f) return;
                left = value.x;
                top = value.y;
                RefreshList();
            }
        }

        public Vector2 Space
        {
            get => space;
            set
            {
                if(value == space) return;
                space = value;
                RefreshList();
            }
        }

        protected override void Awake()
        {
            _sv ??= GetComponent<ScrollRect>();
            if (_sv == null)
                throw new Exception("UI list has no ScrollView.");

            _children = new List<GameObject>();
            _virtualItems = new List<ItemInfo>();
            
            //生成独立对象池
            var poolObj = new GameObject("ListPool");
            poolObj.transform.SetParent(_sv.transform, false);
            _pool = new ObjectPool(this.gameObject.name, poolObj);

            Init();
        }

        protected override void OnValidate()
        {
            _sv ??= GetComponent<ScrollRect>();
            _children ??= new List<GameObject>();
            _sv.horizontal = layoutType is ListLayoutType.Horizontal or ListLayoutType.SingleRow;
            _sv.vertical = layoutType is ListLayoutType.Vertical or ListLayoutType.SingleColumn;
            SetContentAnchors();
            if(Content.childCount <= 0) return;
            EditorApplication.delayCall = () =>
            {
                _children.Clear();
                for (int i = 0; i < Content.childCount; i++)
                    _children.Add(Content.GetChild(i).gameObject);
                _numItems = _children.Count;
                _itemSize = defaultItem.GetComponent<RectTransform>().sizeDelta;
                SetColumnRow();
                SetContentSize();
                RefreshNormalList();
            };
        }

        #region 外部接口

        /// <summary>
        /// 强制重新渲染List
        /// </summary>
        public void RefreshList()
        {
            SetColumnRow();
            SetContentSize();
            if (isVirtual)
                RefreshVirtualList(true);
            else
                RefreshNormalList();
        }

        #endregion

        #region pool

        private GameObject AddChildAt(int index)
        {
            var obj = AddChildFromPool();
            obj.transform.SetSiblingIndex(index);
            return obj;
        }
        
        private GameObject AddChildFromPool()
        {
            GameObject obj = null;
            if (_pool.CacheNum <= 0)
            {
                obj = Instantiate(defaultItem.gameObject, Content, false);
                if(obj.TryGetComponent(out ControlBase cb))
                    cb.OnInit();
            }
            else
            {
                obj = _pool.Pop();
                obj.transform.SetParent(Content, false);
            }
            return obj;
        }

        private void PushToPool(GameObject obj)
        {
            if(obj.TryGetComponent(out ControlBase cb))
                cb.OnRecycle();
            _pool.Push(obj);
        }

        #endregion

        #region list

        private void Init()
        {
            _sv.horizontal = layoutType is ListLayoutType.Horizontal or ListLayoutType.SingleRow;
            _sv.vertical = layoutType is ListLayoutType.Vertical or ListLayoutType.SingleColumn;
            SetContentAnchors();
            
            _itemSize = defaultItem.GetComponent<RectTransform>().sizeDelta;
            SetScrollEvent();
            SetColumnRow();
            numItems = 0;
        }

        /// <summary>
        /// 设置行列数，在每次设置DefaultItem时调用
        /// </summary>
        private void SetColumnRow()
        {
            switch (layoutType)
            {
                case ListLayoutType.Horizontal://水平
                    _columnCount = -1;
                    _rowCount = Mathf.CeilToInt((Viewport.rect.height - top) / (_itemSize.y + space.y)) - 1;
                    break;
                case ListLayoutType.SingleRow://单行
                    _columnCount = -1;
                    _rowCount = 1;
                    break;
                case ListLayoutType.Vertical://竖直
                    _columnCount = Mathf.CeilToInt((Viewport.rect.width - left) / (_itemSize.x + space.x)) - 1;
                    _rowCount = -1;
                    break;
                case ListLayoutType.SingleColumn://单列
                    _columnCount = 1;
                    _rowCount = -1;
                    break;
            }
        }

        /// <summary>
        /// 统一设置content锚点为左上角
        /// </summary>
        private void SetContentAnchors()
        {
            Content.pivot = Vector2.up;
            Content.anchorMin = Vector2.up;
            Content.anchorMax = Vector2.up;
        }

        /// <summary>
        /// 根据列表项个数设置content大小
        /// </summary>
        private void SetContentSize()
        {
            int count = isVirtual ? _numItems : _children.Count;
            var rect = Viewport.rect;
            if (layoutType is ListLayoutType.Horizontal or ListLayoutType.SingleRow)
            {
                int columnCount = count / _rowCount + (count % _rowCount == 0 ? 0 : 1);
                float x = left + columnCount * (_itemSize.x + space.x);
                Content.sizeDelta = new Vector2(x, rect.height);
            }
            else if (layoutType is ListLayoutType.Vertical or ListLayoutType.SingleColumn)
            {
                int rowCount = count / _columnCount + (count % _columnCount == 0 ? 0 : 1);
                float y = top + rowCount * (_itemSize.y + space.y);
                Content.sizeDelta = new Vector2(rect.width, y);
            }
        }

        /// <summary>
        /// 滚动时调用
        /// </summary>
        /// <param name="percent"></param>
        private void OnScroll(Vector2 percent)
        {
            RefreshVirtualList(false);
        }

        private void SetScrollEvent()
        {
            if (isVirtual)
            {
                _sv.onValueChanged.RemoveListener(OnScroll);
                _sv.onValueChanged.AddListener(OnScroll);
            }
            else
                _sv.onValueChanged.RemoveListener(OnScroll);
        }

        /// <summary>
        /// 更新虚拟列表
        /// </summary>
        /// <param name="forceUpdate">是否强制更新view，如果为true，view中所有item都会更新</param>
        private void RefreshVirtualList(bool forceUpdate)
        {
            if (layoutType is ListLayoutType.SingleRow or ListLayoutType.Horizontal)
                HandleHorizontalScroll(forceUpdate);
            else if (layoutType is ListLayoutType.SingleColumn or ListLayoutType.Vertical)
                HandleVerticalScroll(forceUpdate);
        }

        #region virtual list method

        private void HandleVerticalScroll(bool forceUpdate)
        {
            var contentRect = Content.rect;
            //当前view顶部y值
            float posY = -Content.anchoredPosition.y;
            //view底部y值
            float maxY = posY - Viewport.rect.height;
            //是否滚动到底部
            bool isEnd = -maxY >= contentRect.height;

            int newFirstIndex = GetIndexOnPosY(ref posY, forceUpdate);
            //如果滚动后view第一个还是原来的第一个并且不是强制更新，不用更新
            if (newFirstIndex == _firstIndex && !forceUpdate) return;
            
            //-- 滚动后更新逻辑 --//

            int oldFirstIndex = _firstIndex;
            _firstIndex = newFirstIndex;
            int curIndex = newFirstIndex;
            bool needRender = false;//是否需要渲染
            int childCount = Content.childCount;
            bool isForward = oldFirstIndex > newFirstIndex;//是否向前滚动
            int lastIndex = oldFirstIndex + childCount - 1;//滚动前view中最后一个
            int reuseIndex = isForward ? lastIndex : oldFirstIndex;//最适合重用的itemIndex
            Vector2 curPos = new Vector2(left, posY);

            //添加新的
            _itemInfoVer++;
            while (curIndex < _numItems && (isEnd || curPos.y > maxY))
            {
                var ii = _virtualItems[curIndex];

                //如果强制更新，也要更新已经渲染的item
                if (forceUpdate && ii.obj != null)
                {
                    PushToPool(ii.obj);
                    ii.obj = null;
                }

                if (ii.obj == null)
                {
                    //搜索最适合重用的item
                    if (isForward)
                    {
                        for (int i = reuseIndex; i >= oldFirstIndex; i--)
                        {
                            var ii2 = _virtualItems[i];
                            if (ii2.obj != null && ii2.updateFlag != _itemInfoVer)
                            {
                                ii.obj = ii2.obj;
                                ii2.obj = null;
                                if (i == reuseIndex)
                                    reuseIndex--;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = reuseIndex; i <= lastIndex; i++)
                        {
                            var ii2 = _virtualItems[i];
                            if (ii2.obj != null && ii2.updateFlag != _itemInfoVer)
                            {
                                ii.obj = ii2.obj;
                                ii2.obj = null;
                                if (i == reuseIndex)
                                    reuseIndex++;
                                break;
                            }
                        }
                    }

                    //如果找到可以重用的item
                    if (ii.obj != null)
                    {
                        ii.obj.transform.SetSiblingIndex(isForward ? curIndex - newFirstIndex : Content.childCount);
                    }
                    else
                    {
                        if (isForward)
                            ii.obj = AddChildAt(curIndex - newFirstIndex);
                        else
                            ii.obj = AddChildAt(Content.childCount);
                    }

                    needRender = true;
                }
                else
                    needRender = forceUpdate;

                //更新渲染
                if (needRender)
                    itemRenderer?.Invoke(curIndex, ii.obj);
                
                //更新位置
                ii.updateFlag = _itemInfoVer;
                ((RectTransform)ii.obj.transform).anchoredPosition = curPos;
                if (curIndex == newFirstIndex) //要显示多一条才不会穿帮
                    maxY -= _itemSize.y;
                curPos.x += _itemSize.x + space.x;
                if (curIndex % _columnCount == _columnCount - 1)
                {
                    curPos.x = left;
                    curPos.y -= _itemSize.y + space.y;
                }

                curIndex++;
            }

            //回收旧的
            for (int i = 0; i < childCount; i++)
            {
                var ii = _virtualItems[oldFirstIndex + i];
                if (ii.obj != null && ii.updateFlag != _itemInfoVer)
                {
                    PushToPool(ii.obj);
                    ii.obj = null;
                }
            }
        }

        private void HandleHorizontalScroll(bool forceUpdate)
        {
            var contentRect = Content.rect;
            //当前view左边界x值
            float posX = -Content.anchoredPosition.x;
            //view右边界x值
            float maxX = posX + Viewport.rect.width;
            //是否滚动到底部
            bool isEnd = maxX >= contentRect.width;

            int newFirstIndex = GetIndexOnPosX(ref posX, forceUpdate);
            //如果滚动后view第一个还是原来的第一个并且不是强制更新，不用更新
            if (newFirstIndex == _firstIndex && !forceUpdate) return;
            
            //-- 滚动后更新逻辑 --//

            int oldFirstIndex = _firstIndex;
            _firstIndex = newFirstIndex;
            int curIndex = newFirstIndex;
            bool needRender = false;//是否需要渲染
            int childCount = Content.childCount;
            bool isForward = oldFirstIndex > newFirstIndex;//是否向左滚动
            int lastIndex = oldFirstIndex + childCount - 1;//滚动前view中最后一个
            int reuseIndex = isForward ? lastIndex : oldFirstIndex;//最适合重用的itemIndex
            Vector2 curPos = new Vector2(posX, -top);

            //添加新的
            _itemInfoVer++;
            while (curIndex < _numItems && (isEnd || curPos.x < maxX))
            {
                var ii = _virtualItems[curIndex];

                //如果强制更新，也要更新已经渲染的item
                if (forceUpdate && ii.obj != null)
                {
                    PushToPool(ii.obj);
                    ii.obj = null;
                }

                if (ii.obj == null)
                {
                    //搜索最适合重用的item
                    if (isForward)
                    {
                        for (int i = reuseIndex; i >= oldFirstIndex; i--)
                        {
                            var ii2 = _virtualItems[i];
                            if (ii2.obj != null && ii2.updateFlag != _itemInfoVer)
                            {
                                ii.obj = ii2.obj;
                                ii2.obj = null;
                                if (i == reuseIndex)
                                    reuseIndex--;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = reuseIndex; i <= lastIndex; i++)
                        {
                            var ii2 = _virtualItems[i];
                            if (ii2.obj != null && ii2.updateFlag != _itemInfoVer)
                            {
                                ii.obj = ii2.obj;
                                ii2.obj = null;
                                if (i == reuseIndex)
                                    reuseIndex++;
                                break;
                            }
                        }
                    }

                    //如果找到可以重用的item
                    if (ii.obj != null)
                    {
                        ii.obj.transform.SetSiblingIndex(isForward ? curIndex - newFirstIndex : Content.childCount);
                    }
                    else
                    {
                        if (isForward)
                            ii.obj = AddChildAt(curIndex - newFirstIndex);
                        else
                            ii.obj = AddChildAt(Content.childCount);
                    }

                    needRender = true;
                }
                else
                    needRender = forceUpdate;

                //更新渲染
                if (needRender)
                    itemRenderer?.Invoke(curIndex, ii.obj);
                
                //更新位置
                ii.updateFlag = _itemInfoVer;
                ((RectTransform)ii.obj.transform).anchoredPosition = curPos;
                if (curIndex == newFirstIndex) //要显示多一条才不会穿帮
                    maxX += _itemSize.x;
                curPos.y -= _itemSize.y + space.y;
                if (curIndex % _rowCount == _rowCount - 1)
                {
                    curPos.y = -top;
                    curPos.x += _itemSize.x + space.x;
                }

                curIndex++;
            }

            //回收旧的
            for (int i = 0; i < childCount; i++)
            {
                var ii = _virtualItems[oldFirstIndex + i];
                if (ii.obj != null && ii.updateFlag != _itemInfoVer)
                {
                    PushToPool(ii.obj);
                    ii.obj = null;
                }
            }
        }

        private int GetIndexOnPosY(ref float posY, bool forceUpdate)
        {
            //如果总数小于一行
            if (_numItems < _columnCount)
            {
                posY = 0;
                return 0;
            }
            
            //如果content下有item
            if (Content.childCount > 0 && !forceUpdate)
            {
                //View中第一个item
                var firstItem = Content.GetChild(0);
                float firstRowPosY = ((RectTransform)firstItem).anchoredPosition.y;
                float curRowPosY = firstRowPosY;
                
                //如果第一个item底部在posY以上，找到posY以下的第一行第一个
                if (firstRowPosY - _itemSize.y > posY)
                {
                    //向下搜索每行第一个
                    for (int i = _firstIndex + _columnCount; i < _virtualItems.Count; i += _columnCount)
                    {
                        curRowPosY -= _itemSize.y + space.y;
                        //如果当前行底部在posY以下，则为找到
                        if (curRowPosY - _itemSize.y <= posY)
                        {
                            posY = curRowPosY;
                            return i;
                        }
                    }

                    //找不到说明content所有内容都在posY以上
                    //返回最后一行第一个
                    posY = curRowPosY;
                    return _numItems - _columnCount;
                }
                //如果第一个item底部在posY以下
                else
                {
                    //向上搜索每行第一个
                    for (int i = _firstIndex; i >= 0; i -= _columnCount)
                    {
                        //如果当前行顶部在posY以上，则为找到
                        if (curRowPosY > posY)
                        {
                            posY = curRowPosY;
                            return i;
                        }
                        curRowPosY += _itemSize.y + space.y;
                    }

                    posY = 0;
                    return 0;
                }
            }
            //如果content还没加item，遍历所有行，找到第一个
            else
            {
                float curRowPosY = -top;
                //向下搜索
                for (int i = 0; i < _numItems; i += _columnCount)
                {
                    //如果当前行底部在posY以下，则为找到
                    if (curRowPosY - _itemSize.y <= posY)
                    {
                        posY = curRowPosY;
                        return i;
                    }
                    curRowPosY -= _itemSize.y + space.y;
                }

                posY = curRowPosY;
                return _numItems - _columnCount;
            }
        }

        private int GetIndexOnPosX(ref float posX, bool forceUpdate)
        {
            //如果总数小于一行
            if (_numItems < _columnCount)
            {
                posX = 0;
                return 0;
            }
            
            //如果content下有item
            if (Content.childCount > 0 && !forceUpdate)
            {
                //View中第一个item
                var firstItem = Content.GetChild(0);
                float firstColumnPosX = ((RectTransform)firstItem).anchoredPosition.x;
                float curColumnPosX = firstColumnPosX;
                
                //如果第一个item右边界在posX左边，找到posX以右的第一列第一个
                if (firstColumnPosX + _itemSize.x <= posX)
                {
                    //向右搜索每列第一个
                    for (int i = _firstIndex + _rowCount; i < _virtualItems.Count; i += _rowCount)
                    {
                        curColumnPosX += _itemSize.x + space.x;
                        //如果当前列右边界在posX以右，则为找到
                        if (curColumnPosX + _itemSize.x > posX)
                        {
                            posX = curColumnPosX;
                            return i;
                        }
                    }
                    
                    posX = curColumnPosX;
                    return _numItems - _rowCount;
                }
                //如果第一个item左边界在posX右边
                else
                {
                    //向左搜索每列第一个
                    for (int i = _firstIndex; i >= 0; i -= _rowCount)
                    {
                        //如果当前列左边界在posX以左，则为找到
                        if (curColumnPosX <= posX)
                        {
                            posX = curColumnPosX;
                            return i;
                        }
                        curColumnPosX -= _itemSize.x + space.x;
                    }

                    posX = 0;
                    return 0;
                }
            }
            //如果content还没加item，遍历所有列，找到第一个
            else
            {
                float curColumnPosX = left;
                //向右搜索
                for (int i = 0; i < _numItems; i+= _rowCount)
                {
                    //如果当前列右边界在posX以右，则为找到
                    if (curColumnPosX + _itemSize.x > posX)
                    {
                        posX = curColumnPosX;
                        return i;
                    }
                    curColumnPosX += _itemSize.y + space.y;
                }

                posX = curColumnPosX;
                return _numItems - _rowCount;
            }
        }

        #endregion

        /// <summary>
        /// 更新普通列表，每次重设numItems时调用
        /// </summary>
        private void RefreshNormalList()
        {
            Vector2 curPos = new Vector2(left, -top);
            GameObject obj = null;
            Action<int> updatePos = layoutType switch
            {
                ListLayoutType.Horizontal or ListLayoutType.SingleRow => UpdateHorizontalOrSingleRow,
                ListLayoutType.Vertical or ListLayoutType.SingleColumn => UpdateVerticalOrSingleColumn,
                _ => null
            };

            for (int i = 0; i < _children.Count; i++)
            {
                int index = i;
                obj = _children[index];
                ((RectTransform)obj.transform).anchoredPosition = curPos;
                itemRenderer?.Invoke(index, obj);
                updatePos!.Invoke(i);
            }

            #region 内部方法
            void UpdateHorizontalOrSingleRow(int index)
            {
                if ((index + 1) % _rowCount == 0)
                {
                    curPos.y = -top;
                    curPos.x += _itemSize.x + space.x;
                }
                else
                    curPos.y += -space.y - _itemSize.y;
            }

            void UpdateVerticalOrSingleColumn(int index)
            {
                if ((index + 1) % _columnCount == 0)
                {
                    curPos.x = left;
                    curPos.y += -space.y - _itemSize.y;
                }
                else
                    curPos.x += _itemSize.x + space.x;
            }

            #endregion
        }

        #endregion
    }
}