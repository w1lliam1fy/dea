class GangMemberQuery {
  constructor(memberId, guildId) {
    this.guildId = guildId;
    this.$or = [
      { memberIds: { $elemMatch: { $eq: memberId } } },
      { leaderId: memberId }
    ];
  }
}

module.exports = GangMemberQuery;