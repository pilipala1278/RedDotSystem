using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class RedDotModel
{
    //根节点
    private readonly RedDotTrieNode _root;
    private Dictionary<ERedDotType, ERedDotType[]> _redDotPathDict;

    public RedDotModel()
    {
        _root = new RedDotTrieNode(ERedDotType.Root, null, 0);
        _redDotPathDict = new Dictionary<ERedDotType, ERedDotType[]>(new RedDotTypeComparer());
    }

    /// <summary>
    /// 添加红点并注册计算方法
    /// 如果是无自身计算的节点 calcFunc可传null (或只调用AddRedDotNode方法)
    /// </summary>
    /// <param name="calcFunc">计算方法</param>
    /// <param name="paramPath">节点路径</param>
    /// <returns></returns>
    public void RegisterRedDotNode(Func<int> calcFunc, params ERedDotType[] paramPath)
    {
        if (paramPath == null)
        {
            return;
        }

        RedDotTrieNode node = AddRedDotNode(paramPath);
        node.RegisterCalculateFunc(calcFunc);
    }

    public RedDotTrieNode AddRedDotNode(params ERedDotType[] paramPath)
    {
        RedDotTrieNode node = GetOrAddNode(paramPath);
        _redDotPathDict[node.RedDotType] = paramPath;
        return node;
    }

    /// <summary>
    /// 删除节点时需考虑是否有子节点 否则删除会影响子节点
    /// </summary>
    public bool RemoveRedDotNodeByRedDotType(ERedDotType redDotType)
    {
        if (!GetRedDotPathByType(redDotType, out ERedDotType[] paramPath))
        {
            return false;
        }

        RedDotTrieNode tempNode = _root;
        foreach (ERedDotType type in paramPath)
        {
            if (!tempNode.Children.TryGetValue(type, out tempNode))
            {
                return false;
            }
        }

        _redDotPathDict.Remove(redDotType);
        if ((!tempNode.IsRoot) && tempNode.Parent.Children.Remove(tempNode.RedDotType))
        {
            tempNode.Clear();
            return true;
        }

        return false;
    }

    public RedDotTrieNode GetRedDotNode(ERedDotType redDotType)
    {
        if (!GetRedDotPathByType(redDotType, out ERedDotType[] paramPath))
        {
            return null;
        }

        RedDotTrieNode tempNode = _root;
        foreach (ERedDotType type in paramPath)
        {
            if (!tempNode.Children.TryGetValue(type, out tempNode))
            {
                return null;
            }
        }
        return tempNode;
    }

    public int GetRedDotNodeCount(ERedDotType redDotType)
    {
        return GetRedDotNode(redDotType)?.Count ?? 0;
    }

    /// <summary>
    /// 添加监听时请确保已经注册过传入节点类型的计算方法
    /// </summary>
    /// <param name="action"></param>
    /// <param name="redDotType"></param>
    public void AddRefreshListener(ERedDotType redDotType, Action<ERedDotType, int> refreshAction)
    {
        if (refreshAction == null)
        {
            return;
        }

        RedDotTrieNode node = GetRedDotNode(redDotType);
        node?.AddRefreshListener(refreshAction);
    }

    public void RemoveRefreshListener(ERedDotType redDotType, Action<ERedDotType, int> refreshAction)
    {
        if (refreshAction == null)
        {
            return;
        }

        RedDotTrieNode node = GetRedDotNode(redDotType);
        node?.RemoveRefreshListener(refreshAction);
    }

    public void RemoveAllRefreshListener(ERedDotType redDotType)
    {
        RedDotTrieNode node = GetRedDotNode(redDotType);
        node?.RemoveAllRefreshListener();
    }

    private bool GetRedDotPathByType(ERedDotType redDotType, out ERedDotType[] paramPath)
    {
        if (!_redDotPathDict.TryGetValue(redDotType, out paramPath))
        {
            Debug.LogError($"未找到红点类型: <{redDotType}> 的节点路径, 请确保该类型已经初始化或检查是否被删除");
            paramPath = null;
            return false;
        }

        return true;
    }

    private RedDotTrieNode GetOrAddNode(params ERedDotType[] paramPath)
    {
        RedDotTrieNode tempNode = _root;
        for (int i = 0; i < paramPath.Length; i++)
        {
            ERedDotType type = paramPath[i];
            if (!tempNode.Children.TryGetValue(type, out RedDotTrieNode childNode))
            {
                childNode = new RedDotTrieNode(type, tempNode, i);
                tempNode.Children[type] = childNode;
            }

            tempNode = childNode;
        }

        return tempNode;
    }

    /// <summary>
    /// 打印树结构为 Json 格式
    /// </summary>
    /// <param name="isLogData">是否打印节点里数据</param>
    public void PrintJson(bool isLogData)
    {
        var josnObj = BuildJson(_root, isLogData);
        Debug.LogError(JsonConvert.SerializeObject(josnObj));
    }

    private object BuildJson(RedDotTrieNode node, bool isLogData)
    {
        if (node.Children.Count > 0)
        {
            var childrenJson = new Dictionary<string, object>();
            foreach (var child in node.Children.Values)
            {
                if (isLogData)
                {
                    var childJsonData = new Dictionary<string, object>()
                    {
                        { "Count", child.Count },
                        { "Deep", child.Deep },
                        { "Child", BuildJson(child, isLogData)}
                    };
                    childrenJson[child.RedDotType.ToString()] = childJsonData;
                }
                else
                {
                    childrenJson[child.RedDotType.ToString()] = BuildJson(child, isLogData);
                }
            }
            return childrenJson;
        }

        return new { };
    }
}

