All commands are catagorized by modules. Each of the following sections is a module, and to gain more information about a specific module, you may use the `$help [Module name]` command, or simply read below.

The syntax of the command usage is:

`Optional paramater: []`

`Required paramater: <>`

##Table Of Contents
- [Administration](#administration)
- [BotOwners](#botowners)
- [Crime](#crime)
- [Gambling](#gambling)
- [Gangs](#gangs)
- [General](#general)
- [Items](#items)
- [Moderation](#moderation)
- [NSFW](#nsfw)
- [Owners](#owners)
- [Polls](#polls)
- [System](#system)
- [Trivia](#trivia)

### Administration

These commands may only be used by a user with the set mod role with a permission level of 2, the Administrator permission.

Command | Description | Usage
---------------- | --------------| -------
AddRank|Adds a rank role for the DEA cash system.|`$AddRank <@rankRole> <cashRequired>`
DisableWelcomeMessage|Disables the welcome message from being sent in direct messages and in the welcome channel.|`$DisableWelcomeMessage`
ModifyRank|Modfies a rank role for the DEA cash system.|`$ModifyRank <@rankRole> <newCashRequired>`
RemoveRank|Removes a rank role for the DEA cash system.|`$RemoveRank <@rankRole>`
RoleIDs|Gets the ID of all roles in the guild.|`$RoleIDs`
SetGambleChannel|Sets the gambling channel.|`$SetGambleChannel <#gambleChannel>`
SetModLog|Sets the moderation log.|`$SetModLog <#modLogChannel>`
SetMutedRole|Sets the muted role.|`$SetMutedRole <@mutedRole>`
SetPrefix|Sets the guild specific prefix.|`$SetPrefix <prefix>`
SetUpdateChannel|Sets the channel where DEA will send messages informing you of its most recent updates and new features.|`$SetUpdateChannel <#channel>`
SetWelcomeChannel|Set the channel where DEA will send a welcome message to all new users that join.|`$SetWelcomeChannel <#channel>`
SetWelcomeMessage|Sets the welcome message that DEA will send in either the Welcome Channel or the users DM's.|`$SetWelcomeMessage <message>`

### BotOwners

These commands may only be used by the bot owners provided by the Owner Ids in the Credentials.json file.

Command | Description | Usage
---------------- | --------------| -------
Blacklist|Blacklist a user from DEA entirely to the fullest extent.|`$Blacklist <userId>`
LeaveGuild|Leaves any guild by guild ID.|`$LeaveGuild <guildId>`
SendGlobalUpdate|Sends a global update message into all DEA Update channels.|`$SendGlobalUpdate <updateMessage>`
SetGame|Sets the game of DEA.|`$SetGame <game>`

### Crime
Command | Description | Usage
---------------- | --------------| -------
AddBounty|Add a bounty of any user.|`$AddBounty <@userToSet> <bounty>`
Bully|Bully anyone's nickname to whatever you please.|`$Bully <@userToBully> <nickname>`
Collect|Collect a portion from your slaves.|`$Collect`
Enslave|Enslave any users at low health.|`$Enslave <@userToEnslave>`
Jump|Jump some random nigga in the hood.|`$Jump`
Rob|Slam anyone's bank account.|`$Rob <resources> <@user>`
Shoot|Attempt to shoot a user.|`$Shoot <@userToShoot>`
Stab|Attempt to stab a user.|`$Stab <@userToStab>`
Steal|Snipe some goodies from your local stores.|`$Steal`
Suicide|Kill yourself.|`$Suicide`
Whore|Sell your body for some quick cash.|`$Whore`

### Gambling
Command | Description | Usage
---------------- | --------------| -------
100x9500|Roll 100.00 on a 100.00 sided die, win 9500X your bet.|`$100x9500 <bet>`
21+|Roll 20.84 or higher on a 100.00 sided die, win 0.2X your bet.|`$21+ <bet>`
50x2|Roll 50.01 or higher on a 100.00 sided die, win your bet.|`$50x2 <bet>`
53x2|Roll 52.50 or higher on a 100.00 sided die, win your bet.|`$53x2 <bet>`
75+|Roll 75.01 or higher on a 100.00 sided die, win 2.8X your bet.|`$75+ <bet>`

### Gangs
Command | Description | Usage
---------------- | --------------| -------
ChangeGangName|Changes the name of your gang.|`$ChangeGangName <newName>`
CreateGang|Allows you to create a gang at a hefty price.|`$CreateGang <name>`
Deposit|Deposit cash into your gang's funds.|`$Deposit <cash>`
DestroyGang|Destroys a gang entirely taking down all funds with it.|`$DestroyGang`
Gang|Gives you all the info about any gang.|`$Gang [gangName]`
GangLb|Shows the wealthiest gangs.|`$GangLb`
JoinGang|Sends a request to join a gang.|`$JoinGang <gangName>`
KickGangMember|Kicks a user from your gang.|`$KickGangMember <@gangMember>`
LeaveGang|Allows you to break all ties with a gang.|`$LeaveGang`
Raid|Raid another gang in attempt to steal some of their wealth.|`$Raid <resources> <gangName>`
Withdraw|Withdraw cash from your gang's funds.|`$Withdraw <cash>`

### General
Command | Description | Usage
---------------- | --------------| -------
Bounties|View the most targeted traffickers.|`$Bounties`
Cooldowns|View all your command cooldowns.|`$Cooldowns [@user]`
Donate|Sauce some cash to one of your mates.|`$Donate <@user> <money>`
Leaderboards|View the richest Drug Traffickers.|`$Leaderboards`
ModRoles|View all the moderator roles.|`$ModRoles`
Money|View the wealth of anyone.|`$Money [@user]`
Rank|View the detailed ranking information of any user.|`$Rank [@user]`
Ranks|View all ranks.|`$Ranks`

### Items
Command | Description | Usage
---------------- | --------------| -------
Eat|Eat a chosen food in your inventory to gain health.|`$Eat <item>`
Fish|Go fishing for some food.|`$Fish`
Hunt|Go hunting for some food.|`$Hunt`
Inventory|View the inventory of any user.|`$Inventory [@user]`
Item|Get all the information on any item.|`$Item <item>`
OpenCrate|Open a crate!|`$OpenCrate <crate>`
Shop|List of available shop items.|`$Shop [item]`
Trade|Request to trade with any user.|`$Trade <@userToTrade> <exchangeItemQuantity> <itemInExchange> <requestedItemQuantity> <requestedItem>`

### Moderation

These commands may only be used by a user with the set mod role with a permission level of 1, or the Administrator permission.

Command | Description | Usage
---------------- | --------------| -------
Ban|Bans a user.|`$Ban <@userToBan> [reason]`
Chill|Prevents users from talking in a specific channel for x amount of seconds.|`$Chill [seconds] [reason]`
Clear|Deletes x amount of messages.|`$Clear [quantity] [reason]`
CustomMute|Temporarily mutes a user for x amount of hours.|`$CustomMute <hours> <@userToMute> [reason]`
Kick|Kicks a user.|`$Kick <@userToKick> [reason]`
Mute|Permanently mutes a user.|`$Mute <@userToMute> [reason]`
Unban|Unban a user.|`$Unban <username> [reason]`
Unmute|Unmutes a muted user.|`$Unmute <@userToUnmute> [reason]`

### NSFW
Command | Description | Usage
---------------- | --------------| -------
Ass|Sauce me some booty how about that.|`$Ass`
Hentai|The real shit goes down with custom hentai tags.|`$Hentai [tag]`
Tits|Motorboat that shit.|`$Tits`

### Owners

These commands may only be used by a user with the set mod role with a permission level of 3, or the ownership of the server.

Command | Description | Usage
---------------- | --------------| -------
100k|Sets the user's balance to $100,000.00.|`$100k [@user]`
AddModRole|Adds a moderator role.|`$AddModRole <@modRole> [permissionLevel]`
AddTo|Add cash to every users balance in a specific role.|`$AddTo <@role> <money>`
ModifyCash|Add cash into a user's balance.|`$ModifyCash <money> [@user]`
ModifyHealth|Modify a user's health.|`$ModifyHealth <modifyHealth> [@user]`
ModifyInventory|Modify a user's inventory.|`$ModifyInventory <quantity> <item> [@user]`
ModifyModRole|Modfies a moderator role.|`$ModifyModRole <@modRole> <permissionLevel>`
RemoveFrom|Remove cash to every users balance in a specific role.|`$RemoveFrom <@role> <money>`
RemoveModRole|Removes a moderator role.|`$RemoveModRole <@modRole>`
Reset|Resets all user data for the entire server or a specific role.|`$Reset [@role]`
ResetUser|Resets all data for a specific user.|`$ResetUser [@user]`
SetGlobalMultiplier|Sets the global chatting multiplier.|`$SetGlobalMultiplier <globalMultiplier>`

### Polls
Command | Description | Usage
---------------- | --------------| -------
CreatePoll|Creates a poll.|`$CreatePoll <poll> <choices> [daysToLast] [elderOnly] [modOnly]`
Poll|View the information of any poll.|`$Poll <index>`
Polls|Sends you a list of all polls in progress.|`$Polls`
RemovePoll|Removes a poll.|`$RemovePoll <index>`
Vote|Vote on any poll.|`$Vote <pollIndex> <choiceIndex>`

### System
Command | Description | Usage
---------------- | --------------| -------
CashInfo|Information about the DEA Cash System.|`$CashInfo`
Cleanup|Deletes DEA's most recent messages to prevent chat flood.|`$Cleanup`
Help|All command information.|`$Help [commandOrModule]`
Invite|Invite DEA to your server!|`$Invite`
Modules|All command modules.|`$Modules`
Statistics|All the statistics about DEA.|`$Statistics`

### Trivia
Command | Description | Usage
---------------- | --------------| -------
AddQuestion|Adds a trivia question.|`$AddQuestion <question> <answer>`
ChangeAutoTriviaSettings|Enables/disables the auto trivia feature: Sends a trivia question in the default text channel every 2 minutes.|`$ChangeAutoTriviaSettings`
ModifyAnswer|Modify a trivia answer.|`$ModifyAnswer <question> <answer>`
ModifyQuestion|Modify a trivia question.|`$ModifyQuestion <question> <newQuestion>`
RemoveQuestion|Removes a trivia question.|`$RemoveQuestion <question>`
Trivia|Randomly select a trivia question to be asked, and reward whoever answers it correctly.|`$Trivia`
TriviaAnswers|Sends you a list of all trivia answers.|`$TriviaAnswers [question]`
TriviaQuestions|Sends you a list of all trivia questions.|`$TriviaQuestions`
