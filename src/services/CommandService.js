const db = require('../database');
const patron = require('patron.js');
const config = require('../config.json');
const util = require('../utility');
const ChatService = require('./ChatService.js');

class CommandService {
  async run(client, handler) {
    client.on('message', async function(msg) {
      handle(msg, handler).catch(console.error);
    });
  }
}

module.exports = new CommandService();

const handle = async function(msg, handler) {
  const inGuild = msg.guild !== null;

  msg.dbUser = inGuild ? await db.userRepo.getUser(msg.author.id, msg.guild.id) : null;
  msg.dbGuild = inGuild ? await db.guildRepo.getGuild(msg.guild.id) : null;
  msg.member = inGuild ? msg.guild.member(msg.author) : null;

  if (!msg.content.startsWith(config.prefix) && inGuild) {
    await ChatService.applyCash(msg);
    return ChatService.gangBang(msg);
  } else if (msg.author.bot) {
    return;
  }

  const result = await handler.run(msg, config.prefix);

  if (!result.isSuccess) {
    let message;

    switch (result.commandError) {
      case patron.CommandError.CommandNotFound:
        return;
      case patron.CommandError.Cooldown: {
        const cooldown = util.NumberUtil.msToTime(result.remaining);
        return util.Messenger.trySendError(msg.channel, 'Hours: ' + cooldown.hours + '\nMinutes: ' + cooldown.minutes + '\nSeconds: ' + cooldown.seconds, 
          util.StringUtil.upperFirstChar(result.command.name) + ' Cooldown');
      }
      case patron.CommandError.Exception:
        if (result.error.code !== undefined) { // TODO: Check if instance of DiscordApiError when 12.0 is stable.
          if (result.error.code === 400) { 
            message = 'There seems to have been a bad request. Please report this issue with msg to John#0969.';
          } else if (result.error.code === 404 || result.error.code === 50013) {
            message = 'DEA does not have permission to do that. This issue may be fixed by moving the DEA role to the top of the roles list, and giving DEA the "Administrator" server permission.';
          } else if (result.error.code === 50007) {
            message = 'DEA does not have permission to DM this user. Enabling the DM Privacy Settings for this server may solve this issue.';
          } else if (result.error.code >= 500 && result.error.code < 600) {
            message = 'Looks like Discord fucked up. An error has occured on Discord\'s part which is entirely unrelated with DEA. Sorry, nothing we can do.';
          } else {
            message = result.errorReason;
          }
        } else {
          message = result.errorReason;
          console.error(result.error);
        }
        break;
      case patron.CommandError.InvalidArgCount:
        message = 'You are incorrectly using this command.\n**Usage:** `' + config.prefix + result.command.getUsage() + '`\n**Example:** `' + config.prefix + result.command.getExample() + '`';
        break;
      default:
        message = result.errorReason;
        break;
    }

    return util.Messenger.tryReplyError(msg.channel, msg.author, message);
  }
};
