## Features

- Allows removing resource costs for most player actions (e.g., crafting, building)
- Allows configuring virtual resources by player permission
- Allows deploying items for free
- Allows reloading weapons without ammo

## Required dependencies

- [Item Retriever](https://umod.org/plugins/item-retriever) -- Simply install. No configuration or permissions needed.

## How it works

Players may be granted virtual resources such as wood, stones, etc. When a player has a virtual resource, they can perform most actions that require the resource, without having that resource in their inventory. Even if the required resource is in their inventory, it won't be consumed, unless the amount you granted is insufficient for the action.

For example, let's say you grant a player `100` virtual `wood`. That player will be able to craft a Wood Storage Box as many times as they want, without any cost, because it requires only `100`&nbsp;`wood` to craft and because virtual items are never depleted. However, if the player wants to craft a `Tool Cupboard` which requires `1000`&nbsp;`wood`, they will need at least `900` wood in their inventory, and `900` will be consumed.

### Supported actions

- Crafting items
- Building/repairing/upgrading structures
- Repairing items at a repair bench
- Reloading/switching weapon ammo
- Unlocking tech tree blueprints
- Adding code locks to cars at a car lift
- Repairing car modules at a car lift
- Purchasing items from vending machines and NPC vendors

Note: It's not possible to configure which resources can be used for which actions, because the plugin hooks into functions that are shared by many different game actions, so the plugin cannot determine which action prompted the item request.

### Exploit warning

Extreme caution is advised when installing and configuring this plugin for servers where economy matters. There are multiple ways players can extract virtual items, which may disrupt the economic balance on your server.

This plugin should be safe for servers where economy is irrelevant, such as creative servers, battlefield servers, and aim train servers.

### Permission

The following permissions come with the plugin's **default configuration**.

- `virtualitems.ruleset.build` -- Provides 100000 wood, stone, metal fragments and high quality metal.
- `virtualitems.ruleset.unlimited_ammo` -- Provides 100000 of all ammo types. Caution is advised with allowing incendiary ammo.
- `virtualitems.ruleset.craft_most_items` -- Provides 100000 of most ingredients, **excluding** CCTV, charcoal, explosives, beancan grenade, gunpower, sulfur, scrap, tech trash. This makes it more difficult for players to simply craft explosives.
- `virtualitems.ruleset.craft_all_items` -- Provides 100000 of all ingredients.
- `virtualitems.ruleset.craft_all_items_unlimited_ammo` -- Provides 100000 of all ingredients and ammo types. Caution is advised with allowing incendiary ammo.

You may define additional rulesets in the config. Each one will generate a permission of the format `virtualitems.ruleset.<name>`. If multiple ruleset permissions are granted to a player, the last one will apply, according to the order in the config.

## Configuration

Default configuration:

```json
{
  "Rulesets": [
    {
      "Name": "build",
      "All deployables are free": false,
      "Free deployables": [],
      "Items": {
        "metal.fragments": 100000,
        "metal.refined": 100000,
        "stones": 100000,
        "wood": 100000
      }
    },
    {
      "Name": "unlimited_ammo",
      "All deployables are free": false,
      "Free deployables": [],
      "Items": {
        "ammo.grenadelauncher.buckshot": 100000,
        "ammo.grenadelauncher.he": 100000,
        "ammo.grenadelauncher.smoke": 100000,
        "ammo.handmade.shell": 100000,
        "ammo.nailgun.nails": 100000,
        "ammo.pistol": 100000,
        "ammo.pistol.fire": 100000,
        "ammo.pistol.hv": 100000,
        "ammo.rifle": 100000,
        "ammo.rifle.explosive": 100000,
        "ammo.rifle.hv": 100000,
        "ammo.rifle.incendiary": 100000,
        "ammo.rocket.basic": 100000,
        "ammo.rocket.fire": 100000,
        "ammo.rocket.hv": 100000,
        "ammo.rocket.smoke": 100000,
        "ammo.shotgun": 100000,
        "ammo.shotgun.fire": 100000,
        "ammo.shotgun.slug": 100000,
        "arrow.bone": 100000,
        "arrow.fire": 100000,
        "arrow.hv": 100000,
        "arrow.wooden": 100000,
        "snowball": 100000,
        "speargun.spear": 100000
      }
    },
    {
      "Name": "craft_most_items",
      "All deployables are free": false,
      "Free deployables": [],
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
      "All deployables are free": false,
      "Free deployables": [],
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
    },
    {
      "Name": "craft_all_items_unlimited_ammo",
      "All deployables are free": false,
      "Free deployables": [],
      "Items": {
        "ammo.grenadelauncher.buckshot": 100000,
        "ammo.grenadelauncher.he": 100000,
        "ammo.grenadelauncher.smoke": 100000,
        "ammo.handmade.shell": 100000,
        "ammo.nailgun.nails": 100000,
        "ammo.pistol": 100000,
        "ammo.pistol.fire": 100000,
        "ammo.pistol.hv": 100000,
        "ammo.rifle": 100000,
        "ammo.rifle.explosive": 100000,
        "ammo.rifle.hv": 100000,
        "ammo.rifle.incendiary": 100000,
        "ammo.rocket.basic": 100000,
        "ammo.rocket.fire": 100000,
        "ammo.rocket.hv": 100000,
        "ammo.shotgun": 100000,
        "ammo.shotgun.fire": 100000,
        "ammo.shotgun.slug": 100000,
        "arrow.bone": 100000,
        "arrow.fire": 100000,
        "arrow.hv": 100000,
        "arrow.wooden": 100000,
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
        "snowball": 100000,
        "spear.wooden": 100000,
        "speargun.spear": 100000,
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

- `Rulesets` -- Each ruleset determines how many of each ingredient players can have for free, as well as whether any items are free to deploy. Each ruleset defined here generates a permission of the format `virtualitems.ruleset.<name>`. Granting a ruleset to a player determines which items/ingredients they will have for free. Granting multiple rulesets to a player will cause only the last to apply (based on the order in the config).
  - `Name` -- Name of the ruleset. This determines the generated permission: `virtualitems.ruleset.<name>`.
  - `All deployables are free` (`true` or `false`) -- While `true`, deploying any item won't consume the item, allowing the player to repeatedly deploy it as many times as they want, without having to craft more. If you don't want all deployables to be free, you can instead specify the item short names under `Free deployables`.
  - `Free deployables` -- If you want to allow only select items to be deployed for free, as an alternative to `All deployables are free`, specify the item short names here.
  - `Items` -- This section determines the amount of each item/ingredient (item short name) that players with this ruleset will have.
