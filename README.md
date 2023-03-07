## Features

- Allows removing resource costs for most player actions (e.g., crafting, building)
- Allows configuring virtual resources by player permission

## Required dependencies

- [Item Retriever](https://umod.org/plugins/item-retriever) -- Simply install. No configuration or permissions needed.

## How it works

Players may be granted virtual resources such as wood, stones, etc. When a player has a virtual resource, they can perform most actions that require the resource, without having that resource in their inventory. Even if the required resource is in their inventory, it won't be consumed, unless the amount you granted is insufficient for the action.

For example, let's say you grant a player `100` virtual `wood`. That player will be able to craft a Wood Storage Box as many times as they want, without any cost, because it requires only `100`&nbsp;`wood` to craft and because virtual items are never depleted. However, if the player wants to craft a `Tool Cupboard` which requires `1000`&nbsp;`wood`, they will need at least `900` wood in their inventory, and `900` will be consumed.

### Supported actions

- Crafting items
- Building structures
- Repairing structures
- Upgrading structures
- Repairing items at a repair bench
- Unlocking tech tree blueprints
- Adding code locks to cars at a car lift
- Repairing car modules at a car lift

Note: It's not possible to configure which resources can be used for which actions, because the plugin hooks into functions that are shared by many different game actions, so the plugin cannot determine which action prompted the item request.

### Unsupported actions

Unsupported actions will require actual resources to perform them, even if the client UI shows that the action is possible.

- Switching weapon ammo
- Reloading weapon ammo
- Purchasing items from vending machines
- Purchasing vehicles from NPC vendors

These actions may be supported in a future version of the plugin.

### Exploit warning

Extreme caution is advised when installing and configuring this plugin for servers where economy matters. There are multiple ways players can extract virtual items, which may disrupt the economic balance on your server.

This plugin should be safe for servers where economy is irrelevant, such as creative servers, battlefield servers, and aim train servers.

### Permission

The following permissions come with the plugin's **default configuration**.

- `virtualitems.ruleset.build` -- Provides 100000 wood, stone, metal fragments and high quality metal.
- `virtualitems.ruleset.craft_most_items` -- Provides 100000 of most ingredients, excluding CCTV, charcoal, explosives, beancan grenade, gunpower, sulfur, scrap, tech trash. This makes it more difficult for players to simply craft explosives.
- `virtualitems.ruleset.craft_all_items` -- Provides 100000 of all ingredients.

You may define additional rulesets in the config. Each one will generate a permission of the format `virtualitems.ruleset.<name>`. If multiple ruleset permissions are granted to a player, the last one will apply, according to the order in the config.

## Configuration

```json
{
  "Rulesets": [
    {
      "Name": "build",
      "Items": {
        "metal.fragments": 100000,
        "metal.refined": 100000,
        "stones": 100000,
        "wood": 100000
      }
    },
    {
      "Name": "craft_most_items",
      "Items": {
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
        "skull.human": 100000,
        "skull.wolf": 100000,
        "smgbody": 100000,
        "spear.wooden": 100000,
        "stash.small": 100000,
        "stones": 100000,
        "syringe.medical": 100000,
        "targeting.computer": 100000,
        "tarp": 100000,
        "wood": 100000
      }
    },
    {
      "Name": "craft_all_items",
      "Items": {
        "bone.fragments": 100000,
        "can.tuna.empty": 100000,
        "cctv.camera": 100000,
        "charcoal": 100000,
        "cloth": 100000,
        "electric.rf.broadcaster": 100000,
        "electric.rf.receiver": 100000,
        "explosives": 100000,
        "fat.animal": 100000,
        "gears": 100000,
        "grenade.beancan": 100000,
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
        "scrap": 100000,
        "semibody": 100000,
        "sewingkit": 100000,
        "sheetmetal": 100000,
        "skull.human": 100000,
        "skull.wolf": 100000,
        "smgbody": 100000,
        "spear.wooden": 100000,
        "stash.small": 100000,
        "stones": 100000,
        "sulfur": 100000,
        "syringe.medical": 100000,
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
  - `Items` -- This map determines the amount of each ingredient (item short name) that players with this ruleset will have.
