const patron = require('patron.js');
const db = require('../../database');
const util = require('../../utility');
const config = require('../../config.json');
const ModerationService = require('../../services/ModerationService.js');

class Mute extends patron.Command {
  constructor() {
    super({
      name: 'unmute',
      group: 'moderation',
      description: 'Unmute any member.',
      botPermissions: ['MANAGE_ROLES'],
      args: [
        new patron.Argument({
          name: 'member',
          key: 'member',
          type: 'member',
          example: '"Jimmy Johnson#3636"'
        }),
        new patron.Argument({
          name: 'reason',
          key: 'reason',
          type: 'string',
          default: '',
          example: 'bribed me 50k',
          isRemainder: true
        })
      ]
    });
  }

  async run(msg, args) {
    if (msg.dbGuild.roles.muted === null) {
      return util.Messenger.replyError(msg.channel, msg.author, 'You must set a muted role with the `' + config.prefix + 'setmute @Role` command before you can unmute users.');
    } else if (!args.member.roles.has(msg.dbGuild.roles.muted)) {
      return util.Messenger.replyError(msg.channel, msg.author, 'This user is not muted.');
    }

    const role = msg.guild.roles.get(msg.dbGuild.roles.muted);

    if (role === undefined) {
      return util.Messenger.replyError(msg.channel, msg.author, 'The set muted role has been deleted. Please set a new one with the `' + config.prefix + 'setmute Role` command.');
    }

    await args.member.removeRole(role);
    await util.Messenger.reply(msg.channel, msg.author, 'You have successfully unmuted ' + args.member.user.tag + '.');
    await db.muteRepo.deleteMute(args.member.id, msg.guild.id);
    await ModerationService.tryInformUser(msg.guild, msg.author, 'unmute', args.member.user, args.reason);
    await ModerationService.tryModLog(msg.dbGuild, msg.guild, 'Unmute', config.unmuteColor, args.reason, msg.author, args.member.user);
  }
}

module.exports = new Mute();
