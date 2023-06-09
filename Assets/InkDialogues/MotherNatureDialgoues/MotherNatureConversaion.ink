EXTERNAL playVoiceLine(speaker, voiceLine)
EXTERNAL weaponSwap(weaponIndex, upgradeWeapon, price)

CONST branchUpgradePrice = 10
CONST oakUpgradePrice = 20
CONST redwoodUpgradePrice = 30
VAR currentMoney = 0

VAR canUpgradeBranch = true
VAR canUpgradeOak = true
VAR canUpgradeRedwood = true

~playVoiceLine("MotherNature", "allrightchild")
<color=\#005A04>All right child, are you ready to embark on your adventure? </color> #speaker: Mother Nature
-> main

=== main ===
    + [Who are you?]
        ~playVoiceLine("Player", "firstofall")
        First of all, who are you!? #speaker: Mabel
    + [Upgrade Weapon]
        ~playVoiceLine("MotherNature","upgradewhatweapon")
        <color=\#005A04>What weapon shall I upgrade?</color> #speaker: MotherNature
        ++ [Branch ({branchUpgradePrice})]
            {
            - canUpgradeBranch == false:
                ~playVoiceLine("MotherNature","youalreadyhavethat")
                <color=\#005A04>You already have that upgrade.</color> #speaker: MotherNature
                -> main
            -currentMoney < branchUpgradePrice:
                ~playVoiceLine("MotherNature","imsorry")
                <color=\#005A04>I'm sorry, but you need to collect more acorns.</color> #speaker: MotherNature
                -> main
            -else:
                ~playVoiceLine("MotherNature", "hereyougo")
                <color=\#005A04>All right, here you go!</color> #speaker: MotherNature
                ~ canUpgradeBranch = false
                //call method to upgrade weapon
                ~weaponSwap(0, true, branchUpgradePrice)
                -> main
            }
        ++ [Oak ({oakUpgradePrice})]
            {
            - canUpgradeOak == false:
                ~playVoiceLine("MotherNature","youalreadyhavethat")
                <color=\#005A04>You already have that upgrade.</color> #speaker: MotherNature
                -> main
            -currentMoney < oakUpgradePrice:
                    ~playVoiceLine("MotherNature","imsorry")
                    <color=\#005A04>I'm sorry, but you need to collect more acorns.</color> #speaker: MotherNature
                    -> main
            - else:
                ~playVoiceLine("MotherNature", "hereyougo")
                <color=\#005A04>All right, here you go!</color> #speaker: MotherNature
                ~ canUpgradeOak = false
                //call method to upgrade weapon
                ~weaponSwap(1, true, oakUpgradePrice)
                -> main
            }
        ++ [Redwood ({redwoodUpgradePrice})]
            {
            - canUpgradeRedwood == false:
                ~playVoiceLine("MotherNature","youalreadyhavethat")
                <color=\#005A04>You already have that upgrade.</color> #speaker: MotherNature
                -> main
            -currentMoney < redwoodUpgradePrice:
                    ~playVoiceLine("MotherNature","imsorry")
                    <color=\#005A04>I'm sorry, but you need to collect more acorns.</color> #speaker: MotherNature
                    -> main
            -else:
                ~playVoiceLine("MotherNature", "hereyougo")
                <color=\#005A04>All right, here you go!</color> #speaker: MotherNature
                ~ canUpgradeRedwood = false
                //call method to upgrade weapon
                ~weaponSwap(2, true, redwoodUpgradePrice)
                -> main
            }
        ++ [Nevermind]
            ~playVoiceLine("MotherNature", "allrightchild")
            <color=\#005A04>All right child, are you ready to embark on your adventure? </color> #speaker: Mother Nature
            -> main
    + [Change Weapon.]
        -> WeaponSwap
    + [No thanks.]
        ~playVoiceLine("MotherNature", "farewell")
        <color=\#005A04>Farewell child.</color> #speaker: MotherNature
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
        <color=\#005A04>Great, is that everything?</color> #speaker: MotherNature
        -> main
    + [Oak]
        ~playVoiceLine("MotherNature","isthateverything")
        ~weaponSwap(1, false, 0)
        <color=\#005A04>Great, is that everything?</color> #speaker: MotherNature
        -> main
    + [Redwood]
        ~playVoiceLine("MotherNature","isthateverything")
        ~weaponSwap(2, false, 0)
        <color=\#005A04>Great, is that everything?</color> #speaker: MotherNature
        -> main
    + [Nevermind]
    ~playVoiceLine("MotherNature", "allrightchild")
            <color=\#005A04>All right child, are you ready to embark on your adventure? </color> #speaker: Mother Nature
        -> main