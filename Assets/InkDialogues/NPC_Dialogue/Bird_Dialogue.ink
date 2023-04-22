EXTERNAL playVoiceLine(speaker, voiceLine)

~playVoiceLine("Bird", "cheep")
<color=\#4F86C1>Uhh, cheep cheep?</color> #speaker: Bird
-> main
=== main ===
    ~playVoiceLine("Player", "holdonasecond")
    Hold on a second, you can talk?! #speaker: Mabel
    ~playVoiceLine("Bird", "yougotme")
    <color=\#4F86C1>Yeah, you got me.</color> #speaker: Bird
    -> END