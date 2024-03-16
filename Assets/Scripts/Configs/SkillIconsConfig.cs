using System;using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
[CreateAssetMenu(fileName = "SkillsConfig", menuName = "ScriptableObjects/SkillsConfig", order = 3)]
public class SkillIconsConfig : BaseConfig
{
    public List<SkillIconData> SkillIcons;
}

[System.Serializable]
public class SkillIconData
{
    public Sprite SkillSprite;
    [FormerlySerializedAs("SkilId")] public SkillId skillId;
}

public enum SkillId
{
    PushStrike,
    FireBolt,
    PullShot,
    Whirlwind
}
