const patron = require('patron.js');
const db = require('../../database');
const util = require('../../utility');
const config = require('../../config.json');
const ModerationService = require('../../services/ModerationService.js');

class Mute extends patron.Command {
  constructor() {
    super({
      name: 'mute',
      group: 'moderation',
      description: 'Mute any user.',
      args: [
        new patron.Argument({
          name: 'member',
          key: 'member',
          type: 'member',
          example: '"Billy Steve#0711"'
        }),
        new patron.Argument({
          name: 'number of hours',
          key: 'hours',
          type: 'float',
          example: '48',
          default: 24
        }),
        new patron.Argument({
          name: 'reason',
          key: 'reason',
          type: 'string',
          default: '',
          example: 'was spamming like a chimney',
          isRemainder: true
        })
      ]
    });
  }

  async run(msg, args) {
    if (msg.dbGuild.roles.muted === null) {
      return util.Messenger.replyError(msg.channel, msg.author, 'You must set a muted role with the `' + config.prefix + 'setmute @Role` command before you can mute users.');
    } else if (args.member.roles.has(msg.dbGuild.roles.muted)) {
      return util.Messenger.replyError(msg.channel, msg.author, 'This user is already muted.');
    }

    const role = msg.guild.roles.get(msg.dbGuild.roles.muted);

    if (role === undefined) {
      return util.Messenger.replyError(msg.channel, msg.author, 'The set muted role has been deleted. Please set a new one with the `' + config.prefix + 'setmute @Role` command.');
    }
    
    const formattedHours =  + args.hours + ' hour' + (args.hours === 1 ? '' : 's');

    await args.member.addRole(role);
    await db.muteRepo.insertMute(args.member.id, msg.guild.id, util.NumberUtil.hoursToMs(args.hours));
    await util.Messenger.reply(msg.channel, msg.author, 'You have successfully muted ' + args.member.user.tag + ' for ' + formattedHours + '.');
    await ModerationService.tryInformUser(msg.guild, msg.author, 'mute', args.member.user, args.reason);
    await ModerationService.tryMogLog(msg.dbGuild, msg.guild, 'Mute', config.muteColor, args.reason, msg.author, args.member.user, 'Length', formattedHours);
  }
}

module.exports = new Mute();
