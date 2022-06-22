# 📲 Commands & their usage.

### Parameter declaration:
- ` <param> ` Required. The command cannot execute without this parameter set.
- ` [param] ` Optional, not required. If not set, this command will execute without this value in mind.

## 🪡 Slash Commands

Commands executed by typing `/` in the message box. 

### /help

> Introduces the user to Barriot, how it works, what it's for and how to execute commands.

### /inbox

> Views your inbox, provided by developers for updates and breaking changes.

### /vote

> Drops a button to head to the voting website to get rewards for voting!

### /changelog

> Gets the current version's changelog.

### /about

> Gets server & uptime data for Barriot.

### /settings

> Brings up a menu to change embed color, set a featured acknowledgement & hide/unhide commands.

### /daily

> Gets your daily bump reward.

### /profile ` [user] `

- ` [user] ` The user to view a profile for.

> Views your or another user's Barriot profile. Includes buttons to view the users' acknowledgements & statistics. This command is also used to bump other users.

### /challenge ` [user] `

- ` [user] ` The user to view a profile for.

> Challenges a user to a minigame, the winner is rewarded. When executed, a number of minigames are given as options to play.

### /pins ` [page] `

- ` [page] ` The page to view.

> Gets your list of active pins. This command is also used to modify existing pins.

### /reminders ` [page] `

- ` [page] ` The page to view.

> Gets your list of active reminders. This command is also used to modify existing reminders.

### /remind ` <time> <message> [frequency] [span] `

- ` <time> ` The time to pass until this reminder is sent.
- ` <message> ` The message to remind you of.
- ` [frequency] ` The frequency of how many times this reminder should be repeated. If span is not set, it will be default.
- ` [span] ` The timespan between repeated sending. Default is 1 day. If set, frequency must be set as well.

> Creates a new reminder, when defining a frequency and span, it will repeat every <span> for <frequency> times.

### /poll

> Creates a new poll in the current channel, which will persist for 15 days. Use the result button to view current votes.

### /user-info ` [user] `

- ` [user] ` The user to view a profile for.

> Gets common information about a user like their roles & when they joined a server/discord. When executed, a button can be pressed to navigate either to the banner or avatar if they exist.

### /riddle

> Displays a riddle, click the underlying button to receive the answer.

### /dadjoke

> Displays a stupidly cheap dad joke.

### /question

> Something Barriot will try to answer.

### /showerthought

> A shower thought to think about.

### /random-fact

> Gets an interesting fact you probably did not know before.

### /dice

> Rolls dice.

### /coinflip

> Flips a coin.

### /ping

> Pings the bot.

### /math ` <calc> `

- ` <calc> ` The calculation to compute.

> Calculates your input.

### /channel

> Manages a channel.

## 👤 User Commands

Commands executed through right-clicking a `user` and navigating to the 'apps' menu. 

### Profile

> Functions as an overload for the slashcommand [/profile](#profile).

### Info

> Functions as an overload for the slashcommand [/user-info](#user-info).

### Challenge

> Functions as an overload for the slashcommand definition [/challenge](#challenge).

## 💬 Message Commands

Commands executed through right-clicking a `message` and navigating to the 'apps' menu. 

### Translate

> Translates the message to your preferred language. This preferred language can be changed by using the buttons below the translation result.

### Quote

> Quotes the message, and sends a copy of it to your DM. This quote system does **not** save messages, it only DM's you the message so you can see it later.

### Pin

> Pins the message, adding it to [/pins](#pins).
