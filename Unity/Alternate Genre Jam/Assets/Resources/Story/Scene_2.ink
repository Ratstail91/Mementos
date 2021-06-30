=== Scene_2 ===

~SetBackground("tent_1", 3)

A hospital tent.

A place for the lucky.

You deliver the supplies to a sunken-eyed doctor. A streak of blood on his uniform glistens under the harsh artificial lamps.

~ temp weather = GetVariable("weather")

Doctor: "Thanks. {weather == "wind":It's getting like a hurricane out there.|You've made it just in time for the rain.}"

* "Thanks, but I'm not too worried about the weather."
  The doctor nods, and returns to his work.
  -> DoctorResponse
* "The {weather} never shot anyone."
  Doctor: "Just making polite conversation." He smiles, forcibly.
  -> DoctorResponse
* "..."
  There's nothing to really say.
  -> DoctorResponse

=== DoctorResponse ===

...

You sit on a small uncomfortable chair, next to a wheezing mass of flesh. You observe him, casually, as he struggles for just a few more breaths.

~AddClickable("locket", "locket", -2.5, 2.5)
~AddClickable("ring", "ring", 2.5, 2.5)

The patient's personal belongings are gathered on his side-table.

You sigh.

There's really no chance for this one. He doesn't have a place among the lucky.

You pull out a flask of something, and chug it down. You offer it to noone in particular, before putting it away.

~SetBackground("blank", 3)
~RemoveClickable("locket")
~RemoveClickable("ring")

Time passes. The wheezing fades.

~SetBackground("tent_1", 3)

It's time to go.

-> Scene_3

