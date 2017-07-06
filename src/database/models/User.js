class User {
  constructor(userId, guildId) {
    this.userId = userId;
    this.guildId = guildId;
    this.cash = 0;
    this.bounty = 0;
    this.health = 100;
    this.inventory = [];
    this.slaveOwnerId = '';
  }
}

module.exports = User;