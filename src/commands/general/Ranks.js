const db = require('../../database');
const patron = require('patron.js');
const util = require('../../utility');

class Ranks extends patron.Command {
  constructor() {
    super({
      name: 'ranks',
      group: 'general',
      description: 'View all ranks in this guild.'
    });
  }

  async run(msg, args) {
    const dbGuild = await db.guildRepo.getGuild(msg.guild.id);
    const rankList = dbGuild.roles.rank.sort((a, b) => a.cashRequired - b.cashRequired);

    if (dbGuild.roles.rank.length === 0) {
      return util.Messenger.replyError(msg.channel, msg.author, 'There are no rank roles yet!');
    }

    let description = '';
    for(let i = 0; i < rankList.length; i++) {
      const rank = msg.guild.roles.find((x) => x.id === rankList[i].id);
      console.log(rankList[i]);

      description+= util.StringUtil.boldify(rank.name) + ': ' + util.NumberUtil.USD(rankList[i].cashRequired) + '.\n';
    }

    return util.Messenger.reply(msg.channel, msg.author, description);
  }
}

module.exports = new Ranks();
