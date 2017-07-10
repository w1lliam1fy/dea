All commands are catagorized by groups. Each of the following sections is a group.

The syntax of the command usage is:

`Optional paramater: []`

`Required paramater: <>`

##Table Of Contents
- [Administration](#administration)
- [Gambling](#gambling)
- [General](#general)
- [Moderation](#moderation)
- [Owners](#owners)
- [Reputation](#reputation)
- [System](#system)

### Administration

These commands may only be used by a user with the set mod role with a permission level of 2, the Administrator permission.

Command | Description | Usage
---------------- | --------------| -------
Addrank|Add a rank.|`$addrank <@role> <cashRequired>`
Disablewelcome|Disables the welcome message.|`$disablewelcome`
Removerank|Remove a rank role.|`$removerank <@role>`
Setmodlogchannel|Sets the mod log channel.|`$setmodlogchannel <mod log channel>`
Setmutedrole|Sets the muted role.|`$setmutedrole <@Muted Role>`
Setwelcome|Sets the welcome message.|`$setwelcome <message>`

### Gambling
Command | Description | Usage
---------------- | --------------| -------
25+|Roll 25.00 or higher on a 100.00 sided die to win 0.2X your bet.|`$25+ <bet>`
55x2|Roll 55.0 or higher on a 100.00 sided die to win your bet.|`$55x2 <bet>`
75+|Roll 75.00 or higher on a 100.00 sided die to win 2.6X your bet.|`$75+ <bet>`
99+|Roll 99.00 or higher on a 100.00 sided die to win 90X your bet.|`$99+ <bet>`

### General
Command | Description | Usage
---------------- | --------------| -------
Bal|View the wealth of anyone.|`$bal [@user]`
Leaderboards|View the richest Drug Traffickers.|`$leaderboards`
Modroles|View all mod roles in this guild.|`$modroles`
Rank|View the rank of anyone.|`$rank [@user]`
Ranks|View all ranks in this guild.|`$ranks`
Transfer|Transfer money to any member.|`$transfer <@member> <transfer>`

### Moderation

These commands may only be used by a user with the set mod role with a permission level of 1, or the Administrator permission.

Command | Description | Usage
---------------- | --------------| -------
Mute|Mute any user.|`$mute <@member> [number of hours] [reason]`
Unmute|Unmute any user.|`$unmute <@member> [reason]`

### Owners

These commands may only be used by a user with the set mod role with a permission level of 3, or the ownership of the server.

Command | Description | Usage
---------------- | --------------| -------
Addmodrole|Add a mod role.|`$addmodrole <@role> <permissonLevel>`
Modifycash|Allows you to modify the cash of any member.|`$modifycash <amount> [@member]`
Removemodrole|Remove a mod role.|`$removemodrole <@role>`
Reset|Resets all user data in your server.|`$reset`
Resetuser|Reset any users data.|`$resetuser [@user]`

### Reputation

The repuration group has been added in order to show you have gained a certain status and respect in a server, which will allow you access to more commands and features with a higher reputation.

Command | Description | Usage
---------------- | --------------| -------
Lowreps|View the users with the lowest reputation.|`$lowreps`
Rep|Give reputation to any user.|`$rep <@user>`
Reps|View the users with the highest reputation.|`$reps`
Unrep|Remove reputation from any user.|`$unrep <@user>`

### System
Command | Description | Usage
---------------- | --------------| -------
Help|All command information.|`$help [command]`
Info|All the information about the DEA cash system.|`$info`
Invite|Add DEA to your server.|`$invite`
Statistics|Statistics about DEA.|`$statistics`
