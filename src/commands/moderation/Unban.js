const patron = require('patron.js');
const util = require('../../utility');
const config = require('../../config.json');
const ModerationService = require('../../services/ModerationService.js');

class Unban extends patron.Command {
  constructor() {
    super({
      name: 'unban',
      group: 'moderation',
      description: 'Lift the ban hammer on any member.',
      botPermissions: ['BAN_MEMBERS'],
      args: [
        new patron.Argument({
          name: 'username',
          key: 'username',
          type: 'string',
          example: '"Nig Nog Nag#8686"'
        }),
        new patron.Argument({
          name: 'reason',
          key: 'reason',
          type: 'string',
          example: 'mb he was actually a good apple',
          default: '',
          remainder: true
        })
      ]
    });
  }

  async run(msg, args) {
    const fetchedBans = await msg.guild.fetchBans();
    const matches = fetchedBans.filterArray(x => (x.username + x.discriminator).toLowerCase().includes(args.username));

    if (matches.length === 1) {
      const user = matches[0];

      msg.guild.unban(user).catch(() => null);
      await util.Messenger.reply(msg.channel, msg.author, 'You have successfully unbanned ' + user.tag + '.');
      await ModerationService.tryModLog(msg.dbGuild, msg.guild, 'Unban', config.unbanColor, args.reason, msg.author, user);
      return ModerationService.tryInformUser(msg.guild, msg.author, 'unbanned', user, args.reason);
    } else if (matches.length > 5) {
      return util.Messenger.replyError(msg.channel, msg.author, 'Multiple matches found, please be more specific.');
    } else if (matches.length > 1) {
      const formattedMatches = util.StringUtil.formatUsers(matches);

      return util.Messenger.replyError(msg.channel, msg.author, 'Multiple matches found: ' + formattedMatches + '.');
    }

    return util.Messenger.replyError(msg.channel, msg.author, 'No matches found.');
  }
}

module.exports = new Unban();
