using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SunTesst : MonoBehaviour
{
    void Start()
    {

    }


}

public interface ITrieTreeNode
{
    /// <summary>
    /// 节点名称
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// 节点值
    /// </summary>
    int Value { get; set; }

    /// <summary>
    /// 父节点
    /// </summary>
    ITrieTreeNode Parent { get; set; }

    /// <summary>
    /// 当前节点的子节点
    /// </summary>
    Dictionary<string, ITrieTreeNode> Children { get; }

    /// <summary>
    /// 红点数量发送变化时触发
    /// </summary>
    Action<int> OnVlaueChanged { get; }

    /// <summary>
    /// 增加节点值
    /// </summary>
    void IncreaseNodeValue();

    /// <summary>
    /// 减少节点值
    /// </summary>
    void DecreaseNodeValue();

    /// <summary>
    /// 查询并统计当前节点的所有子节点值
    /// </summary>
    void GetNodeVlaue();

    /// <summary>
    /// 标记需要通知的父节点为脏
    /// </summary>
    void MarkParentDirty();
}

public class TreeNode
{
    public string name;
    public int num;
    public Dictionary<string, TreeNode> childDic = new Dictionary<string, TreeNode>();
    public Action<int> onValueChange;
    public TreeNode parentNode;
    public int deep = 0;
    public TreeNode GetChild(string name)
    {
        if (!childDic.ContainsKey(name))
        {
            return null;
        }
        return childDic[name];
    }
    public TreeNode AddChild(string name)
    {
        if (!childDic.ContainsKey(name))
        {
            var tempNode = new TreeNode(name);
            childDic.Add(name, tempNode);
            childDic[name].num += num;
            return tempNode;
        }
        return childDic[name];
    }
    public void AddRedPointNum(int num = 1)
    {
        this.name += name;
    }
    public TreeNode(string mName)
    {
        name = mName;
    }

    public void AddListener(Action<int> action)
    {
        onValueChange += action;
        action.Invoke(num);
    }
    public void RemoveListener(Action<int> action)
    {
        onValueChange -= action;
        action.Invoke(num);
    }
    public void IncreaseNodeValue()
    {
        if (childDic.Count > 0)
        {
            Debug.LogError("只能在子节点添加红点");
            return;
        }
        num++;
        onValueChange?.Invoke(num);
    }
    public void DecreaseNodeValue()
    {
        if (childDic.Count > 0)
        {
            Debug.LogError("只能在子节点直接删除红点");
            return;
        }
        if (num > 0)
        {
            num--;
            onValueChange?.Invoke(num);
        }
    }
    //这个不是修改 这个是更新
    public void GetNodeValue()
    {
        var currentNum = 0;
        foreach (var child in childDic.Values)
        {
            currentNum += child.num;
        }
        if (num != currentNum)
        {
            num = currentNum;
            onValueChange?.Invoke(num);
        }
    }
}


public class RedSystem
{
    #region MyRegion

    private static RedSystem instance = new RedSystem();

    public static RedSystem Instance
    {
        get { return instance; }
    }
    public void Init()
    {
        root = new TreeNode("root");
        ditTreeNodes = new List<TreeNode>();
    }
    #endregion
    private TreeNode root;
    private List<TreeNode> ditTreeNodes;

    ///
    private TreeNode AddOrGetNode(params string[] path)
    {
        var tempNode = root;
        foreach (var pathName in path)
        {
            if (tempNode.GetChild(pathName) == null)
            {
                var tempChild = tempNode.AddChild(pathName);
                tempChild.parentNode = tempNode;
                Debug.Log("红点系统添加了新的" + pathName + "节点");
            }

            tempNode = tempNode.GetChild(pathName);
        }

        if (tempNode.deep == 0)
        {
            tempNode.deep = path.Length;
        }

        return tempNode;
    }

