const db = require('../database');
const util = require('../utility');
const config = require('../config.json');
const ModerationService = require('../services/ModerationService.js');

module.exports = async (client) => {
  client.setInterval(async () => {
    util.Logger.log('Auto unmute running...');
    const mutes = await db.muteRepo.findMany({});

    for (const mute of mutes)
    {
      util.Logger.log(mute.mutedAt);
      util.Logger.log(mute.muteLength);
      util.Logger.log(mute.mutedAt + mute.muteLength > Date.now());
      if (mute.mutedAt + mute.muteLength > Date.now()) {
        return;
      }
      util.Logger.log('Deleting mute by id...');
      await db.muteRepo.deleteById(mute._id);
      util.Logger.log('Fetching guild...');
      const guild = client.guilds.get(mute.guildId);

      if (guild === undefined) {
        return;
      }
      util.Logger.log('Fetching member...');
      const member = guild.member(mute.userId);

      if (member === null) {
        return;
      }
      util.Logger.log('Fetching dbGuild...');
      const dbGuild = await db.guildRepo.getGuild(guild.id);
      util.Logger.log('Fetching role...');
      const role = guild.roles.get(dbGuild.roles.muted);

      if (role === undefined) {
        return;
      }
      
      if (!guild.me.hasPermission('MANAGE_ROLES') && role.position >= guild.me.highestRole.position) {
        util.Logger.log('Doesn\'t have role perms.');
        return;
      }
      util.Logger.log('Removing role...');
      await member.removeRole(role);
      util.Logger.log('Mod logging...');
      await ModerationService.tryModLog(dbGuild, guild, 'Automatic Unmute', config.unmuteColor, '', null, member.user);
    }
  }, config.autoUmuteInterval);
};