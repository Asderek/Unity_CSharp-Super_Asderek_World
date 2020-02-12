using UnityEngine;
using UnityEditor;

[System.Serializable]
public class AbilityDetails : AbstractMenu
{

    public Ability.AbilityId currentAbility = Ability.AbilityId.NONE;

    public Ability abilityMenu;

    public Area imageArea;
    public Area textArea;

    public void Init()
    {
        base.Init(new LimitedIndex(0));
        imageArea.SetParent(innerArea);
        textArea.SetParent(innerArea);
        abilityMenu = UIManager.GetInstance().ability;
    }

    public override void Draw()
    {
        base.DrawContext();

        if (currentAbility == Ability.AbilityId.NONE)
            return;

        if (!abilityMenu.abilities[currentAbility].saveInfo.obtained)
            return;

        GUI.DrawTexture(imageArea.GetRect(), abilityMenu.abilities[currentAbility].texture, ScaleMode.ScaleToFit);
        AbstractMenu.Label(textArea.GetRect(), abilityMenu.abilities[currentAbility].description.description,textArea.GetPercentSize()*0.7f);

    }

    public override void DrawElement(Rect rect, int elementIndex, bool isActive)
    {
    }

    public override float GetAxis()
    {
        return 0;
    }
}