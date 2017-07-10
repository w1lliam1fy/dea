class EventService {
  initiate(client) {
    require('../events/disconnect.js')(client);
    require('../events/error.js')(client);
    require('../events/ready.js')(client);
    require('../events/reconnect.js')(client);
    require('../events/warn.js')(client);
    require('../events/guildCreate.js')(client);
    require('../events/guildMemberAdd.js')(client);
    require('../events/guildMemberUpdate.js')(client);
    require('../events/roleDelete.js')(client);
  }
}

module.exports = new EventService();
