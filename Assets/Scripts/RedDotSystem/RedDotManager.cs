using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDotManager
{
    private const float k_refresh_interval = 0.1f;

    private static RedDotManager _instance = null;
    public static RedDotManager Instance
    {
        get
        {
            _instance ??= new RedDotManager();
            return _instance;
        }
    }

    private bool _isInit = false;
    private RedDotModel _redDotModel;
    private HashSet<RedDotTrieNode> _dirtyNodeSet;
    private float _timer;

    public void Init()
    {
        _redDotModel = new RedDotModel();
        _dirtyNodeSet = new HashSet<RedDotTrieNode>();

        _isInit = true;
    }

    #region 红点节点相关
    public void RegisterAndAddRedDot(Func<int> calcFunc, params ERedDotType[] paramPath)
    {
        _redDotModel.RegisterRedDotNode(calcFunc, paramPath);
    }

    public void AddRedDotNode(params ERedDotType[] paramPath)
    {
        _redDotModel.AddRedDotNode(paramPath);
    }

    public void RemoveRedDot(ERedDotType redDotType)
    {
        RefreshRedDot(redDotType);
        _redDotModel.RemoveRedDotNodeByRedDotType(redDotType);
    }

    public RedDotTrieNode GetRedDot(ERedDotType redDotType)
    {
        return _redDotModel.GetRedDotNode(redDotType);
    }

    public int GetRedDotValue(ERedDotType redDotType)
    {
        return _redDotModel.GetRedDotNodeCount(redDotType);
    }
    #endregion

    #region 刷新监听相关
    public void AddRefreshListener(ERedDotType redDotType, Action<ERedDotType, int> refreshAction, bool isAutoRefresh = false)
    {
        _redDotModel.AddRefreshListener(redDotType, refreshAction);
        if (isAutoRefresh)
        {
            RefreshRedDot(redDotType);
        }
    }

    public void RemoveRefreshListener(ERedDotType redDotType, Action<ERedDotType, int> refreshAction)
    {
        _redDotModel.RemoveRefreshListener(redDotType, refreshAction);
    }

    public void RemoveAllRefreshListener(ERedDotType redDotType)
    {
        _redDotModel.RemoveAllRefreshListener(redDotType);
    }
    #endregion

    public void RefreshRedDot(ERedDotType redDotType)
    {
        RedDotTrieNode node = _redDotModel.GetRedDotNode(redDotType);
        if (node == null || node.IsRoot)
        {
            return;
        }

        if (_dirtyNodeSet.Add(node))
        {
            node = node.Parent;
            while (!node.IsRoot)
            {
                _dirtyNodeSet.Add(node);
                node = node.Parent;
            }
        }
    }

    public void OnUpdate(float deltaTime)
    {
        if (!_isInit)
        {
            return;
        }

        _timer += deltaTime;
        if (_timer >= k_refresh_interval)
        {
            _timer = 0;

            if (_dirtyNodeSet.Count > 0)
            {
                List<RedDotTrieNode> _sortNodeList = new(_dirtyNodeSet);
                _sortNodeList.Sort((x, y) =>
                {
                    if (x == null && y == null) return 0;
                    if (x == null) return 1;
                    if (y == null) return -1;
                    return y.Deep.CompareTo(x.Deep);
                });

                foreach (RedDotTrieNode node in _sortNodeList)
                {
                    //针对失效红点进行剔除 以免造成无用计算
                    if (node != null && node.IsValid)
                    {
                        node.CalculateCount();
                    }
                }

                _dirtyNodeSet.Clear();
            }
        }
    }

    #region Debug
    public void PrintJson(bool isLogData = false)
    {
        _redDotModel.PrintJson(isLogData);
    }
    #endregion
}