    public void AddListener(Action<int> action, params string[] path)
    {
        if (action != null)
        {
            AddOrGetNode(path).AddListener(action);
        }
    }

    public void AddPoint(int num = 1, params string[] path)
    {
        var node = AddOrGetNode(path);
        node.AddRedPointNum(num);
    }

    private TreeNode GetNode(params string[] path)
    {
        var tempNode = root;
        foreach (var pathName in path)
        {
            if (tempNode.GetChild(pathName) == null)
            {
                tempNode.AddChild(pathName);
                Debug.Log("红点系统添加了新的" + pathName + "节点");
            }
            tempNode = tempNode.GetChild(pathName);
        }

        return tempNode;
    }
    public int GetNodePointNum(params string[] path)
    {
        var tempNode = GetNode(path);
        return tempNode.num;
    }
    //这种方式符合单一职责原则，即红点系统负责管理回调的有效性，节点负责执行回调的逻辑。
    public void AddNodeNum(Action<int> callBack = null, params string[] path)
    {

        if (callBack != null)
        {
            var tempNode = AddOrGetNode(path);
            tempNode.AddListener(callBack);
        }
        AddNodeNum(path);
    }
    public void AddNodeNum(params string[] path)
    {
        var tempNode = AddOrGetNode(path);
        tempNode.IncreaseNodeValue();
        //父节点一条路上也要刷新
        tempNode = tempNode.parentNode;
        while (tempNode.parentNode != null)
        {
            if (!ditTreeNodes.Contains(tempNode))
            {
                ditTreeNodes.Add(tempNode);
            }

            tempNode = tempNode.parentNode;
        }
    }
    public void DeleteNode(params string[] path)
    {
        var tempNode = AddOrGetNode(path);
        tempNode.DecreaseNodeValue();
        //父节点一条路上也要刷新
        tempNode = tempNode.parentNode;
        while (tempNode.parentNode != null)
        {
            if (!ditTreeNodes.Contains(tempNode))
            {
                ditTreeNodes.Add(tempNode);
            }
            tempNode = tempNode.parentNode;
        }
    }
    public void Update()
    {
        if (ditTreeNodes.Count <= 0)
        {
            return;
        }
        //先排序
        for (int i = 0; i < ditTreeNodes.Count; i++)
        {
            for (int j = i; j < ditTreeNodes.Count - 1; j++)
            {
                if (ditTreeNodes[j].deep < ditTreeNodes[j + 1].deep)
                {
                    (ditTreeNodes[j], ditTreeNodes[j + 1]) = (ditTreeNodes[j + 1], ditTreeNodes[j]);
                }
            }
        }
        foreach (var nodes in ditTreeNodes)
        {
            //检查自己的是否需要刷新值
            nodes.GetNodeValue();
        }
        ditTreeNodes.Clear();
    }
}



///// <summary>
///// 红点管理器
///// </summary>
//public class RedotManager
//{
//    /// <summary>
//    /// 单例声明
//    /// </summary>
//    private static RedotManager instance;

//    public static RedotManager Instance
//    {
//        get
//        {
//            if (instance == null)
//            {
//                instance = new RedotManager();
//            }

//            return instance;
//        }
//    }

//    /// <summary>
//    /// 空的根节点
//    /// </summary>
//    public TreeNode Root { get; }

//    /// <summary>
//    /// 脏节点池
//    /// </summary>
//    private List<TreeNode> dirtyNodes { get; set; }

//    /// <summary>
//    /// 构造函数中 创建根节点
//    /// </summary>
//    public RedotManager()
//    {
//        Root = new TreeNode("Root");
//        dirtyNodes = new List<TreeNode>();
//    }

//    /// <summary>
//    /// 供外部调用的添加节点方法
//    /// </summary>
//    /// <param name="param_path"></param>
//    /// <returns></returns>
//    public TreeNode AddNode(params string[] param_path)
//    {
//        var node = GetOrAddTreeNode(param_path);
//        return node;
//    }

