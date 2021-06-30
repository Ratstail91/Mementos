EXTERNAL SetVariable(key, value)
EXTERNAL GetVariable(key)
EXTERNAL SetBackground(fname, seconds)
EXTERNAL SetMusic(fname, fadeOut, fadeIn)
EXTERNAL PlaySound(fname)
EXTERNAL AddClickable(fname, ink, x, y)
EXTERNAL RemoveClickable(fname)
EXTERNAL AddZoomedImage(fname, x, y)
EXTERNAL RemoveZoomedImage(fname)

INCLUDE Scene_1.ink
INCLUDE Scene_2.ink
INCLUDE Scene_3.ink
// INCLUDE Scene_4.ink

=== Scene_End ===

~temp photograph = GetVariable("photograph")
~temp ring = GetVariable("ring")
~temp locket = GetVariable("locket")

{ring == "stolen" || locket == "stolen" || photograph == "":
    ~SetBackground("blank", 3)
  - else:
    ~SetBackground("home_1", 3)
}
~SetMusic("battlefield_2", 3, 3)

The End.

-> END
