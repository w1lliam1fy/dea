const db = require('../database');
const config = require('../config.json');
const ModerationService = require('../services/ModerationService.js');

module.exports = async (client) => {
  client.setInterval(async () => {
    console.log('Auto unmute being run:');
    const mutes = await db.muteRepo.findMany({});

    for (const mute of mutes)
    {
      if (mute.mutedAt + mute.muteLength > Date.now()) {
        return;
      }
      console.log('Mute time over, removing mute manually...');
      await db.muteRepo.deleteById(mute._id);
      console.log('Fetching guild...');
      const guild = client.guilds.get(mute.guildId);

      if (guild === undefined) {
        console.log('Guild is undefined.');
        return;
      }
      console.log('Fetching member...');
      const member = guild.member(mute.userId);

      if (member === null) {
        console.log('Member is null.');
        return;
      }
      console.log('Fetching dbGuild and role...');
      const dbGuild = await db.guildRepo.getGuild(guild.id);
      const role = guild.roles.get(dbGuild.roles.muted);

      if (role === undefined) {
        console.log('Role is undefined.');
        return;
      }

      if (!guild.me.hasPermission('MANAGE_ROLES') && role.position >= guild.me.highestRole.position) {
        console.log('DEA does not have permissions to remove the muted role.');
        return;
      }
      console.log('Removing role...');
      await member.removeRole(role);
      console.log('Mod logging...');
      await ModerationService.tryModLog(dbGuild, guild, 'Automatic Unmute', config.unmuteColor, '', null, member.user);
      console.log('Done.');
    }
  }, config.autoUmuteInterval);
};