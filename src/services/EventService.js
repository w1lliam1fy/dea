class EventService {
  initiate(client) {
    require('../events/disconnect.js')(client);
    require('../events/error.js')(client);
    require('../events/ready.js')(client);
    require('../events/reconnect.js')(client);
    require('../events/warn.js')(client);
  }
}

module.exports = new EventService();
