const discord = require('discord.js');
const util = require('../utility');
const config = require('../config.json');

class ModerationService {
  getPermLevel(dbGuild, member) {
    if (member.guild.ownerID === member.id) {
      return 3;
    }

    let permLevel = 0;

    for (const modRole of dbGuild.roles.mods.sort((a, b) => b.permissionLevel - a.permissionLevel)) {
      if (member.guild.roles.has(modRole.id) && member.roles.has(modRole.id)) {
        permLevel = modRole.permissionLevel;
      }
    }

    return member.hasPermission('ADMINISTRATOR') && permLevel < 2 ? 2 : permLevel;
  }

  tryInformUser(guild, moderator, action, user, reason = '') {
    return util.Messenger.tryDM(user, util.StringUtil.boldify(moderator.tag) + ' has attempted to ' + action + ' you' + (util.StringUtil.isNullOrWhiteSpace(reason) ? '.' : ' for the following reason: ' + reason + '.'), guild);
  }

  tryMogLog(dbGuild, guild, action, color, reason = '', moderator = null, subject = null, extraInfoType = '', extraInfo = '') {
    if (dbGuild.channels.modLog === null) {
      return;
    }

    const channel = guild.channels.get(dbGuild.channels.modLog);
    
    if (channel === undefined) {
      return;
    }

    var embed = new discord.RichEmbed()
      .setColor(color)
      .setFooter('Case ' + dbGuild.CaseNumber, 'http://i.imgur.com/BQZJAqT.png')
      .setTimestamp();

    if (moderator !== null) {
      embed.setAuthor(moderator.tag, moderator.avatarURL, config.inviteLink);
    }

    let description = '**Action:** ' + action + '\n';

    if (!util.StringUtil.isNullOrWhiteSpace(extraInfoType)) {
      description += '**'+ extraInfoType + ':** ' + extraInfo + '\n';
    }

    if (subject != null) {
      description += '**User:** ' + subject + ' (' + subject.id + ')\n';
    }

    if (!util.StringUtil.isNullOrWhiteSpace(reason)) {
      description += '**Reason:** ' + reason + '\n';
    }

    embed.setDescription(description);

    return util.Messenger.trySendEmbed(channel, embed);
  }
}

module.exports = new ModerationService();
