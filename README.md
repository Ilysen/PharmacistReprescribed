# Pharmacist: Represcribed
This is a modern revival of the lovely RimWorld mod [Pharmacist](https://github.com/fluffy-mods/Pharmacist), by Fluffy. For more thorough description including screenshots, check the [Steam Workshop page](https://steamcommunity.com/sharedfiles/filedetails/?id=3527418098).

Like the original mod, the code for Pharmacist: Represcribed is licensed under the [MIT License](https://opensource.org/licenses/MIT). **All** code prior to commit `a4399cff346155b54263df072a791d5f59ccaf85` was written by the original author - Fluffy - and a number of contributors; you can see a full list on [the original repository](https://github.com/fluffy-mods/Pharmacist/graphs/contributors). My work stands on their shoulders, and wouldn't exist without all the stuff they did to make it possible.

Sprite assets used in this mod are sourced from [Font Awesome 6.7.2](https://fontawesome.com/icons), under the [CC-BY-4.0 license](https://creativecommons.org/licenses/by/4.0/).

The full text of both licenses can be found in [LICENSE.md](LICENSE.md).

### To-Do
This is only for me (the author) to keep track of stuff that I may or may not work on at some point. Don't take this as a roadmap or anything like that!
- Setting to turn off main button
- Move settings popup to a button in the health window

## Changelog
### ???, 2026
#### Version 0.2
* General backend cleanup. Overall functionality should be essentially identical.
* Bottom bar touchups:
	* Now appears after the World tab but before the History tab.
	* Now has an icon instead of the word Pharmacist, making it less visually intrusive.
	* Can now be hidden (this is cosmetic; existing configurations will remain active) through a toggle in the mod settings.
* *Internal:* Reshuffled a lot of class names and namespaces, i.e. `Pharmacist` -> `Ceres.PharmacistReprescribed`. Also removed some old redundant copyright notices, documentation, etc from the original mod. now that I'm a bit more experienced with RW's codebase, I feel more confident in changing this stuff knowing it won't cause any weird bugs!

### 9 December, 2025
#### Version 0.1.4
* Tentative fix for the "No care" setting not working as intended. Might have issues.

### 4 December, 2025
#### Version 0.1.3
* Some minor refactoring to handle drafted tends in parity with vanilla. Pawns without care settings will now use the best possible medicine *from the healer's inventory only* and then switch to tending by hand once it's expended.

### 27 July, 2025
#### Version 0.1.2
* Further refined changes the from 0.1.1. Drafted tend should nominally function similar to vanilla now - using the target's medical care setting, and using medicine from the inventory first - while still respecting search radius and other stuff.
* Merge pull request #1: Updated Spanish Translations (thank you!)

### 19 July, 2025
#### Version 0.1.1
* Drafted tending will skip pharmacist settings; in such cases, doctors will always use the best medicine they have on hand.

### 16 July, 2025
#### Version 0.1
* Initial release.
