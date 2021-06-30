=== Scene_3 ===

~temp dogtag = GetVariable("dogtag")
~temp ring = GetVariable("ring")
~temp locket = GetVariable("locket")

~SetBackground("dock_1", 3)

An officer stands by the dock, watching the men slowly lumber onto the ship.

You pause in front of him, and he addresses you with a curt nod.

{dogtag == "true":
    "Sir, I'd like to report the theft of a dogtag." You hold out the broken tag.
    He raises an eyebrow.
    "Found on an enemy soldier, Sir."
    He accepts the small token of your fallen friend.
    You hesitate before continuing.
  - else:
    "Sir."
}

{ring == "stolen" || locket == "stolen":
    Officer: "Is there a problem, soldier?"
    You finger the {ring == "stolen":ring}{ring == "stolen" && locket == "stolen": and the }{locket == "stolen":locket} through your pants pocket.
    "No, Sir, just feeling kind of unlucky right now."
  - else:
    You continue along the dock.
    Are you supposed to be one of the lucky ones?
}

-> Scene_End
