using System;
using System.Collections.Generic;
using Jrpg.System;
using Jrpg.ItemComponents;
using Jrpg.CharacterSystem;
using Jrpg.CharacterSystem.StatusEffects;
using Jrpg.InventorySystem.Items;

public class PoisonHandler : IPublishHandler
{
    public Dictionary<string, object> GetMessage(Dictionary<string, object> parameters)
    {
        var afflictedCharacter = (Character)(parameters["AfflictedCharacter"]);
        var item = (ItemInfo)(parameters["Item"]);

        MainGame.Store().StatusEffectManager.ApplyEffect(afflictedCharacter, StatusEffectType.Poison);

        return new Dictionary<string, object> {
                { "MemberName", afflictedCharacter.Name },
                { "Message", $"was poisoned by eating {item.Name}" }
            };
    }
}
