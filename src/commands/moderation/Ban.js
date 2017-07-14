const patron = require('patron.js');
const util = require('../../utility');
const config = require('../../config.json');
const ModerationService = require('../../services/ModerationService.js');

class Ban extends patron.Command {
  constructor() {
    super({
      name: 'ban',
      group: 'moderation',
      description: 'Swing the ban hammer on any member.',
      botPermissions: ['BAN_MEMBERS'],
      args: [
        new patron.Argument({
          name: 'user',
          key: 'user',
          type: 'user',
          example: '"Chimney Up My Ass#0007"'
        }),
        new patron.Argument({
          name: 'reason',
          key: 'reason',
          type: 'string',
          example: 'terrible apple',
          default: '',
          remainder: true
        })
      ]
    });
  }

  async run(msg, args) {
    await msg.guild.ban(args.user);
    await util.Messenger.reply(msg.channel, msg.author, 'You have successfully banned ' + args.user.tag + '.');
    await ModerationService.tryInformUser(msg.guild, msg.author, 'banned', args.user, args.reason);
    await ModerationService.tryModLog(msg.dbGuild, msg.guild, 'Ban', config.banColor, args.reason, msg.author, args.user);
  }
}

module.exports = new Ban();