public class RedDotTrieNode
{
    public ERedDotType RedDotType { get; }
    public int Count { get; private set; }
    public int Deep { get; }
    public bool IsRoot { get { return Parent == null; } }
    public bool IsValid { get; private set; }
    public RedDotTrieNode Parent { get; }
    public Dictionary<ERedDotType, RedDotTrieNode> Children { get; }

    private Action<ERedDotType, int> _onRefreshCallback;
    private Func<int> _onCalculateFunc;

    public RedDotTrieNode(ERedDotType type, RedDotTrieNode parent, int deep)
    {
        RedDotType = type;
        Parent = parent;
        Deep = deep;
        Children = new Dictionary<ERedDotType, RedDotTrieNode>(new RedDotTypeComparer());
        IsValid = true;
    }

    public void Clear()
    {
        IsValid = false;
    }

    public void RegisterCalculateFunc(Func<int> calcFunc)
    {
        if (_onCalculateFunc != null)
        {
            Debug.LogError($"RegisterCalculateFunc 添加失败 类型:{RedDotType} 已添加过计算方法, 请勿重复添加!");
            return;
        }

        _onCalculateFunc = calcFunc;
    }

    public int CalculateCount()
    {
        int prevCount = Count;
        //计算自身
        Count = _onCalculateFunc?.Invoke() ?? 0;

        //计算子节点
        foreach (var childNode in Children.Values)
        {
            Count += childNode.Count;
        }

        if (prevCount != Count)
        {
            TriggerRefresh();
        }

        return Count;
    }

    public void TriggerRefresh()
    {
        _onRefreshCallback?.Invoke(RedDotType, Count);
    }

    public void AddRefreshListener(Action<ERedDotType, int> refreshAction)
    {
        if (refreshAction != null)
        {
            _onRefreshCallback += refreshAction;
        }
    }

    public void RemoveRefreshListener(Action<ERedDotType, int> refreshAction)
    {
        if (refreshAction != null)
        {
            _onRefreshCallback -= refreshAction;
        }
    }

    public void RemoveAllRefreshListener()
    {
        _onRefreshCallback = null;
    }
}
