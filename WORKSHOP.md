This file contains the text used for the Steam Workshop page. It isn't automatically updated or anything by it being here; it's just here for documentation purposes and to make editing it easier!

___

```bbcode
[b]Last tested on game version:[/b] 1.6.4518

A continuation/fork/successor of the lovely mod Pharmacist, originally created by Fluffy. For a full description of what the mod does, you can consult the [url=https://steamcommunity.com/sharedfiles/filedetails/?id=1365242717]original page[/url], but in short: Pharmacist allows you to set the maximum level of medicine that can be used for different severities of injury. Doctors will determine which medicine to use based on the type of wound, the Pharmacist's configured settings, and the target's individually-configured medical care (as in vanilla.)

You can find this mod's source code on [url=https://github.com/Ilysen/PharmacistReprescribed]GitHub[/url], under the MIT license. Non-Steam releases can be found here too!

Sprite assets are sourced from [url=https://fontawesome.com/icons]Font Awesome 6.7.2[/url], under the CC-BY-SA 4.0 license.

[i]Note:[/i] If you're using a save that used a previous version of Pharmacist, settings might not migrate cleanly, which could result in some weird behavior. This can apparently be permanently fixed by just changing any setting in the tab though!

[h2]Changes to the original[/h2]
Represcribed makes a number of tweaks and improvements to the original Pharmacist:
[list]
[*]Population categories have been tweaked to more closely match vanilla designations. "Animal" is now "Tamed animals"; "Guest" is now "Other". Slaves and entities have their own settings that will appear if Ideology or Anomaly are active, respectively.
[*]Added an "Ongoing conditions" classification. This includes non-fatal diseases like gut worms and fibrous mechanites, but also includes long-term illnesses like blood rot and asthma. It does [i]not[/i] include immunity-based diseases like plague, flu, and infections; Pharmacist still prescribes for those based on severity and immunity gain progress. Otherwise, anything that must be regularly tended will generally fall into this category.
[*]Adds a configurable medicine search radius, defaulting to unlimited (as in vanilla.) Doctors will only grab medicine within this many tiles, searching first near the patient and then near themselves; if they can't find any, they'll default to manual care instead.
[*]Re-enables a vanilla feature allowing doctors to grab medicine from pack animals if they can't find it anywhere else. This was actually added all the way back in the Ideology days (version 1.3.3159), but Pharmacist's code was never updated to account for it.
[*]The "Minor cuts threshold" feature, which treats minor injuries as major wounds if a specific number of them are present, now checks for all infectable wounds instead of just bleeding ones. The previous implementation didn't account for things like burns and frostbite, which are actually some of the the likeliest injury types to cause infections!
[list][*]This feature is also now ignored for pawns that can't get infections, like sanguophages or ghouls.[/list]
[*]Fixed an inconsistency that caused doctors to always prioritize medicine from their own inventory. In practice, this means that they will now actually seek better medicine depending on the prescribed treatment, instead of using lower-quality medicine that they happen to have on hand. You can still draft tend to force them to use just what they're holding.
[*]Replaced all the old icons with a new set sourced from Font Awesome.
[list][*]This is primarily done as a licensing thing; Pharmacist mod was from 2018, and the old icons seem to have had a number of license changes since its publication. Sourcing a modern set seemed like a good idea, even if I'm not convinced they look as good. If I can find better ones, I'll put those in instead.[/list]
[/list]

[h2]Aren't there a few existing revivals of Pharmacist?[/h2]
There are, but none of them really did what I wanted. As far as I can tell, Pharmacist [i]and[/i] all of its unofficial updates weren't ever truly modernized, even though they ostensibly worked on newer updates to the game. Some specific glaring issues received fixes on a case-by-case basis, but many long-standing pain points (i.e. diseases like gut worms being treated as minor injuries even though they can seriously hamstring a colony) were never addressed, and nor did the mod ever receive extra functionality to accommodate the DLCs.

So that's why I made this one! Represcribed aims to be a proper modernization - not just a resuscitation to keep it functional across versions. RimWorld's gotten a lot more complex since 2018, so the Pharmacist's advice should too, right?

[h2]FAQ[/h2]
I'm hitting the character limit but these are the important ones:

[h3]Where's the configuration menu?[/h3]
There's a tab for it on the bottom of the screen! Look for the one labeled Pharmacist.

[h3]I'm getting an error that the game is trying to load a duplicate package ID. How do I fix that?[/h3]
Unsubscribe from any other versions of Pharmacist! For compatibility reasons, I've kept the same mod ID in this revival, so any other versions of the mod will conflict.

[h3]What happens with blood rot?[/h3]
Blood rot (along with things like gut worms, etc. -- see  the "Changes to the original" section for more info) is treated under the [b]Ongoing conditions[/b] classification.

[h3]Fibrous/sensory mechanites have upsides. Can we treat those differently than strictly negative conditions?[/h3]
So there's actually a quirk specifically to mechanites that isn't immediately obvious: tending doesn't remove them any faster. Instead, fibrous/sensory mechanites always have a flat duration, and tending will prevent them from going from the [i]mild pain[/i] to [i]intense pain[/i] stage. Since those things are the case, and tending doesn't cause the disease to disappear more quickly, they're currently lumped in with the other long-term conditions.

[h3]Does this version  have automatic blood transfusions?[/h3]
This was a feature implemented in one of the unofficial continuations of the original mod, but I decided against including it here: I'm not a fan of the way the implementation worked in that version, but more importantly I think it's fallen victim to a bit of feature creep. The only way that the automatic transfusion stuff relates to Pharmacist's main feature is that they both do medically-related stuff. I think automatic transfusions would work fine as their own mod, but I don't really wanna add them here!

[h2]Compatibility & troubleshooting[/h2]
[b][i]I have not tested compatibility too much, so you're in unmapped waters.[/i][/b] I made this fork for personal use, and I tend to run a pretty light mod list. I've tried to keep the code as clean as possible, but if you run into any compatibility issues, let me know and I'll take a look.

If you run into issues, please provide a log somewhere! The [url=https://steamcommunity.com/sharedfiles/filedetails/?id=2873415404]Log Publisher from HugsLib[/url] mod will automate this for you, but pastebin or something works just fine too. [b][u]Issue reports without logs will be ignored, as will issue reports with big modlists and no effort made to diagnose the specific conflict.[/u][/b] I just don't have the time and energy to try to square dance around this kinda thing. You gotta help me out here!

[h2]Contributions[/h2]
Pharmacist: Represcribed has received contributions from the following people:
[list]
[*][b]FerchuDev:[/b] Updated Spanish translations
[/list]

[h2]Donations[/h2]
You can find the Ko-fi link of the original creator, Fluffy, here: https://ko-fi.com/fluffymods. They're the one who made the original mod in the first place!

As for me, I am not in this for the cash and I am never going to stop maintaining things even if I don't earn from them, but if you feel like giving support, I'd be extremely grateful! You can find my own Ko-fi link here: https://ko-fi.com/ceresetal
```
