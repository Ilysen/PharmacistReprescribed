This file contains the text used for the Steam Workshop page. It isn't automatically updated or anything by it being here; it's just here for documentation purposes and to make editing it easier!

___

```bbcode
[b]Last tested on game version:[/b] 1.6.4518

A continuation/fork/successor of the lovely mod Pharmacist, originally created by Fluffy. For a full description of what the mod does, you can consult the [url=https://steamcommunity.com/sharedfiles/filedetails/?id=1365242717]original page[/url], but in short: Pharmacist allows you to set the maximum level of medicine that can be used for different severities of injury. Doctors will use the best medicine available, depending on the target's allowed medical care and configured settings.

You can find this mod's source code on [url=https://github.com/Ilysen/PharmacistReprescribed]GitHub[/url], under the MIT license. Non-Steam releases can be found here too!

Sprite assets are sourced from [url=https://fontawesome.com/icons]Font Awesome 6.7.2[/url], under the CC-BY-SA 4.0 license.

[h2]Changes to the original[/h2]
Represcribed makes a number of tweaks and improvements to the original Pharmacist:
[list]
[*]Population categories have been tweaked to more closely match vanilla designations. "Animal" is now "Tamed animals"; "Guest" is now "Other", to more accurately describe its functionality. Slaves and entities have their own settings that will appear if Ideology or Anomaly are active, respectively.
[*]Added an "Ongoing conditions" classification. This includes non-fatal diseases like gut worms and fibrous mechanites, but also includes long-term illnesses like blood rot and asthma. It does [i]not[/i] include immunity-based diseases like plague, flu, and infections; Pharmacist still prescribes for those based on severity and immunity gain progress. Otherwise, anything that must be regularly tended will generally fall into this category.
[*]Adds a configurable medicine search radius, defaulting to unlimited (as in vanilla). Doctors will only grab medicine within this many tiles, searching first near the patient and then near themselves; if they can't find any, they'll default to manual care instead.
[*]Re-enables a vanilla feature allowing doctors to grab medicine from pack animals if they can't find it anywhere else. This was actually added all the way back in the Ideology days (version 1.3.3159), but Pharmacist's code was never updated to account for it.
[*]The "Minor cuts threshold" feature, which treats minor injuries as major wounds if a specific number of them are present with the goal of preventing runaway infections, now checks for all infectable wounds instead of just bleeding ones. The previous implementation meant that it didn't account for things like burns and frostbite - which are actually some of the the likeliest injury types to cause infections!
[list][*]This feature will also be ignored for pawns that can't get infections, like sanguophages or ghouls.[/list]
[*]Fixed an inconsistency in the original mod that caused doctors to always prioritize medicine from their own inventory even when there was better medicine elsewhere on the map. In practice, this means that they will now actually seek better medicine depending on the prescribed treatment, instead of using lower-quality medicine that they happen to have on hand. You can still draft tend to force them to use just what they're holding, though.
[*]Replaced all the original icons with a new set sourced from Font Awesome.
[list][*]This is primarily done as a licensing thing; the original mod was from 2018, and the original icons seem to have had a number of license changes since its publication. Sourcing a modern set seemed like a good idea, even if I'm not convinced they look as good. If I can find better ones, I'll put those in instead.[/list]
[/list]

[h2]Aren't there a few existing revivals of Pharmacist?[/h2]
There are, but none of them really did what I wanted. As far as I can tell, the original Pharmacist [i]and[/i] all of its unofficial updates weren't ever truly modernized, even though they ostensibly worked on newer versions of the game. Some specific glaring issues received fixes on a case-by-case basis, but many long-standing pain points (i.e. diseases like gut worms being treated as minor injuries even though they can seriously hamstring a colony) were never addressed, and nor did the mod ever receive extra functionality to accommodate the DLCs.

So that's why I made this one! Represcribed aims to be a proper modernization of the original Pharmacist - not just a resuscitation to keep it functional across versions. RimWorld's gotten a lot more complex since 2018, so the Pharmacist's advice should too, right?

[h2]Compatibility & troubleshooting[/h2]
[b][i]I have not tested compatibility too much, so you're in unmapped waters.[/i][/b] I made this fork for personal use, and I tend to run a pretty light mod list. I've tried to keep the code as clean as possible, but if you run into any compatibility issues, let me know and I'll take a look.

If you run into issues, please provide a log somewhere! The [url=https://steamcommunity.com/sharedfiles/filedetails/?id=2873415404]Log Publisher from HugsLib[/url] mod will automate this for you, but pastebin or something works just fine too.

[h2]Contributions[/h2]
Pharmacist: Represcribed has received contributions from the following people:
[list]
[*][b]FerchuDev:[/b] Updated Spanish translations

[h2]Future ideas[/h2]
I won't guarantee that any of these will be worked on, but they're on my bucket list:
[list]
[*]An option to use the worst possible medicine for surgical operations that are guaranteed to succeed, like anesthetizing. This is a bit difficult to pull off for a few reasons, but it's on my radar.
[*]Remake the UI. This was on Fluffy's list, but it's a bit daunting so I can see why it never materialized. If nothing else, I'd like to move it from a whole architect tab to just giving it a button alongside the "Set defaults..." dropdown when configuring a pawn's allowed medicine.
[/list]

[h2]Donations[/h2]
You can find the Ko-fi link of the original creator, Fluffy, here: https://ko-fi.com/fluffymods. I'm not sure if they're still active, but they're the one who made the mod in the first place!

As for me, I am not in this for the cash and I am never going to stop maintaining things even if I don't earn from them, but if you feel like giving support, you can use my own Ko-fi link: https://ko-fi.com/ceresetal
```
