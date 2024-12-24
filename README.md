# Unity基于前缀树的红点系统
## 描述
1. 红点系统与ui解耦 ui层通过添加监听的方式进行自刷新
2. 对于大量子节点红点的刷新不会对父节点进行重复刷新
3. 只有红点值改变时才调用ui层添加的事件 避免ui进行无用刷新

## 使用方法
1. 注册红点值计算方法 添加红点节点路径
```javascript
RedDotManager.Instance.Init();

RedDotManager.Instance.AddRedDotNode(ERedDotType.MainMenu);
RedDotManager.Instance.RegisterAndAddRedDot(() => { return _petData; }, ERedDotType.MainMenu, ERedDotType.MainMenu_Pet);

//对于一些配置表的静态红点可以先添加 AddRedDotNode
//然后通过 RegisterAndAddRedDot() 去注册计算方法并不会重复添加红点
RedDotManager.Instance.AddRedDotNode(ERedDotType.MainMenu, ERedDotType.MainMenu_Skill, ERedDotType.Skill_LevelUp);
RedDotManager.Instance.RegisterAndAddRedDot(() => { return _skillLevelUpData; }, ERedDotType.MainMenu, ERedDotType.MainMenu_Skill, ERedDotType.Skill_LevelUp);

```
2. 注册过后的红点类型后续使用 可直接传入类型进行添加监听和刷新逻辑
```javascript
RedDotManager.Instance.AddRefreshListener(ERedDotType.Skill_LevelUp, OnRefreshReddot);
RedDotManager.Instance.RefreshRedDot(ERedDotType.Skill_LevelUp);
```
3. 别忘了移除监听或删除红点 (根据业务决定)
```javascript
RedDotManager.Instance.RemoveRefreshListener();
RedDotManager.Instance.RemoveRedDot();
```
