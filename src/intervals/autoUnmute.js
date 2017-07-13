const db = require('../database');
const config = require('../config.json');
const ModerationService = require('../services/ModerationService.js');

module.exports = async (client) => {
  client.setInterval(async () => {
    const mutes = await db.muteRepo.findMany({});

    for (const mute of mutes)
    {
      if (mute.mutedAt + mute.muteLength > Date.now()) {
        continue;
      }

      await db.muteRepo.deleteById(mute._id);

      const guild = client.guilds.get(mute.guildId);

      if (guild === undefined) {
        continue;
      }

      const member = guild.member(mute.userId);

      if (member === null) {
        continue;
      }

      const dbGuild = await db.guildRepo.getGuild(guild.id);
      const role = guild.roles.get(dbGuild.roles.muted);

      if (role === undefined) {
        continue;
      }
      
      if (!guild.me.hasPermission('MANAGE_ROLES') && role.position >= guild.me.highestRole.position) {
        continue;
      }

      await member.removeRole(role);
      await ModerationService.tryModLog(dbGuild, guild, 'Automatic Unmute', config.unmuteColor, '', null, member.user);
      await ModerationService.tryInformUser(guild, client.user, 'automatically unmute', member.user);
    }
  }, config.autoUmuteInterval);
};