class Gang {
  constructor(name, leaderId, guildId) {
    this.name = name;
    this.leaderId = leaderId;
    this.guildId = guildId;
    this.cash = 0;
    this.memberIds = [];
  }
}

module.exports = Gang;