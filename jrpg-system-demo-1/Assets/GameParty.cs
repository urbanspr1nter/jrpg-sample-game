using System.Collections;
using System.Collections.Generic;
using System.Text;
using Jrpg.CharacterSystem;
using UnityEngine;

public class GameParty : MonoBehaviour
{
    public Animator PlayerAnimator { get; private set; }

    private string AlexaTag;
    private string RogerTag;
    private int ActiveAlexaValue;
    private int ActiveRogerValue;
    private bool UsingItem;
    private Texture2D PartyHudBackground;
    private Texture2D LogBoxBackground;
    private Texture2D NormalStatusBackground;
    private Texture2D PoisonStatusBackground;
    private Character Alexa;
    private Character Roger;

    private void Awake()
    {
        AlexaTag = MainGame.Store().Get<string>("AlexaTag");
        RogerTag = MainGame.Store().Get<string>("RogerTag");

        Alexa = new Character(AlexaTag);
        Roger = new Character(RogerTag);

        PlayerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();

        MainGame.Party().AddMember(AlexaTag, Alexa);
        MainGame.Party().AddMember(RogerTag, Roger);
        MainGame.Party().SetActiveCharacter(AlexaTag);
        MainGame.Store().SetGameState(Jrpg.GameState.GameStateValue.Map);

        var ActiveCharacterMap = MainGame.Store().Config.Get<Dictionary<string, int>>("ActiveCharacter");
        ActiveAlexaValue = ActiveCharacterMap["Alexa"];
        ActiveRogerValue = ActiveCharacterMap["Roger"];
        UsingItem = false;

        // UI Related
        PartyHudBackground = new Texture2D(1, 1);
        PartyHudBackground.SetPixel(0, 0, new Color(0.7f, 0.7f, 0.0f, 0.45f));
        PartyHudBackground.Apply();

        LogBoxBackground = new Texture2D(1, 1);
        LogBoxBackground.SetPixel(0, 0, new Color(0.7f, 0.7f, 0.0f, 0.45f));
        LogBoxBackground.Apply();

        NormalStatusBackground = new Texture2D(1, 1);
        NormalStatusBackground.SetPixel(0, 0, new Color(0.0f, 0.8f, 0.3f, 0.5f));
        NormalStatusBackground.Apply();

        PoisonStatusBackground = new Texture2D(1, 1);
        PoisonStatusBackground.SetPixel(0, 0, new Color(0.8f, 0.0f, 0.3f, 0.5f));
        PoisonStatusBackground.Apply();
    }

    public string GetActiveCharacter()
    {
        return MainGame.Store().MainParty.GetActiveCharacter().Name;
    }

    void Start()
    {
        if (MainGame.Store().MainParty.GetActiveCharacter().Name.Equals(AlexaTag))
        {
            PlayerAnimator.SetInteger("ActiveCharacter", ActiveAlexaValue);
        }
        else
        {
            PlayerAnimator.SetInteger("ActiveCharacter", ActiveRogerValue);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            MainGame.Store().MainParty.SetActiveCharacter(AlexaTag);
            PlayerAnimator.SetInteger("ActiveCharacter", ActiveAlexaValue);
        }
        if (Input.GetKey(KeyCode.X))
        {
            MainGame.Store().MainParty.SetActiveCharacter(RogerTag);
            PlayerAnimator.SetInteger("ActiveCharacter", ActiveRogerValue);
        }

        HandleUseItemEvent();
    }

    private void HandleUseItemEvent()
    {
        if (UsingItem)
            return;

        if (!(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3)))
            return;

        var index = 0;
        if (Input.GetKeyDown(KeyCode.Alpha1) && !UsingItem)
        {
            index = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && !UsingItem)
        {
            index = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && !UsingItem)
        {
            index = 2;
        }

        UsingItem = true;

        var itemList = MainGame.Party().QueryAllItems().FindAll(i => i.Quantity > 0);

        if (index >= itemList.Count)
        {
            index = itemList.Count - 1;
        }

        if (index == -1)
        {
            return;
        }

        var itemInfo = itemList[index];
        MainGame.Log($"Used: {itemInfo.Name}.");

        var activeCharacter = MainGame.Party().GetActiveCharacter();
        if (!itemInfo.Name.Equals("Potion"))
        {
            var parameters = new Dictionary<string, object> { { "AfflictedCharacter", activeCharacter }, { "Item", itemInfo } };
            MainGame.Party().UseItem(itemInfo.Name, activeCharacter, parameters);
        }
        else
        {
            MainGame.Party().UseItem(itemInfo.Name, activeCharacter);

            // Used a potion but it healed more than needed... Lazy for now... Just adjust the HP if over the max
            if(activeCharacter.Statistics[StatisticType.HpCurrent].CurrentValue > StatisticTypeCollection.MaxValues[StatisticType.HpCurrent])
            {
                activeCharacter.Statistics[StatisticType.HpCurrent].CurrentValue = StatisticTypeCollection.MaxValues[StatisticType.HpCurrent];
            }
        }

