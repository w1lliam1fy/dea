const patron = require('patron.js');
const util = require('../../utility');
const config = require('../../config.json');
const ModerationService = require('../../services/ModerationService.js');
const NoModerator = require('../../preconditions/NoModerator.js');

class Kick extends patron.Command {
  constructor() {
    super({
      name: 'kick',
      group: 'moderation',
      description: 'Kick any member.',
      botPermissions: ['KICK_MEMBERS'],
      args: [
        new patron.Argument({
          name: 'member',
          key: 'member',
          type: 'member',
          example: '"Slutty Margret#2222"',
          preconditions: [NoModerator]
        }),
        new patron.Argument({
          name: 'reason',
          key: 'reason',
          type: 'string',
          example: 'bad apple',
          default: '',
          remainder: true
        })
      ]
    });
  }

  async run(msg, args) {
    await args.member.kick(args.reason);
    await util.Messenger.reply(msg.channel, msg.author, 'You have successfully kicked ' + args.member.user.tag + '.');
    await ModerationService.tryInformUser(msg.guild, msg.author, 'kicked', args.member.user, args.reason);
    await ModerationService.tryModLog(msg.dbGuild, msg.guild, 'Kick', config.kickColor, args.reason, msg.author, args.member.user);
  }
}

module.exports = new Kick();
