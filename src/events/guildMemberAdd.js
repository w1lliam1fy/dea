const util = require('../utility');
const db = require('../database');

module.exports = (client) => {
  client.on('guildMemberAdd', async (member) => {
    const dbGuild = await db.guildRepo.getGuild(member.guild.id);

    if (dbGuild.settings.welcomeMessage !== null) {
      return util.Messenger.tryDM(member, dbGuild.settings.welcomeMessage);
    }
  });
};
