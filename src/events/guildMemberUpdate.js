const db = require('../database');

module.exports = (client) => {
  client.on('guildMemberUpdate', async (oldMember, newMember) => {
    if (newMember.guild.member(newMember) === null) {
      return;
    }

    const dbGuild = await db.guildRepo.getGuild(newMember.guild.id);

    if (dbGuild.roles.muted === null || !oldMember.roles.has(dbGuild.roles.muted)) {
      return;
    } else if (!newMember.roles.has(dbGuild.roles.muted) && await db.muteRepo.anyMute(newMember.id, newMember.guild.id)) {
      return db.muteRepo.deleteMute(newMember.id, newMember.guild.id);
    }
  });
};
