const patron = require('patron.js');
const config = require('../config.json');
const util = require('../utility');
const ChatService = require('./ChatService.js');

class CommandService {
  async run(client, handler) {
    client.on('message', async (msg) => {
      if (!msg.content.startsWith(config.prefix)) {
        return;
      } else if (msg.author.bot) {
        return;
      }

      const context = new patron.Context(msg);

      const result = await handler.run(context, config.prefix);

      if (!result.isSuccess) {
        let message;

        switch (result.commandError) {
          case patron.CommandError.InvalidPrefix:
            await ChatService.applyCash(msg);
            return;
          case patron.CommandError.CommandNotFound:
            return;
          case patron.CommandError.Exception:
            if (result.error !== undefined) { // TODO: Check if instance of DiscordApiError when 12.0 is stable.
              if (result.error.code === 400) { 
                message = 'There seems to have been a bad request. Please report this issue with context to John#0969.';
              } else if (result.error.code === 404 || result.error.code === 50013) {
                message = 'DEA does not have permission to do that. This issue may be fixed by moving the DEA role to the top of the roles list, and giving DEA the "Administrator" server permission.';
              } else if (result.error.code === 50007) {
                message = 'DEA does not have permission to send messages to this user.';
              } else if (result.error.code >= 500 && result.error.code < 600) {
                message = 'Looks like Discord fucked up. An error has occured on Discord\'s part which is entirely unrelated with DEA. Sorry, nothing we can do.';
              } else {
                message = result.error.message;
              }
            } else {
              message = result.error.message;
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

        return util.Messenger.tryReplyError(context.channel, context.author, message);
      }
    });
  }
}

module.exports = new CommandService();
