EXTERNAL playVoiceLine(speaker, voiceLine)
EXTERNAL weaponSwap(weaponIndex)

~playVoiceLine("MotherNature", "allrightchild")
<color=\#005A04>All right child, are you ready to embark on your adventure? </color> #speaker: Mother Nature
-> main
=== main ===
    + [Who are you?]
        ~playVoiceLine("Player", "firstofall")
        First of all, who are you!? #speaker: Mabel
    + [So basically...]
        ~playVoiceLine("Player","letmegetthisstraight")
        So let me get this straight, you summoned me to fight this Mal-Wart? #speaker: Mabel
    + [Change Weapon.]
        -> WeaponSwap
    + [No thanks.]
        ~playVoiceLine("MotherNature", "farewell")
        <color=\#005A04>Farewell child.</color>
        -> END
- ~playVoiceLine("MotherNature", "iunderstandyouhavequestions")
<color=\#005A04>I understand you have questions but there's no time right now.</color> #speaker: Mother Nature
-> END
=== function playVoiceLine(speaker, voiceLine) ===
~ return 1

=== WeaponSwap ===
~playVoiceLine("MotherNature","whatweapon")
    <color=\#005A04>Sure, what weapon would you like?</color> #speaker: Mother Nature
    + [Branch]
        ~playVoiceLine("MotherNature","isthateverything")
        ~weaponSwap(0)
        <color=\#005A04>Great, is that everything?</color>
        -> main
    + [Oak]
        ~playVoiceLine("MotherNature","isthateverything")
        ~weaponSwap(1)
        <color=\#005A04>Great, is that everything?</color>
        -> main
    + [Redwood]
        ~playVoiceLine("MotherNature","isthateverything")
        ~weaponSwap(2)
        <color=\#005A04>Great, is that everything?</color>
        -> main
    
