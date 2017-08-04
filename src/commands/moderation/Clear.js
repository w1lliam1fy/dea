const patron = require('patron.js');
const util = require('../../utility');
const config = require('../../config.json');
const ModerationService = require('../../services/ModerationService.js');
const Minimum = require('../../preconditions/Minimum.js');
const Maximum = require('../../preconditions/Maximum.js');

class Clear extends patron.Command {
  constructor() {
    super({
      name: 'clear',
      group: 'moderation',
      description: 'Prune an amount of messages in a text channel.',
      coooldown: 1000,
      botPermissions: ['MANAGE_MESSAGES'],
      args: [
        new patron.Argument({
          name: 'messages to clear',
          key: 'limit',
          type: 'float',
          example: '5',
          preconditions: [new Minimum(config.minClear), new Maximum(config.maxClear)]
        }),
        new patron.Argument({
          name: 'reason',
          key: 'reason',
          type: 'string',
          example: 'one of the appls was spamming like an orange.',
          defaultValue: '',
          remainder: true
        })
      ]
    });
  }

  async run(msg, args) {
    const messages = await msg.channel.fetchMessages({ limit: args.limit });

    await msg.channel.bulkDelete(messages);
    
    const reply = await util.Messenger.reply(msg.channel, msg.author, 'You have successfully deleted ' + args.limit + ' messages.');
    
    ModerationService.tryModLog(msg.dbGuild, msg.guild, 'Clear', config.clearColor, args.reason, msg.author, null, 'Messages Deleted', args.limit);
    await reply.delete(3000);
  }
}

module.exports = new Clear();
