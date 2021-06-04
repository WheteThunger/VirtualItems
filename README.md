## Features

- Allows reducing the cost of specific resources for crafting and building

### How it works

This plugin is similar to Anti Items, but uses a different implementation with different trade-offs. Instead of placing items in hidden slots in the inventory, which comes with a number of problems, this plugin simply networks to clients that they have those items, and then hooks various events to pass server side validation. For example, by networking to your client that you have wood in your inventory, the crafting menu shows that you can make a wooden box, allowing you to click the craft button. When you do, the server would ordinally not allow it since you don't really have the items, but this plugin hooks the craft event and overrides the game behavior to deduct wood from the cost.

The main downside to this plugin is that it has to hook many events to work seamlessly, so the potential for conflicts with other plugins is higher. Also, if any interaction type such as vending machines isn't specifically coded into the plugin, it will appear client-side like you can buy an item, but the server will only allow the purchase if you have the items, so the button may do nothing which can confuse players.

#### Supported interactions

- Building structures
- Upgrading structures
- Crafting items

#### Unsupported interactions

Unsupported interactions will require actual resources to perform them, even if the client UI shows that the action is possible.

- Repairing structures
- Repairing modules at a car lift
- Crafting locks or keys at a car lift
- Repairing items at a repair bench
- Unlocking blueprints at a tech tree
- Vending machines

## Permission

The following permissions come with the plugin's **default configuration**.

- `virtualitems.ruleset.build` -- Provides 100000 wood, stone, metal fragments and high quality metal.
- `virtualitems.ruleset.craft_most_items` -- Provides 100000 of most ingredients, excluding CCTV, charcoal, explosives, beancan grenade, gunpower, sulfur, scrap, tech trash. This makes it more difficult for players to simply craft explosives.
- `virtualitems.ruleset.craft_all_items` -- Provides 100000 of all ingredients.

## Configuration

```json
{
  "Rulesets": [
    {
      "Name": "build",
      "ItemAmounts": {
        "wood": 100000,
        "stones": 100000,
        "metal.fragments": 100000,
        "metal.refined": 100000
      }
    },
    {
      "Name": "craft_most_items",
      "ItemAmounts": {
        "bone.fragments": 100000,
        "can.tuna.empty": 100000,
        "cloth": 100000,
        "electric.rf.broadcaster": 100000,
        "electric.rf.receiver": 100000,
        "fat.animal": 100000,
        "gears": 100000,
        "ladder.wooden.wall": 100000,
        "leather": 100000,
        "lowgradefuel": 100000,
        "metal.fragments": 100000,
        "metal.refined": 100000,
        "metalblade": 100000,
        "metalpipe": 100000,
        "metalspring": 100000,
        "propanetank": 100000,
        "pumpkin": 100000,
        "riflebody": 100000,
        "roadsigns": 100000,
        "rope": 100000,
        "semibody": 100000,
        "sewingkit": 100000,
        "sheetmetal": 100000,
        "skull.wolf": 100000,
        "skull.human": 100000,
        "smgbody": 100000,
        "spear.wooden": 100000,
        "syringe.medical": 100000,
        "stash.small": 100000,
        "stones": 100000,
        "targeting.computer": 100000,
        "tarp": 100000,
        "wood": 100000
      }
    },
    {
      "Name": "craft_all_items",
      "ItemAmounts": {
        "bone.fragments": 100000,
        "can.tuna.empty": 100000,
        "grenade.beancan": 100000,
        "cctv.camera": 100000,
        "charcoal": 100000,
        "cloth": 100000,
        "electric.rf.broadcaster": 100000,
        "electric.rf.receiver": 100000,
        "explosives": 100000,
        "fat.animal": 100000,
        "gears": 100000,
        "gunpowder": 100000,
        "ladder.wooden.wall": 100000,
        "leather": 100000,
        "lowgradefuel": 100000,
        "metal.fragments": 100000,
        "metal.refined": 100000,
        "metalblade": 100000,
        "metalpipe": 100000,
        "metalspring": 100000,
        "propanetank": 100000,
        "pumpkin": 100000,
        "riflebody": 100000,
        "roadsigns": 100000,
        "rope": 100000,
        "semibody": 100000,
        "sewingkit": 100000,
        "sheetmetal": 100000,
        "skull.wolf": 100000,
        "skull.human": 100000,
        "smgbody": 100000,
        "spear.wooden": 100000,
        "syringe.medical": 100000,
        "stash.small": 100000,
        "stones": 100000,
        "sulfur": 100000,
        "scrap": 100000,
        "targeting.computer": 100000,
        "tarp": 100000,
        "techparts": 100000,
        "wood": 100000
      }
    }
  ]
}
```

- `Rulesets` -- List of permission-based rulesets that determine how many of each ingredient players can have for free. Each ruleset defined here generates a permission of the format `virtualitems.ruleset.<name>`. Granting a ruleset to a player determines which ingredients they will have for free. Granting multiple rulesets to a player will cause only the last to apply (based on the order in the config).
  - `Name` -- Name of the ruleset. This determines the generated permission: `virtualitems.ruleset.<name>`.
  - `ItemAmounts` -- This map determines the amount of each ingredient (item short name) that players with this ruleset will have.

## Developer API

```csharp
Dictionary<string, int> API_GetItemAmounts(string userId)
```

Plugins can call this API to get the item amounts that apply to a given user. This is basically the `ItemAmounts` map from the player's ruleset.
