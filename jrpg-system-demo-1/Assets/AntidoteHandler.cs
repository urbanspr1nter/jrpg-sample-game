using System;
using System.Collections.Generic;
using Jrpg.CharacterSystem;
using Jrpg.CharacterSystem.StatusEffects;
using Jrpg.InventorySystem.Items;
using Jrpg.ItemComponents;

public class AntidoteHandler : IPublishHandler
{
    public Dictionary<string, object> GetMessage(Dictionary<string, object> parameters)
    {
        var afflictedCharacter = (Character)(parameters["AfflictedCharacter"]);
        var item = (ItemInfo)(parameters["Item"]);

        MainGame.Store().StatusEffectManager.RemoveEffect(afflictedCharacter, StatusEffectType.Poison);

        return new Dictionary<string, object> {
                { "MemberName", afflictedCharacter.Name },
                { "Message", $"was healed by using {item.Name}" }
            };
    }
}