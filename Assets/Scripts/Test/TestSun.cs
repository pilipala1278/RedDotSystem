using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class TestSun : MonoBehaviour
{
    void Start()
    {
        ReddotMgr reddotMgr = new ReddotMgr();
        reddotMgr.Init();

        reddotMgr.AddReddotNode("主界面", "角色", "武器");
        reddotMgr.AddReddotNode("主界面", "角色", "防具");
        reddotMgr.AddReddotNode("主界面", "角色", "饰品");
        reddotMgr.AddReddotNode("主界面", "伙伴");
        reddotMgr.AddReddotNode("抽卡", "抽技能");

        reddotMgr.PrintReddotTreeJson();

        reddotMgr.RemoveReddotNode("主界面", "角色", "防具");
        reddotMgr.PrintReddotTreeJson();

    }
}

public class ReddotTrieNode
{
    private Action<int> _onVlaueChanged;

    public string Name { get; }
    /// <summary>
    /// 父节点
    /// </summary>
    public ReddotTrieNode Parent { get; }
    /// <summary>
    /// 子节点
    /// </summary>
    public Dictionary<string, ReddotTrieNode> Children { get; }
    public int Count { get; set; }

    public ReddotTrieNode(string name, ReddotTrieNode parent)
    {
        Name = name;
        Parent = parent;
        Children = new Dictionary<string, ReddotTrieNode>();
    }

    public void AddListener(Action<int> action)
    {
        if (action != null)
        {
            _onVlaueChanged += action;
        }
    }

    public void RemoveListener(Action<int> action)
    {
        if (action != null)
        {
            _onVlaueChanged -= action;
        }
    }

    public void RemoveAllListener()
    {
        _onVlaueChanged = null;
    }
}

public class ReddotMgr
{
    private ReddotTrieNode _root;
    private HashSet<ReddotTrieNode> _dirtyNoodSet;

    public void Init()
    {
        _root = new ReddotTrieNode("root", null);
        _dirtyNoodSet = new HashSet<ReddotTrieNode>();
    }

    public ReddotTrieNode AddReddotNode(params string[] paramPath)
    {
        ReddotTrieNode node = GetOrAddNode(paramPath);
        return node;
    }

    /// <summary>
    /// 删除节点时需考虑是否有子节点 否则删除会影响子节点
    /// </summary>
    public bool RemoveReddotNode(params string[] paramPath)
    {
        ReddotTrieNode tempNode = _root;
        foreach (string path in paramPath)
        {
            if (!tempNode.Children.TryGetValue(path, out tempNode))
            {
                return false;
            }
        }

        return tempNode.Parent.Children.Remove(tempNode.Name);
    }

    public ReddotTrieNode GetReddotNode(params string[] paramPath)
    {
        ReddotTrieNode tempNode = _root;
        foreach (string path in paramPath)
        {
            if (!tempNode.Children.TryGetValue(path, out tempNode))
            {
                return null;
            }
        }
        return tempNode;
    }

    public int GetReddotNodeCount(params string[] paramPath)
    {
        return GetReddotNode(paramPath)?.Count ?? 0;
    }

    private ReddotTrieNode GetOrAddNode(params string[] paramPath)
    {
        ReddotTrieNode tempNode = _root;
        foreach (string path in paramPath)
        {
            if (!tempNode.Children.TryGetValue(path, out ReddotTrieNode childNode))
            {
                childNode = new ReddotTrieNode(path, tempNode);
                tempNode.Children[path] = childNode;
            }

            tempNode = childNode;
        }

        return tempNode;
    }

    public ReddotTrieNode AddListener(Action<int> action, params string[] paramPath)
    {
        if (action == null)
        {
            return null;
        }

        ReddotTrieNode node = GetOrAddNode(paramPath);
        node?.AddListener(action);
        return node;
    }

    public void RemoveListener(Action<int> action, params string[] paramPath)
    {
        if (action == null)
        {
            return;
        }

        ReddotTrieNode node = GetOrAddNode(paramPath);
        node?.RemoveListener(action);
    }

    public void RemoveAllListener(params string[] paramPath)
    {
        ReddotTrieNode node = GetOrAddNode(paramPath);
        node?.RemoveAllListener();
    }

    public void AddDirtyNode(ReddotTrieNode node)
    {
        _dirtyNoodSet.Add(node);
    }

    public void OnUpdate()
    {
        if (_dirtyNoodSet.Count > 0)
        {
            foreach (var node in _dirtyNoodSet)
            {
                //当父节点 查询到自己的值有差异 即被修改，会触发外部的监听方法
                //node.GetNodeValue();
            }

            _dirtyNoodSet.Clear();
        }
    }

    // 打印树结构为 JSON 格式
    public void PrintReddotTreeJson()
    {
        var josnObj = BuildJson(_root);
        Debug.LogError(JsonConvert.SerializeObject(josnObj));
    }

    // 递归方法：构建节点及其子节点的 JSON 对象
    private object BuildJson(ReddotTrieNode node)
    {
        // 如果当前节点有子节点，递归构建子节点的 JSON 对象
        if (node.Children.Count > 0)
        {
            var childrenJson = new Dictionary<string, object>();
            foreach (var child in node.Children.Values)
            {
                childrenJson[child.Name] = BuildJson(child); // 递归处理每个子节点
            }
            return childrenJson;
        }

        return new { };
    }
}