//    /// <summary>
//    /// 供外部调用的获取一个节点
//    /// </summary>
//    /// <param name="path_group"></param>
//    /// <returns></returns>
//    public TreeNode GetTreeNode(params string[] path_group)
//    {
//        var tempNode = Root;
//        foreach (var path in path_group)
//        {
//            tempNode = tempNode.GetChild(path);

//            if (tempNode == null)
//            {
//                return null;
//            }
//        }

//        return tempNode;
//    }

//    /// <summary>
//    /// 供外部调用的获取当前节点值的方法
//    /// </summary>
//    /// <param name="path"></param>
//    /// <returns></returns>
//    public int GetNodeValue(params string[] path)
//    {
//        var node = GetTreeNode(path);
//        if (node == null)
//        {
//            return 0;
//        }

//        return node.Value;
//    }

//    /// <summary>
//    /// 移除一个节点
//    /// </summary>
//    /// <param name="path_group"></param>
//    /// <returns></returns>
//    public bool RemoveTreeNode(params string[] path_group)
//    {
//        TreeNode node = Root;
//        foreach (var path in path_group)
//        {
//            node = node.GetChild(path);
//            if (node == null)
//            {
//                return false;
//            }
//        }

//        return node.Parent.RemoveChild(node.Name);
//    }

//    /// <summary>
//    /// 移除所有节点
//    /// </summary>
//    public void RemoveAllTreeNode()
//    {
//        Root.Dispose();
//    }

//    /// <summary>
//    /// 获取或添加一个节点
//    /// </summary>
//    /// <param name="path_group"></param>
//    /// <returns></returns>
//    private TreeNode GetOrAddTreeNode(params string[] path_group)
//    {
//        var tempNode = Root;

//        foreach (var path in path_group)
//        {
//            tempNode = tempNode.GetOrAddChild(path);

//            if (tempNode == null)
//            {
//                return null;
//            }
//        }

//        return tempNode;
//    }

//    /// <summary>
//    /// 额外添加 便于外部直接创建节点并注册红点值发生变化时的监听
//    /// </summary>
//    /// <param name="onChangeValue"></param>
//    /// <param name="param_path"></param>
//    /// <returns></returns>
//    public TreeNode AddListener(Action<int> onChangeValue, params string[] param_path)
//    {
//        if (onChangeValue == null)
//        {
//            return null;
//        }

//        var node = GetOrAddTreeNode(param_path);
//        node?.AddListener(onChangeValue);
//        return node;
//    }

//    /// <summary>
//    /// 移除对应节点的监听
//    /// </summary>
//    /// <param name="onChangeValue"></param>
//    /// <param name="path"></param>
//    public void RemoveListener(Action<int> onChangeValue, params string[] path)
//    {
//        if (onChangeValue == null)
//        {
//            return;
//        }

//        var node = GetTreeNode(path);
//        node.RemoveListener(onChangeValue);
//    }

//    /// <summary>
//    /// 移除对应节点的所有监听
//    /// </summary>
//    /// <param name="path"></param>
//    public void RemoveAllListener(params string[] path)
//    {
//        var node = GetTreeNode(path);
//        node.RemoveAllListener();
//    }


//    /// <summary>
//    /// 外部驱动，帧循环
//    /// </summary>
//    public void Update()
//    {
//        if (dirtyNodes <= 0)
//        {
//            return;
//        }

//        foreach (var node in dirtyNodes)
//        {
//            ///当父节点 查询到自己的值有差异 即被修改，会触发外部的监听方法
//            node.GetNodeValue();
//        }

//        dirtyNodes.Clear();
//    }

//    /// <summary>
//    /// 添加脏节点
//    /// </summary>
//    public void AddDirtyNode(TreeNode node)
//    {
//        if (!dirtyNodes.Contains(node))
//        {
//            dirtyNodes.Add(node);
//        }
//    }

//}