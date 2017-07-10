const db = require('../database');
const config = require('../config.json');
const ModerationService = require('../services/ModerationService.js');

module.exports = async (client) => {
  client.setInterval(async () => {
    const mutes = await db.muteRepo.findMany({});

    for (const mute of mutes)
    {
      if (mute.mutedAt + mute.muteLength > Date.now()) {
        return;
      }

      await db.muteRepo.deleteById(mute._id);

      const guild = client.guilds.get(mute.guildId);
      const member = guild.member(mute.userId);

      if (member === null) {
        return;
      }

      const dbGuild = await db.guildRepo.getGuild(guild.id);
      const role = guild.roles.get(dbGuild.roles.muted);

      if (role === undefined) {
        return;
      }

      if (!guild.me.hasPermission('MANAGE_ROLES') && role.position >= guild.me.highestRole.position) {
        return;
      }

      await member.removeRole(role);
      await ModerationService.tryModLog(dbGuild, guild, 'Automatic Unmute', config.unmuteColor, '', null, member.user);
    }
  }, config.autoUmuteInterval);
};