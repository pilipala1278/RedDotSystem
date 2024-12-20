using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class TestSun : MonoBehaviour
{
    public Button AddNode_ABtn;
    public Button ReduceNode_ABtn;

    public Button AddNode_BBtn;
    public Button ReduceNode_BBtn;

    public Button AddNode_B1Btn;
    public Button ReduceNode_B1Btn;

    public Button AddAllNode_Btn;

    public Text Node_0Text;
    public Text Node_AText;
    public Text Node_BText;
    public Text Node_B1Text;

    private int _node_ACount = 0;
    private int _node_BCount = 0;
    private int _node_B1Count = 0;

    //private string[] _nodeAType = {"主界面", "角色", "A"};
    //private string[] _nodeBType = { "主界面", "角色", "B" };z

    void Start()
    {
        RedDotManager.Instance.Init();

        //var node_a = reddotManager.RedDotModel.AddReddotNode("主界面", "角色", "技能");
        //reddotManager.RedDotModel.GetReddotNode("主界面", "角色", "技能").RegisterCalculateFunc(CalculateNode_A);
        //reddotManager.RedDotModel.GetReddotNode("主界面", "角色", "技能").AddRefreshListener(OnRefreshNodeA);


        //var new_node_a = reddotManager.RedDotModel.RegisterReddotNode(null, "主界面", "角色", "技能");




        //var node_b = reddotManager.RedDotModel.AddReddotNode("主界面", "角色", "装备");
        //node_b.RegisterCalculateFunc(CalculateNode_B);
        //node_b.AddRefreshListener(OnRefreshNodeB);


        //var node_b1 = reddotManager.RedDotModel.AddReddotNode("主界面", "角色", "装备", "装备强化");
        //node_b1.RegisterCalculateFunc(() => { return _node_B1Count; });
        //node_b1.AddRefreshListener(OnRefreshNodeB1);


        //var node_0 = reddotManager.RedDotModel.AddReddotNode("主界面", "角色");
        //node_0.AddRefreshListener(OnRefreshNode0);

        View();

        //reddotManager.RedDotModel.PrintJson();
    }

    private void View()
    {
        //AddNode_ABtn.onClick.AddListener(() =>
        //{
        //    _node_ACount++;
        //    reddotManager.RefreshRedDot("主界面", "角色", "技能");
        //});
        //ReduceNode_ABtn.onClick.AddListener(() =>
        //{
        //    _node_ACount = Mathf.Max(_node_ACount - 1, 0);
        //    reddotManager.RefreshRedDot("主界面", "角色", "技能");
        //});

        //AddNode_BBtn.onClick.AddListener(() =>
        //{
        //    _node_BCount++;
        //    reddotManager.RefreshRedDot("主界面", "角色", "装备");
        //});
        //ReduceNode_BBtn.onClick.AddListener(() =>
        //{
        //    _node_BCount = Mathf.Max(_node_BCount - 1, 0);
        //    reddotManager.RefreshRedDot("主界面", "角色", "装备");
        //});

        //AddNode_B1Btn.onClick.AddListener(() =>
        //{
        //    _node_B1Count++;
        //    reddotManager.RefreshRedDot("主界面", "角色", "装备", "装备强化");
        //});

        //ReduceNode_B1Btn.onClick.AddListener(() =>
        //{
        //    _node_B1Count = Mathf.Max(_node_B1Count - 1, 0);
        //    reddotManager.RefreshRedDot("主界面", "角色", "装备", "装备强化");
        //});

        //AddAllNode_Btn.onClick.AddListener(() =>
        //{
        //    _node_ACount++;
        //    _node_BCount++;
        //    _node_B1Count++;
        //    reddotManager.RefreshRedDot("主界面", "角色", "技能");
        //    reddotManager.RefreshRedDot("主界面", "角色", "装备");
        //    reddotManager.RefreshRedDot("主界面", "角色", "装备", "装备强化");
        //});
    }

    private int CalculateNode_A()
    {
        return _node_ACount;
    }

    private int CalculateNode_B()
    {
        return _node_BCount;
    }


    private void OnRefreshNode0(string name, int count)
    {
        Node_0Text.text = $"{name}:{count}";
        Debug.LogError("刷新角色(父节点)");
    }

    private void OnRefreshNodeA(string name, int count)
    {
        Node_AText.text = $"{name}:{count}";
    }

    private void OnRefreshNodeB(string name, int count)
    {
        Node_BText.text = $"{name}:{count}";
    }

    private void OnRefreshNodeB1(string name, int count)
    {
        Node_B1Text.text = $"{name}:{count}";
    }

    private void Update()
    {
        //reddotManager.OnUpdate(Time.deltaTime);
    }
}