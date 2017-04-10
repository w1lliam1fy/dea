#Commands
All commands are catagorized by modules. Each of the following sections is a module, and to gain more information about a specific module, you may use the `$help [Module name]` command, or simply read below.

The syntax of the command usage is: 
`Optional paramater: []`
`Required paramater: <>`

##Table Of Contents
- [Bot Owners](#botowners)
- [Owners](#owners)
- [Administration](#administration)
- [Moderation](#moderation)
- [General](#gambling)
- [Gambling](#gambling)
- [Gangs](#gangs)
- [Crime](#crime)
- [System](#system)

These commands may only be used by the bot owners.

### BotOwners

Command | Description | Usage
----------------|--------------|-------
SetGame|Sets the game of DEA.|`$SetGame <Game name>`
Global|Send a global announcement in the default channel of all servers.|`$Global <Announcement>`

### Owners

These commands may only be used by a user with the set mod role with a permission leve of 3, or the ownership of the server.

Command | Description | Usage
----------------|--------------|-------
AddModRole|Adds a moderator role with different permission levels.|`$AddModRole <@ModRole> <Perm level>`
RemoveModRole|Removes a moderator role.|`$RemoveModRole <@ModRole>`
ResetCooldowns|Resets all cooldowns for a specific user.|`$ResetCooldowns [@User]`
100k|Sets the user's balance to $100,000.00.|`$100k [@User]`
Add|Add cash into a user's balance.|`$Add <@User> <Cash>]`
Addto|Add cash to every users balance in a specific role.|`$Addto <@Role> <Cash>`
Remove|Remove cash from a user's balance.|`$Remove <@User> <Cash>`
Removefrom|Remove cash to every users balance in a specific role.|`$Removefrom <@Role> <Cash>`
Reset|Resets all user data for the entire server or a specific role.|`$Reset [@Role]`
SetGlobalMultiplier|Sets the global chatting multiplier.|`$SetGlobalMultiplier <Multiplier>`
SetRate|Sets the global temporary multiplier increase rate.|`$SetRate <Increase rate>`

### Administration  

These commands may only be used by a user with the set mod role with a permission leve of 2, the Administrator permission.

Command | Description | Usage
----------------|--------------|-------
RoleIds|DMs the user a list of Id's of all roles in the server| `$RoleIds`
SetPrefix|Sets the guild specific prefix.|`$SetPrefix <Prefix>`
SetMutedRole|Sets the muted role.|`$SetMutedRole <@MutedRole>`
AddRank|Adds a rank roles for the DEA cash system.|`$AddRank <@RankRole> <CashRequired>`
RemoveRank|Removes a rank role for the DEA cash system.|`$RemoveRank <@RankRole>`
SetModLog|Sets the moderation log.|`$SetModLog <#ModLog>`
SetDetailedLog|Sets the detailed logs channel.|`$SetDetailedLogs <#DetailedLogs>`
SetGambleChannel|Sets the gambling channel.|`$SetGambleChannel <#GambleChannel>`

### Moderation 

These commands may only be used by a user with the set mod role with a permission leve of 1, or the Administrator permission.

Command | Description | Usage
----------------|--------------|-------
Ban|Bans a user from the server.|`$Ban <@User> [Reason]`
Kick|Kicks a user from the server.|`$Kick <@User> [Reason]`
Mute|Temporarily mutes a user.|`$Mute <@User> [Reason]`
CustomMute|Temporarily mutes a user for x amount of hours.|`$CustomMute <Hours> <@User> [Reason]`
Unmute|Unmutes a muted user.|`$Unmute <@User> [Reason]`
Clear|Deletes x amount of messages.|`$Clear [Quantity of messages] [Reason]`
Chill|Prevents users from talking in for x amount of seconds.| `$chill <Seconds> [Reason]`

### General 
Command | Description | Usage
----------------|--------------|-------
Investments|Increase your money per message|`$Investments [investment]`
Leaderboards|View the richest Drug Traffickers.|`$Leaderboards`
Rates|View the best chatters.|`$Rates`
Donate|Sauce some cash to one of your mates.|`$Donate <@User> <Amount of cash>`
Rank|View the money/message rate of anyone.|`$Rank [@User]`
Money|View the wealth of anyone.|`$Money [@User]`
Rate|View the money/message rate of anyone.|`$Rate [@User]`
Ranks|View all ranks.|`$Ranks`
ModRoles|View all mod roles.|`$ModRoles`
Cooldowns|View all your command cooldowns|`$Cooldowns`

### Gambling 
Command | Description | Usage
----------------|--------------|-------
21+|Roll 20.84 or higher on a 100 sided die, win 0.2X your bet.|`$21+ <Bet>`
50x2|Roll 50.00 or higher on a 100 sided die, win your bet.|`$50x2 <Bet>`
53x2|Roll 52.50 or higher on a 100 sided die, win your bet.|`$53x2 <Bet>`
75+|Roll 75.01 or higher on a 100 sided die, win 2.8X your bet.|`$75+ <Bet>`
100x90|Roll 100.00 on a 100.00 sided die, win 9500X your bet.|`$100x9500 <Bet>`

### Gangs
Command | Description | Usage
----------------|--------------|-------
CreateGang|Allows you to create a gang at a hefty price.|`$CreateGang <Name>`
AddGangMember|Allows you to add a member to your gang.|`$AddGangMember <@User>`
Gang|Gives you all the info about any gang.|`$Gang [Gang name]`
Ganglb|Shows the wealthiest gangs.|`$Ganglb`
LeaveGang|Allows you to break all ties with a gang.|`$LeaveGang`
KickGangMember|Kicks a user from your gang.|`$KickGangMember <@Member>`
DestroyGang|Destroys a gang entirely taking down all funds with it.|`$DestroyGang`
ChangeGangName|Changes the name of your gang.|`$ChangeGangName <New name>`
TransferLeadership|Transfers the leadership of your gang to another member.|`$TransferLeadership <@Member>`
Deposit|Deposit cash into your gang's funds.|`$Deposit <Cash>`
Withdraw|Withdraw cash from your gang's funds.|`$Withdraw <Cash>`
Raid|Raid another gang in attempt to steal some of their wealth.| `$Raid <Gang name> <Resources>`

### Crime
Command | Description | Usage
----------------|--------------|-------
Whore|Sell your body for some quick cash.|`$Whore`
Jump|Jump some random nigga in the hood.|`$Jump`
Steal|Snipe some goodies from your local stores.|`$Steal`
Bully|Bully anyone's nickname to whatever you please.|`$Bully`
Rob|Lead a large scale operation on a local bank.|`$Rob <Resources>`

### System
Command | Description | Usage
----------------|--------------|-------
Invite|Provides an invite link to add DEA to your Server.|`$Invite`
Information|Information about the DEA Cash System.|`$Information`
Modules|Provides a list of all Command Modules.|`$Modules`
Help|All command information.|`$Help [Command or Module]`
Stats|All the statistics about DEA.|`$Stats`