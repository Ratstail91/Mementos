//=== Scene_1 ===

~SetMusic("battlefield_1", 0, 2)

~SetBackground("battlefield_1", 0)

War never ends, not really.

You're trudging through a wet trench. This was once the bulwark of the front line, the only thing that separated you from the enemy guns was about 300ft of no man's land.

Your squad members are gone now. Some of them were lucky enough to make it home. You'll be returning home too, though you don't know what kind of world you'll be going back to. You wonder if someone like you would be welcome there.

~AddClickable("photograph", "photograph", 0, 2.5)

The occasional packet of empty rations, or personal belongings can still be found hidden in the trench walls. A faded photograph lies abandoned in the mud, content faces smiling at the stained sky.

~RemoveClickable("photograph")

You move on, like you have somewhere to go.

...


The air is getting colder. The rain begins again.

...

You can't hear much besides the wind.

...

* "Fuck off rain."
  ~SetVariable("weather", "rain")
  The rain just ignores you.
  -> WeatherContinue
* "Fuck off wind."
  ~SetVariable("weather", "wind")
  The wind just ignores you.
  -> WeatherContinue

=== WeatherContinue ===

~SetBackground("battlefield_2", 3)

You nearly trip over a body buried in the mud. It's not one of your men.

You stop to take a closer look, to see if there's still some kind of ID on him.

~AddClickable("dogtag", "dogtag", 0, 2.5)

His face still shows the shock of having his guts pierced by a round of ammo, followed by whatever slow death followed. Since the armistice was signed, you don't know what will happen to these men, the ones left out to rot.

You suppose the grave registers would be by soon to bury him properly.

...

He looks about sixteen.

~RemoveClickable("dogtag")

-> Scene_2

