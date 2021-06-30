EXTERNAL SetVariable(key, value)
EXTERNAL GetVariable(key)
EXTERNAL PlaySound(fname)
EXTERNAL AddZoomedImage(fname, x, y)
EXTERNAL RemoveZoomedImage(fname)

~AddZoomedImage("locket", 0, 0)
~PlaySound("locket")

You contemplate the locket. It's a simple heart-shaped locket. It has a smiling woman inside.

~ temp locket = GetVariable("locket")

{locket == "stolen":
    ~SetVariable("locket", "")
    You put the locket down.
    ~RemoveZoomedImage("locket")
  - else:
    * You pick the locket up.
      ~SetVariable("locket", "stolen")
      Let's see what we can do with this...
      ~RemoveZoomedImage("locket")
      -> END
    * You leave the locket where it is.
      ~RemoveZoomedImage("locket")
      -> END
}
