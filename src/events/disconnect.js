const util = require('../utility');

module.exports = (client) => {
  client.on('reconnect', () => {
    util.Logger.log('DEA has disconnected.');
  });
};
