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
    if (msg.dbGuild.roles.rank.length === 0) {
      return util.Messenger.replyError(msg.channel, msg.author, 'There are no rank roles yet!');
    }

    const sortedRanks = msg.dbGuild.roles.rank.sort((a, b) => a.cashRequired - b.cashRequired);

    let description = '';
    for (let i = 0; i < sortedRanks.length; i++) {
      const rank = msg.guild.roles.find((x) => x.id === sortedRanks[i].id);
      
      description+= rank + ': ' + util.NumberUtil.USD(sortedRanks[i].cashRequired) + '\n';
    }

    return util.Messenger.send(msg.channel, description, 'Ranks');
  }
}

module.exports = new Ranks();
