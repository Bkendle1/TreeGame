EXTERNAL playVoiceLine(speaker, voiceLine)
EXTERNAL weaponSwap(weaponIndex, upgradeWeapon, price)

CONST branchUpgradePrice = 10
CONST oakUpgradePrice = 20
CONST redwoodUpgradePrice = 30
VAR currentMoney = 0

-> start
=== start ===
~playVoiceLine("MotherNature", "allrightchild")
<color=\#005A04>All right child, are you ready to embark on your adventure? </color> #speaker: Mother Nature
-> main

=== main ===
    + [Who are you?]
        ~playVoiceLine("Player", "firstofall")
        First of all, who are you!? #speaker: Mabel
    + [Upgrade Weapon]
        //~playVoiceLine("MotherNature","letmegetthisstraight")
        What weapon shall I upgrade? #speaker: MotherNature
        ** [Branch ({branchUpgradePrice})]
            {currentMoney < branchUpgradePrice:
                I'm sorry, but you need to collect more acorns.
                -> main
            -else:
                All right, here you go!
                //call method to upgrade weapon
                ~weaponSwap(0, true, branchUpgradePrice)
                -> main
            }
        ** [Oak ({oakUpgradePrice})]
            {currentMoney < oakUpgradePrice:
                    I'm sorry, but you need to collect more acorns.
                    -> main
            -else:
                All right, here you go!
                //call method to upgrade weapon
                ~weaponSwap(1, true, oakUpgradePrice)
                -> main
            }
        ** [Redwood ({redwoodUpgradePrice})]
            {currentMoney < redwoodUpgradePrice:
                    I'm sorry, but you need to collect more acorns.
                    -> main
            -else:
                All right, here you go!
                //call method to upgrade weapon
                ~weaponSwap(2, true, redwoodUpgradePrice)
                -> main
            }
        ** ->
            I'm sorry, but you need to collect more acorns.
            -> END
    + [Change Weapon.]
        -> WeaponSwap
    + [No thanks.]
        ~playVoiceLine("MotherNature", "farewell")
        <color=\#005A04>Farewell child.</color>
        -> END
- ~playVoiceLine("MotherNature", "iunderstandyouhavequestions")
<color=\#005A04>I understand you have questions but there's no time right now.</color> #speaker: Mother Nature
-> END


=== WeaponSwap ===
~playVoiceLine("MotherNature","whatweapon")
    <color=\#005A04>Sure, what weapon would you like?</color> #speaker: Mother Nature
    + [Branch]
        ~playVoiceLine("MotherNature","isthateverything")
        ~weaponSwap(0, false, 0)
        <color=\#005A04>Great, is that everything?</color>
        -> main
    + [Oak]
        ~playVoiceLine("MotherNature","isthateverything")
        ~weaponSwap(1, false, 0)
        <color=\#005A04>Great, is that everything?</color>
        -> main
    + [Redwood]
        ~playVoiceLine("MotherNature","isthateverything")
        ~weaponSwap(2, false, 0)
        <color=\#005A04>Great, is that everything?</color>
        -> main