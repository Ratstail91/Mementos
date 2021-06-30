EXTERNAL SetVariable(key, value)
EXTERNAL GetVariable(key)
EXTERNAL PlaySound(fname)
EXTERNAL AddZoomedImage(fname, x, y)
EXTERNAL RemoveZoomedImage(fname)

~AddZoomedImage("ring", 0, 0)
~PlaySound("ring")

You contemplate the ring. It's a small wedding band, probably made of cheap gold.

~ temp ring = GetVariable("ring")

{ring == "stolen":
    ~SetVariable("ring", "")
    You put the ring down.
    ~RemoveZoomedImage("ring")
  - else:
    * You pick the ring up.
      ~SetVariable("ring", "stolen")
      It's not like he'll need it.
      ~RemoveZoomedImage("ring")
      -> END
    * You leave the ring where it is.
      ~RemoveZoomedImage("ring")
      -> END
}
