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
      console.log('Deleting mute by id...');
      await db.muteRepo.deleteById(mute._id);
      console.log('Fetching guild...');
      const guild = client.guilds.get(mute.guildId);

      if (guild === undefined) {
        return;
      }
      console.log('Fetching member...');
      const member = guild.member(mute.userId);

      if (member === null) {
        return;
      }
      console.log('Fetching dbGuild...');
      const dbGuild = await db.guildRepo.getGuild(guild.id);
      console.log('Fetching role...');
      const role = guild.roles.get(dbGuild.roles.muted);

      if (role === undefined) {
        return;
      }
      
      if (!guild.me.hasPermission('MANAGE_ROLES') && role.position >= guild.me.highestRole.position) {
        console.log('Doesn\'t have role perms.');
        return;
      }
      console.log('Removing role...');
      await member.removeRole(role);
      console.log('Mod logging...');
      await ModerationService.tryModLog(dbGuild, guild, 'Automatic Unmute', config.unmuteColor, '', null, member.user);
    }
  }, config.autoUmuteInterval);
};