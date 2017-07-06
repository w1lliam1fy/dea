const BaseRepository = require('./BaseRepository.js');
const PushUpdate = require('../updates/PushUpdate.js');
const PullUpdate = require('../updates/PullUpdate.js');
const IncMoneyUpdate = require('../updates/IncMoneyUpdate.js');
const GangNameQuery = require('../queries/GangNameQuery.js');
const GangMemberQuery = require('../queries/GangMemberQuery.js');
const Gang = require('../models/Gang.js');

class GangRepository extends BaseRepository {
  constructor(collection) { 
    super(collection);
  }

  anyGang(name, guildId) {
    return this.any(new GangNameQuery(name, guildId));
  }

  inGang(memberId, guildId) {
    return this.any(new GangMemberQuery(memberId, guildId));
  }

  findGangByName(name, guildId) {
    return this.findOne(new GangNameQuery(name, guildId));
  }

  findGangByMemberId(memberId, guildId) {
    return this.findOne(new GangMemberQuery(memberId, guildId));
  }
	
  insertGang(name, leaderId, guildId) {
    return this.insertOne(new Gang(name, leaderId, guildId));
  }

  updateGang(memberId, guildId, update) {
    return this.updateOne(new GangMemberQuery(memberId, guildId), update);
  }

  findGangAndUpdate(memberId, guildId, update) {
    return this.findOneAndUpdate(new GangMemberQuery(memberId, guildId), update);
  }

  modifyCash(memberId, guildId, change) {
    return this.updateGang(memberId, guildId, new IncMoneyUpdate('cash', change));
  }

  findAndModifyCash(memberId, guildId, change) {
    return this.findGangAndUpdate(memberId, guildId, new IncMoneyUpdate('cash', change));
  }

  addGangMember(memberId, guildId, newMemberId) {
    return this.updateGang(memberId, guildId, new PushUpdate('guildIds', newMemberId));
  }

  removeGangMember(memberId, guildId, memberIdToRemove) {
    return this.updateGang(memberId, guildId, new PullUpdate('guildIds', memberIdToRemove));
  }

  static isFull(gang) {
    return gang.length === 4;
  }

  static memberOf(gang, userId) {
    if (gang.leaderId === userId) {
      return true;
    } else if (gang.memberIds.some((value) => value === userId)) {
      return true;
    } else {
      return false;
    }
  }	
}

module.exports = GangRepository;