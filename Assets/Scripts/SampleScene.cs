using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScene : MonoBehaviour
{
    public ButtonRightLeftClick MainMenuBtn;
    public ButtonRightLeftClick MainMenu_SkillBtn;
    public ButtonRightLeftClick MainMenu_PetBtn;
    public ButtonRightLeftClick MainMenu_Skill_LevelUpBtn;
    public ButtonRightLeftClick MainMenu_Skill_StarUpBtn;
    public ButtonRightLeftClick MainMenu_Pet_NewBtn;

    private int _petData;
    private int _skillLevelUpData;
    private int _skillStarLevelUpData;

    private int _newPetData = 3;

    private void Awake()
    {
        RedDotManager.Instance.Init();

        RedDotManager.Instance.AddRedDotNode(ERedDotType.MainMenu);
        RedDotManager.Instance.AddRedDotNode(ERedDotType.MainMenu, ERedDotType.MainMenu_Skill);
        RedDotManager.Instance.RegisterAndAddRedDot(() => { return _petData; }, ERedDotType.MainMenu, ERedDotType.MainMenu_Pet);
        RedDotManager.Instance.RegisterAndAddRedDot(() => { return _skillLevelUpData; }, ERedDotType.MainMenu, ERedDotType.MainMenu_Skill, ERedDotType.Skill_LevelUp);
        RedDotManager.Instance.RegisterAndAddRedDot(() => { return _skillStarLevelUpData; }, ERedDotType.MainMenu, ERedDotType.MainMenu_Skill, ERedDotType.Skill_StarLevelUp);
    }

    void Start()
    {
        MainMenu_PetBtn.OnLeftClick = () =>
        {
            _petData = Mathf.Max(_petData + 1, 0);
            RedDotManager.Instance.RefreshRedDot(ERedDotType.MainMenu_Pet);
        };
        MainMenu_PetBtn.OnRightClick = () =>
        {
            _petData = Mathf.Max(_petData - 1, 0);
            RedDotManager.Instance.RefreshRedDot(ERedDotType.MainMenu_Pet);
        };

        MainMenu_Skill_LevelUpBtn.OnLeftClick = () =>
        {
            _skillLevelUpData = Mathf.Max(_skillLevelUpData + 1, 0);
            RedDotManager.Instance.RefreshRedDot(ERedDotType.Skill_LevelUp);
        };
        MainMenu_Skill_LevelUpBtn.OnRightClick = () =>
        {
            _skillLevelUpData = Mathf.Max(_skillLevelUpData - 1, 0);
            RedDotManager.Instance.RefreshRedDot(ERedDotType.Skill_LevelUp);
        };

        MainMenu_Skill_StarUpBtn.OnLeftClick = () =>
        {
            _skillStarLevelUpData = Mathf.Max(_skillStarLevelUpData + 1, 0);
            RedDotManager.Instance.RefreshRedDot(ERedDotType.Skill_StarLevelUp);
        };
        MainMenu_Skill_StarUpBtn.OnRightClick = () =>
        {
            _skillStarLevelUpData = Mathf.Max(_skillStarLevelUpData - 1, 0);
            RedDotManager.Instance.RefreshRedDot(ERedDotType.Skill_StarLevelUp);
        };

        MainMenu_Pet_NewBtn.OnLeftClick = () =>
        {
            _newPetData = Mathf.Max(_newPetData + 1, 0);
            RedDotManager.Instance.RefreshRedDot(ERedDotType.Pet_New);
        };
        MainMenu_Pet_NewBtn.OnRightClick = () =>
        {
            _newPetData = Mathf.Max(_newPetData - 1, 0);
            RedDotManager.Instance.RefreshRedDot(ERedDotType.Pet_New);
        };
        MainMenu_Pet_NewBtn.gameObject.SetActive(false);

        RedDotManager.Instance.AddRefreshListener(ERedDotType.MainMenu, OnRefreshReddot);
        RedDotManager.Instance.AddRefreshListener(ERedDotType.MainMenu_Skill, OnRefreshReddot);
        RedDotManager.Instance.AddRefreshListener(ERedDotType.MainMenu_Pet, OnRefreshReddot);
        RedDotManager.Instance.AddRefreshListener(ERedDotType.Skill_LevelUp, OnRefreshReddot);
        RedDotManager.Instance.AddRefreshListener(ERedDotType.Skill_StarLevelUp, OnRefreshReddot);


    }

    private void OnRefreshReddot(ERedDotType reddotType, int count)
    {
        if (reddotType == ERedDotType.MainMenu)
        {
            MainMenuBtn.SetNameText(reddotType.ToString());
            MainMenuBtn.SetValue(count);
        }
        else if (reddotType == ERedDotType.MainMenu_Skill)
        {
            MainMenu_SkillBtn.SetNameText(reddotType.ToString());
            MainMenu_SkillBtn.SetValue(count);
        }
        else if (reddotType == ERedDotType.MainMenu_Pet)
        {
            MainMenu_PetBtn.SetNameText(reddotType.ToString());
            MainMenu_PetBtn.SetValue(count);
        }
        else if (reddotType == ERedDotType.Skill_LevelUp)
        {
            MainMenu_Skill_LevelUpBtn.SetNameText(reddotType.ToString());
            MainMenu_Skill_LevelUpBtn.SetValue(count);
        }
        else if (reddotType == ERedDotType.Skill_StarLevelUp)
        {
            MainMenu_Skill_StarUpBtn.SetNameText(reddotType.ToString());
            MainMenu_Skill_StarUpBtn.SetValue(count);
        }
    }

    private void OnDynamicAddRedDot()
    {
        Debug.Log("动态添加红点");
        MainMenu_Pet_NewBtn.gameObject.SetActive(true);

        RedDotManager.Instance.RegisterAndAddRedDot(() => { return _newPetData; }, ERedDotType.MainMenu, ERedDotType.MainMenu_Pet, ERedDotType.Pet_New);
        RedDotManager.Instance.AddRefreshListener(ERedDotType.Pet_New, OnRefreshPetNew, true);
    }

    private void OnRefreshPetNew(ERedDotType reddotType, int count)
    {
        MainMenu_Pet_NewBtn.SetNameText(reddotType.ToString());
        MainMenu_Pet_NewBtn.SetValue(count);

        Debug.Log("---- OnRefreshPetNew");
    }

    private void OnDynamicRemoveRedDot()
    {
        MainMenu_Pet_NewBtn.gameObject.SetActive(false);

        //RedDotManager.Instance.RemoveRefreshListener(ERedDotType.Pet_New, OnRefreshPetNew);
        RedDotManager.Instance.RemoveRedDot(ERedDotType.Pet_New);

        Debug.Log("动态删除红点");

        RedDotManager.Instance.PrintJson();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnDynamicAddRedDot();
            //OnDynamicRemoveRedDot();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {            
            OnDynamicRemoveRedDot();
        }

        RedDotManager.Instance.OnUpdate(Time.deltaTime);
    }
}
