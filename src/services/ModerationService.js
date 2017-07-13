const discord = require('discord.js');
const util = require('../utility');
const config = require('../config.json');
const db = require('../database');

class ModerationService {
  getPermLevel(dbGuild, member) {
    if (member.guild.ownerID === member.id) {
      return 3;
    }

    let permLevel = 0;

    for (const modRole of dbGuild.roles.mod.sort((a, b) => a.permissionLevel - b.permissionLevel)) {
      if (member.guild.roles.has(modRole.id) && member.roles.has(modRole.id)) {
        permLevel = modRole.permissionLevel;
      }
    }

    return member.hasPermission('ADMINISTRATOR') && permLevel < 2 ? 2 : permLevel;
  }

  tryInformUser(guild, moderator, action, user, reason = '') {
    return util.Messenger.tryDM(user, util.StringUtil.boldify(moderator.tag) + ' has ' + action + ' you' + (util.StringUtil.isNullOrWhiteSpace(reason) ? '.' : ' for the following reason: ' + reason + '.'), guild);
  }

  async tryModLog(dbGuild, guild, action, color, reason = '', moderator = null, user = null, extraInfoType = '', extraInfo = '') {
    if (dbGuild.channels.modLog === null) {
      return;
    }

    const channel = guild.channels.get(dbGuild.channels.modLog);
    
    if (channel === undefined) {
      return;
    }

    var embed = new discord.RichEmbed()
      .setColor(color)
      .setFooter('Case #' + dbGuild.misc.caseNumber, 'http://i.imgur.com/BQZJAqT.png')
      .setTimestamp();

    if (moderator !== null) {
      embed.setAuthor(moderator.tag, moderator.avatarURL, config.inviteLink);
    }

    let description = '**Action:** ' + action + '\n';

    if (!util.StringUtil.isNullOrWhiteSpace(extraInfoType)) {
      description += '**'+ extraInfoType + ':** ' + extraInfo + '\n';
    }

    if (user != null) {
      description += '**User:** ' + user.tag + ' (' + user.id + ')\n';
    }

    if (!util.StringUtil.isNullOrWhiteSpace(reason)) {
      description += '**Reason:** ' + reason + '\n';
    }

    embed.setDescription(description);

    await util.Messenger.trySendEmbed(channel, embed);
    await db.guildRepo.upsertGuild(dbGuild.guildId, { $inc: { 'misc.caseNumber': 1 } });
  }
}

module.exports = new ModerationService();