        UsingItem = false;
    }

    private string BuildStatusEffectTypesString(Character character)
    {
        var statusEffectTypes = new StringBuilder();
        foreach (var statusEffectType in MainGame.Store().StatusEffectManager.StatusEffectTypes(character))
        {
            statusEffectTypes.Append(statusEffectType.ToString().Substring(0, 3).ToUpper());
            statusEffectTypes.Append(" ");
        }

        return statusEffectTypes.ToString();
    }

    private void OnGUI()
    {
        var bigTitleStyle = new GUIStyle() { fontSize = 18, fontStyle = FontStyle.Bold };
        var mediumTitleStyle = new GUIStyle() { fontSize = 16, fontStyle = FontStyle.Normal };
        var smallTitleStyle = new GUIStyle() { fontSize = 14, fontStyle = FontStyle.Bold };
        var textStyle = new GUIStyle() { fontSize = 12, fontStyle = FontStyle.Bold };

        RenderCharacterStatistics(bigTitleStyle, smallTitleStyle, textStyle);
        RenderInventoryList(bigTitleStyle, mediumTitleStyle);
        RenderLogBox(bigTitleStyle);
    }

    private void RenderCharacterStatusEffects(Character character, Rect rect, GUIStyle textStyle)
    {
        // new Rect(958, 70, 96, 16)
        var statusEffectTypes = BuildStatusEffectTypesString(character);

        if (statusEffectTypes.Length > 0)
        {
            GUI.skin.box.normal.background = PoisonStatusBackground;
            GUI.Box(rect, GUIContent.none);
            GUI.Label(new Rect(rect.x + 2, rect.y + 2, 500, 100), $" Effects {statusEffectTypes}", textStyle);
        }
        else if (statusEffectTypes.Length == 0)
        {
            GUI.skin.box.normal.background = NormalStatusBackground;
            GUI.Box(rect, GUIContent.none);
            GUI.Label(new Rect(rect.x + 2, rect.y + 2, 500, 100), $" Effects NONE", textStyle);
        }
    }

    private void RenderCharacterStatistics(GUIStyle bigTitleStyle, GUIStyle smallTitleStyle, GUIStyle textStyle)
    {
        GUI.skin.box.normal.background = PartyHudBackground;
        GUI.Box(new Rect(780, 10, 360, 400), GUIContent.none);

        var ActiveCharacter = MainGame.Party().GetActiveCharacter();
        var SecondaryCharacter = MainGame.Party().GetMembers().Find(x => !x.Name.Equals(ActiveCharacter.Name));

        GUI.Label(new Rect(900, 30, 500, 100), ActiveCharacter.Name, bigTitleStyle);
        GUI.Label(
            new Rect(960, 20, 500, 100),
            $"{CharacterStatistics.LabelLevel}: {ActiveCharacter.Statistics[StatisticType.Level].CurrentValue}",
            textStyle
        );
        GUI.Label(
            new Rect(960, 35, 500, 100),
            $"{CharacterStatistics.LabelHpCurrent}: {ActiveCharacter.Statistics[StatisticType.HpCurrent].CurrentValue}/{ActiveCharacter.Statistics[StatisticType.HpMax].CurrentValue}",
            textStyle
        );
        GUI.Label(
            new Rect(960, 50, 500, 100),
            $"{CharacterStatistics.LabelMpCurrent}: {ActiveCharacter.Statistics[StatisticType.MpCurrent].CurrentValue}/{ActiveCharacter.Statistics[StatisticType.MpMax].CurrentValue}",
            textStyle
        );
        RenderCharacterStatusEffects(ActiveCharacter, new Rect(958, 70, 96, 16), textStyle);

        GUI.Label(new Rect(900, 85, 500, 100), SecondaryCharacter.Name, smallTitleStyle);
        GUI.Label(
            new Rect(960, 90, 500, 100),
            $"{CharacterStatistics.LabelLevel}: {SecondaryCharacter.Statistics[StatisticType.Level].CurrentValue}",
            textStyle
        );
        GUI.Label(
            new Rect(960, 105, 500, 100),
            $"{CharacterStatistics.LabelHpCurrent}: {SecondaryCharacter.Statistics[StatisticType.HpCurrent].CurrentValue}/{SecondaryCharacter.Statistics[StatisticType.HpMax].CurrentValue}",
            textStyle
        );
        GUI.Label(
            new Rect(960, 120, 500, 100),
            $"{CharacterStatistics.LabelMpCurrent}: {SecondaryCharacter.Statistics[StatisticType.MpCurrent].CurrentValue}/{SecondaryCharacter.Statistics[StatisticType.MpMax].CurrentValue}",
            textStyle
        );
        RenderCharacterStatusEffects(SecondaryCharacter, new Rect(958, 138, 96, 16), textStyle);
    }

    private void RenderInventoryList(GUIStyle bigTitleStyle, GUIStyle mediumTitleStyle)
    {
        GUI.Label(new Rect(800, 125, 500, 100), "Inventory", bigTitleStyle);
        var currY = 166;
        var i = 1;
        var availableItems = MainGame.Store().MainParty.QueryAllItems().FindAll(x => x.Quantity > 0);
        foreach (var item in availableItems)
        {
            if (item.Quantity == 0)
            {
                continue;
            }

            GUI.Label(new Rect(816, currY, 500, 100), $"[NUM {i}] {item.Name} ({item.Quantity})", mediumTitleStyle);
            i++;
            currY += 20;
        }
    }

    private void RenderLogBox(GUIStyle bigTitleStyle)
    {
        GUI.skin.box.normal.background = LogBoxBackground;
        GUI.Box(new Rect(20, 550, 800, 240), GUIContent.none);
        var x = 24;
        var y = 555;
        foreach (var message in MainGame.TopLog())
        {
            GUI.Label(new Rect(x, y, 790, 20), message, bigTitleStyle);
            y += 32;
        }
    }
}
